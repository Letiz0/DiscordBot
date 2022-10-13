using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class MyChoiceProvider : IChoiceProvider
    {
        public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
        {
            return new DiscordApplicationCommandOptionChoice[]
            {
                new DiscordApplicationCommandOptionChoice("testing", "testing"),
                new DiscordApplicationCommandOptionChoice("testing2", "test option 2")
            };
        }
    }
}
