using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace WebShop.Controllers
{
    public class KorisnikController : Controller
    {
        public GraphClient client;
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

        public ActionResult Filtriraj(String pol,String Elegantni,String Sportski, String Poslovni,String Casual,
            String Rock,String Orsay,String TomTaylor,String Bata,String Nike,String Adidas,String Kappa,String cenaOd,
            String cenaDo,String odeca,String obuca,String aksesoari,String Street,String Email)
        {
            Pretraga pretraga= new Pretraga();

            pretraga.Pol = pol;
            pretraga.CenaOd = float.Parse(cenaOd);
            pretraga.CenaDo = float.Parse(cenaDo);
            pretraga.Odeca = odeca;
            pretraga.Obuca = obuca;
            pretraga.Aksesoar = aksesoari;
            if (Elegantni == "true")
                pretraga.Stilovi.Add("Elegantni");
            if (Sportski == "true")
                pretraga.Stilovi.Add("Sportski");
            if (Casual == "true")
                pretraga.Stilovi.Add("Casual");
            if (Poslovni == "true")
                pretraga.Stilovi.Add("Poslovni");
            if (Street == "true")
                pretraga.Stilovi.Add("Street");
            if (Rock == "true")
                pretraga.Stilovi.Add("Rock");
           

            if (Orsay == "true")
                pretraga.Marke.Add("Orsay");
            if (TomTaylor == "true")
                pretraga.Marke.Add("Tom Taylor");
            if (Nike == "true")
                pretraga.Marke.Add("Nike");
            if (Adidas == "true")
                pretraga.Marke.Add("Adidas");
            if (Bata == "true")
                pretraga.Marke.Add("Bata");
            if (Kappa == "true")
                pretraga.Marke.Add("Kappa");
            pretraga.Email = Email;

            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
             try
                {
                    client.Connect();
               }
               catch (Exception ex)
                {

               }

            int CenaOd = int.Parse(cenaOd);
            int CenaDo = int.Parse(cenaDo);
            string queryStr = "";
            if(Elegantni == "false" && Sportski == "false" && Casual == "false" && Street == "false" && Poslovni == "false" && Rock == "false")
            {
                queryStr += "start n = node(*) where(n: Proizvod) and exists(n.Pol) and n.Pol = ~ { pol } ";
                if (Orsay != null)
                    queryStr += "and n.Marka  = ~ { Orsay } ";
                if (TomTaylor != null)
                    queryStr += "and n.Marka  = ~ { TomTaylor } ";
                if (Bata != null)
                    queryStr += "and n.Marka  = ~ { Bata } ";
                if (Nike != null)
                    queryStr += "and n.Marka  = ~ { Nike } ";
                if (Adidas != null)
                    queryStr += "and n.Marka  = ~ { Adidas } ";
                if (Kappa != null)
                    queryStr += "and n.Marka  = ~ { Kappa } ";
                if (odeca != null)
                    queryStr += "and n.Podtip  = ~ { Odeca } ";
                if (obuca != null)
                    queryStr += "and n.Podtip  = ~ { Obuca } ";
                if (aksesoari != null)
                    queryStr += "and n.Podtip  = ~ { Aksesoari } ";
                if (cenaOd != null)
                    queryStr += "and n.Cena  > { CenaOd } ";
                if (cenaDo != null) 
                    queryStr += "and n.Cena  < { CenaDo } ";
                queryStr += "return n";
            }
            

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("proizvod", queryStr);
            
            var query = new Neo4jClient.Cypher.CypherQuery(queryStr,
                                                            queryDict, CypherResultMode.Set);               
            List<Proizvod> proizvodi = ((IRawGraphClient)client).ExecuteGetCypherResults<Proizvod>(query).ToList();

            foreach (Proizvod p in proizvodi)
            {
                pretraga.Proizvodi.Add(p);
            }


            return View("~/Views/Korisnik/Filter.cshtml",pretraga);
        }

        //public ActionResult PrikaziKolekciju(PrikazKolekcije pk)
        //{
        //    client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
        //    try
        //    {
        //        client.Connect();
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    String kolekcija = pk.PrikazKolekcijePrikaziKolekciju.Pol;
        //    Dictionary<string, object> queryDict = new Dictionary<string, object>();
        //    queryDict.Add("kolekcija", kolekcija);
        //    //start u nasoj (novijoj verziji) ne mora, moze da se pocne od match     
        //    var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (p:Proizvod) and exists(p.kolekcija) and p.kolekcija =~ kolekcija return p",
        //                                                    queryDict, CypherResultMode.Set);    //umesto upita kao string, postoji Fluent Cypher gde pisemo .match, .where..    //vraca skup cvorova koji se kastuju u klasu Actor - nazivi propertija u cvoru mora da se poklapaju sa nazivima propertija u klasi
        //                                                                                         //polazi od cvora n, vraca sve cvorove po tipu Actor i ima properti name...  rezultat je graf koji se pretvara u listu cvorova, return n-vraca se citav cvor, struktura tog cvora je kao klasa Actor             
        //    List<Proizvod> proizvodi = ((IRawGraphClient)client).ExecuteGetCypherResults<Proizvod>(query).ToList();

        //    foreach (Proizvod a in proizvodi)
        //    {
        //        //DateTime bday = a.getBirthday();
        //        TempData["msg"] = "<script>alert(" + a.podtip + ");</script>";
        //    }

        //    return View("~/Views/KorisnikM/Index.cshtml");
        //}
    }
}