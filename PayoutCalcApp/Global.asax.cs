using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using PayoutCalcApp.Infrastructure;

namespace PayoutCalcApp
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //caching the dropdown for working hours
            Task.Factory.StartNew(HoursDropdownMapper.CacheHoursDropdown);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            Server.ClearError();
            Response.Redirect($"~/Error/PageErrorFound/?msg={lastError.Message}");
        }
    }
}
