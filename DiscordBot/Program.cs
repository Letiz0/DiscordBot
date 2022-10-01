using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Models;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using DiscordBot;

var builder = new ServiceCollection()
    .AddSingleton<Bot>()
    .AddSingleton<ImageService>()
    .AddDbContext<DiscordBotContext>(options =>
        options.UseSqlServer("Data Source=.;Initial Catalog=DiscordBot;Trusted_Connection=True;"))
    .BuildServiceProvider();

var bot = builder.GetRequiredService<Bot>();
bot.MainAsync().GetAwaiter().GetResult();



