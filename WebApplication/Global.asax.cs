using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        // 간단한 에러페이지 추가
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string RedirectAction;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        RedirectAction = "NotFound";
                        break;
                    default:
                        RedirectAction = "General";
                        break;
                }
                
                Server.ClearError();

                Response.RedirectToRoute("Default", new { controller = "Error", action = RedirectAction });
            }
        }
    }
}