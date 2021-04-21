using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class KorisnikProizvodi
    {
        public Korisnik Korisnik { get; set; }
        public List<Proizvod> Proizvodi { get; set; }

        
        public KorisnikProizvodi()
        {
            Proizvodi = new List<Proizvod>();
        }

        public KorisnikProizvodi(Korisnik kor,List<Proizvod> lp)
        {
            this.Korisnik = kor;
            this.Proizvodi = lp;
        }
    }
}