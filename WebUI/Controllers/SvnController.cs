using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SvnDomainModel;

namespace WebUI.Controllers
{
    public class SvnController : Controller
    {
        //
        // GET: /Svn/
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SetDetails()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SetDetails(SvnDetails svnDetails, string SvnUser, string SvnPassword)
        {
            var aCookie = new HttpCookie("SvnInfo");
            aCookie.Values["SvnUser"] = SvnUser;
            aCookie.Values["SvnPassword"] = SvnPassword;
            aCookie.Expires = DateTime.Now.AddYears(10);
            aCookie.HttpOnly = true;
            Response.Cookies.Add(aCookie);

            svnDetails.SvnUser = SvnUser;
            svnDetails.SvnPassword = SvnPassword;

            return Redirect("~");
        }

    }
}
