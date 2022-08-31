using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityProject;
using EntityProject.Context;
using iLeadTheWayProject.Areas.Admin.Controllers;
using Microsoft.Ajax.Utilities;
using TweetSharp;

namespace iLeadTheWayProject.Controllers
{
    public class DataController : Controller
    {
        public readonly LeadTheWay ObjEdbContext = new LeadTheWay();
        //
        // GET: /Data/
        public ActionResult Index()
        {
            return View();
        }
        // http://localhost:44930/Data/FatchTwitterData
        public ActionResult FatchTwitterData()
        {
            try
            {
                var service = new TwitterService(AdminController.ConsumerKey,AdminController.ConsumerSecret);
                service.AuthenticateWith(AdminController.AccessToken, AdminController.AccessTokenSecret);
                //var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
                var tweets = service.Search(new SearchOptions { Q = "#iLedTheWay", Count = 180 });
                
                //var status = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions { Count = 200 });
                // var tweet2 = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions { Count = 1000, MaxId = status.Last().Id });

                var status = tweets.Statuses;
                var twitterStatuses = status as IList<TwitterStatus> ?? status.ToList();
                //var count = twitterStatuses.Count();
                var count = 0;
                foreach (var item in twitterStatuses)
                {
                    var qry = (from s in ObjEdbContext.ObjTwitterDatas where s.Text == item.Text && s.ScreenName == item.User.ScreenName select s).FirstOrDefault();
                    if (qry != null) continue;
                    var objTwitterData = new TwitterData
                    {
                        ScreenName = item.User.ScreenName,
                        UserName = item.User.Name,
                        Text = item.Text,
                        RetweetCount = item.RetweetCount,
                        FavouritesCount = item.User.FavouritesCount,
                        ProfileImageUrl = item.User.ProfileImageUrl,
                        TimeStam = item.CreatedDate,
                        RetweetUrlId = "https://twitter.com/intent/retweet?tweet_id=" + item.Id,
                        ReplyaUrlId = "https://twitter.com/intent/tweet?in_reply_to=" + item.Id,
                        FavoriteUrlId = "https://twitter.com/intent/favorite?tweet_id=" + item.Id,
                        IsValid ="Pending"
                    };
                    ObjEdbContext.ObjTwitterDatas.Add(objTwitterData);
                    ObjEdbContext.SaveChanges();
                    ModelState.Clear();
                    count = count + 1;
                }
                ViewBag.SuccessMessage = count + " Rows inserted...";
                return View("Index");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Some exception occured";
                return View("Index");
            }
        }

      
	}
}