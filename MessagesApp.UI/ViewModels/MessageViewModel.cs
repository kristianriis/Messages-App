using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessagesApp.UI.Data;
using MessagesApp.UI.Events;
using MessagesApp.UI.Factories;
using MessagesApp.UI.Helpers;
using MessagesApp.UI.Models;
using MessagesApp.UI.Services;

namespace MessagesApp.UI.ViewModels;

public partial class MessageViewModel : PageViewModel, IAsyncInitializable
{
    // Dependencies
    private readonly IMessageService _messageService;
    private readonly MessageSorter _messageSorter;
    private readonly IGravatarService _gravatarService;
    private readonly ViewModelEvents _viewModelEvents;
    private readonly StringInputChecker _stringInputChecker;
    private readonly MessageReadStatusChecker _messageReadStatusChecker;

    // Booleans
    private bool _isLoaded = false;
    private bool _isLoadingMessages = false;
    private bool _firstLoadBeenPerformed = false;
    private bool _isSubscribed = false;

    // UI-bound collections and properties
    public ObservableCollection<Message> Messages { get; set; } = new();

    public ObservableCollection<string> SortOptions { get; } = new()
    {
        "Sort by Date",
        "Sort by Name",
    };

    [ObservableProperty] private string _selectedSortOption = "Sort by Date";
    [ObservableProperty] private Message? _newMessage = new Message();
    [ObservableProperty] private Message? _selectedMessage = new Message();
    [ObservableProperty] private bool _isMessageSelected = false;

    private bool SortByDate => SelectedSortOption == "Sort by Date";

    // Constructor - should probably be slimmed down with a ServiceContext
    public MessageViewModel(
        MessageSorter messageSorter,
        IMessageService messageService,
        IGravatarService gravatarService,
        ViewModelEvents viewModelEvents,
        StringInputChecker stringInputChecker,
        MessageReadStatusChecker messageReadStatusChecker)
    {
        PageName = ApplicationPageNames.MessagePage;

        _messageSorter = messageSorter;
        _messageService = messageService;
        _gravatarService = gravatarService;
        _viewModelEvents = viewModelEvents;
        _stringInputChecker = stringInputChecker;
        _messageReadStatusChecker = messageReadStatusChecker;

        Setup();
    }

    // Setup and event binding
    private void Setup()
    {
        SubscribeEvents();
    }

    public override void SubscribeEvents()
    {
        if (_isSubscribed)
            return;

        _messageService.MessagesChanged += OnMessagesChanged;
        _viewModelEvents.SwitchMessageViewState += OnSwitchMessageViewState;
        _viewModelEvents.UpdateUserName += OnUserNameUpdate;
        _viewModelEvents.NotifyPageChanged += OnPageChanged;

        _isSubscribed = true;
    }

    public override void Dispose()
    {
        _messageService.MessagesChanged -= OnMessagesChanged;
        _viewModelEvents.SwitchMessageViewState -= OnSwitchMessageViewState;
        _viewModelEvents.UpdateUserName -= OnUserNameUpdate;
        _viewModelEvents.NotifyPageChanged -= OnPageChanged;
    }

    // Initialization
    public async Task InitializeAsync()
    {
        if (!_firstLoadBeenPerformed)
        {
            await LoadMessagesAsync();
            _firstLoadBeenPerformed = true;
        }
    }

    // Commands
    [RelayCommand]
    private void GoBack()
    {
        IsMessageSelected = false;
        SelectedMessage = null;
        CheckForUnreadMessages();
    }

    [RelayCommand]
    private void DeleteMessageInList(Message message)
    {
        if (Messages.Contains(message))
            Messages.Remove(message);
    }

    [RelayCommand]
    public async Task DeleteMessage()
    {
        if (SelectedMessage == null)
            return;

        var email = SelectedMessage.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("[DeleteMessage] Selected message has no mail assigned");
            return;
        }

        var messageToDelete = Messages.FirstOrDefault(m => m.Email == email);
        if (messageToDelete != null)
        {
            Messages.Remove(messageToDelete);
            messageToDelete?.Dispose();
            await _messageService.DeleteMessageAsync(email, messageToDelete.Subject);
            SelectedMessage = null;
        }
    }

    // Logic
    private async Task LoadMessagesAsync()
    {
        if (_isLoadingMessages)
            return;

        _isLoadingMessages = true;
        _isLoaded = false;
        
        try
        {
            var messages = await _messageService.GetMessagesAsync();
            var sorted = _messageSorter.SortMessages(messages, SortByDate);

            Messages.Clear();
            foreach (var msg in sorted)
                Messages.Add(msg);

            _isLoaded = true;
            _isLoadingMessages = false;
            
            CheckForUnreadMessages();
            await _gravatarService.FillGravatars(messages);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LoadMessages] Error loading messages: {ex.Message}");
        }
    }

    private void SortMessages()
    {
        if (Messages.Count == 0)
            return;
        try
        {
            var sortedMessages = _messageSorter.SortMessages(Messages.ToList(), SortByDate);

            Messages.Clear();
            foreach (var msg in sortedMessages)
                Messages.Add(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SortMessages] Couldn't sort: {ex.Message}");
        }
    }

    private void CheckForUnreadMessages()
    {
        bool hasUnreadMessages = Messages.Any(m => !m.HasBeenRead);
        _viewModelEvents.RaiseNotifyReadStatus(hasUnreadMessages);
    }

    private void CheckReadStatusOfMessage(Message? oldValue, Message? newValue, bool onPageLeave = false)
    {
        IsMessageSelected = newValue != null;
        _messageReadStatusChecker.CheckReadStatusOfMessage(SelectedMessage, oldValue, newValue, onPageLeave);
    }

    // Reactive property callbacks
    partial void OnSelectedSortOptionChanged(string? oldValue, string? newValue)
    {
        if (_isLoaded && Messages.Count > 0)
            SortMessages();
    }

    partial void OnSelectedMessageChanged(Message? oldValue, Message? newValue)
    {
        CheckReadStatusOfMessage(oldValue, newValue);
    }

    // Event handlers
    private async void OnMessagesChanged(object? sender, EventArgs e)
    {
        try
        {
            await LoadMessagesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OnMessagesChanged] Error: {ex.Message}");
        }
    }

    private void OnSwitchMessageViewState()
    {
        GoBack();
    }

    private void OnUserNameUpdate(string? name, string? email)
    {
        if (email == null)
            return;

        name ??= "user";

        if (!_stringInputChecker.IsValidEmail(email, out var output))
            return;

        foreach (var msg in Messages)
            msg.CombinedOwnerName = $"{_stringInputChecker.CapitalizeFirstLetter(name)} <{output}>";

        _ = _messageService.SaveDataAsync();
        _viewModelEvents.RaiseSwitchToMessagePage();
    }

    private void OnPageChanged()
    {
        CheckReadStatusOfMessage(SelectedMessage, null, true);
    }
}