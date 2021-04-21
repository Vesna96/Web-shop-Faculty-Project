using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class KorisnikMController : Controller
    {
        // GET: KorisnikM
        public ActionResult Index() 
        {
            if (Session["email"] == null)
                return RedirectToAction("PrijaviSe", "Prijava");
            else
                return View();
        }

        public ActionResult OdjaviSe()
        {
            return RedirectToAction("Index","Prijava");
        }
    }
}