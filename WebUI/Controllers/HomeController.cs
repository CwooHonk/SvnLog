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
            if (!System.IO.File.Exists(mSvnExecutablePath))
            {
                ViewData["ErrorMessage"] = string.Format("SVN.exe could not be found at the following location: {0}. Check the location in the Web.Config.", mSvnExecutablePath);
                return View("Error");
            }

            svnDetails.TrunckPath = Trunck;
            svnDetails.BranchPath = Branch;

            ViewData["TrunckPath"] = Trunck;
            ViewData["BranchPath"] = Branch;

            return View();
        }

        public ActionResult GetSvnLog(SvnDetails svnDetails, string TrunckPath, string BranchPath)
        {
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

            svnDetails.BranchPhysicalLocation = Log.GetBranchLocation();

            return PartialView("ViewUserControl", svnDetails.Changes.OrderByDescending(a => a.Revision));
        }

        public ActionResult MergeSvnFiles(SvnDetails svnDetails, string SelectedRevisions)
        {
            var svnRepro = new Svn(mSvnExecutablePath, svnDetails.TrunckPath, svnDetails.BranchPath);
            var revisions = SelectedRevisions.Split(',').OrderBy(x => int.Parse(x));
            var RevisionRange = svnRepro.GetRevisionRange(svnDetails.Changes.Select(x => x.Revision.ToString()), revisions);

            try
            {
                svnRepro.MergeChanges(RevisionRange, svnDetails.BranchPhysicalLocation, svnDetails.BranchPath);
            }
            catch (Svn.SvnProcess.SvnException ex)
            {
                ViewData["ErrorMessage"] = ex.SvnError;
                return View("Error");
            }

            return null;
        }
    }
}
