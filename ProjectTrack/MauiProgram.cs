using Microsoft.Extensions.Logging;
using DataAccess.Service;
using DataAccess.Service.Interface;
using MudBlazor.Services;
using ProjectTrack.Service;

namespace ProjectTrack
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Configure the application
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                });

            // Register Blazor WebView and Debug Tools
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddMudServices();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Services.AddSingleton<TransactionService>();
           
            builder.Services.AddSingleton<AuthService>();

#endif

            // Register Services for Dependency Injection
            RegisterServices(builder.Services);

            return builder.Build();
        }

        /// <summary>
        /// Registers all required services for Dependency Injection.
        /// </summary>
        /// <param name="services">Service collection to register services.</param>
        private static void RegisterServices(IServiceCollection services)
        {
            // Register the UserService as the implementation for IUserInterface
            services.AddScoped<IUserInterface, UserService>();


            // Additional services can be registered here if needed
            // Example: services.AddSingleton<SomeOtherService>();
        }
    }
}
