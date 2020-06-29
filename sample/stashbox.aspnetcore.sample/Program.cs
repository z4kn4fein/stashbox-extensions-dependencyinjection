using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Stashbox.AspNetCore.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(String[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseStashbox()
                .ConfigureWebHostDefaults(
                    webBuilder => webBuilder
                        .UseStartup<Startup>());
        }
    }
}
