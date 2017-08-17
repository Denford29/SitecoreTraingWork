using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace events.tac.local.Areas.ATDW.Models
{
    public class AttributesJsonModel
    {
        public string AttributeTypeId { get; set; }

        public string Description { get; set; }

        public List<AttributeItemModel> Attributes { get; set; }
            = new List<AttributeItemModel>();

        public class AttributeItemModel
        {
            public string AttributeId
            {
                get;
                set;
            }

            public string Description
            {
                get;
                set;
            }
        }
    }
}