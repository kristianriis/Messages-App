using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MessagesApp.UI.Backend;
using MessagesApp.UI.Helpers;
using MessagesApp.UI.Models;

namespace MessagesApp.UI.Services;

public interface IMessageService
{
    Task SaveDataAsync();
    Task<List<Message>> GetMessagesAsync();
    Task AddMessageAsync(Message message);
    Task<bool> DeleteMessageAsync(string email, string subject);
    public event EventHandler MessagesChanged;
}

public class MessageService : IMessageService
{
    private readonly MessageHandler _messageHandler;
    private readonly UserData _userData;
    private readonly StringInputChecker _stringInputChecker;
    private bool _isDoneLoading;

    public event EventHandler MessagesChanged;

    public MessageService(MessageHandler messageHandler, UserData userData, StringInputChecker stringInputChecker)
    {
        _messageHandler = messageHandler;
        _userData = userData;
        _stringInputChecker = stringInputChecker;
        _messageHandler.MessagesChanged += OnMessagesChanged;
        LoadTimer();
    }

    private async Task LoadTimer()
    {
        await Task.Delay(4000);
        _isDoneLoading = true;
    }

    public async Task SaveDataAsync()
    {
        if (_isDoneLoading)
        {
            await _messageHandler.SaveMessagesToFileAsync();
        }
    }

    private void OnMessagesChanged(object? sender, EventArgs? e)
    {
        MessagesChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<List<Message>> GetMessagesAsync()
    {
        return await _messageHandler.GetMessagesAsync();
    }

    public async Task AddMessageAsync(Message message)
    {
        message.Date = DateTime.Now;
        message.Sender = _stringInputChecker.CapitalizeFirstLetter(message.Sender);
        message.CombinedOwnerName = $"{_userData.Username ?? "User"} <{_userData.Email ?? "user@email.com"}>";
        await _messageHandler.AddMessageAsync(message);
    }

    public async Task<bool> DeleteMessageAsync(string email, string subject)
    {
        return await _messageHandler.DeleteMessageAsync(email, subject);
    }
}