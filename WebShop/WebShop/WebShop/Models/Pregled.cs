using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Pregled
    {
        public DateTime DatumPregleda { get; set; }

        public Pregled()
        {
            DatumPregleda = new DateTime();
        }

        public Pregled(DateTime dt)
        {
            this.DatumPregleda = dt;
        }
    }
}