using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DiscordBot.Services;
using DiscordBot.Models;

namespace DiscordBot
{
    internal class SlashCommands : ApplicationCommandModule
    {
        public ImageService? Service { private get; set; }

        [SlashCommand("p", "呼叫圖片")]
        public async Task GetImageCommand(InteractionContext ctx,
            [Autocomplete(typeof(ImagesAutocompleteProvider))]
            [Option("keyword", "關鍵字")] string imageId)
        {          
            var image = Bot.Images.First(x => x.Id == Convert.ToInt32(imageId));
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(image.Url));

            await Service!.AddCalledCountAsync(image);
            Bot.SortImagesByCalledCount();
        }

        

        [SlashCommand("a", "增加圖片")]
        public async Task AddImageCommand(InteractionContext ctx,
            [Option("keyword", "關鍵字")] string keyword,
            [Option("url", "圖片網址")] string url)
        {            
            (bool valid, string message) = Validations.ImageValidation(keyword, url);

            var respond = ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));

            if (!valid)
            {
                await respond;
                return;
            }

            ImageRepository image = new()
            {
                Keyword = keyword,
                Url = url
            };

            await Service!.AddAsync(image);
            Bot.Images = await Service.GetAsync();

            await respond;
        }

        [SlashCommand("d", "移除圖片")]
        public async Task RemoveImageCommand(InteractionContext ctx,
            [Autocomplete(typeof(ImagesAutocompleteProvider))]
            [Option("keyword", "關鍵字")] string keyword)
        {
            
        }
    }
}
