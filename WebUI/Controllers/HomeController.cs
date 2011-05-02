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

            svnDetails.TrunckPath = Trunck;
            svnDetails.BranchPath = Branch;

            ViewData["TrunckPath"] = Trunck;
            ViewData["BranchPath"] = Branch;

            return View();
        }

        [HttpPost]
        public ActionResult GetSvnLog(SvnDetails svnDetails, string TrunckPath, string BranchPath)
        {
            svnDetails.BranchPath = BranchPath;
            svnDetails.TrunckPath = TrunckPath;
          
            var Log = new Svn(mSvnExecutablePath, svnDetails.TrunckPath, svnDetails.BranchPath);
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

            SetLoadingImage();

            return PartialView("ViewUserControl", svnDetails);
        }


        private void SetLoadingImage()
        {
            var images = new List<string>();
            foreach(var iamge in Directory.GetFiles(Server.MapPath(@"~\Content\LoadingImages")))
            {
                var imageFile = new FileInfo(iamge);
                images.Add(imageFile.Name);
            }

            var rand = new Random();
            var randNumber = rand.Next(0, images.Count()-1);
            ViewData["LoadingImage"] = "/Content/LoadingImages/" + images[randNumber];
        }

        [HttpPost]
        public ActionResult MergeSvnFiles(SvnDetails svnDetails, string SelectedRevisions)
        {
            var svnRepro = new Svn(mSvnExecutablePath, svnDetails.TrunckPath, svnDetails.BranchPath);
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

            //Set the image so another one is shown if merge some other files.
            SetLoadingImage();

            return PartialView("ValidationSummary");
        }
    }
}
