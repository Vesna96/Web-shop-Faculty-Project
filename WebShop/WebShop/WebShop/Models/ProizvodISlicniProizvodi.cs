using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class ProizvodISlicniProizvodi
    {
        public Proizvod Proizvod { get; set; }
        public List<Proizvod> SlicniProizvodi { get; set; }

        public ProizvodISlicniProizvodi()
        {
            Proizvod = new Proizvod();
            SlicniProizvodi = new List<Proizvod>();
        }

        public ProizvodISlicniProizvodi(Proizvod p, List<Proizvod> sp)
        {
            this.SlicniProizvodi = sp;
            this.Proizvod = p;
        }
    }
}