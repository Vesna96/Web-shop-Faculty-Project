using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Pretraga
    {

        public String Pol { get; set; }
        public float CenaOd { get; set; }
        public float CenaDo { get; set; }
        public List<String> Stilovi { get; set; }
        public String Odeca { get; set; }
        public String Obuca { get; set; }
        public String Aksesoar { get; set; }
        public List<String> Marke { get; set; }
        public String Email { get; set; }
        public List<Proizvod> Proizvodi { get; set; }

        public Pretraga()
        {
            Stilovi = new List<String>();
            Marke = new List<String>();
            Proizvodi = new List<Proizvod>();
        }

        public Pretraga(String pol, float cenaOd,float cenaDo,List<String> stilovi,String odeca,String obuca,String aksesoar,List<String> marke,String kor,List<Proizvod> lp)
        {
            this.Pol = pol;
            this.CenaDo = CenaDo;
            this.CenaOd = CenaOd;
            this.Stilovi = stilovi;
            this.Odeca = odeca;
            this.Aksesoar = aksesoar;
            this.Obuca = obuca;
            this.Marke = marke;
            this.Email = kor;
            this.Proizvodi = lp;

        }
    }
}