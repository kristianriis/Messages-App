using System;
using MessagesApp.UI.Models;
using MessagesApp.UI.Services;

namespace MessagesApp.UI.Helpers;

public class MessageReadStatusChecker(IMessageService messageService)
{
    public void CheckReadStatusOfMessage(Message? curMessage, Message? oldValue, Message? newValue, bool onPageLeave = false)
    {
        if (oldValue != null && (curMessage != oldValue || onPageLeave))
        {
            var timeOpen = DateTime.UtcNow - (oldValue.OpenedAt ?? DateTime.UtcNow);
            if (!oldValue.HasBeenRead && timeOpen > TimeSpan.FromSeconds(3))
            {
                oldValue.HasBeenRead = true;
                if (onPageLeave)
                {
                    _ = messageService.SaveDataAsync();
                }
            }

            oldValue.OpenedAt = null;
        }

        if (newValue != null)
        {
            newValue.OpenedAt = DateTime.UtcNow;
        }
    }
}