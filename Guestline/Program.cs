using Guestline.Models.Commands;
using Guestline.Models;
using Guestline.Models.Database;
using Guestline.Services;
using Guestline.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Guestline
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();
            LoadData(serviceProvider);

            var commandBuilderService = serviceProvider.GetRequiredService<ICommandBuilderService>();
            var userCommandHandlerService = serviceProvider.GetRequiredService<IUserCommandHandlerService>();

            string input;
            Console.WriteLine("To exit a program use command 'exit'");
            do
            {
                Console.WriteLine("Enter command:");
                input = Console.ReadLine() ?? string.Empty;

                var commandBuildResults = commandBuilderService.BuildCommand(input);

                if (!commandBuildResults.Success || commandBuildResults.Value == null)
                {
                    Console.WriteLine(commandBuildResults.Error);
                    continue;
                }

                var output = userCommandHandlerService.Handle(commandBuildResults.Value);
                Console.WriteLine(output.ToConsoleOutput());
            } while (input != "exit");
        }

        private static void LoadData(ServiceProvider serviceProvider)
        {
            string hotelsFilePath = "./hotels.json";
            string bookingFilePath = "./bookings.json";

            var hotelRawContent = File.ReadAllText(hotelsFilePath);
            var bookingRawContent = File.ReadAllText(bookingFilePath);

            var loaderSerivce = serviceProvider.GetRequiredService<IDataLoaderService>();

            loaderSerivce.LoadHotels(hotelRawContent);
            loaderSerivce.LoadBookings(bookingRawContent);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>();

            services.AddTransient<IDateOnlyProvider, DateOnlyProvider>();
            services.AddTransient<ICommandHandlerService<AvailabilityCommand, AvailabilityCommandResult>, AvailabilityCommandHandlerService>();
            services.AddTransient<ICommandHandlerService<SearchCommand, SearchCommandResult>, SearchCommandHandlerService>();
            services.AddTransient<IDataLoaderService, DataLoaderService>();
            services.AddTransient<ICommandBuilderService, CommandBuilderService>();
            services.AddTransient<IUserCommandHandlerService, UserCommandHandlerService>();
        }
    }
}
