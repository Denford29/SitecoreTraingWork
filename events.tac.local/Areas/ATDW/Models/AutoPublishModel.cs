namespace events.tac.local.Areas.ATDW.Models
{
    /// <summary>
    /// create the model used for publishing
    /// </summary>
    public class AutoPublishModel
    {
        /// <summary>
        /// get or set a flag to initiate publishing
        /// </summary>
        public bool InitiatePublish { get; set; }

        /// <summary>
        /// get or set the domain url string
        /// </summary>
        public string DomainUrl { get; set; }

        /// <summary>
        /// get or set the error message string
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// get or set the success message string
        /// </summary>
        public string SuccessMessage { get; set; }
    }
}