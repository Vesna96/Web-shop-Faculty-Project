using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Velicina
    {
        public String Oznaka { get; set; }
        public Velicina()
        {

        }
        public Velicina(String oznaka)
        {
            this.Oznaka = oznaka;
        }
    }
}