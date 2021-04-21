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

            DateTime danasnjiDatum = DateTime.Now;

            KorisnikProizvodi kp = new KorisnikProizvodi();
            kp.Korisnik = logKorisnik[0];

            List<Proizvod> kupljeniProizvodiSB = new List<Proizvod>(); // Proizvodi koje su kupovali slicni korisnici po stilu oblacenja i po boji zadnjih 10 dana
            kupljeniProizvodiSB = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)", "(k1:Korisnik) -[:istiPoBoji]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (kupovina:Kupovina) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => (k1.Email == korisnik.Email))
                //.AndWhere((Kupovina kupovina) => (danasnjiDatum.Date.CompareTo(kupovina.DatumKupovine.Date)) < 10 )
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();
            kp.Proizvodi = kupljeniProizvodiSB;

            if(kp.Proizvodi.Count >= 20)
            {
                foreach (Proizvod pp in kp.Proizvodi)
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
                return View("~/Views/Korisnik/Index.cshtml", kp);
            }

            List<Proizvod> pregledaniProizvodiILjakovaniSB = new List<Proizvod>(); // Proizvodi koje su pregledali i lajkovali slicni korisnici po stilu oblacenja i po boji
            pregledaniProizvodiILjakovaniSB = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)", "(k1:Korisnik) -[:istiPoBoji]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:lajkovai] - (k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (Pregled:Pregled) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => k1.Email == korisnik.Email)
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();
            foreach(Proizvod p in pregledaniProizvodiILjakovaniSB)
            {
                if(kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                     kp.Proizvodi.Add(p);
                if(kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> pretrazeniProizvodiSB = new List<Proizvod>(); // Proizvodi koje su pretrazivali slicni korisnici po stilu oblacenja i po boji
            pretrazeniProizvodiSB = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)", "(k1:Korisnik) -[:istiPoBoji]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (pretraga:Pretraga) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => k1.Email == korisnik.Email)
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();
            foreach (Proizvod p in pretrazeniProizvodiSB)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }


            List<Proizvod> pregledaniProizvodiSB = new List<Proizvod>(); // Proizvodi koje su pregledali slicni korisnici po stilu oblacenja po boji
            pregledaniProizvodiSB = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)", "(k1:Korisnik) -[:istiPoBoji]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (Pregled:Pregled) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => k1.Email == korisnik.Email)
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in pregledaniProizvodiSB)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////
            List<Proizvod> kupljeniProizvodiS = new List<Proizvod>(); // Proizvodi koje su kupovali slicni korisnici po stilu oblacenja
            kupljeniProizvodiS = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (kupovina:Kupovina) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => (k1.Email == korisnik.Email))
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in kupljeniProizvodiS)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> pregledaniProizvodiILjakovaniS = new List<Proizvod>(); // Proizvodi koje su pregledali i lajkovali slicni korisnici po stilu oblacenja
            pregledaniProizvodiILjakovaniS = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:lajkovai] - (k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (Pregled:Pregled) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => k1.Email == korisnik.Email)
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in pregledaniProizvodiILjakovaniS)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> pretrazeniProizvodiS = new List<Proizvod>(); // Proizvodi koje su pretrazivali slicni korisnici po stilu oblacenja
            pretrazeniProizvodiS = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (pretraga:Pretraga) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => k1.Email == korisnik.Email)
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in pretrazeniProizvodiS)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

           
            List<Proizvod> pregledaniProizvodiS = new List<Proizvod>(); // Proizvodi koje su pregledali slicni korisnici po stilu oblacenja
            pregledaniProizvodiS = client.Cypher.Match("(k1:Korisnik) -[:istiPoStilu]-(k2:Korisnik)",
                "(proizvod:Proizvod) <- [:pripadaProizvodu] - (Pregled:Pregled) -[:pripadaKorisniku]->(k2:Korisnik)")
                .Where((Korisnik k1) => k1.Email == korisnik.Email)
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in pregledaniProizvodiS)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> lajkovaniProizvodiKorisnika = new List<Proizvod>(); // Proizvodi koje su pregledali slicni korisnici po stilu oblacenja
            lajkovaniProizvodiKorisnika = client.Cypher.Match("(k:Korisnik) -[:lajkovao]-> (p:Proizvod)")
                .Where((Korisnik k) => k.Email == korisnik.Email)
                .Return(p => p.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in lajkovaniProizvodiKorisnika)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> korisnikoviPregledi = new List<Proizvod>(); // Proizvodi koje su pregledali slicni korisnici po stilu oblacenja
            korisnikoviPregledi = client.Cypher.Match("(p:Proizvod) <-[:pripadaProizvodu]-(pregled:Pregled) -[:pripadaKorisniku]-> (k:Korisnik)")
                .Where((Korisnik k) => k.Email == korisnik.Email)
                .Return(p => p.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in korisnikoviPregledi)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> korisnikovePretrage= new List<Proizvod>(); // Proizvodi koje su pregledali slicni korisnici po stilu oblacenja
            korisnikovePretrage = client.Cypher.Match("(p:Proizvod) <-[:pripadaProizvodu]-(pretraga:Pretraga) -[:pripadaKorisniku]-> (k:Korisnik)")
                .Where((Korisnik k) => k.Email == korisnik.Email)
                .Return(p => p.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in korisnikovePretrage)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            List<Proizvod> proizvodiZaNovogKorisnika = new List<Proizvod>(); // Proizvodi koje su pregledali slicni korisnici po stilu oblacenja
            proizvodiZaNovogKorisnika = client.Cypher.Match("(pregled:Pregled) -[:pripadaProizvodu]-> (p:Proizvod)")
                .Return(p => p.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in proizvodiZaNovogKorisnika)
            {
                if (kp.Proizvodi.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    kp.Proizvodi.Add(p);
                if (kp.Proizvodi.Count >= 20)
                {
                    foreach (Proizvod pp in kp.Proizvodi)
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
                    return View("~/Views/Korisnik/Index.cshtml", kp);
                }
            }

            foreach (Proizvod p in kp.Proizvodi)
            {
                List<Slika> slike = new List<Slika>();
                slike = client.Cypher.OptionalMatch("(slika:Slika) <-[r:imaSliku]- (proizvod:Proizvod)")
                    .Where((Proizvod proizvod) => proizvod.Sifra == p.Sifra)
                    .Return(slika => slika.As<Slika>())
                    .Results.ToList();

                foreach (Slika s in slike)
                {
                    if (s != null)
                        p.PutanjeSlika.Add(s.PutanjaSlike);
                }
            }


                return View("~/Views/Korisnik/Index.cshtml", kp);

        }
    }
}