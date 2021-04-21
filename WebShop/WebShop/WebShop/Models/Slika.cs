using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models
{
    public class Slika
    {
        public String PutanjaSlike { get; set; }

        public Slika()
        {

        }

        public Slika(String putanja)
        {
            this.PutanjaSlike = putanja;
        }
    }
}