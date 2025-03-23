using System;
using System.Text.Json.Serialization;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MessagesApp.UI.Models;

public partial class Message : ObservableObject, IDisposable
{
    [ObservableProperty] 
    private string _sender = string.Empty;
    
    [ObservableProperty] 
    private string _email = string.Empty;
    
    [JsonIgnore]
    [ObservableProperty] 
    private Bitmap _gravatarImage; 

    [ObservableProperty]
    private string _subject = string.Empty;

    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private string _combinedSenderName = string.Empty;

    [ObservableProperty]
    private string _combinedOwnerName = string.Empty;

    [ObservableProperty] 
    private bool _hasBeenRead = false; 
    public DateTime? OpenedAt { get; set; }

    [ObservableProperty] 
    private double _gravatarOpacity = 0; 
    
    partial void OnSenderChanged(string value) => UpdateCombinedName();
    partial void OnEmailChanged(string value) => UpdateCombinedName();

    private void UpdateCombinedName()
    {
        CombinedSenderName = $"{Sender} <{Email}>"; 
    }
    
    public void Dispose()
    {
        GravatarImage?.Dispose();
        GravatarImage = null; 
    }
}