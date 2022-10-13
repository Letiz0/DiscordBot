using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("p", "呼叫圖片")]
        public async Task AutocompleteCommand(InteractionContext ctx,
            [Autocomplete(typeof(AutocompleteProvider))]
            [Option("keyword", "keyword")] string imageId)
        {
            var result = Bot.Images.First(x => x.Id == Convert.ToInt32(imageId)).Url;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(result));
        }
    }
}
