using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform;
using MessagesApp.UI.Models;

namespace MessagesApp.UI.Backend
{
    public class MessageHandler
    {
        //File paths & internal storage
        private readonly string _defaultFilePath = "avares://MessagesApp.UI/Assets/Messages.json";
        private readonly string _userFilePath;
        private List<Message> _messages = new();
        private bool _isInitialized = false;

        public event EventHandler MessagesChanged;

        public MessageHandler()
        {
            _userFilePath = GetWritablePath();
            _ = LoadMessagesFromFileAsync(); // Async fire-and-forget init
        }

        //Gets the writable file path for saving messages
        private string GetWritablePath()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MessagesApp", "messages.json"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(appDataPath));
            return appDataPath;
        }

        //Loads messages from file
        public async Task LoadMessagesFromFileAsync()
        {
            try
            {
                Console.WriteLine($"Checking for user messages file: {_userFilePath}");

                if (File.Exists(_userFilePath))
                {
                    Console.WriteLine($"Loading user json from: {_userFilePath}");
                    string json = await File.ReadAllTextAsync(_userFilePath);

                    if (string.IsNullOrWhiteSpace(json) || !json.TrimStart().StartsWith("["))
                    {
                        Console.WriteLine("Json broken. Resetting file to default.");
                        File.Delete(_userFilePath);
                        await LoadDefaultMessagesAsync();
                        return;
                    }

                    _messages = JsonSerializer.Deserialize<List<Message>>(json) ?? new List<Message>();
                    Console.WriteLine($"Successfully loaded {_messages.Count} messages.");
                }
                else
                {
                    Console.WriteLine("User messages not found, loading default messages");
                    await LoadDefaultMessagesAsync();
                }

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Issue loading messages: {ex}");
            }
        }

        //Loads embedded default messages (fallback)
        private async Task LoadDefaultMessagesAsync()
        {
            try
            {
                using var stream = AssetLoader.Open(new Uri(_defaultFilePath));
                using var reader = new StreamReader(stream);
                string json = await reader.ReadToEndAsync();

                _messages = JsonSerializer.Deserialize<List<Message>>(json) ?? new List<Message>();
                await SaveMessagesToFileAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load default messages: {ex}");
                _messages = new List<Message>();
            }
        }

        //Saves current messages to disk
        public async Task SaveMessagesToFileAsync()
        {
            try
            {
                foreach (var message in _messages)
                {
                    message.GravatarImage = null; 
                }

                string json = JsonSerializer.Serialize(_messages, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_userFilePath, json);

                MessagesChanged?.Invoke(this, EventArgs.Empty);
                Console.WriteLine($"Saved {_messages.Count} messages to {_userFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save messages: {ex}");
            }
        }

        //Message retrieval
        public async Task<List<Message>> GetMessagesAsync()
        {
            if (!_isInitialized)
            {
                await LoadMessagesFromFileAsync();
            }

            return _messages;
        }

        //Adds a new message and saves
        public async Task AddMessageAsync(Message message)
        {
            _messages.Add(message);
            await SaveMessagesToFileAsync();
        }

        //Deletes message by email and saves
        public async Task<bool> DeleteMessageAsync(string email, string subject)
        {
            var message = _messages.FirstOrDefault(m => m.Email == email && m.Subject == subject);
            if (message == null)
            {
                Console.WriteLine($"Message with email '{email}' not found.");
                return false;
            }

            _messages.Remove(message);
            await SaveMessagesToFileAsync();
            return true;
        }
    }
}
