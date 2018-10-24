using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebInterface
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, config) =>
                {
                    var resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddKeyPerFile(directoryPath: resourcesPath, optional: true);
                })
                .UseStartup<Startup>();
    }
}
