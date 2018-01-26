using MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SleepyHttp
{
    class Program
    {
        static HttpListener _httpListener;
        static Config _config;

        static void Main(string[] args)
        {
            _config = Config.Load();

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(_config.HostPath);

            Console.WriteLine("Started");
            Console.WriteLine(_config.HostPath);

            _httpListener.Start();
            _httpListener.BeginGetContext(StartHttpGet, null);

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        static void StartHttpGet(IAsyncResult ar)
        {
            Console.ForegroundColor = ConsoleColor.White;
            HttpListenerContext context;
            try
            {
                context = _httpListener.EndGetContext(ar);

                Console.WriteLine("{0} {1}", context.Request.HttpMethod, context.Request.Url.AbsoluteUri);

                WriteFile(_config.HostPath, context);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.ToString());
                return;
            }

            _httpListener.BeginGetContext(StartHttpGet, null);
        }

        public static void WriteFile(string prefix, HttpListenerContext context)
        {
            var response = context.Response;

            var localPrefix = Directory.GetCurrentDirectory().Replace("\\", "/") + "/";

            var filePath = context.Request.Url.AbsoluteUri.Replace(prefix, localPrefix);

            if (filePath.Length == localPrefix.Length)
                filePath += "index.html";

            var fileExtension = Path.GetExtension(filePath).ToLower();
            var mimeType = MimeTypeMap.GetMimeType(fileExtension);

            // Resolve ".." dots
            filePath = Path.GetFullPath(filePath).Replace("\\", "/");

            if(filePath.StartsWith(localPrefix) == false)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("not authorized {0}", filePath);

                WriteMessage(context.Response, 401, "not authorized");
            }
            else if (File.Exists(filePath) == false)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("not found {0}", filePath);

                WriteMessage(context.Response, 404, "not found");
            }
            else
            {
                context.Response.StatusCode = 200;
                context.Response.AddHeader("Content-Type", mimeType);

                var bytes = File.ReadAllBytes(filePath);
                response.ContentLength64 = bytes.Length;
                response.OutputStream.Write(bytes, 0, bytes.Length);
                response.OutputStream.Close();
            }
        }

        static void WriteMessage(HttpListenerResponse response, int status, string msg, string mimeType = "text/plain")
        {
            response.StatusCode = status;
            response.AddHeader("Content-Type", mimeType);

            var bytes = System.Text.Encoding.UTF8.GetBytes(msg);
            response.ContentLength64 = bytes.Length;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.OutputStream.Close();
        }


        
    }
}
