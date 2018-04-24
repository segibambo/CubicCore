using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Cubic.Data;
using Cubic.Data.Utility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cubic
{
    public class Program
    {
        static public IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
            //BuildWebHost(args).Run();
            
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<APPContext>();
                    // DataInitializer.SeedData(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            ProcessSeed();


            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static void ProcessSeed()
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            //Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var sqlPath = ExtensionUtility.MapPath("~/App_Data/install/seedItem.sql");
                var customCommands = new List<string>();
                customCommands.AddRange(ExtensionUtility.ParseCommands(sqlPath));
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    if (customCommands != null)
                    {
                        foreach (var strcommand in customCommands)
                        cmd.CommandText = strcommand;
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
