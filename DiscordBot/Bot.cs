using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Services;
using DiscordBot.Models;

namespace DiscordBot
{
    internal class Bot
    {
        private readonly ImageService _service;
        List<ImageRepository> _images = new();

        public Bot(ImageService service)
        {
            _service = service;            
        }

        public async Task MainAsync()
        {
            _images = await _service.GetAsync();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MTAyNTM3MzMxNzExODU2NjQ4Mg.GGQRPP.XcdTmN9vJgYKpGBgGiMjUYof7bZEkjvfNHzfcU",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Author.IsBot)
                {
                    return;
                }

                if (e.Message.Content.StartsWith("!加圖") && e.Message.Content.Split(' ').Length == 3)
                {
                    string[] command = e.Message.Content.Split(' ');
                    var respond = await AddImageAsync(command[1], command[2]);
                    await e.Message.RespondAsync(respond);

                    return;
                }

                if (e.Message.Content == "!圖庫")
                {
                    await e.Message.RespondAsync(GetAllKeywords());
                    return;
                }

                if (e.Message.Content.StartsWith("!刪圖") && e.Message.Content.Split(' ').Length == 2)
                {
                    string[] command = e.Message.Content.Split(' ');
                    var respond = await DeleteImageAsync(command[1]);
                    await e.Message.RespondAsync(respond);

                    return;
                }

                if (GetImage(e.Message.Content) != null)
                {
                    var image = GetImage(e.Message.Content);
                    await e.Message.RespondAsync(image);
                }
                
            };
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private string? GetImage(string message)
        {
            if (!message.Contains('!'))
            {
                return null;
            }

            foreach (var image in _images)
            {
                if (message.Contains("!" + image.Keyword))
                {
                    return image.Url;
                }
            }

            return null;
        }

        private async Task<string> AddImageAsync(string keyword, string url)
        {
            if (_images.Select(x => x.Keyword).ToList().Contains(keyword))
            {
                return "此關鍵字已存在於圖庫內";
            }

            if (keyword.StartsWith("http"))
            {
                return "格式反了==";
            }

            if (!(url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".gif")))
            {
                return "只接受jpg, png, gif結尾之連結";
            }

            ImageRepository image = new ImageRepository
            { 
                Keyword = keyword,
                Url = url
            };

            await _service.AddAsync(image);
            _images = await _service.GetAsync();

            return keyword + " 成功加入圖庫";
        }

        private async Task<string> DeleteImageAsync(string keyword)
        {
            if (!_images.Select(x => x.Keyword).ToList().Contains(keyword))
            {
                return "此關鍵字不在圖庫內";
            }

            var image = _images.Where(x => x.Keyword == keyword).FirstOrDefault();
            await _service.DeleteAsync(image!);
            _images = await _service.GetAsync();

            return keyword + " 已成功刪除";
        }

        private string GetAllKeywords()
        {
            string keywords = "";

            foreach (var image in _images)
            {
                keywords += image.Keyword + "\n";
            }

            return keywords;
        }
    }
}
