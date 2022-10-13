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
            List<DiscordAutoCompleteChoice> autocomplete = new();

            foreach (var item in Bot.Images.Where(x => x.Keyword.Contains(ctx.FocusedOption.Value.ToString())))
            {
                autocomplete.Add(new DiscordAutoCompleteChoice(item.Keyword, "" + item.Id));
            }
            
           return autocomplete;
        }
    }
}
