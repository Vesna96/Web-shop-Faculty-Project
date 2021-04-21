using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Boja
    {
        public String Naziv { get; set; }
        public Boja()
        {

        }
        public Boja(String n)
        {
            this.Naziv = n;
        }
    }
}