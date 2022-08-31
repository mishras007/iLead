using System.Data.Entity;
namespace EntityProject.Context
{
   public class LeadTheWay:DbContext
    {
       public DbSet<AdminDetail> ObjRegistrations { get; set; }
       public DbSet<YouTubeData> ObjTubeDatas { get; set; }
       public DbSet<TwitterData> ObjTwitterDatas { get; set; }
       public DbSet<BlogPost> Objblog { get; set; }
       public DbSet<City> ObjCities { get; set; }
       public DbSet<State> ObjStates { get; set; }
       public DbSet<RegistrationUser> ObjRegistrationUsers { get; set; }
       public DbSet<PledgesTaken> ObjPledgesTakens { get; set; }
       public DbSet<IpDetail> ObjiIpDetails { get; set; }
    }
}
