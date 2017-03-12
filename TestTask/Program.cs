using System;
using System.Collections.Generic;
using Entities;
using Logic;
using Service;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            string port = null;

            if (args.Length > 0)
            {
                if (args[0].Contains("http"))
                {
                    port = args[0];
                }
                else
                {
                    port = "http://" + args[0];
                }
            }
            else
            {
                port = "http://localhost:8080";
            }

            FileService fileService = new FileService(port);

            fileService.Start();
            Console.WriteLine("File service started. Press any key to stop service.");
            Console.ReadKey();
            fileService.Stop();
        }
    }
}
