using System.Net;
using System.Runtime.InteropServices;

namespace HttpMouseMover
{
    class Program
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, uint dwExtraInfo);
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        static async Task HandleRequest(HttpListenerContext context)
        {
            // query format "/move?dx=10&dy=-20"
            var query = context.Request.QueryString;

            if (int.TryParse(query["dx"], out int dx) && int.TryParse(query["dy"], out int dy))
            {
                mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
            }

            context.Response.Close();
        }

        static async Task StartServer()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            Console.WriteLine("http server started");

            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => HandleRequest(context));
            }
        }

        static void Main(string[] args)
        {
            StartServer().GetAwaiter().GetResult();
        }
    }
}