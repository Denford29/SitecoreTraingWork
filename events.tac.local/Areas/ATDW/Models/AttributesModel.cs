using System.Collections.Generic;
using System.Web.Mvc;

namespace events.tac.local.Areas.ATDW.Models
{
    /// <summary>
    /// model used for attributes
    /// </summary>
    public class AttributesModel
    {
        /// <summary>
        /// get or set the select list of attribute types available, and initialise it as an empty list 
        /// </summary>
        public List<SelectListItem> AttributeTypesAvailable { get; set; }
        = new List<SelectListItem>();

        /// <summary>
        /// get or set the selected attribute type
        /// </summary>
        public string SelectedAttributeType { get; set; }

        /// <summary>
        /// get or ser an error message to display
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// get or ser an success message to display
        /// </summary>
        public string SuccessMessage { get; set; }

    }
}
