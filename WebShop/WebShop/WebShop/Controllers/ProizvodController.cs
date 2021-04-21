using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4jClient;
using WebShop.Models;

namespace WebShop.Controllers
{ 
    public class ProizvodController : Controller
    {
        public GraphClient client;

        public ActionResult Index()
        {
            return View();
        }

        

        public ActionResult Like(KorisnikProizvod kp)
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }

            client.Cypher.Match("(korisnik:Korisnik)", "(proizvod:Proizvod)")
                .Where((Korisnik korisnik) => korisnik.Email == kp.Email)
                .AndWhere((Proizvod proizvod) => proizvod.Sifra == kp.Sifra)
                .Create("(korisnik)-[:lajkovao]->(proizvod)")
                .ExecuteWithoutResults();

            List<Boja> boje = new List<Boja>();
            List<Velicina> velicine = new List<Velicina>();
            List<Stil> stilovi = new List<Stil>();
            boje = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaBoju]->(boja:Boja)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(boja => boja.As<Boja>())
                 .Results.ToList();
            velicine = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaVelicinu]->(velicina:Velicina)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(velicina => velicina.As<Velicina>())
                 .Results.ToList();
            stilovi = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(stil => stil.As<Stil>())
                 .Results.ToList();

            kp.Boje = boje;
            kp.Stilovi = stilovi;
            kp.Velicine = velicine;

            kp.Like = true;
           
            return View("~/Views/Proizvod/Index.cshtml", kp);
        }

        public ActionResult Dislike(KorisnikProizvod kp)
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }

            List<Boja> boje = new List<Boja>();
            List<Velicina> velicine = new List<Velicina>();
            List<Stil> stilovi = new List<Stil>();
            boje = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaBoju]->(boja:Boja)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(boja => boja.As<Boja>())
                 .Results.ToList();
            velicine = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaVelicinu]->(velicina:Velicina)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(velicina => velicina.As<Velicina>())
                 .Results.ToList();
            stilovi = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(stil => stil.As<Stil>())
                 .Results.ToList();

            kp.Boje = boje;
            kp.Stilovi = stilovi;
            kp.Velicine = velicine;

            client.Cypher.OptionalMatch("(korisnik:Korisnik) -[r:lajkovao]->(proizvod:Proizvod) ")
                .Where((Korisnik korisnik) => korisnik.Email == kp.Email)
                .AndWhere((Proizvod proizvod) => proizvod.Sifra == kp.Sifra)
                .Delete("r")
                .ExecuteWithoutResults();

            kp.Like = false;
            return View("~/Views/Proizvod/Index.cshtml", kp);
        }

        public ActionResult Naruci(KorisnikProizvod kp,String Boja,String Velicina)
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "naprednebaze");
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {

            }
            List<Boja> boje = new List<Boja>();
            List<Velicina> velicine = new List<Velicina>();
            List<Stil> stilovi = new List<Stil>();
            boje = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaBoju]->(boja:Boja)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(boja => boja.As<Boja>())
                 .Results.ToList();
            bool imaBoja = false;
            bool imaVelicina = false;
            
            
            velicine = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[ImaVelicinu]->(velicina:Velicina)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(velicina => velicina.As<Velicina>())
                 .Results.ToList();

            stilovi = client.Cypher.OptionalMatch("(proizvod:Proizvod)-[PripadaStilu]->(stil:Stil)")
                 .Where((Proizvod proizvod) => (proizvod.Sifra == kp.Sifra))
                 .Return(stil => stil.As<Stil>())
                 .Results.ToList();

            kp.Boje = boje;
            kp.Stilovi = stilovi;
            kp.Velicine = velicine;

            foreach (Boja b in boje)
            {
                if (b.Naziv.CompareTo(Boja) == 0)
                    imaBoja = true;
            }
            foreach (Velicina b in velicine)
            {
                if (b.Oznaka.CompareTo(Velicina) == 0)
                    imaVelicina = true;
            }

            if (!(imaVelicina && imaBoja))
                return View("~/Views/Proizvod/Index.cshtml", kp);

            

           

            Kupovina NovaKupovina = new Kupovina(DateTime.Now,Boja,Velicina);
            
            client.Cypher.Match("(korisnik:Korisnik)", "(proizvod:Proizvod)")
                .Where((Korisnik korisnik) => korisnik.Email == kp.Email)
                .AndWhere((Proizvod proizvod) => proizvod.Sifra == kp.Sifra)
                .Create("(k:Kupovina { NovaKupovina })")
                .WithParam("NovaKupovina",NovaKupovina)
                .Create("(k)-[:pripadaKorisniku]->(korisnik)")
                .Create("(k)-[:pripadaProizvodu]->(proizvod)")
                .ExecuteWithoutResults();

            return View("~/Views/Proizvod/Index.cshtml", kp);
        }
    }

    
}