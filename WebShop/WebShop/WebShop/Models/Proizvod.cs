using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Proizvod
    {
        public String Tip {get; set;}
        public String Podtip { get; set; }
        public String Pol { get; set; }
        public String Marka { get; set; }
        public float Cena { get; set; }
        public int Sifra { get; set; }
        public String Materijal { get; set; }
        public List<String> PutanjeSlika { get; set; }
        

        public Proizvod()
        {
            PutanjeSlika = new List<String>();
        }
        public Proizvod(String t,String pt,String k, String m,float c,int s,String mat,List<String> putanje)
        {
            this.Tip = t;
            this.Podtip = pt;
            this.Pol = k;
            this.Marka = m;
            this.Cena = c;
            this.Sifra = s;
            this.Materijal = mat;
            this.PutanjeSlika = putanje;
        }
    
    }
}