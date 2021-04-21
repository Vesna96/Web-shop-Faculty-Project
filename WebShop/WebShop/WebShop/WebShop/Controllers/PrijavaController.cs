using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class PrijavaController : Controller
    {
        // GET: Prijava

        public GraphClient client;
        public ActionResult Index()
        {
            if (Session["korisnik"] != null )
            {
                
                    return RedirectToAction("Indeks", "Korisnik", new { email = Session["email"].ToString() });
            }
            else
                return View();
            
        }

        [HttpPost]
        public ActionResult PrijaviSe(Korisnik korisnik)
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }
            string korisnikEmail = ".*" + korisnik.Email + ".*";
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("korisnikEmail", korisnikEmail);

            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Korisnik) and exists(n.Email) and n.Email =~ {korisnikEmail} return n",
                                                            queryDict, CypherResultMode.Set);
            List<Korisnik> logKorisnik = ((IRawGraphClient)client).ExecuteGetCypherResults<Korisnik>(query).ToList();

            if(logKorisnik.Count == 0) //Nije validna email adresa
            {
                return RedirectToAction("Index");
            }
            if(logKorisnik[0].Lozinka != korisnik.Lozinka) //Lozinka nije validna
            {
                return RedirectToAction("Index");
            }

            
           
            
            Session["email"] = korisnik.Email;
            


            return View("~/Views/Korisnik/Index.cshtml", logKorisnik[0]);
            
           
            
        }
    }
}