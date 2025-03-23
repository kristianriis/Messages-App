using MessagesApp.UI.ViewModels;

namespace MessagesApp.UI.Services; 

public interface INavigationService
{
    void NavigateToMessages();
}
public class NavigationService(MainWindowViewModel mainWindowViewModel) : INavigationService
{
    public void NavigateToMessages()
    {
        if (mainWindowViewModel.GoToMessagesCommand.CanExecute(null))
        {
            mainWindowViewModel.GoToMessagesCommand.Execute(null);
        }
    }
}