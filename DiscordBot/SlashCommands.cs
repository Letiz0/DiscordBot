using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Services;

namespace DiscordBot
{
    internal class SlashCommands : ApplicationCommandModule
    {
        public ImageService _service { private get; set; }

        [SlashCommand("p", "呼叫圖片")]
        public async Task AutocompleteCommand(InteractionContext ctx,
            [Autocomplete(typeof(AutocompleteProvider))]
            [Option("keyword", "keyword")] string imageId)
        {          
            var image = Bot.Images.First(x => x.Id == Convert.ToInt32(imageId));
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(image.Url));

            await _service.AddCalledCountAsync(image);
        }
    }
}
