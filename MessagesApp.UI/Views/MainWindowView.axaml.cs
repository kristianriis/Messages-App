using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MessagesApp.UI.ViewModels;

namespace MessagesApp.UI.Views
{
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();

            Initialized += MainWindowView_Initialized;
            PropertyChanged += MainWindowView_PropertyChanged;
        }

        private void MainWindowView_Initialized(object sender, EventArgs e)
        {
            GetWindowSize(this);
        }

        private void MainWindowView_PropertyChanged(object sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == ClientSizeProperty)
            {
                GetWindowSize(this);
            }
        }

        public void GetWindowSize(Window window)
        {
            if (window != null)
            {
                Size clientSize = window.ClientSize;
                double width = clientSize.Width;
                double height = clientSize.Height;
            }
        }
    }
}