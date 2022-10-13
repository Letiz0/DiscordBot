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
        public async Task ChoiceProviderCommand(InteractionContext ctx,
            [ChoiceProvider(typeof(MyChoiceProvider))]
            [Option("關鍵字", "圖片之關鍵字")] string option)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(option));
        }
    }
}
