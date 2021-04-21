using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Korisnik
    {
        public String Id { get; set; }
        public String Ime { get; set; }
        public String Prezime { get; set; }
        public String Pol { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public String Email { get; set; }
        public String Lozinka { get; set; }
        public String StilOblacenja { get; set; }
        public String OmiljenaBoja { get; set; }
        public String Adresa { get; set; }

        public Korisnik()
        {

        }
        public Korisnik(String id,String ime,String prezime,String pol,DateTime god,String email,String lozinka,String stil,String boja,String adresa)
        {
            this.Id = id;
            this.Ime = ime;
            this.Prezime = prezime;
            this.Pol = pol;
            this.DatumRodjenja = god;
            this.Email = email;
            this.Lozinka = lozinka;
            this.StilOblacenja = stil;
            this.OmiljenaBoja = boja;
            this.Adresa = adresa;

        }
      
    }
}