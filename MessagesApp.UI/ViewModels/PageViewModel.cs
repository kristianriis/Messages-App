using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MessagesApp.UI.Data;

namespace MessagesApp.UI.ViewModels;

public partial class PageViewModel : ViewModelBase, IDisposable
{
    [ObservableProperty]
    private ApplicationPageNames _pageName;

    public virtual void SubscribeEvents()
    {
    }
    
    public virtual void Dispose()
    {
        
    }
}