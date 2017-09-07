using System.Collections.Generic;
using System.Web.Mvc;

namespace events.tac.local.Areas.ATDW.Models
{
    /// <summary>
    /// model for category classifications
    /// </summary>
    public class CategoryClassificationsModel
    {
        /// <summary>
        /// get or set a select list of categories available, and initialise it as an empty list 
        /// </summary>
        public List<SelectListItem> CategoriesAvailable { get; set; }
            = new List<SelectListItem>();

        /// <summary>
        /// get or set the selected category
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// get or set the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// get or set the success message
        /// </summary>
        public string SuccessMessage { get; set; }
    }
}