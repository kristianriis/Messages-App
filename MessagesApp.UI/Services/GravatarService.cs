using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using MessagesApp.UI.Models;

namespace MessagesApp.UI.Services
{
    public interface IGravatarService
    {
        Task FillGravatars(List<Message> messages, int batchSize = 1);
    }
    
    public class GravatarService(HttpClient httpClient) : IGravatarService
    {
        //Gravatar cache for reuseability
        private readonly ConcurrentDictionary<string, Bitmap> _gravatarCache = new();
        
        //Accessible loading function for gravatars. Keeps breaking in deployed app if batchsize is higher than one, so restricted it here. 
        public async Task FillGravatars(List<Message> messages, int batchSize = 1)
        {
            var gravatarResults = new Dictionary<Message, Bitmap?>();

            for (int i = 0; i < messages.Count; i += batchSize)
            {
                var batch = messages.Skip(i).Take(batchSize)
                    .Select(async msg =>
                    {
                        try
                        {
                            var image = await LoadImageFromEmail(msg.Email);
                            gravatarResults[msg] = image;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load gravatar for {msg.Email}: {ex.Message}");
                            gravatarResults[msg] = null;
                        }
                    });

                await Task.WhenAll(batch);
                await Task.Delay(1); // Keeps batching logic
            }
            
            //Hacky cover up my slow loading of gravatars with a fade. 
            foreach (var pair in gravatarResults)
            {
                var msg = pair.Key;
                msg.GravatarImage = pair.Value;
                msg.GravatarOpacity = 0;
                _ = Task.Run(async () =>
                {
                    for (double opacity = 0.0; opacity <= 1.0; opacity += 0.01)
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            msg.GravatarOpacity = opacity;
                        });

                        await Task.Delay(1); 
                    }
                });
            }
        }

        private string GetGravatarUrl(string email, int size = 100)
        {
            if (string.IsNullOrWhiteSpace(email))
                return $"https://www.gravatar.com/avatar/00000000000000000000000000000000?s={size}&d=wavatar";

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(email.Trim().ToLower());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return $"https://www.gravatar.com/avatar/{sb}?s={size}&d=wavatar";
            }
        }

        private async Task<Bitmap> LoadImageFromEmail(string email, int size = 100)
        {
            string gravatarUrl = GetGravatarUrl(email, size);

            if (_gravatarCache.TryGetValue(email, out var cachedImage))
            {
                return cachedImage;
            }

            try
            {
                using var stream = await httpClient.GetStreamAsync(gravatarUrl);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var bitmap = new Bitmap(memoryStream);
                _gravatarCache.TryAdd(email, bitmap);

                return bitmap;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Couldn't load Gravatar image for {email}: {exception.Message}");
                return null;
            }
        }
    }
}