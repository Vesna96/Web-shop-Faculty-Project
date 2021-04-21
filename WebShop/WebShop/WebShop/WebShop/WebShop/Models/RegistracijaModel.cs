using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class RegistracijaModel
    {
        public ListaBoja ListaBoja { get; set; }
        public ListaStilova ListaStilova { get; set; }
        public Korisnik Korisnik { get; set; }

       public RegistracijaModel()
        {
            ListaBoja = new ListaBoja();
            Korisnik = new Korisnik();
            ListaStilova = new ListaStilova();
        }
       public RegistracijaModel(ListaBoja boje, Korisnik korisnik)
        {
            this.ListaBoja = boje;
            this.Korisnik = korisnik;
        }
    }
}