using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services
{
    internal class ImageService
    {
        private readonly DiscordBotContext _context;

        public ImageService(DiscordBotContext context)
        {
            _context = context;
        }

        public async Task<List<ImageRepository>> GetAsync()
        {
            var result = await _context.ImageRepositories.ToListAsync();
            return result;
        }

        public async Task AddAsync(ImageRepository image)
        {
            _context.Add(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ImageRepository image)
        {
            _context.ImageRepositories.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task AddCalledCountAsync(ImageRepository image)
        {
            image.CalledCount++ ;
            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
