using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace events.tac.local.Models
{
    public class SiteCarouselModel
    {
        public List<CarouselImageModel> CarouselItems
        {
            get;
            set;
        }
        = new List<CarouselImageModel>();
    }
}