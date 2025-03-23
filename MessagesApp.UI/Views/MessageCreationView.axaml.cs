using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MessagesApp.UI.ViewModels;

namespace MessagesApp.UI.Views;

public partial class MessageCreationView : UserControl
{
    public MessageCreationView()
    {
        InitializeComponent();
        DetachedFromVisualTree += OnDetached;
    }
    
    private void OnDetached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}