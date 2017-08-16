using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Common.Models;
using Common.Controllers;
using Json;

namespace Server.Controllers
{
    /// <summary>
    /// Обработка запроса клиента
    /// </summary>
    public class Client
    {
        public Client(HttpListenerContext context)
        {
            // Путь к файлу для загрузки/сохранения данных
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            // json-строка запроса
            string jsonRequestString = String.Empty;
            // json-строка ответа клиенту
            string jsonResponseString = String.Empty;            


            // Объект запроса от клиента
            HttpListenerRequest request = context.Request;
            // Объект ответа клиенту
            HttpListenerResponse response = context.Response;

            // Получаем json-строку из тела запроса клиента
            using (StreamReader str = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                jsonRequestString = str.ReadToEnd();
            }

            Console.WriteLine("Получен запрос от: " + request.UserAgent);
            Console.WriteLine("---" + jsonRequestString);

            // Преобразуем json-строку запроса в словарь
            Dictionary<string, object> data = new Dictionary<string, object>(JsonParser.FromJson(jsonRequestString));
            // Обращаемся к контроллеру организаций и подгружаем список
            OrganizationControllers contoller = new OrganizationControllers();
            contoller.Load(filename);

            // Если не указан тип запроса, то возвращаем ошибку
            if (!data.ContainsKey("type"))
            {
                jsonResponseString = JsonParser.ToJson(new Dictionary<string, object>() { { "error", "Не указан параметр type." }, { "data", "" } });
            }
            else
            {
                string name = String.Empty;
                string taxid = String.Empty;

                // Получаем данные из словаря
                if (data.ContainsKey("name"))
                {
                    name = data["name"].ToString();
                }

                // Получаем данные из словаря
                if (data.ContainsKey("taxid"))
                {
                    taxid = data["taxid"].ToString();
                }

                // Создаем организацию
                Organization org = new Organization() { Name = name, TaxId = taxid };

                // Обрабатыаем разные типы запросов
                if (data["type"].ToString() == "search")
                {
                    if (contoller.Search(org))
                    {
                        jsonResponseString = JsonParser.ToJson(new Dictionary<string, object>() { { "error", "" }, { "data", "Организация найдена" } });
                    }
                    else
                    {
                        jsonResponseString = JsonParser.ToJson(new Dictionary<string, object>() { { "error", "" }, { "data", "Организация не найдена" } });
                    }
                }
                else if (data["type"].ToString() == "add")
                {
                    if (contoller.Add(org))
                    {
                        jsonResponseString = JsonParser.ToJson(new Dictionary<string, object>() { { "error", "" }, { "data", "Организация создана" } });
                        contoller.Save(filename);
                    }
                    else
                    {
                        jsonResponseString = JsonParser.ToJson(new Dictionary<string, object>() { { "error", "" }, { "data", "Организация уже существует" } });
                    }
                }
            }

            // Формируем ответ и отправляем его клиенту
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponseString);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "application/json; charset=UTF-8";
            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
