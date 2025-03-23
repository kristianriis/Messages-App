using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessagesApp.UI.Data;
using MessagesApp.UI.Events;
using MessagesApp.UI.Helpers;
using MessagesApp.UI.Models;
using MessagesApp.UI.Services;

namespace MessagesApp.UI.ViewModels;

public partial class MessageCreationViewModel : PageViewModel
{
    // Dependencies
    private readonly IMessageService _messageService;
    private readonly INavigationService _navigationService;
    private readonly StringInputChecker _stringInputChecker;
    private readonly ViewModelEvents _viewModelEvents;

    // State
    private bool _isSubscribed;

    // UI-bound properties
    [ObservableProperty] private Message? _newMessage = new Message();
    [ObservableProperty] private bool _falseEmailInput = false;
    [ObservableProperty] private bool _falseNameInput = false;

    // Constructor
    public MessageCreationViewModel(
        IMessageService messageService,
        INavigationService navigationService,
        StringInputChecker stringInputChecker,
        ViewModelEvents events)
    {
        PageName = ApplicationPageNames.CreationPage;
        _messageService = messageService;
        _navigationService = navigationService;
        _stringInputChecker = stringInputChecker;
        _viewModelEvents = events;

        Setup();
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
    private async Task CreateNewMessageAsync()
    {
        await AddMessageAsync();
    }

    // Core logic
    private async Task AddMessageAsync()
    {
        FalseNameInput = string.IsNullOrWhiteSpace(NewMessage.Sender);
        FalseEmailInput = !_stringInputChecker.IsValidEmail(NewMessage.Email, out var email);

        if (FalseNameInput || FalseEmailInput)
            return;

        await _messageService.AddMessageAsync(NewMessage);
        NewMessage = new Message();
        _navigationService.NavigateToMessages();
    }

    // Event handlers
    private void OnPageChanged()
    {
        FalseEmailInput = false;
        FalseNameInput = false;
    }
}