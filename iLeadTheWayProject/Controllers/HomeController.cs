using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Web.Services;
using EntityProject;
using EntityProject.Context;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json;
namespace iLeadTheWayProject.Controllers
{
    public class HomeController : Controller
    {
        public readonly LeadTheWay ObjEdbContext = new LeadTheWay();

        public ActionResult Home()
        {
            var qry = (from sa1 in ObjEdbContext.ObjPledgesTakens select sa1).FirstOrDefault();
            if (qry == null) return View();
            var count = Convert.ToInt64(qry.CountPledges);
            var number= Convert.ToDecimal(count).ToString("##,###");
            // ViewData["Count"] = number;
            ViewBag.Number = number;
            return View();
        }
        public JsonResult GetAllData(int AllcountTweet, int AllcountFacts, int AllcountVideo)
        {
            try
            {
                    //Twitter Data
                    var jsonReaderTweets = new StreamReader(Server.MapPath("~/JsonData/TwitterData.json"));
                    var rawJsonTweets = jsonReaderTweets.ReadToEnd();
                    jsonReaderTweets.Close();
                    var dtlsTweets = JsonConvert.DeserializeObject<List<TwitterData>>(rawJsonTweets);//Get List Of all Data Json File
                    var countTweet = dtlsTweets.Count();
                    var countDataTweet = countTweet - (3 * AllcountTweet);

                    //Json file Data retrieve Facts
                    var jsonReaderFacts = new StreamReader(Server.MapPath("~/JsonData/FactsData.json"));
                    var rawJsonFacts = jsonReaderFacts.ReadToEnd();
                    jsonReaderFacts.Close();
                    var dtlsFacts = JsonConvert.DeserializeObject<List<BlogPost>>(rawJsonFacts);//Get List Of all Data Json File
                    var countFacts = dtlsFacts.Count();
                    var countDataFacts = countFacts - (2 * AllcountFacts);

                    var jsonReaderVideo = new StreamReader(Server.MapPath("~/JsonData/YouTubeData.json"));
                    var rawJsonVideo = jsonReaderVideo.ReadToEnd();
                    jsonReaderVideo.Close();
                    var dtlVideos = JsonConvert.DeserializeObject<List<YouTubeData>>(rawJsonVideo);//Get List Of all Data Json File
                    var countVideo = dtlVideos.Count();
                    var countDataVideo = countVideo - (1 * AllcountVideo);

                    //var toTweetId = context.ObjTwitterDatas.FirstOrDefault().TwitterDataId;
                    //var toFactsId = context.Objblog.FirstOrDefault().BlogPostId;
                    //var toVideoId = context.ObjTubeDatas.FirstOrDefault().YouTubeDataId;

                    var queryYoutube = dtlVideos.Where(x => x.YouTubeDataId >= 1 && x.YouTubeDataId <= countDataVideo).OrderByDescending(x => x.YouTubeDataId);
                    var chkYoutubeCount = queryYoutube.Count();

                    var queryFacts = dtlsFacts.Where(x => x.BlogPostId >= 1 && x.BlogPostId <= countDataFacts).OrderByDescending(x => x.BlogPostId);
                    var chkFactsCount = queryFacts.Count();
                    if (chkYoutubeCount >= 1 && chkFactsCount >= 2)
                    {
                        var allDataYoutube = queryYoutube.Where(x => x.YouTubeDataId >= 1 && x.YouTubeDataId <= countDataVideo ).Take(1).OrderByDescending(x => x.YouTubeDataId).ToList();
                        var allDatafacts = queryFacts.Where(x => x.BlogPostId >= 1 && x.BlogPostId <= countDataFacts ).Take(2).OrderByDescending(x => x.BlogPostId).ToList();
                        var queryTweets = dtlsTweets.Where(x => x.TwitterDataId >= 1 && x.TwitterDataId <= countDataTweet).OrderByDescending(x => x.TwitterDataId);
                        var allDataTweet = queryTweets.Take(3).ToList();
                        var result = allDataTweet.Concat(allDataYoutube.Concat(allDatafacts.Cast<object>()));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (chkYoutubeCount < 1 && chkFactsCount >= 2)
                    {
                        var allDatafacts = queryFacts.Where(x => x.BlogPostId >= 1 && x.BlogPostId <= countDataFacts && x.IsSelected).Take(2).OrderByDescending(x => x.BlogPostId).ToList();
                        var queryTweets = dtlsTweets.Where(x => x.TwitterDataId >= 1 && x.TwitterDataId <= countDataTweet).OrderByDescending(x => x.TwitterDataId);
                        var allDataTweet = queryTweets.Take(4).ToList();
                        var result = allDataTweet.Concat(allDatafacts.Cast<object>());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (chkYoutubeCount <= 1 && chkFactsCount == 1)
                    {
                        var allDatafacts = queryFacts.Where(x => x.BlogPostId >= 1 && x.BlogPostId <= countDataFacts && x.IsSelected).Take(1).OrderByDescending(x => x.BlogPostId).ToList();
                        var queryTweets = dtlsTweets.Where(x => x.TwitterDataId >= 1 && x.TwitterDataId <= countDataTweet).OrderByDescending(x => x.TwitterDataId);
                        var allDataTweet = queryTweets.Take(5).ToList();
                        var result = allDataTweet.Concat(allDatafacts.Cast<object>());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (chkYoutubeCount < 1 && chkFactsCount == 0)
                    {
                        var queryTweets = dtlsTweets.Where(x => x.TwitterDataId >= 1 && x.TwitterDataId <= countDataTweet).OrderByDescending(x => x.TwitterDataId);
                        var allDataTweet = queryTweets.Take(6).ToList();
                        var result = allDataTweet.Cast<object>();
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else if (chkYoutubeCount == 1 && chkFactsCount == 1)
                    {
                        var allDatafacts = queryFacts.Where(x => x.BlogPostId >= 1 && x.BlogPostId <= countDataFacts).Take(1).OrderByDescending(x => x.BlogPostId).ToList();
                        var allDataYoutube = queryYoutube.Where(x => x.YouTubeDataId >= 1 && x.YouTubeDataId <= countDataVideo).Take(1).OrderByDescending(x => x.YouTubeDataId).ToList();
                        var queryTweets = dtlsTweets.Where(x => x.TwitterDataId >= 1 && x.TwitterDataId <= countDataTweet).OrderByDescending(x => x.TwitterDataId);
                        var allDataTweet = queryTweets.Take(4).ToList();
                        var result = allDataTweet.Concat(allDataYoutube.Concat(allDatafacts.Cast<object>()));
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var allDataYoutube = queryYoutube.Where(x => x.YouTubeDataId >= 1 && x.YouTubeDataId <= countDataVideo).Take(1).OrderByDescending(x => x.YouTubeDataId).ToList();
                        var queryTweets = dtlsTweets.Where(x => x.TwitterDataId >= 1 && x.TwitterDataId <= countDataTweet).OrderByDescending(x => x.TwitterDataId);
                        var allDataTweet = queryTweets.Take(5).ToList();
                        var result = allDataTweet.Concat(allDataYoutube.Cast<object>());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetTweetData(int tweetcount)
        {
            try
            {
                    //Json file Data retrieve 
                    var jsonReader = new StreamReader(Server.MapPath("~/JsonData/TwitterData.json"));
                    var rawJson = jsonReader.ReadToEnd();
                    jsonReader.Close();
                    var dtls = JsonConvert.DeserializeObject<List<TwitterData>>(rawJson);//Get List Of all Data Json File
                    var count = dtls.Count();
                    var countData = count - (6 * tweetcount);
                    var dataContainer =dtls.Where(x => x.TwitterDataId <= countData && x.TwitterDataId >= 1).OrderByDescending(x => x.TwitterDataId);
                    var dataContainer2 = dataContainer.Take(6).ToList();
                    return Json(dataContainer2, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception e)
            {
                return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetAllFacts(int factcount)
        {
            try
            {
                //Json file Data retrieve 
                var jsonReader = new StreamReader(Server.MapPath("~/JsonData/FactsData.json"));
                var rawJson = jsonReader.ReadToEnd();
                jsonReader.Close();
                var dtls = JsonConvert.DeserializeObject<List<BlogPost>>(rawJson);//Get List Of all Data Json File
                var count = dtls.Count();
                var countData = count - (6 * factcount);
                var dataContainer = dtls.Where(x => x.BlogPostId <= countData && x.BlogPostId >= 1).OrderByDescending(x => x.BlogPostId);
                var dataContainer2 = dataContainer.Take(6).ToList();
                return Json(dataContainer2, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAllVideo(int videocount)
        {
            try
            {
                //Json file Data retrieve 
                var jsonReader = new StreamReader(Server.MapPath("~/JsonData/YouTubeData.json"));
                var rawJson = jsonReader.ReadToEnd();
                jsonReader.Close();
                var dtls = JsonConvert.DeserializeObject<List<YouTubeData>>(rawJson);//Get List Of all Data Json File
                var count = dtls.Count();
                var countData = count - (6 * videocount);
                var dataContainer = dtls.Where(x => x.YouTubeDataId <= countData && x.YouTubeDataId >= 1).OrderByDescending(x => x.YouTubeDataId);
                var dataContainer2 = dataContainer.Take(6).ToList();
                return Json(dataContainer2, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        //Vidhyadhar Change
        public ActionResult DelpScheme()
        {
            ViewData["State"] = ObjEdbContext.ObjStates.ToList();
            return View();
        }
        [HttpPost]
        [WebMethod]
        public ActionResult GetCity(string stateId)
        {
            if (stateId == null) return Json(0);
            try
            {
                var qry = (from sa in ObjEdbContext.ObjCities where sa.StateName == stateId select sa).ToList();

                return Json(qry, JsonRequestBehavior.AllowGet);
                //return Json(qry);
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(0);
        }
        public JsonResult AddDetail(FormCollection form)
        {
            try
            {
               
                var objRegistrationUser = new RegistrationUser();
                var emailId = form["EmailId1"];
                var fName = form["FirstName1"];
                var lName = form["LastName1"];
                var mNumber = form["MobileNumber1"];
                var state = form["State"];
                var city = form["City"];
                var stateid = Convert.ToInt64(state);
                var stateName = "";
                var qry = (from s in ObjEdbContext.ObjStates where s.StateId == stateid select s).FirstOrDefault();
                if (qry != null)
                {
                    stateName = qry.StateName;
                }
                var checkPreeRegister = (from s in ObjEdbContext.ObjRegistrationUsers where s.EmailId == emailId && s.MobileNumber == mNumber select s).ToList();
                if (checkPreeRegister.Count != 0)
                {
                    return Json(0);
                }
                objRegistrationUser.FirstName = fName;
                objRegistrationUser.LastName = lName;
                objRegistrationUser.MobileNumber = mNumber;
                objRegistrationUser.State = stateName;
                objRegistrationUser.EmailId = emailId;
                objRegistrationUser.City = city;
                ObjEdbContext.ObjRegistrationUsers.AddOrUpdate(objRegistrationUser);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                return Json(1);
            }
            catch (Exception)
            {
                return Json(0);
            }
        }
       [WebMethod]
        public JsonResult PledgesTaken(string pubIp)
        {
            try
            {
                //Get Pladges Taken Id For Update
                var plgCount = (from sa1 in ObjEdbContext.ObjPledgesTakens select sa1).FirstOrDefault();
                if (plgCount != null)
                {
                    var count = Convert.ToInt64(plgCount.CountPledges);
                    var id = plgCount.PledgesTakenId;
                    //Check Cookies Create Or Not
                    var httpCookie1 = HttpContext.Request.Cookies["Pledges1"];
                    if (Session["TackPledges"] == null && httpCookie1 == null)
                    {
                        //Get list of Block Ip
                        var checkIpAddress = (from sa1 in ObjEdbContext.ObjiIpDetails where sa1.PublicIp == pubIp && sa1.IsBlock == true select sa1).ToList();
                        if (checkIpAddress.Count != 0)
                        {
                            return Json(count, JsonRequestBehavior.AllowGet);
                        }
                        //Create Cookie And Sessions.......
                        var cookie = new HttpCookie("Pledges1", "Pledges1") { Expires = DateTime.Now.AddDays(5) };
                        HttpContext.Response.Cookies.Add(cookie);
                        if (HttpContext.Request.Cookies["Pledges1"] != null)
                        {
                            var cookieValue = HttpContext.Request.Cookies["Pledges1"].Value;
                            Session["TackPledges"] = cookieValue;
                        }
                        //increment Counter
                        count++;
                        //Update Count in DataBase
                        var qry1 = (from sa1 in ObjEdbContext.ObjPledgesTakens where sa1.PledgesTakenId == id select sa1).FirstOrDefault();
                        if (qry1 != null)
                        {
                            qry1.CountPledges = count.ToString(CultureInfo.InvariantCulture);
                            if (!TryUpdateModel(qry1, new[] { "CountPledges" }))
                            {

                            }
                        }
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                        //Save IPAddress In Database
                        var objip = new IpDetail
                        {
                            PublicIp = pubIp,
                            PrivateIp = "Null",
                            DateTime = DateTime.Now,
                            IsBlock = false
                        };
                        ObjEdbContext.ObjiIpDetails.AddOrUpdate(objip);
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                        //fetched Counter In Database
                        var reFatchCount = (from sa1 in ObjEdbContext.ObjPledgesTakens select sa1).FirstOrDefault();
                        if (reFatchCount != null) count = Convert.ToInt64(reFatchCount.CountPledges);
                        return Json(count, JsonRequestBehavior.AllowGet);
                    }
                    var res = count;
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
    }
}