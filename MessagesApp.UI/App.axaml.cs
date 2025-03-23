using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MessagesApp.UI.Services;
using MessagesApp.UI.ViewModels;
using MessagesApp.UI.Views;

namespace MessagesApp.UI;

public partial class App : Application
{
    private IMessageService _messageService; 
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += OnExit;
            DisableAvaloniaDataAnnotationValidation();

            _messageService = Program.GetService<IMessageService>();
            var mainWindowViewModel = Program.GetService<MainWindowViewModel>(); // Resolve from DI

            desktop.MainWindow = new MainWindowView
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _messageService?.SaveDataAsync(); 
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}