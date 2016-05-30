using DAL;
using ForumGrabber.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.XPath;

namespace ForumGrabber.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(CriteriaModel model)
        {

            model.Url = @"http://forum.onliner.by/viewtopic.php?t=1918091";
            //model.UserId = "173114";
            //model.SearchById = false;
            //model.Name = "tornado_77";

            int postsCount = 0;

            for (int i = 0; i < model.PagesCount && i < 1000; i++)
            {
                WebClient request = new WebClient();
                request.Encoding = Encoding.UTF8;
                string url = model.Url + "&start=" + (i * 20).ToString();
                string page = request.DownloadString(url);

                using (TextReader reader = new StringReader(page))
                {
                    HtmlDocument html = new HtmlDocument();
                    html.Load(reader);

                    HtmlNodeCollection nodes;

                    if (model.SearchById)
                    {
                        nodes = html.DocumentNode.SelectNodes(string.Format("//div[@data-user_id='{0}']", model.UserId));
                    }
                    else
                    {
                        nodes = html.DocumentNode.SelectNodes(string.Format("//a[@title='{0}']", model.Name));
                    }

                    
                    DAL.User user = null;
                    if (nodes != null)
                    {
                        foreach (var searchedNode in nodes)
                        {
                            HtmlNode node;

                            if (model.SearchById)
                            {
                                node = searchedNode;
                                model.Name = node.ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerHtml;
                            }
                            else
                            {
                                node = searchedNode.ParentNode.ParentNode.ParentNode.ParentNode;
                                if (node.Name == "tr")
                                {
                                    node = node.ParentNode.ParentNode.ParentNode.ParentNode;
                                }
                                model.UserId = node.Attributes["data-user_id"].Value;
                            }




                            string postId = node.ParentNode.Id.Substring(1);
                            var nodeDatetime = node.SelectSingleNode(string.Format("//small[@id='{0}']", postId));
                            string datetime = nodeDatetime.ChildNodes[3].InnerText;


                            if (user == null)
                            {
                                user = ForumManager.GetUserById(model.UserId);
                                if (user == null)
                                {
                                    user = ForumManager.CreateUser(model.UserId, model.Name);
                                }

                            }


                            string format = "d MMMM yyyy HH:mm";
                            CultureInfo provider = CultureInfo.CreateSpecificCulture("ru-RU");
                            DateTime dt = DateTime.ParseExact(datetime, format, provider);

                            var nodeMessage = node.SelectSingleNode(string.Format("//div[@id='message_{0}']", postId));
                            string content = nodeMessage.InnerHtml;

                            ForumManager.SavePost(user, postId, dt, content);
                            postsCount++;
                        }
                    }
                }
                Thread.Sleep(5000);
            }

            ViewBag.PostsCount = postsCount;
            return View();
        }
    }
}
