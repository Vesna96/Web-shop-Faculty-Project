﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           

            routes.MapRoute(
                name: "reg",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Registracija", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "dodajKorisnika",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Registracija", action = "DodajKorisnika", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "log",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Prijava", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
              name: "korM",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Korisnik", action = "Index", id = UrlParameter.Optional }
          );

            routes.MapRoute(
             name: "rutaaa",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Korisnik", action = "Prikazi", id = UrlParameter.Optional }
         );






        }
    }
}