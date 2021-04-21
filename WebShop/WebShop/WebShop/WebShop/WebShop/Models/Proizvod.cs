using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Proizvod
    {
        public String tip {get; set;}
        public String podtip { get; set; }
        public String kolekcija { get; set; }
        public String marka { get; set; }
        public float cena { get; set; }
        public int sifra { get; set; }
        public String materijal { get; set; }

        public Proizvod()
        {

        }
        public Proizvod(String t,String pt,String k, String m,float c,int s,String mat)
        {
            this.tip = t;
            this.podtip = pt;
            this.kolekcija = k;
            this.marka = m;
            this.cena = c;
            this.sifra = s;
            this.materijal = mat;
        }
    
    }
}