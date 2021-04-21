using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class KorisnikProizvod
    {
        public String Ime { get; set; }
        public String Prezime { get; set; }
        public String Podtip { get; set; }
        public String Marka { get; set; }
        public String Materijal { get; set; }
        public int Sifra { get; set; }
        public float Cena { get; set; }
        public String Pol { get; set; }
        public List<Boja> Boje { get; set; }
        public List<Velicina> Velicine { get; set; }
        public List<Stil> Stilovi { get; set; }

        public KorisnikProizvod()
        {
            Boje = new List<Boja>();
            Velicine = new List<Velicina>();
            Stilovi = new List<Stil>();
        }
        public KorisnikProizvod(String i,String p,String podtip,String m,String mat,int sifra,float cena,String pol,List<Boja> boje,List<Velicina> velicine,List<Stil> stilovi)
        {
            this.Ime = i;
            this.Prezime = p;
            this.Podtip = podtip;
            this.Marka = m;
            this.Materijal = mat;
            this.Sifra = sifra;
            this.Cena = cena;
            this.Pol = pol;
            this.Boje = boje;
            this.Velicine = velicine;
            this.Stilovi = stilovi;
        }
    }
}