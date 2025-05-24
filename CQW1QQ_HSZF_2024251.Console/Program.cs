using CQW1QQ_HSZF_2024251.Application;
using CQW1QQ_HSZF_2024251.Application.Interfaces;
using CQW1QQ_HSZF_2024251.Console.Commands;
using CQW1QQ_HSZF_2024251.Console.Injection;
using CQW1QQ_HSZF_2024251.Persistence.MsSql;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Data;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace CQW1QQ_HSZF_2024251.Console
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandApp(RegisterServices());
            app.Configure(config => ConfigureCommands(config));

            return app.Run(args);
        }

        private static ITypeRegistrar RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IFridgeRepository, FridgeRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IPersonRepository, PersonRepository>();
            services.AddSingleton<IPantryRepository, PantryRepository>();

            services.AddSingleton<IStorageService, StorageService>();
            services.AddSingleton<IProductService, ProductService>();

            services.AddSingleton<DbContext, HouseholdDbContext>();

            services.AddDbContext<HouseholdDbContext>();

            return new TypeRegistrar(services);
        }
        private static IConfigurator ConfigureCommands(IConfigurator config)
        {
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetApplicationName("Household Database");
            config.ValidateExamples();

            config.AddCommand<StartCommand>("start")
                .WithDescription("This command starts the application.");


            return config;
        }
    }

}
}
