using Avalonia;
using System;
using MessagesApp.UI.Backend;
using MessagesApp.UI.Data;
using MessagesApp.UI.Events;
using MessagesApp.UI.Factories;
using MessagesApp.UI.Helpers;
using MessagesApp.UI.Models;
using MessagesApp.UI.Services;
using MessagesApp.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessagesApp.UI;

sealed class Program
{
    private static IHost? _host;

    [STAThread]
    public static void Main(string[] args)
    {
        _host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // ViewModels
                services.AddSingleton<MainWindowViewModel>();              
                services.AddSingleton<MessageViewModel>();               
                services.AddTransient<MessageCreationViewModel>();        
                services.AddTransient<SettingsViewModel>();             

                // Services
                services.AddSingleton<IMessageService, MessageService>(); 
                services.AddSingleton<IGravatarService, GravatarService>(); 
                services.AddSingleton<INavigationService, NavigationService>(); 
                services.AddSingleton<ViewModelEvents>();                 
                services.AddSingleton<UserData>();                         
                services.AddSingleton<MessageHandler>();                  
                services.AddHttpClient(); 

                // Helpers / Utilities
                services.AddSingleton<MessageSorter>();                   
                services.AddSingleton<StringInputChecker>();               
                services.AddTransient<MessageReadStatusChecker>();         

                // Page factory
                services.AddSingleton<PageFactory>();
                services.AddSingleton<Func<ApplicationPageNames, PageViewModel>>(sp => name => name switch
                {
                    ApplicationPageNames.SettingsPage => sp.GetRequiredService<SettingsViewModel>(),
                    ApplicationPageNames.MessagePage => sp.GetRequiredService<MessageViewModel>(),
                    ApplicationPageNames.CreationPage => sp.GetRequiredService<MessageCreationViewModel>(),
                    _ => throw new ArgumentException("Invalid page name", nameof(name))
                });
            })
            .Build();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    public static T GetService<T>() where T : class
    {
        return _host?.Services.GetRequiredService<T>() ?? throw new InvalidOperationException("Service not available.");
    }
}