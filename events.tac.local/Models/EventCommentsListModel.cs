using System;
using System.Collections.Generic;
using Sitecore.ContentSearch.SearchTypes;

namespace events.tac.local.Models
{
    public class EventCommentsListModel
    {
        public List<CommentListItemModel> PageComments
        {
            get;
            set;
        }
        = new List<CommentListItemModel>();
    }

    public class CommentListItemModel : SearchResultItem
    {
        public string CommentorName
        {
            get;
            set;
        }

        public DateTime CommentDate
        {
            get;
            set;
        }

        public string CommentText
        {
            get;
            set;
        }

    }
}