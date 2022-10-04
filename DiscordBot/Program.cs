using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using DiscordBot.Models;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using DiscordBot;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
               .AddUserSecrets<Program>()
               .Build();

var builder = new ServiceCollection()
    .AddSingleton<Bot>()
    .AddSingleton<ImageService>()
    .AddDbContext<DiscordBotContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DiscordBotDb")))    
    .BuildServiceProvider();

var bot = builder.GetRequiredService<Bot>();
bot.MainAsync().GetAwaiter().GetResult();



