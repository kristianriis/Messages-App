using System.Collections.Generic;
using System.Linq;
using MessagesApp.UI.Models;

namespace MessagesApp.UI.Helpers;

public interface IMessageSorter
{
    List<Message> SortMessages(List<Message> messages, bool sortByDate);
}
public class MessageSorter : IMessageSorter
{
    public List<Message> SortMessages(List<Message> messages, bool sortByDate)
    {
        return sortByDate 
            ? messages.OrderByDescending(m => m.Date).ToList()
            : messages.OrderBy(m => m.Sender).ToList();
    }
}