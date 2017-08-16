using Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            // Посылаем запрос на поиск
            SendRequest("search");
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // Посылаем запрос на добавление
            SendRequest("add");
        }

        private void SendRequest(string type)
        {
            // Наименование организации
            string name = nameBox.Text.Trim();
            // ИНН организации
            string taxId = taxIdBox.Text.Trim();
            // json-строка тела запроса
            string jsonString = String.Empty;

            // Хотя бы одно поле должно быть заполнено
            if (name == "" && taxId == "")
            {
                MessageBox.Show(this, "Заполните хотя бы одно поле!");
            }

            // Формируем json-строку тела запроса
            jsonString = JsonParser.ToJson(new Dictionary<string, object>() { { "type", type }, { "name", name }, { "taxid", taxId } });
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // Формируем запрос
            HttpWebRequest request = HttpWebRequest.CreateHttp("http://localhost:9025");
            request.UserAgent = "Client";
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";
            request.ContentLength = buffer.Length;

            // Помещаем в тело запрос json-строку
            using (Stream outputStream = request.GetRequestStream())
            {
                outputStream.Write(buffer, 0, buffer.Length);
                outputStream.Close();
            }

            // Обрабатывааем ответ от сервера
            GetResponse(request.GetResponse());
        }

        private void GetResponse(WebResponse response)
        {
            // json-строка ответа
            string jsonString = String.Empty;

            // Достаем из тела ответа json-строку
            using (StreamReader inputStream = new StreamReader(response.GetResponseStream()))
            {
                jsonString = inputStream.ReadToEnd();
            }

            // Преобразуем json-строку в словарь
            Dictionary<string, object> result = new Dictionary<string, object>(JsonParser.FromJson(jsonString));

            // Если есть ошибка, то выводим только ее
            if (result.ContainsKey("error") && result["error"].ToString() != "")
            {
                resultBox.Text = result["error"].ToString();
            }
            // Иначе выводим данные
            else if (result.ContainsKey("data"))
            {
                resultBox.Text = result["data"].ToString();
            }
        }

    }
}
