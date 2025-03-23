using System;

namespace MessagesApp.UI.Events;


public class ViewModelEvents
{
    public event Action<bool>? NotifyReadStatus;
    public event Action? SwitchMessageViewState;
    public event Action? SwitchToMessagePage;
    public event Action? NotifyPageChanged; 
    public event Action<string?, string?> UpdateUserName; 

    public void RaiseNotifyReadStatus(bool hasUnread)
    {
        NotifyReadStatus?.Invoke(hasUnread);
    }

    public void RaiseSwitchMessageViewState() => SwitchMessageViewState?.Invoke();
    
    public void RaiseUpdateUserName(string name, string email) => UpdateUserName?.Invoke(name, email);
    
    public void RaiseSwitchToMessagePage() => SwitchToMessagePage?.Invoke();
    
    public void RaiseNotifyPageChanged() => NotifyPageChanged?.Invoke();
    
}
