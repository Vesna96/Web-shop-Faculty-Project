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
        public String Email { get; set; }
        public bool Like { get; set; }
        public String Slika { get; set; }

        public List<Proizvod> SlicniProizvodi { get; set; }
        

        public KorisnikProizvod()
        {
            Boje = new List<Boja>();
            Velicine = new List<Velicina>();
            Stilovi = new List<Stil>();
            SlicniProizvodi = new List<Proizvod>();
        }
        public KorisnikProizvod(String i,String p,String podtip,String m,String mat,int sifra,float cena,String pol,List<Boja> boje,List<Velicina> velicine,List<Stil> stilovi, String slika,List<Proizvod> lp)
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
            this.Slika = slika;
            this.SlicniProizvodi = lp;
        }
    }
}