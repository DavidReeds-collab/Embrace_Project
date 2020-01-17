using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace CVSImport.Controllers.Surface
{
    public class CVSImportController : SurfaceController
    {
        public ActionResult ImportCSV()
        {
            return Content("Done");
        }
    }
}