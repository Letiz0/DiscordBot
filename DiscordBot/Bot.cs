using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DiscordBot.Services;
using DiscordBot.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.CommandsNext;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot
{
    internal class Bot
    {
        private readonly string _token;
        private readonly ImageService _service;
        public static List<ImageRepository> Images { get; private set; } = new();
        IConfiguration _configuration = new ConfigurationBuilder()
            .AddUserSecrets<Bot>()
            .Build();

        public Bot(ImageService service)
        {

            _token = _configuration["DiscordBotToken"];
            _service = service;
        }

        public async Task MainAsync()
        {
            Images = await _service.GetAsync();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            discord.MessageCreated += async (s, e) =>
            {
                var message = e.Message.Content;

                if (e.Message.Author.IsBot)
                {
                    return;
                }

                if (message == "!B哥")
                {
                    await e.Message.RespondAsync("<@!346117587509313536>");
                    return;
                }

                if (message == "!說要加薪")
                {
                    await e.Message.RespondAsync("也沒加 也不是儲備幹部的薪水 說有業續達標 績效獎金 戶頭看還是31000 講的跟做的都一套還信誓旦旦跟我說說到做到 沒差 我月底走人也是說到做到 每天花1個小時 每天下來白工");
                    return;
                }

                if (message == "!不夠愛")
                {
                    await e.Message.RespondAsync("裝備錢是我花的 飾品也是我送的 書也是我給的 金牌也是我打的 時裝也是我花錢 每天周末就被當透明人 然後還被嫌不夠愛");
                    return;
                }

                if ((message.StartsWith("!加圖") || message.StartsWith("！加圖")) && message.Split(' ').Length == 3)
                {
                    string[] command = message.Split(' ');
                    var respond = await AddImageAsync(command[1], command[2]);
                    await e.Message.RespondAsync(respond);

                    return;
                }

                if (message == "!圖庫" || message == "！圖庫")
                {
                    await e.Message.RespondAsync(GetAllKeywords());
                    return;
                }

                if ((message.StartsWith("!刪圖") || message.StartsWith("！刪圖")) && message.Split(' ').Length == 2)
                {
                    string[] command = message.Split(' ');
                    var respond = await DeleteImageAsync(command[1]);
                    await e.Message.RespondAsync(respond);

                    return;
                }

                if (GetImage(message) is ImageRepository image)
                {
                    await _service.AddCalledCountAsync(image);
                    await e.Message.RespondAsync(image.Url);
                }

            };

            var services = new ServiceCollection()
                .AddSingleton<ImageService>()
                .AddDbContext<DiscordBotContext>(options =>
                    options.UseSqlServer(_configuration.GetConnectionString("DiscordBotDb")))
                .BuildServiceProvider();

            var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = services
            });

            slash.RegisterCommands<SlashCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private ImageRepository? GetImage(string message)
        {
            if (!(message.Contains('!') || message.Contains('！')))
            {
                return null;
            }

            var image = Images.Where(x => "!" + x.Keyword == message || "！" + x.Keyword == message)
                .FirstOrDefault();

            return image;
        }

        private async Task<string> AddImageAsync(string keyword, string url)
        {
            if (Images.Select(x => x.Keyword).ToList().Contains(keyword))
            {
                return "此關鍵字已存在於圖庫內";
            }

            if (keyword.StartsWith("http"))
            {
                return "格式反了==";
            }

            if (!(url.ToLower().Contains("jpg") || url.ToLower().Contains("png") || url.ToLower().Contains("gif") || url.ToLower().Contains("mp4")))
            {
                return "只接受jpg, png, gif, mp4之連結";
            }

            if (Images.Select(x => x.Url).ToList().Contains(url))
            {
                return "已存在此圖片, 關鍵字是 " + Images.Where(x => x.Url == url).FirstOrDefault()!.Keyword;
            }

            ImageRepository image = new ImageRepository
            {
                Keyword = keyword,
                Url = url
            };

            await _service.AddAsync(image);
            Images = await _service.GetAsync();

            return keyword + " 成功加入圖庫";
        }

        private async Task<string> DeleteImageAsync(string keyword)
        {
            if (!Images.Select(x => x.Keyword).ToList().Contains(keyword))
            {
                return "此關鍵字不在圖庫內";
            }

            var image = Images.Where(x => x.Keyword == keyword).FirstOrDefault();
            await _service.DeleteAsync(image!);
            Images = await _service.GetAsync();

            return keyword + " 已成功刪除";
        }

        private string GetAllKeywords()
        {
            string keywords = "";

            foreach (var image in Images)
            {
                keywords += image.Keyword + "\n";
            }

            return keywords;
        }
    }
}
