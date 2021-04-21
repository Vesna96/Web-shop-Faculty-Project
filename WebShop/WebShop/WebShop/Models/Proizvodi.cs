using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Proizvodi
    {
        public List<Proizvod> ListaProizvoda { get; set; }

        public Proizvodi()
        {
            ListaProizvoda = new List<Proizvod>();
        }

        public Proizvodi(List<Proizvod> lp)
        {
            this.ListaProizvoda = lp;
        }
    }
}