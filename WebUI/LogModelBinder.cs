using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SvnDomainModel;

namespace WebUI
{
    public class LogModelBinder : IModelBinder
    {
        private const string SvnLogKey = "_SvnLogList";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.Model != null)
                throw new InvalidOperationException("Cannot update instances");

            var LogMessags = (SvnDetails)controllerContext.HttpContext.Session[SvnLogKey];
            if (LogMessags == null)
            {
                LogMessags = new SvnDetails();
                controllerContext.HttpContext.Session[SvnLogKey] = LogMessags;
            }

            return LogMessags;
        }
    }
}