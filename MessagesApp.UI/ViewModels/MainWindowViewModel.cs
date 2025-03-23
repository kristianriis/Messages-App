using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessagesApp.UI.Data;
using MessagesApp.UI.Events;
using MessagesApp.UI.Factories;
using MessagesApp.UI.Services;

namespace MessagesApp.UI.ViewModels;

public partial class MainWindowViewModel : PageViewModel
{
    // Dependencies
    private readonly PageFactory _pageFactory;
    private readonly ViewModelEvents _viewModelEvents;
    private readonly IMessageService _messageService;

    // State
    private bool _isSubscribed;

    // UI-bound properties
    [ObservableProperty] private bool _isSideMenuExpanded = false;
    [ObservableProperty] private bool _hasUnreadMessages;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MessagePageActive))]
    [NotifyPropertyChangedFor(nameof(CreationPageActive))]
    [NotifyPropertyChangedFor(nameof(SettingsPageActive))]
    private PageViewModel _currentPage;

    // Computed properties
    public bool MessagePageActive => CurrentPage.PageName == ApplicationPageNames.MessagePage;
    public bool CreationPageActive => CurrentPage.PageName == ApplicationPageNames.CreationPage;
    public bool SettingsPageActive => CurrentPage.PageName == ApplicationPageNames.SettingsPage;

    // Constructor
    public MainWindowViewModel(PageFactory pageFactory, ViewModelEvents viewModelEvents, IMessageService messageService)
    {
        _pageFactory = pageFactory;
        _viewModelEvents = viewModelEvents;
        _messageService = messageService;

        Setup();
        GoToMessages();
    }

    public MainWindowViewModel()
    {
        if (Design.IsDesignMode)
        {
            // Design-time preview support
        }
    }

    // Setup
    private void Setup()
    {
        SubscribeEvents();
    }

    public override void SubscribeEvents()
    {
        if (_isSubscribed) return;

        _viewModelEvents.NotifyReadStatus += UpdateUnreadStatus;
        _viewModelEvents.SwitchToMessagePage += HandleSwitchToMessagePage;
        _isSubscribed = true;
    }

    // Event handlers
    private void HandleSwitchToMessagePage() => GoToMessages();
    private void UpdateUnreadStatus(bool hasUnread) => HasUnreadMessages = hasUnread;

    // Commands
    [RelayCommand]
    private void SideMenuResize() => IsSideMenuExpanded = !IsSideMenuExpanded;

    [RelayCommand]
    private void GoToMessages()
    {
        if (CurrentPage?.PageName is ApplicationPageNames.CreationPage or ApplicationPageNames.SettingsPage)
        {
            _viewModelEvents.RaiseSwitchMessageViewState();
        }

        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.MessagePage);
        _viewModelEvents.RaiseNotifyPageChanged();
    }

    [RelayCommand]
    private void GoTomessageCreation()
    {
        _ = _messageService.SaveDataAsync();
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.CreationPage);
        _viewModelEvents.RaiseNotifyPageChanged();
    }

    [RelayCommand]
    private void GoToSettings()
    {
        _ = _messageService.SaveDataAsync();
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.SettingsPage);
        _viewModelEvents.RaiseNotifyPageChanged();
    }
}