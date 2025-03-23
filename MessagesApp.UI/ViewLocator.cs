using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MessagesApp.UI.ViewModels;

namespace MessagesApp.UI;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var viewName = param.GetType().FullName!
            .Replace("ViewModels", "Views")
            .Replace("ViewModel", "View");

        var viewType = Type.GetType(viewName);
        if (viewType == null)
        {
            viewType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(viewName))
                .FirstOrDefault(t => t != null);
        }

        if (viewType != null)
        {
            return (Control)Activator.CreateInstance(viewType)!;
        }

        return new TextBlock { Text = "Not Found: " + viewName };
    }


    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}