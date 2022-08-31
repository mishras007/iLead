using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityProject
{
    public class TwitterData
    {
        public int TwitterDataId { get; set; }
        public string ScreenName { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public int RetweetCount { get; set; }
        public int FavouritesCount { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ReplyaUrlId { get; set; }
        public string RetweetUrlId { get; set; }
        public string FavoriteUrlId { get; set; }
        public string IsValid { get; set; }
        public DateTime TimeStam { get; set; }
       
    }
}
