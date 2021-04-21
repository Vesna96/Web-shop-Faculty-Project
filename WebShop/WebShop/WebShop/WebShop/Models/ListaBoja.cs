using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebShop.Models
{
    public class ListaBoja
    {
        public IEnumerable<SelectListItem> Boje { get; set; }
        
        public ListaBoja()
        {

        }
        public ListaBoja(IEnumerable<SelectListItem> boje)
        {
            this.Boje = boje;
        }
    }

    
}