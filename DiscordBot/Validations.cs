using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class Validations
    {
        public static (bool, string) ImageValidation(string keyword, string url)
        {
            if (!(url.ToLower().Contains("jpg") || url.ToLower().Contains("png") || url.ToLower().Contains("gif") || url.ToLower().Contains("mp4")))
            {
                return (false, "只接受jpg, png, gif, mp4之連結");
            }

            if (Bot.Images.Select(x => x.Keyword).ToList().Contains(keyword))
            {
                return (false, "此關鍵字已存在於圖庫內");
            }

            if (Bot.Images.Select(x => x.Url).ToList().Contains(url))
            {
                return (false, "已存在此圖片, 關鍵字是 " + Bot.Images.Where(x => x.Url == url).FirstOrDefault()!.Keyword);
            }

            return (true, keyword + " 成功加入圖庫");
        }
    }
}
