using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityProject
{
   public class IpDetail
    {
       public int IpDetailId { get; set; }
       public string PublicIp { get; set; }
       public string PrivateIp { get; set; }
       public DateTime DateTime { get; set; }
       public bool IsBlock { get; set; }
    }
}
