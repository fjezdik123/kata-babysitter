using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayoutCalcApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult PageErrorFound(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return Redirect("~/Payout");
            }
            return View((object)msg);
        }
    }
}