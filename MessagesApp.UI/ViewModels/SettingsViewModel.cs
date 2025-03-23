using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessagesApp.UI.Data;
using MessagesApp.UI.Events;

namespace MessagesApp.UI.ViewModels;

public partial class SettingsViewModel : PageViewModel
{
    // Dependencies
    private readonly ViewModelEvents _viewModelEvents;

    // State
    private bool _isSubscribed;

    // UI-bound properties
    [ObservableProperty] private string _userName;
    [ObservableProperty] private string _userEmail;
    [ObservableProperty] private bool _falseInput = false;

    // Constructor
    public SettingsViewModel(ViewModelEvents viewModelEvents)
    {
        PageName = ApplicationPageNames.SettingsPage;
        _viewModelEvents = viewModelEvents;
    }

    // Setup
    private void Setup()
    {
        SubscribeEvents();
    }

    public override void SubscribeEvents()
    {
        if (_isSubscribed)
            return;

        _viewModelEvents.NotifyPageChanged += OnPageChanged;
        _isSubscribed = true;
    }

    public override void Dispose()
    {
        _viewModelEvents.NotifyPageChanged -= OnPageChanged;
        _isSubscribed = false;
    }

    // Commands
    [RelayCommand]
    private void SaveSettings()
    {
        _viewModelEvents.RaiseUpdateUserName(UserName, UserEmail);
        FalseInput = true;
    }

    // Event handlers
    private void OnPageChanged()
    {
        FalseInput = false;
    }
}