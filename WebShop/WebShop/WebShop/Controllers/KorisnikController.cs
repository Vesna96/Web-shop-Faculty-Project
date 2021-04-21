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
        public String getMaxId()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where exists(n.Id) return max(n.Id)",
                                                            new Dictionary<string, object>(), CypherResultMode.Set);

            String maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<String>(query).ToList().FirstOrDefault();

            return maxId;
        }
        public ActionResult Index()
        {
            if (Session["email"] == null)
                return RedirectToAction("PrijaviSe", "Prijava");
            else
                return View();
        }

        public ActionResult Index(KorisnikProizvodi kp)
        {
            if (Session["email"] == null)
                return RedirectToAction("PrijaviSe", "Prijava");
            else
                return View(kp);
        }

        public ActionResult OdjaviSe()
        {
            return RedirectToAction("Index", "Prijava");
        }

        [HttpPost]
        public ActionResult Prikazi(String Email,String ime,String prezime,String podtip,String materijal,String marka,float cena,String pol,int sifra,String putanja)
        {
          


            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }

            Pregled vecPregledano = client.Cypher.OptionalMatch("(proizvod:Proizvod) <-[r2:pripadaProizvodu]- (pregled: Pregled) -[r1: pripadaKorisniku]-> (korisnik: Korisnik)")
                .Where((Korisnik korisnik) => korisnik.Email == Email)
                .AndWhere((Proizvod proizvod) => proizvod.Sifra == sifra)
                .Return(pregled => pregled.As<Pregled>())
                .Results.FirstOrDefault();

            if (vecPregledano == null)
            {
                Pregled NoviPregled = new Pregled(DateTime.Now);
                client.Cypher.Match("(korisnik:Korisnik)", "(proizvod:Proizvod)")
                    .Where((Korisnik korisnik) => korisnik.Email == Email)
                    .AndWhere((Proizvod proizvod) => proizvod.Sifra == sifra)
                    .Create("(p:Pregled { NoviPregled })")
                    .WithParam("NoviPregled", NoviPregled)
                    .Create("(p)-[:pripadaKorisniku]->(korisnik)")
                    .Create("(p)-[:pripadaProizvodu]->(proizvod)")
                    .ExecuteWithoutResults();
            }
            else
            {
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                client.Cypher.Match("(proizvod:Proizvod) <-[r2:pripadaProizvodu]- (pregled: Pregled) -[r1: pripadaKorisniku]-> (korisnik: Korisnik)")
                    .Where((Korisnik korisnik) => korisnik.Email == Email)
                    .AndWhere((Proizvod proizvod) => proizvod.Sifra == sifra)
                    .Set("pregled.DatumPregleda = {dt}")
                    .WithParam("dt", dt)
                    .ExecuteWithoutResults();
            }



            List<Boja> boje = new List<Boja>();
            List<Velicina> velicine = new List<Velicina>();
            List<Stil> stilovi = new List<Stil>();
            boje = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaBoju]->(boja:Boja)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == sifra))
                 .Return(boja => boja.As<Boja>())
                 .Results.ToList();
            velicine = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaVelicinu]->(velicina:Velicina)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == sifra))
                 .Return(velicina => velicina.As<Velicina>())
                 .Results.ToList();
            stilovi = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == sifra))
                 .Return(stil => stil.As<Stil>())
                 .Results.ToList();

            Proizvod p = new Proizvod();
            p = client.Cypher.OptionalMatch("(korisnik:Korisnik)-[lajkovao]->(proizvod:Proizvod)")
                .Where((Korisnik korisnik) => (korisnik.Email == Email))
                .AndWhere((Proizvod proizvod) => (proizvod.Sifra == sifra))
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.FirstOrDefault<Proizvod>();


            List<Proizvod> slicni = new List<Proizvod>();
            slicni = client.Cypher.Match("(proizvod:Proizvod)")
                .Where((Proizvod proizvod) => (proizvod.Podtip == podtip) && (proizvod.Marka == marka) && (proizvod.Sifra != sifra))
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach(Proizvod pp in slicni)
            {
                List<Slika> slike = new List<Slika>();
                slike = client.Cypher.OptionalMatch("(slika:Slika) <-[r:imaSliku]- (proizvod:Proizvod)")
                    .Where((Proizvod proizvod) => proizvod.Sifra == pp.Sifra)
                    .Return(slika => slika.As<Slika>())
                    .Results.ToList();

                foreach (Slika s in slike)
                {
                    if (s != null)
                        pp.PutanjeSlika.Add(s.PutanjaSlike);
                }
            }


            KorisnikProizvod kp = new KorisnikProizvod(ime,prezime,podtip,marka,materijal,sifra,cena,pol,boje,velicine,stilovi,putanja,slicni);

            if (p == null)
                kp.Like = false;
            else
                kp.Like = true;
            
            return View("~/Views/Proizvod/Index.cshtml", kp);
        }

        [HttpPost]
        public ActionResult Filtriraj(KorisnikProizvodi kp, String pol, String Elegantni, String Sportski, String Poslovni, String Casual,
            String Rock, String Orsay, String TomTaylor, String Bata, String Nike, String Adidas, String Kappa, String cenaOd,
            String cenaDo, String odeca, String obuca, String aksesoari, String Street)
        {

            String bata = (Bata == "true") ? "Bata" : "";
            String adidas = (Adidas == "true") ? "Adidas" : "";
            String nike = (Nike == "true") ? "Nike" : "";
            String orsay = (Orsay == "true") ? "Orsay" : "";
            String tomTaylor = (TomTaylor == "true") ? "TomTaylor" : "";
            String kappa = (Kappa == "true") ? "Kappa" : "";

            String elegantni = (Elegantni == "true") ? "Elegantni" : "";
            String poslovni = (Poslovni == "true") ? "Poslovni" : "";
            String street = (Street == "true") ? "Street" : "";
            String sportski = (Sportski == "true") ? "Sportski" : "";
            String casual = (Casual == "true") ? "Casual" : "";
            String rock = (Rock == "true") ? "rock" : "";

            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }

            float CenaOd = float.Parse(cenaOd);
            float CenaDo = float.Parse(cenaDo);
            List<Proizvod> p1 = new List<Proizvod>();

            bool stilFleg = (elegantni == "") && (poslovni == "") && (street == "") && (sportski == "") && (casual == "") && (rock == "");
            bool markaFleg = (bata == "") && (nike == "") && (adidas == "") && (tomTaylor == "") && (orsay == "") && (kappa == "");

           

            if (stilFleg && markaFleg)
            {
                p1 = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                    .Where((Proizvod proizvod) => proizvod.Pol == pol)
                    .AndWhere((Proizvod proizvod) => (proizvod.Podtip == odeca) || (proizvod.Podtip == obuca) || (proizvod.Podtip == aksesoari))
                    .AndWhere((Proizvod proizvod) => proizvod.Cena > CenaOd && proizvod.Cena < CenaDo)
                    .Return(proizvod => proizvod.As<Proizvod>())
                    .Results.ToList();

                
            }
            else
            {
                if((stilFleg == false) && (markaFleg == false))
                {
                    p1 = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                    .Where((Proizvod proizvod) => proizvod.Pol == pol)
                    .AndWhere((Stil stil) => stil.Naziv == elegantni || stil.Naziv == poslovni || stil.Naziv == street || stil.Naziv == sportski || stil.Naziv == casual || stil.Naziv == rock)
                    .AndWhere((Proizvod proizvod) => (proizvod.Podtip == odeca) || (proizvod.Podtip == obuca) || (proizvod.Podtip == aksesoari))
                    .AndWhere((Proizvod proizvod) => proizvod.Cena > CenaOd && proizvod.Cena < CenaDo)
                    .AndWhere((Proizvod proizvod) => proizvod.Marka == nike || proizvod.Marka == adidas || proizvod.Marka == bata || proizvod.Marka == tomTaylor || proizvod.Marka == kappa || proizvod.Marka == orsay)
                    .Return(proizvod => proizvod.As<Proizvod>())
                    .Results.ToList();
                }
                else
                {
                    if(stilFleg == true)
                    {
                    p1 = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                   .Where((Proizvod proizvod) => proizvod.Pol == pol)

                   .AndWhere((Proizvod proizvod) => (proizvod.Podtip == odeca) || (proizvod.Podtip == obuca) || (proizvod.Podtip == aksesoari))
                   .AndWhere((Proizvod proizvod) => proizvod.Cena > CenaOd && proizvod.Cena < CenaDo)
                   .AndWhere((Proizvod proizvod) => proizvod.Marka == nike || proizvod.Marka == adidas || proizvod.Marka == bata || proizvod.Marka == tomTaylor || proizvod.Marka == kappa || proizvod.Marka == orsay)
                   .Return(proizvod => proizvod.As<Proizvod>())
                   .Results.ToList();
                    }
                    else
                    {
                        p1 = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                    .Where((Proizvod proizvod) => proizvod.Pol == pol)
                    .AndWhere((Stil stil) => stil.Naziv == elegantni || stil.Naziv == poslovni || stil.Naziv == street || stil.Naziv == sportski || stil.Naziv == casual || stil.Naziv == rock)
                    .AndWhere((Proizvod proizvod) => (proizvod.Podtip == odeca) || (proizvod.Podtip == obuca) || (proizvod.Podtip == aksesoari))
                    .AndWhere((Proizvod proizvod) => proizvod.Cena > CenaOd && proizvod.Cena < CenaDo)
                    .Return(proizvod => proizvod.As<Proizvod>())
                    .Results.ToList();

                    }
                }
            }

            

            Pretraga NovaPretraga = new Pretraga(pol, CenaOd, CenaDo,DateTime.Now);
            NovaPretraga.CenaDo = CenaDo;
            NovaPretraga.CenaOd = CenaOd;
            string maxId = getMaxId();

            try
            {
                int mId = Int32.Parse(maxId);
                NovaPretraga.Id = (++mId).ToString();
            }
            catch (Exception exception)
            {
                NovaPretraga.Id = "";
            }

            client.Cypher.Match("(korisnik:Korisnik)")
                   .Where((Korisnik korisnik) => (korisnik.Email == kp.Korisnik.Email))
                   .Create("(p:Pretraga { NovaPretraga })")
                   .WithParam("NovaPretraga", NovaPretraga)
                   .Create("(p) -[:pretragaKorisnika]->(korisnik)")
                   .ExecuteWithoutResults();

            if (p1[0] == null)
            {
                //kp.Proizvodi = null;
                return View("~/Views/Korisnik/Index.cshtml", kp);
            }

            foreach (Proizvod p in p1)
            {
                List<Slika> slike = new List<Slika>();
                slike = client.Cypher.OptionalMatch("(slika:Slika) <-[r:imaSliku]- (proizvod:Proizvod)")
                    .Where((Proizvod proizvod) => proizvod.Sifra == p.Sifra)
                    .Return(slika => slika.As<Slika>())
                    .Results.ToList();

                client.Cypher.Match("(pretraga:Pretraga)","(proizvod:Proizvod)")
                   .Where((Proizvod proizvod) => proizvod.Sifra == p.Sifra)
                   .AndWhere((Pretraga pretraga) => pretraga.Id == NovaPretraga.Id)
                   .Create("(proizvod) <-[:pretragaProizvoda]-(pretraga)")
                   .ExecuteWithoutResults();



                foreach (Slika s in slike)
                {
                    if(s != null)
                        p.PutanjeSlika.Add(s.PutanjaSlike);
                }
                     

                kp.Proizvodi.Add(p);
                
            }


            return View("~/Views/Korisnik/Index.cshtml", kp);
        }

        
    }
}