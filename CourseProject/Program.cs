using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseProject.Data;
using CourseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CourseProject {
    public class Program {

        public static async Task Main(string[] args) {
            var host = CreateHostBuilder(args).Build();

            await CreateDbIfNotExists(host);

            host.Run();
        }

        private static async Task CreateDbIfNotExists(IHost host) {

            using (var scope = host.Services.CreateScope()) {

                var services = scope.ServiceProvider;

                try {
                    var context = services.GetRequiredService<CarContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    DbInitializer.Initialize(context);
                    await DbInitializer.InitializeRolesAsync(userManager, rolesManager);
                }
                catch(Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
