using System;
using System.Threading.Tasks;
using MessagesApp.UI.Data;
using MessagesApp.UI.ViewModels;

namespace MessagesApp.UI.Factories;

public class PageFactory
{
    private readonly Func<ApplicationPageNames, PageViewModel> _pageFactory;

    public PageFactory(Func<ApplicationPageNames, PageViewModel> pageFactory)
    {
        _pageFactory = pageFactory;
    }

    public PageViewModel GetPageViewModel(ApplicationPageNames pageName)
    {
        var page = _pageFactory.Invoke(pageName);

        if (page is IAsyncInitializable asyncPage)
        {
            _ = asyncPage.InitializeAsync(); 
        }

        return page;
    }
}
    
public interface IAsyncInitializable
{
    Task InitializeAsync();
}