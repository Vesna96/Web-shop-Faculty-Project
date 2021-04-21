using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using Neo4jClient;
using Neo4jClient.Cypher;
using System.Collections;


namespace WebShop.Controllers
{
    public class RegistracijaController : Controller
    {
        public GraphClient client;



        // GET: Registracija
        public ActionResult Index()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }

            var query = new Neo4jClient.Cypher.CypherQuery("match (b:Boja) return b.Naziv", new Dictionary<string, object>(), CypherResultMode.Set);
            List<String> list = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query).ToList();
            ListaBoja boje = new ListaBoja();

            List<SelectListItem> listSelect = new List<SelectListItem>();
            SelectListItem prazno = new SelectListItem();
            prazno.Text = "Izaberi boju";
            prazno.Value = "";
            listSelect.Add(prazno);
            foreach (var boja in list)
            {
                SelectListItem s = new SelectListItem()
                {
                    Text = boja,
                    Value = boja,
                    Selected = false
                };
                listSelect.Add(s);
            }

            var query1 = new Neo4jClient.Cypher.CypherQuery("match (s:Stil) return s.Naziv", new Dictionary<string, object>(), CypherResultMode.Set);
            List<String> list1 = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query1).ToList();
            ListaStilova stilovi = new ListaStilova();

            List<SelectListItem> listSelectStil = new List<SelectListItem>();
            SelectListItem prazno1 = new SelectListItem();
            prazno1.Text = "Izaberi stil";
            prazno1.Value = "";
            listSelectStil.Add(prazno1);
            foreach (var stil in list1)
            {
                SelectListItem s = new SelectListItem()
                {
                    Text = stil,
                    Value = stil,
                    Selected = false
                };
                listSelectStil.Add(s);
            }



            RegistracijaModel rm = new RegistracijaModel();
            rm.ListaBoja.Boje = listSelect;
            rm.ListaStilova.Stilovi = listSelectStil;

            return View(rm);
        }



        [HttpPost]
        public ActionResult DodajKorisnika(RegistracijaModel rm, String potvrdaLozinke)
        {
            if (rm.Korisnik.Ime == null || rm.Korisnik.Prezime == null || rm.Korisnik.Email == null
                || rm.Korisnik.Pol == null || rm.Korisnik.Lozinka == null)  //Ako korisnik nije uneo sve podatke
            {
                String msg = "Morate popuniti sva polje koja sadrze  *";
                ViewBag.PopuniSve = msg; //ViewBag se ne vidi ne znam zasto
                return RedirectToAction("Index");
            }
            if (rm.Korisnik.Lozinka != potvrdaLozinke) //Ako lozinka i potvda lozinke se ne poklapaju
            {
                String msg = "Lozinke se ne poklapaju";
                ViewBag.Message = msg;
                return RedirectToAction("Index");
            }
            

            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch(Exception ex)
            {

            }

            string korisnikEmail = ".*" + rm.Korisnik.Email + ".*";
            Dictionary<string, object> queryDict1 = new Dictionary<string, object>();
            queryDict1.Add("korisnikEmail", korisnikEmail);

            var query1 = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where (n:Korisnik) and exists(n.Email) and n.Email =~ {korisnikEmail} return n",
                                                            queryDict1, CypherResultMode.Set);
            List<Korisnik> logKorisnik = ((IRawGraphClient)client).ExecuteGetCypherResults<Korisnik>(query1).ToList();

            if (logKorisnik.Count > 0)
            {
                String msg = "Korisnik sa tom email adresom vec postoji";
                ViewBag.emailMsg = msg;
                return RedirectToAction("Index");
            }



            string maxId = getMaxId();
            try
            {
                int mId = Int32.Parse(maxId);
                rm.Korisnik.Id = (mId++).ToString();
            }
            catch (Exception exception)
            {
                rm.Korisnik.Id = "";
            }

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("Ime", rm.Korisnik.Ime);
            queryDict.Add("Prezime", rm.Korisnik.Prezime);
            queryDict.Add("Pol", rm.Korisnik.Pol);
            queryDict.Add("DatumRodjenja", rm.Korisnik.DatumRodjenja);
            queryDict.Add("Email", rm.Korisnik.Email);
            queryDict.Add("Lozinka", rm.Korisnik.Lozinka);
            queryDict.Add("StilOblacenja", rm.Korisnik.StilOblacenja);
            queryDict.Add("OmiljenaBoja", rm.Korisnik.OmiljenaBoja);
            queryDict.Add("Adresa", rm.Korisnik.Adresa);


            var query = new Neo4jClient.Cypher.CypherQuery("CREATE (n:Korisnik {Id:'" + rm.Korisnik.Id + "', Ime:'" + rm.Korisnik.Ime
                                                            + "', Prezime:'" + rm.Korisnik.Prezime + "', Pol:'" + rm.Korisnik.Pol
                                                            + "', DatumRodjenja:'" + rm.Korisnik.DatumRodjenja + "', Email:'" 
                                                            + rm.Korisnik.Email + "', Lozinka: '" + rm.Korisnik.Lozinka
                                                            + "', StilOblacenja:'" + rm.Korisnik.StilOblacenja + "', OmiljenaBoja:'" + rm.Korisnik.OmiljenaBoja
                                                              + "', Adresa:'" + rm.Korisnik.Adresa
                                                             + "'}) return n",
                                                            queryDict, CypherResultMode.Set);

            List<Korisnik> korisnici = ((IRawGraphClient)client).ExecuteGetCypherResults<Korisnik>(query).ToList();

            

           

            return View("~/Views/Prijava/Index.cshtml");
        }

        public String getMaxId()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where exists(n.id) return max(n.id)",
                                                            new Dictionary<string, object>(), CypherResultMode.Set);

            String maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query).ToList().FirstOrDefault();

            return maxId;
        }
    }
}