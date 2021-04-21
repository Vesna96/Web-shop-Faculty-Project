using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Kupovina
    {
        public DateTime DatumKupovine { get; set; }
        public String Boja { get; set; }
        public String Velicina { get; set; }
        public Kupovina()
        {
            DatumKupovine = new DateTime();
        }

        public Kupovina(DateTime dt,String boja, String veli)
        {
            this.DatumKupovine = dt;
            this.Boja = boja;
            this.Velicina = veli;
        }
    }
}