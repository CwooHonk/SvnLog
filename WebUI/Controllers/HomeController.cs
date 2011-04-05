using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using SvnDomainModel;

namespace WebUI.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {

        private string mSvnExecutablePath;

        public HomeController()
        {
            mSvnExecutablePath = ConfigurationManager.AppSettings["SvnPath"];
        }

        public ActionResult Index(string Trunck, string Branch)
        {
            ViewData["TrunckPath"] = Trunck;
            ViewData["BranchPath"] = Branch;

            return View();
        }

        public ActionResult GetSvnLog(List<Svn.LogEntry> changes, string TrunckPath, string BranchPath)
        {
            //TODO: Put this somewhere at start up.
            //if (!System.IO.File.Exists(mSvnExecutablePath))
            //{
            //    return string.Format("<div>Svn.exe could not be found at the specifiec location ({0}). Check the web.config.</div>", mSvnExecutablePath);
            //}
            var Log = new Svn(mSvnExecutablePath, TrunckPath, BranchPath);
            var stuff = Log.GetChanges();

            //Add each log to the class we are going to pass between sessions so we can keep a list of all the posibile revisions (dont want to rely on the client supplying it)
            foreach (var thingy in stuff)
                changes.Add(thingy);

            return PartialView("ViewUserControl", changes.OrderByDescending(a => a.Revision).ToList());
        }

        public ActionResult MergeSvnFiles(List<Svn.LogEntry> changes, string TrunckPath1, string BranchPath1, string FromRevision, string ToRevision)
        {
            return null;
        }
    }
}
