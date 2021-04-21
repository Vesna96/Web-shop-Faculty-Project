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
        public DateTime DatumPretrage { get; set; }
        public String Email { get; set; }
        public String Id { get; set; }
        public Pretraga()
        {

        }

        public Pretraga(String pol, float cenaOd,float cenaDo,DateTime datum)
        {
            this.Pol = pol;
            this.CenaDo = CenaDo;
            this.CenaOd = CenaOd;
            this.DatumPretrage = datum;


        }
    }
}