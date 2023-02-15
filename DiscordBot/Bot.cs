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
        public static List<ImageRepository> Images { get; set; } = new();

        private readonly string _token;
        private readonly ImageService _service;
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddUserSecrets<Bot>()
            .Build();

        public Bot(ImageService service)
        {
            _token = _configuration["DiscordBotToken"];
            _service = service;
        }

        public static void SortImagesByCalledCount()
        {
            Images.Sort((x, y) => y.CalledCount.CompareTo(x.CalledCount));
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

                if ((message.StartsWith("!加圖") || message.StartsWith("！加圖")) && message.Split(' ').Length == 3)
                {
                    string[] command = message.Split(' ');
                    var respond = await AddImageAsync(command[1], command[2]);
                    await e.Message.RespondAsync(respond);
                    await e.Message.RespondAsync("驚嘆號即將移除，請改為使用斜線指令/a " + $"<@!{e.Author.Id}>");
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(discord, 966394654402691094));

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

                //if(message == "!測試")
                //{
                //    var respond = "";

                //    foreach (var item in e.Guild.Emojis.Skip(75).Take(15))
                //    {
                //        respond += $"{item.Value}, {item.Key} \n";
                //    }                    

                //    await e.Message.RespondAsync(respond);
                //}

                //if (e.Author.Id == 112499724191313920)
                //{
                //    await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(discord, 966394654402691094));
                //}

                if (GetImage(message) is ImageRepository image)
                {
                    await e.Message.RespondAsync(image.Url);
                    await e.Message.RespondAsync("驚嘆號即將移除，請改為使用斜線指令/p " + $"<@!{e.Author.Id}>");
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(discord, 966394654402691094));
                    await _service.AddCalledCountAsync(image);
                    SortImagesByCalledCount();
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
