using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Stil
    {
        public String Naziv { get; set; }
        public Stil() { }
        public Stil(String naziv)
        {
            this.Naziv = naziv;
        }
    }
}