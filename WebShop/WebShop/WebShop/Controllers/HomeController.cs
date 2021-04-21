using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models;
using Neo4jClient;

namespace WebShop.Controllers
{
    public class HomeController : Controller
    {
        public GraphClient client;
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

            

            List<Proizvod> lp = new List<Proizvod>();
            Proizvodi proizvodi = new Proizvodi();

            lp = client.Cypher.Match("(pregled:Pregled) -[:pripadaProizvodu]-> (p:Proizvod)")
                .Return(p => p.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in lp)
            {
                if (proizvodi.ListaProizvoda.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    proizvodi.ListaProizvoda.Add(p);
                if (proizvodi.ListaProizvoda.Count >= 20)
                {
                    foreach (Proizvod pp in proizvodi.ListaProizvoda)
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
                    return View( proizvodi);
                }
            }

            foreach (Proizvod pp in proizvodi.ListaProizvoda)
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

            return View(proizvodi);
        }

        public ActionResult Prikazi(String tip,String sifra,String podtip,String putanja,String marka,String materijal,String cena,String pol)
        {

            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }
            float Cena = float.Parse(cena);
            int Sifra = int.Parse(sifra);
            List<String> ls = new List<String>();
            ls.Add(putanja);
            Proizvod proizvodp = new Proizvod(tip,podtip,pol,marka,Cena,Sifra,materijal,ls);

            List<Proizvod> slicni = new List<Proizvod>();
            slicni = client.Cypher.Match("(proizvod:Proizvod)")
                .Where((Proizvod proizvod) => (proizvod.Podtip == podtip) && (proizvod.Marka == marka) && (proizvod.Sifra != Sifra))
                .Return(proizvod => proizvod.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod pp in slicni)
            {
                List<Slika> slike = new List<Slika>();
                slike = client.Cypher.OptionalMatch("(slika:Slika) <-[r:imaSliku]- (p:Proizvod)")
                    .Where((Proizvod p) => p.Sifra == pp.Sifra)
                    .Return(slika => slika.As<Slika>())
                    .Results.ToList();

                foreach (Slika s in slike)
                {
                    if (s != null)
                        pp.PutanjeSlika.Add(s.PutanjaSlike);
                }
            }

            ProizvodISlicniProizvodi psp = new ProizvodISlicniProizvodi(proizvodp,slicni);

            return View("~/Views/Home/Prikaz.cshtml", psp);
        }

        public ActionResult Reload()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }



            List<Proizvod> lp = new List<Proizvod>();
            Proizvodi proizvodi = new Proizvodi();

            lp = client.Cypher.Match("(pregled:Pregled) -[:pripadaProizvodu]-> (p:Proizvod)")
                .Return(p => p.As<Proizvod>())
                .Results.ToList();

            foreach (Proizvod p in lp)
            {
                if (proizvodi.ListaProizvoda.Find((Proizvod pro) => pro.Sifra == p.Sifra) == null)
                    proizvodi.ListaProizvoda.Add(p);
                if (proizvodi.ListaProizvoda.Count >= 20)
                {
                    foreach (Proizvod pp in proizvodi.ListaProizvoda)
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
                    return View("~/Views/Home/Index.cshtml", proizvodi);
                }
            }

            foreach (Proizvod pp in proizvodi.ListaProizvoda)
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
            
            return View("~/Views/Home/Index.cshtml",proizvodi);
        }

    }
}