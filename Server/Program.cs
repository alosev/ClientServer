using System;
using System.Net;
using System.Threading.Tasks;
using Server.Controllers;
using System.Threading;

namespace Server
{
    class Program
    {
        // Хост сервера
        private static string _host = "localhost";
        // Порт сервера
        private static string _port = "9025";

        static void Main()
        {
            Start();
            Console.ReadKey();
        }

        private static void Start()
        {
            // Минимальное количество потоков в пуле
            ThreadPool.SetMinThreads(2, 2);
            // Максимальное количество потоков в пуле
            ThreadPool.SetMaxThreads(8, 8);

            // Запускаем обработчик
            Listen();
        }

        private static async Task Listen()
        {
            // Собираем url сервера
            string url = $"http://{_host}:{_port}/";

            // Ставим url на прослушку сервером
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);

            Console.WriteLine("Ожидание запросов на {0}", url);

            // Стартуем прослушку
            listener.Start();

            while (true)
            {
                // Ждем сходящих обращений и отправляем их в пул потоков
                ThreadPool.QueueUserWorkItem(ClientThread, await listener.GetContextAsync());
            }
        }

        private static void ClientThread(Object stateInfo)
        {
            new Client((HttpListenerContext)stateInfo);
        }
    }
}
