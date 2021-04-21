using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebShop.Models
{
    public class ListaStilova
    {
        public IEnumerable<SelectListItem> Stilovi { get; set; }

        public ListaStilova()
        {

        }
        public ListaStilova(IEnumerable<SelectListItem> stilovi)
        {
            this.Stilovi = stilovi;
        }
    }
}