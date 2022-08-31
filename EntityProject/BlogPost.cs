using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  System.Web;
using System.Web.Mvc;

namespace EntityProject
{
  public class BlogPost
    {
        public int BlogPostId { get; set; }
        public DateTime PostedOn { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public bool IsSelected { get; set; }
    }
}
