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

        [HttpGet]
        public ActionResult Index(SvnDetails svnDetails, string Trunck, string Branch)
        {
            if (!System.IO.File.Exists(mSvnExecutablePath))
            {
                ViewData["ErrorMessage"] = string.Format("SVN.exe could not be found at the following location: {0}. Check the location in the Web.Config.", mSvnExecutablePath);
                return View("Error");
            }

            var requestSvnCookie = Request.Cookies[SvnDetails.SvnCookieName];
            if (requestSvnCookie == null || svnDetails.SvnUser == string.Empty)
            {
                Response.Redirect(Url.Action("SetDetails", "Svn"));
            }
            else
            {
                if (svnDetails.SvnUser == null)
                {
                    svnDetails.SvnUser = requestSvnCookie[SvnDetails.SvnCookieUserName];
                    svnDetails.SvnPassword = requestSvnCookie[SvnDetails.SvnCookieUserPassword];
                }
            }

            svnDetails.TrunckPath = Trunck;
            svnDetails.BranchPath = Branch;

            ViewData["TrunckPath"] = Trunck;
            ViewData["BranchPath"] = Branch;
            ViewData["SvnUser"] = svnDetails.SvnUser;

            return View();
        }

        [HttpPost]
        public ActionResult GetSvnLog(SvnDetails svnDetails, string TrunckPath, string BranchPath)
        {
            svnDetails.BranchPath = BranchPath;
            svnDetails.TrunckPath = TrunckPath;
          
            var Log = new Svn(mSvnExecutablePath, svnDetails.TrunckPath, svnDetails.BranchPath, svnDetails.SvnUser, svnDetails.SvnPassword);
            var AllChanges = new List<Svn.LogEntry>();

            try
            {
                AllChanges = Log.GetChanges();
            }
            catch (SvnProcess.SvnException ex)
            {
                foreach (var error in ex.SvnError)
                    ModelState.AddModelError("", error);
                ModelState.AddModelError("", ex.Command);
            }

            //Add each log to the class we are going to pass between sessions so we can keep a list of all the posibile revisions (dont want to rely on the client supplying it)
            svnDetails.Changes.Clear();
            foreach (var Change in AllChanges)
                svnDetails.Changes.Add(Change);

            return PartialView("ViewUserControl", svnDetails);
        }

        [HttpPost]
        public ActionResult MergeSvnFiles(SvnDetails svnDetails, string SelectedRevisions)
        {
            var svnRepro = new Svn(mSvnExecutablePath, svnDetails.TrunckPath, svnDetails.BranchPath, svnDetails.SvnUser, svnDetails.SvnPassword);
            var revisions = SelectedRevisions.Split(',').OrderBy(x => int.Parse(x));

            try
            {
                svnRepro.MergeChanges(svnDetails, revisions);
            }
            catch (SvnProcess.SvnException ex)
            {
                foreach(var error in ex.SvnError)
                    ModelState.AddModelError("", error);
                ModelState.AddModelError("", ex.Command);
            }

            return PartialView("ValidationSummary");
        }
    }
}
