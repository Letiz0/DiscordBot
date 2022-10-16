using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class AutocompleteProvider : IAutocompleteProvider
    {
        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            IEnumerable<DiscordAutoCompleteChoice> options;

            if (ctx.FocusedOption.Value.ToString() == "")
            {
                options = Bot.Images.Take(25).Select(x => new DiscordAutoCompleteChoice(x.Keyword, x.Id.ToString()));
            }
            else
            {
                options = Bot.Images.Where(x => x.Keyword.Contains(ctx.FocusedOption.Value.ToString()!))
                    .Select(x => new DiscordAutoCompleteChoice(x.Keyword, x.Id.ToString()));
            }

            return options;
        }
    }
}
