using System.Web;

namespace events.tac.local.Models
{
    /// <summary>
    /// create a class to display event intro details
    /// </summary>
    public class EventIntro
    {
        public EventIntro()
        {
        }

        public HtmlString Heading
        {
            get;
            set;
        }

        public HtmlString Intro
        {
            get;
            set;
        }

        public HtmlString Body
        {
            get;
            set;
        }

        public HtmlString EventImage
        {
            get;
            set;
        }

        public HtmlString Hightlights
        {
            get;
            set;
        }

        public HtmlString StartDate
        {
            get;
            set;
        }

        public HtmlString Duration
        {
            get;
            set;
        }

        public HtmlString Difficult
        {
            get;
            set;
        }
    }
}