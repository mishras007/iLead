using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityProject
{
    public class YouTubeData
    {
        public int YouTubeDataId { set; get; }
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string Descriptions { get; set; }
        public string ImageUrl { get; set; }
        public bool IsValid { get; set; }
    }
}
