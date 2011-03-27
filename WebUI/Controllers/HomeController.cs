using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;

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

        public ActionResult GetSvnLog(string TrunckPath, string BranchPath)
        {
            //TODO: Put this somewhere at start up.
            //if (!System.IO.File.Exists(mSvnExecutablePath))
            //{
            //    return string.Format("<div>Svn.exe could not be found at the specifiec location ({0}). Check the web.config.</div>", mSvnExecutablePath);
            //}
            var Log = new SvnDomainModel.Svn(mSvnExecutablePath, TrunckPath, BranchPath);
            var output = Log.GetChanges();

            return PartialView("ViewUserControl", output.OrderByDescending(a => a.Revision).ToList());
        }

        public ActionResult MergeSvnFiles(string TrunckPath1, string BranchPath1, string FromRevision, string ToRevision)
        {
            return null;
        }
    }
}
