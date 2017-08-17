using System.Collections.Generic;
using System.Web.Mvc;

namespace events.tac.local.Areas.ATDW.Models
{
    public class AttributesModel
    {
        public List<SelectListItem> AttributeTypesAvailable { get; set; }
        = new List<SelectListItem>();

        public string SelectedAttributeType { get; set; }

        public string ErrorMessage { get; set; }

        public string SuccessMessage { get; set; }

    }
}
