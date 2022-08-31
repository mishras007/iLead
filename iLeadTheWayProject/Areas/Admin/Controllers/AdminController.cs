using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using EntityProject;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using EntityProject.Context;
using iLeadTheWayProject.HelperClass;
using System.IO;
using LinqToTwitter;
using Newtonsoft.Json;
using TweetSharp;
using SearchType = System.Web.UI.WebControls.Expressions.SearchType;

namespace iLeadTheWayProject.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public readonly LeadTheWay ObjEdbContext = new LeadTheWay();
        public static string ConsumerKey = "5M2qaJomEdz9opOqFzjY3djmJ";
        public static string ConsumerSecret = "qNiJPBWASW3xligFitPYdqnG5cIxyZRpAc5TAe6Bd9yivwQEFG";
        public static string AccessToken = "540829809-i1dsuvhpf8RAoszndUHdytiFx5g5EDfk2pOL3yRi";
        public static string AccessTokenSecret = "P8t0V4bd8hFbyqXNMlyqaKunvw9Cdd4w7seFiMqGtnP5L";
        // GET: /Admin/Admin/
        public ActionResult Index()
        {
            try
            {
                // Check User Is Login Or Not if Not Login Redirect to Login Page
                if (Session["UserName"] == null) return RedirectToAction("Login");
                var activeTwittes = ObjEdbContext.ObjTwitterDatas.Count(x => x.IsValid == "Approval");
                ViewBag.activeTwittesCount = activeTwittes;
                var activeVidevo = ObjEdbContext.ObjTubeDatas.Count(x => x.IsValid);
                ViewBag.activeVideoCount = activeVidevo;
                var count = (ObjEdbContext.ObjRegistrations.Count());
                ViewBag.UserCount = count;
                var tcount = (ObjEdbContext.ObjTwitterDatas.Count());
                ViewBag.TwitterCount = tcount;
                var ucount = (ObjEdbContext.ObjTubeDatas.Count());
                ViewBag.YouTubeCount = ucount;
                return View(ObjEdbContext.ObjRegistrations.ToList());
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View("Login");
            }
        }

        //User Section
        public ActionResult LogIn()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LogIn(string userName, string password)
        {
            try
            {
                var query1 = (from s in ObjEdbContext.ObjRegistrations where s.UserName == userName select s).FirstOrDefault();
                if (query1 != null)
                {
                    var hashCode = query1.VCode;
                    //Password Hasing Process Call Helper Class Method
                    var encodingPasswordString = Helper.EncodePassword(password, hashCode);
                    //Check Login Detail User Name Or Password
                    var query = (from s in ObjEdbContext.ObjRegistrations where s.UserName == userName && s.Password.Equals(encodingPasswordString) select s).FirstOrDefault();
                    if (query != null)
                    {
                        Session["UserId"] = query.AdminDetailId;
                        Session["UserName"] = query.UserName;
                        Session["UserFirstName"] = query.FirstName;
                        Session["UserLastName"] = query.LastName;
                        Session["UserEmailId"] = query.EmailId;
                        //redirect Home Page
                        return RedirectToAction("GetTwitterData", "Admin");
                    }
                    ViewBag.ErrorMessage = " Invallid User Name or Password";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = " Error!!! Something went wrong" + e;
                return View();
            }
            return View();
        }
        //Logout Action
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            return RedirectToAction("LogIn");
        }
        //New User  Registration
        public ActionResult NewRegistration()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewRegistration(AdminDetail objUserRegistration)
        {
            try
            {
                //Check User Present Or Not
                var chkmail = (from s in ObjEdbContext.ObjRegistrations where s.UserName == objUserRegistration.UserName && s.EmailId == objUserRegistration.EmailId select s).FirstOrDefault();
                if (chkmail == null)
                {
                    //Genereate Verification Key
                    var keyNew = GeneratePassword(10);
                    var password = Helper.EncodePassword(objUserRegistration.Password, keyNew);
                    // Save Record Database
                    objUserRegistration.Password = password;
                    objUserRegistration.CreateDate = DateTime.Now;
                    objUserRegistration.ModifyDate = DateTime.Now;
                    objUserRegistration.VCode = keyNew;
                    ObjEdbContext.ObjRegistrations.AddOrUpdate(objUserRegistration);
                    ObjEdbContext.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("LogIn", "Admin");
                }
                ViewBag.ErrorMessage = "User Allredy Exixts!!!!!!!!!!";
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddVideo(YouTubeData objYoutube)
        {
            try
            {
                //var context 
                var id = objYoutube.VideoId; 
                const string str = "https://www.youtube.com/embed/";
                objYoutube.VideoId = str + id;
                objYoutube.Title = objYoutube.Title;
                objYoutube.ImageUrl = "//i.ytimg.com/vi/" + id + "/mqdefault.jpg";
                objYoutube.IsValid = false;
                //objYoutube.da
                ObjEdbContext.ObjTubeDatas.AddOrUpdate(objYoutube);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("GetYouTube", "Admin");
            }

            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View("GetYouTube");
            }
        }
        //Genrate New Randam Key For Password Or Send Email And Hash Key For Password
        public static string GeneratePassword(int length)
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var chars = new char[length];

            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        //Ajax Call Check UserName Exist Or Not For New Registration for New Registration Entry Form
        public JsonResult CheckName(FormCollection form)
        {
            try
            {
                var name = form["UserName"];
                var qry = (from s in ObjEdbContext.ObjRegistrations where s.UserName == name select s).FirstOrDefault();
                if (qry != null)
                {
                    return Json(1);
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(0);
        }
        //Ajax Call Check Email Id Exist Or Not For New Registration for New Registration Entry Form
        public JsonResult CheckEmail(FormCollection form)
        {
            try
            {
                var emailId = form["EmailId"];
                var qry = (from s in ObjEdbContext.ObjRegistrations where s.EmailId == emailId select s).FirstOrDefault();
                if (qry != null)
                {
                    return Json(1);
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(0);
        }
        [HttpPost]
        public ActionResult ForgetPass(string emailId)
        {
            try
            {
                var chkmail = (from s in ObjEdbContext.ObjRegistrations where s.EmailId == emailId select s).FirstOrDefault();
                if (chkmail != null)
                {
                    var userName = chkmail.UserName;
                    var name = chkmail.FirstName + " " + chkmail.LastName;
                    //var toEmail = chkmail.EmailId;
                    //Genereate Verification Key
                    var keyNew = GeneratePassword(10);
                    //EmailManager.SendConfirmationEmail(objNewUserRegister.EmailId);
                    var verifyUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Admin/Admin/ResetPassword/" + keyNew;
                    // Send Email Process
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 25,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential("vidhyadharga77@gmail.com", ""),
                        EnableSsl = true
                    };
                    using (var message = new MailMessage())
                    {
                        message.From = new MailAddress("vidhyadharga77@gmail.com");
                        message.Subject = "Please Reset your Password";
                        message.Body =
                            "<html><head><meta content=\"text/html; charset=utf-8\" /></head><body><p>Dear " + name +
                            ", </p><p>Your User Name<b> " + userName +
                            "</b> Go This Link And Type Email Id OR User Name Text Box Then Reset Your Password...</p><p>To Reset Your Password, please click the following link:</p>"
                            + "<p><a href=\"" + verifyUrl + "\"" + keyNew + "\" target=\"_blank\">" + verifyUrl + ""
                            + "</a></p><div>Best regards,</div><div>Inkswip</div><p>Do not forward "
                            + "this email. The verify link is private.</p></body></html>";
                        message.IsBodyHtml = true;
                        message.Priority = MailPriority.High;
                        message.To.Add(emailId);
                        //message.To.Add("vidhyadharga77@gmail.com");
                        //smtp.Send(message);
                    }
                    return RedirectToAction("LogIn", "Admin");
                }
                ViewBag.EmailMessage = "Please Enter Currect Email Id";
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ViewBag.EmailMessage = "Somthing Error" + e;
                return RedirectToAction("Login");
            }
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetPassword(UserRegistration objUserRegistration)
        {
            try
            {
                //Check User Present Or Not
                var queryUserName = (from s in ObjEdbContext.ObjRegistrations where s.UserName == objUserRegistration.UserName || s.EmailId == objUserRegistration.EmailId select s).FirstOrDefault();
                if (queryUserName == null)
                    return View();
                //var userName = objUserRegistration.UserName;
                //var toEmail = objUserRegistration.EmailId;
                //Genereate Verification Key
                var keyNew = GeneratePassword(10);
                var password = Helper.EncodePassword(objUserRegistration.Password, keyNew);
                // Save Record Database
                queryUserName.Password = password;
                queryUserName.ModifyDate = DateTime.Now;
                queryUserName.VCode = keyNew;
                ObjEdbContext.ObjRegistrations.AddOrUpdate(queryUserName);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        public ActionResult Viewuser(int id = 0)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                var user = ObjEdbContext.ObjRegistrations.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View("Index");
            }
        }
        //Edit Exixting User
        public ActionResult EditUser(int id = 0)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                var student = ObjEdbContext.ObjRegistrations.Find(id);
                if (student == null)
                {
                    return View("Index");
                }
                return View(student);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View("Index");
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditUser(AdminDetail objUserRegistration)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                if (!ModelState.IsValid)
                    return View("Index");
                objUserRegistration.ModifyDate = DateTime.Now;
                ObjEdbContext.Entry(objUserRegistration).State = EntityState.Modified;
                ObjEdbContext.ObjRegistrations.AddOrUpdate(objUserRegistration);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                ViewBag.successMessage = "Data has been updated succeessfully";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        public ActionResult DeleteUser(int id)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var user = ObjEdbContext.ObjRegistrations.Find(id);
                ObjEdbContext.ObjRegistrations.Remove(user);
                ObjEdbContext.SaveChanges();
                ViewBag.successMessage = "Data has been deleted succeessfully";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View("Index");
            }
        }
        public ActionResult ResetPasswordForUser()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ResetPasswordForUser(UserRegistration objUserRegistration)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                //Check User Present Or Not
                var queryUserName = (from s in ObjEdbContext.ObjRegistrations where s.UserName == objUserRegistration.UserName || s.EmailId == objUserRegistration.EmailId select s).FirstOrDefault();
                if (queryUserName == null)
                    return View();
                //Genereate Verification Key
                var keyNew = GeneratePassword(10);
                var password = Helper.EncodePassword(objUserRegistration.Password, keyNew);
                // Save Record Database
                queryUserName.Password = password;
                queryUserName.ModifyDate = DateTime.Now;
                queryUserName.VCode = keyNew;
                ObjEdbContext.ObjRegistrations.AddOrUpdate(queryUserName);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        //Twitter Tweets 
        public ActionResult GetTwitterData()
        {
            if (Session["UserName"] != null)
            {
                if (Session["UserName"] == null) return RedirectToAction("LogIn");
                var pendingdata = (from sa1 in ObjEdbContext.ObjTwitterDatas where sa1.IsValid == "Pending" select sa1);
                ViewData["Pending"] = pendingdata.ToList();
                return View();
            }
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult AddTwitterData(TwitterData objTwitterData)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var service = new TwitterService(ConsumerKey, ConsumerSecret);
                service.AuthenticateWith(AccessToken, AccessTokenSecret);
                //var tweets = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
                var tweets = service.Search(new SearchOptions { Q = "#iLedTheWay",Count = 100});
                //var status = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions { Count = 200 });
                // var tweet2 = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions { Count = 1000, MaxId = status.Last().Id });
                IEnumerable<TwitterStatus> status = tweets.Statuses;
                foreach (var item in status)
                {
                    var qry = (from s in ObjEdbContext.ObjTwitterDatas where s.Text == item.Text && s.ScreenName == item.User.ScreenName select s).FirstOrDefault();
                    if (qry != null) continue;
                    objTwitterData.ScreenName = item.User.ScreenName;
                    objTwitterData.UserName = item.User.Name;
                    objTwitterData.Text = item.Text;
                    objTwitterData.RetweetCount = item.RetweetCount;
                    objTwitterData.FavouritesCount = item.User.FavouritesCount;
                    objTwitterData.ProfileImageUrl = item.User.ProfileImageUrl;
                    objTwitterData.TimeStam = item.CreatedDate;
                    objTwitterData.RetweetUrlId = "https://twitter.com/intent/retweet?tweet_id=" + item.Id;
                    objTwitterData.ReplyaUrlId = "https://twitter.com/intent/tweet?in_reply_to=" + item.Id;
                    objTwitterData.FavoriteUrlId = "https://twitter.com/intent/favorite?tweet_id=" + item.Id;
                    objTwitterData.IsValid = "Pending";
                    ObjEdbContext.ObjTwitterDatas.Add(objTwitterData);
                    ObjEdbContext.SaveChanges();
                    ModelState.Clear();
                }
            }
            catch (Exception e)
            {

                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("GetTwitterData");
            }
            return RedirectToAction("GetTwitterData");
        }
        public ActionResult TwittesView(int id = 0)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login", "Admin");
                var user = ObjEdbContext.ObjTwitterDatas.Find(id);
                ViewData["TwitterData"] = user as TwitterData;
                return View(user);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        //YouTube Video Channel 
        public ActionResult GetYouTube()
        {
            try
            {
                if (Session["UserName"] != null)
                {
                    return View(ObjEdbContext.ObjTubeDatas.ToList());
                }
                return RedirectToAction("Login", "Admin");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("GetYouTube");
            }

        }
        public ActionResult Save(YouTubeData objYouTubeData)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyCT8kXaxJ2l29vYg4HBdYy36H-PhAH-Teg" });
                var channelsListRequest = yt.Channels.List("contentDetails");
                channelsListRequest.ForUsername = "kkrofficial";
                var channelsListResponse = channelsListRequest.Execute();
                foreach (var channel in channelsListResponse.Items)
                {
                    // of videos uploaded to the authenticated user's channel.
                    var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
                    var nextPageToken = "";
                    while (nextPageToken != null)
                    {
                        var playlistItemsListRequest = yt.PlaylistItems.List("snippet");
                        playlistItemsListRequest.PlaylistId = uploadsListId;
                        playlistItemsListRequest.MaxResults = 50;
                        playlistItemsListRequest.PageToken = nextPageToken;
                        // Retrieve the list of videos uploaded to the authenticated user's channel.
                        var playlistItemsListResponse = playlistItemsListRequest.Execute();
                        foreach (var playlistItem in playlistItemsListResponse.Items)
                        {
                            // Print information about each video.
                            //Console.WriteLine("Video Title= {0}, Video ID ={1}", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
                            var qry = (from s in ObjEdbContext.ObjTubeDatas where s.Title == playlistItem.Snippet.Title select s).FirstOrDefault();
                            if (qry == null)
                            {
                                objYouTubeData.VideoId = "https://www.youtube.com/embed/" + playlistItem.Snippet.ResourceId.VideoId;
                                objYouTubeData.Title = playlistItem.Snippet.Title;
                                objYouTubeData.Descriptions = playlistItem.Snippet.Description;
                                objYouTubeData.ImageUrl = playlistItem.Snippet.Thumbnails.High.Url;
                                objYouTubeData.IsValid = true;
                                ObjEdbContext.ObjTubeDatas.Add(objYouTubeData);
                                ObjEdbContext.SaveChanges();
                                ModelState.Clear();

                            }
                        }
                        nextPageToken = playlistItemsListResponse.NextPageToken;
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("GetYouTube");
            }

            return RedirectToAction("GetYouTube");
        }
        [HttpPost]
        public ActionResult UpdateIsValidYouTube(int[] youTubeId, int[] youTubeId1)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            if (youTubeId == null && youTubeId1 == null) return Json(0);
            try
            {
                // Chenge Staus For check
                if (youTubeId != null)
                    foreach (var s in youTubeId)
                    {
                        var qry = (from sa in ObjEdbContext.ObjTubeDatas where sa.YouTubeDataId == s select sa).FirstOrDefault();
                        if (qry == null)
                            return View("Index");
                        qry.IsValid = true;
                        if (!TryUpdateModel(qry, new[] { "IsValid" }))
                        {

                        }
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                    }
                // Chenge Staus For Uncheck
                foreach (var s1 in youTubeId1)
                {
                    var qry1 = (from sa1 in ObjEdbContext.ObjTubeDatas where sa1.YouTubeDataId == s1 select sa1).FirstOrDefault();
                    if (qry1 == null)
                        return View("Index");
                    qry1.IsValid = false;
                    if (!TryUpdateModel(qry1, new[] { "IsValid" }))
                    {

                    }
                    ObjEdbContext.SaveChanges();
                    ModelState.Clear();
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(1);
        }
        public ActionResult ViewBlog()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            ViewData["Blog"] = ObjEdbContext.Objblog.OrderByDescending(x => x.PostedOn).ToList();
            return View();
        }


        public ActionResult DetailBlog(int id)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                var blog = ObjEdbContext.Objblog.Find(id);
                ViewData["BlogData"] = blog as BlogPost;
                if (blog == null)
                {
                    return HttpNotFound();
                }
                return View(blog);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("ViewBlog");
            }
        }
        public ActionResult CreateBlog()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateBlog(BlogPost model, HttpPostedFileBase file)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var imagePath = "";
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName != null)
                    {
                        var path = Path.Combine(Server.MapPath("~/Areas/Admin/Uploads"), Path.GetFileName(file.FileName));
                        imagePath = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Areas/Admin/Uploads/" + file.FileName;
                        file.SaveAs(path);
                        ViewBag.ImagePath = imagePath;
                    }
                    //return (RedirectToAction("Index", new { message = ImagePath }));
                    model.PostedOn = DateTime.Now;
                    model.ImageUrl = imagePath;
                    model.IsSelected = true;
                    ObjEdbContext.Objblog.Add(model);
                    ObjEdbContext.SaveChanges();
                    ModelState.Clear();
                    ViewBag.HtmlContent = model.Content;

                    return View(model);
                }
                else
                {
                    model.PostedOn = DateTime.Now;
                    model.ImageUrl = imagePath;
                    model.IsSelected = true;
                    ObjEdbContext.Objblog.Add(model);
                    ObjEdbContext.SaveChanges();
                    ModelState.Clear();
                }
                ViewBag.HtmlContent = model.Content;

                return View(model);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();

            }
        }
        public ActionResult UpdateBlog(int id)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                var blog = ObjEdbContext.Objblog.Find(id);
                if (blog == null)
                {
                    return RedirectToAction("ViewBlog");
                }
                return View(blog);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("ViewBlog");
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult UpdateBlog(BlogPost model)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                if (!ModelState.IsValid)
                    return View("Index");
                model.PostedOn = DateTime.Now;
                ObjEdbContext.Entry(model).State = EntityState.Modified;
                ObjEdbContext.Objblog.AddOrUpdate(model);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                ViewBag.successMessage = "Data has been updated succeessfully";
                return RedirectToAction("ViewBlog");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        public ActionResult DeleteBlog(int id)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var blog = ObjEdbContext.Objblog.Find(id);
                ObjEdbContext.Objblog.Remove(blog);
                ObjEdbContext.SaveChanges();
                return RedirectToAction("ViewBlog");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("ViewBlog");
            }

        }
        [HttpPost]
        public ActionResult UpdateBlogIsValid(int[] blogData, int[] blogData1)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            if (blogData != null || blogData1 != null)
            {
                try
                {
                    // Chenge Staus For checked
                    if (blogData != null)
                        foreach (var s in blogData)
                        {
                            var qry = (from sa in ObjEdbContext.Objblog where sa.BlogPostId == s select sa).FirstOrDefault();
                            if (qry == null)
                                return RedirectToAction("ViewBlog");
                            qry.IsSelected = true;
                            if (!TryUpdateModel(qry, new[] { "IsSelected" }))
                            {

                            }
                            ObjEdbContext.SaveChanges();
                            ModelState.Clear();
                        }
                    // Chenge Staus For Unchecked
                    foreach (var s1 in blogData1)
                    {
                        var qry1 = (from sa1 in ObjEdbContext.Objblog where sa1.BlogPostId == s1 select sa1).FirstOrDefault();
                        if (qry1 == null)
                            return RedirectToAction("ViewBlog");
                        qry1.IsSelected = false;
                        if (!TryUpdateModel(qry1, new[] { "IsSelected" }))
                        {

                        }
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                    }
                }
                catch (Exception)
                {
                    return Json(0);
                }
                return Json(1);
            }
            return Json(0);
        }
        public ActionResult RegisterUser()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            return View(ObjEdbContext.ObjRegistrationUsers.ToList());
        }
        public ActionResult ExportData()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            var gv = new GridView { DataSource = ObjEdbContext.ObjRegistrationUsers.ToList() };
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Pre_RegisterUser.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("RegisterUser");
        }

        public ActionResult ExportDataIpAddress()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            var gv = new GridView { DataSource = ObjEdbContext.ObjiIpDetails.ToList() };
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=IPAddress_Detail.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("RegisterUser");
        }
        public ActionResult GetIpAddress()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            return View(ObjEdbContext.ObjiIpDetails.OrderByDescending(s => s.DateTime).ToList());
        }
        [HttpPost]
        public ActionResult BlocIpAddress(string[] ckeckipData)
        {
            if (ckeckipData == null) return Json(0);
            try
            {
                // Chenge Staus For check
                foreach (var s in ckeckipData)
                {
                    var qry = (from sa in ObjEdbContext.ObjiIpDetails where sa.PublicIp == s select sa).ToList();
                    foreach (var ip in qry)
                    {
                        ip.IsBlock = true;
                        if (!TryUpdateModel(qry, new[] { "IsBlock" }))
                        {

                        }
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                    }
                }
                return Json(1);
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(1);
        }
        [HttpPost]
        public ActionResult UnBlockIpAddress(string[] uncheckipData1)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                if (uncheckipData1 != null)
                {
                    foreach (var s1 in uncheckipData1)
                    {
                        var qry1 = (from sa1 in ObjEdbContext.ObjiIpDetails where sa1.PublicIp == s1 select sa1).ToList();
                        foreach (var ipDetail in qry1)
                        {
                            ipDetail.IsBlock = false;
                            if (!TryUpdateModel(qry1, new[] { "IsBlock" }))
                            {

                            }
                            ObjEdbContext.SaveChanges();
                            ModelState.Clear();
                        }

                    }
                    return Json(1);
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(1);
        }
        public ActionResult DeleteVideo(int id)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var video = ObjEdbContext.ObjTubeDatas.Find(id);
                ObjEdbContext.ObjTubeDatas.Remove(video);
                ObjEdbContext.SaveChanges();
                return RedirectToAction("ViewBlog");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("ViewBlog");
            }

        }
        public ActionResult GetCount()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            var blog = ObjEdbContext.ObjPledgesTakens.FirstOrDefault();
            return View(blog);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GetCount(PledgesTaken objPledgesTaken)
        {
            try
            {
                if (Session["UserName"] == null) return RedirectToAction("Login");
                if (!ModelState.IsValid)
                    return View("Index");

                ObjEdbContext.Entry(objPledgesTaken).State = EntityState.Modified;
                if (objPledgesTaken != null) ObjEdbContext.ObjPledgesTakens.AddOrUpdate(objPledgesTaken);
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                ViewBag.successMessage = "Data has been updated succeessfully";
                var blog = ObjEdbContext.ObjPledgesTakens.FirstOrDefault();
                return View(blog);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }
        public ActionResult ChangeTwitterStatus(int id)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                var getStatusid = (from sa1 in ObjEdbContext.ObjTwitterDatas where sa1.TwitterDataId == id select sa1).FirstOrDefault();
                if (getStatusid != null)
                {
                    var getStatus = getStatusid.IsValid;
                    if (getStatus == "Approval")
                    {
                        getStatusid.IsValid = "UnApproval";
                        if (!TryUpdateModel(getStatusid, new[] { "IsValid" }))
                        {

                        }
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                        return RedirectToAction("GetTwitterData");
                    }
                }
                if (getStatusid != null)
                {
                    getStatusid.IsValid = "Approval";
                    if (!TryUpdateModel(getStatusid, new[] { "IsValid" }))
                    {

                    }
                }
                ObjEdbContext.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("GetTwitterData");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return RedirectToAction("GetTwitterData");
            }

        }

        public ActionResult ApprovalTwitter()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            var data = (from sa1 in ObjEdbContext.ObjTwitterDatas where sa1.IsValid == "Approval" select sa1).OrderByDescending(x => x.TimeStam).ToList();
            ViewData["ApprovalData"] = data;
            return View();
        }
        public ActionResult UnApprovalTwitter()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            ViewData["UnApprovalData"] = (from sa1 in ObjEdbContext.ObjTwitterDatas where sa1.IsValid == "UnApproval" select sa1).OrderByDescending(x => x.TimeStam).ToList();
            return View();
        }
        public ActionResult UnApprovalTwitterStatus(int[] twitterData1)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                if (twitterData1 != null)
                {
                    foreach (var s1 in twitterData1)
                    {
                        var qry1 = (from sa1 in ObjEdbContext.ObjTwitterDatas where sa1.TwitterDataId == s1 select sa1).ToList();
                        foreach (var ipDetail in qry1)
                        {
                            ipDetail.IsValid = "UnApproval";
                            if (!TryUpdateModel(qry1, new[] { "IsBlock" }))
                            {

                            }
                            ObjEdbContext.SaveChanges();
                            ModelState.Clear();
                        }
                    }
                    return Json(1);
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(1);

        }

        public ActionResult ApprovalTwitterStatus(int[] twitterData)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                if (twitterData != null)
                {
                    foreach (var s1 in twitterData)
                    {
                        var qry1 = (from sa1 in ObjEdbContext.ObjTwitterDatas where sa1.TwitterDataId == s1 select sa1).ToList();
                        foreach (var ipDetail in qry1)
                        {
                            ipDetail.IsValid = "Approval";
                            if (!TryUpdateModel(qry1, new[] { "IsBlock" }))
                            {

                            }
                            ObjEdbContext.SaveChanges();
                            ModelState.Clear();
                        }
                    }
                    return Json(1);
                }
            }
            catch (Exception)
            {
                return Json(0);
            }
            return Json(1);
        }

        public ActionResult FatchTwitterDataDatewise()
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            return View();
        }
        [HttpPost]
        public ActionResult FatchTwitterDataDatewise(string d1, string d2)
        {
            if (Session["UserName"] == null) return RedirectToAction("LogIn");
            try
            {
                //var service = new TwitterService(ConsumerKey, ConsumerSecret);
                //service.AuthenticateWith(AccessToken, AccessTokenSecret);
                //var dt = DateTime.ParseExact(d2, "yyyy-mm-dd", CultureInfo.InvariantCulture);
                //var convertDateNext = dt.AddDays(1).ToString("yyyy-mm-dd");
                //service.IncludeRetweets = false;
                //var tweets = service.Search(new SearchOptions { Q = "#iLedTheWay since:" + d1 + " until:" + convertDateNext + "", Count = 100});
               
                //var status = tweets.Statuses;
                //var twitterStatuses = status as IList<TwitterStatus> ?? status.ToList();
                ////var count = twitterStatuses.Count();
                //var count = 0;
                string consumerKey1 = "NCzI0dO85Gorc8nHtVPih1zQQ";
                string consumerSecret1 = "VZXBiWI9kIBRzWbjJnvWkLwymQBxqGC2iJymRwucaIH3v5nwfu";
                string accessToken1 = "2612994457-wfkSuZW1KLxDk2f7Uc5EfmTHxxR2mD6oL57Ziym";
                string accessTokenSecret1 = "ZabpnYBSMDLUEEwu0hkHqUDcxH6V7bSKU6jDSyJETnpuS";
                string Query = "#iLedTheWay since:2015-11-23 until:2015-11-24";
                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore
                    {
                        ConsumerKey = consumerKey1,
                        ConsumerSecret = consumerSecret1,
                        OAuthToken = accessToken1,
                        OAuthTokenSecret = accessTokenSecret1
                    }
                };

                var context = new TwitterContext(auth);
                int count = 0;
                var searchResults =
                             (from search in context.Search where search.Type == LinqToTwitter.SearchType.Search && search.Query == Query && search.IncludeEntities == true && search.Count == 1000 select search).SingleOrDefault();
                if (searchResults != null)
                    foreach (var item in searchResults.Statuses)
                    {
                        //var qry = (from s in ObjEdbContext.ObjTwitterDatas where s.Text == item.Text && s.ScreenName == item.User.ScreenName select s).FirstOrDefault();
                        //if (qry != null) continue;
                        var objTwitterData = new TwitterData
                        {
                            ScreenName = item.User.ScreenNameResponse,
                            UserName = item.User.Name,
                            Text = item.Text,
                            RetweetCount = item.RetweetCount,
                            FavouritesCount = item.User.FavoritesCount,
                            ProfileImageUrl = item.User.ProfileImageUrl,
                            TimeStam = item.CreatedAt,
                            RetweetUrlId = "https://twitter.com/intent/retweet?tweet_id=" + item.User.UserIDResponse,
                            ReplyaUrlId = "https://twitter.com/intent/tweet?in_reply_to=" + item.User.UserIDResponse,
                            FavoriteUrlId = "https://twitter.com/intent/favorite?tweet_id=" + item.User.UserIDResponse,
                            IsValid = "Pending"
                        };
                        ObjEdbContext.ObjTwitterDatas.Add(objTwitterData);
                        ObjEdbContext.SaveChanges();
                        ModelState.Clear();
                    }
                //foreach (var item in twitterStatuses)
                //{
                //    var qry = (from s in ObjEdbContext.ObjTwitterDatas where s.Text == item.Text && s.ScreenName == item.User.ScreenName select s).FirstOrDefault();
                //    if (qry != null) continue;
                //    var objTwitterData = new TwitterData
                //    {
                //        ScreenName = item.User.ScreenName,
                //        UserName = item.User.Name,
                //        Text = item.Text,
                //        RetweetCount = item.RetweetCount,
                //        FavouritesCount = item.User.FavouritesCount,
                //        ProfileImageUrl = item.User.ProfileImageUrl,
                //        TimeStam = item.CreatedDate,
                //        RetweetUrlId = "https://twitter.com/intent/retweet?tweet_id=" + item.Id,
                //        ReplyaUrlId = "https://twitter.com/intent/tweet?in_reply_to=" + item.Id,
                //        FavoriteUrlId = "https://twitter.com/intent/favorite?tweet_id=" + item.Id,
                //        IsValid = "Pending"
                //    };
                //    ObjEdbContext.ObjTwitterDatas.Add(objTwitterData);
                //    ObjEdbContext.SaveChanges();
                //    ModelState.Clear();
                //    count = count + 1;
                //}
                ViewBag.SuccessMessage = count+" Rows inserted...";
                return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }

        public ActionResult UpdateJsonData()
        {
            var context = new LeadTheWay();

            //Create Json File Twitter
            var getTwitterDataList = context.ObjTwitterDatas.Where(x => x.IsValid == "Approval").ToList();
            var selectData = getTwitterDataList.AsEnumerable().Where(x => x.IsValid == "Approval").Select((obj, index) => new TwitterData
            {
                ScreenName = obj.ScreenName,
                ProfileImageUrl = obj.ProfileImageUrl,
                Text = obj.Text,
                UserName = obj.UserName,
                IsValid = obj.IsValid,
                RetweetCount = obj.RetweetCount,
                ReplyaUrlId = obj.ReplyaUrlId,
                RetweetUrlId = obj.RetweetUrlId,
                FavoriteUrlId = obj.FavoriteUrlId,
                FavouritesCount = obj.FavouritesCount,
                TimeStam = obj.TimeStam,
                TwitterDataId = index + 1
            });
            var bindTwitterData = selectData.ToList();
            var jsonString = JsonConvert.SerializeObject(bindTwitterData);
            if (jsonString != null)
            {
                if (!Directory.Exists(Server.MapPath("~/JsonData")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/JsonData"));
                }
            }
            System.IO.File.WriteAllText(Server.MapPath("~/JsonData/TwitterData.json"), jsonString);
            //End Of Append Dta For Json File
            //GetData YouTube 
            var getYouTubedata = context.ObjTubeDatas.Where(x => x.IsValid).ToList();
            var selectYouTubeData = getYouTubedata.AsEnumerable().Where(x => x.IsValid).Select((obj, index) => new YouTubeData()
            {
                VideoId = obj.VideoId,
                ImageUrl = obj.ImageUrl,
                Title = obj.Title,
                Descriptions = obj.Descriptions,
                IsValid = obj.IsValid,
                YouTubeDataId = index + 1
            });
            var bindYouTubeData = selectYouTubeData.ToList();
            var jsonString1 = JsonConvert.SerializeObject(bindYouTubeData);
            if (jsonString1 != null)
            {
                if (!Directory.Exists(Server.MapPath("~/JsonData")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/JsonData"));
                }
            }
            System.IO.File.WriteAllText(Server.MapPath("~/JsonData/YouTubeData.json"), jsonString1);
            //End Of Append Dta For Json File
            //AddData Facts
            var getFactsData = context.Objblog.Where(x => x.IsSelected).ToList();
            var selectFactData = getFactsData.AsEnumerable().Where(x => x.IsSelected).Select((obj, index) => new BlogPost()
            {
                Content = obj.Content,
                ImageUrl = obj.ImageUrl,
                PostedOn = obj.PostedOn,
                IsSelected = obj.IsSelected,
                BlogPostId = index + 1
            });
            var bindFactsData = selectFactData.ToList();
       
            var jsonString2 = JsonConvert.SerializeObject(bindFactsData);
            if (jsonString2 != null)
            {
                if (!Directory.Exists(Server.MapPath("~/JsonData")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/JsonData"));
                }
            }
            System.IO.File.WriteAllText(Server.MapPath("~/JsonData/FactsData.json"), jsonString2);
            return null;
        }
    }
}