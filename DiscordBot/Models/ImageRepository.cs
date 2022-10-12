using System;
using System.Collections.Generic;

namespace DiscordBot.Models
{
    public partial class ImageRepository
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string Keyword { get; set; } = null!;
        public int CalledCount { get; set; }
    }
}
