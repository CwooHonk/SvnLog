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

        public ActionResult Index(SvnDetails svnDetails, string Trunck, string Branch)
        {
            svnDetails.TrunckPath = Trunck;
            svnDetails.BranchPath = Branch;

            ViewData["TrunckPath"] = Trunck;
            ViewData["BranchPath"] = Branch;

            return View();
        }

        public ActionResult GetSvnLog(SvnDetails svnDetails, string TrunckPath, string BranchPath)
        {
            //TODO: Put this somewhere at start up.
            //if (!System.IO.File.Exists(mSvnExecutablePath))
            //{
            //    return string.Format("<div>Svn.exe could not be found at the specifiec location ({0}). Check the web.config.</div>", mSvnExecutablePath);
            //}

            if (svnDetails.BranchPath == null)
            {
                svnDetails.BranchPath = BranchPath;
                svnDetails.TrunckPath = TrunckPath;
            }

            var Log = new Svn(mSvnExecutablePath, svnDetails.TrunckPath, svnDetails.BranchPath);
            var AllChanges = Log.GetChanges();

            //Add each log to the class we are going to pass between sessions so we can keep a list of all the posibile revisions (dont want to rely on the client supplying it)
            svnDetails.Changes.Clear();
            foreach (var Change in AllChanges)
                svnDetails.Changes.Add(Change);

            return PartialView("ViewUserControl", svnDetails.Changes.OrderByDescending(a => a.Revision).ToList());
        }

        public ActionResult MergeSvnFiles(SvnDetails svnDetails, string SelectedRevisions)
        {
            return null;
        }
    }
}
