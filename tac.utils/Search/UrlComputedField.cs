using Sitecore.ContentSearch.ComputedFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace TAC.Utils.Search
{
    class UrlComputedField : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            //check if the indexable is not null
            Assert.ArgumentNotNull(indexable, "indexable");
            //make sure its a sitecore indexable item
            SitecoreIndexableItem scIndexable = indexable as SitecoreIndexableItem;
            //get the item from the indexable 
            Item item = (Item)scIndexable;
            //create the url options to use to get the link by cloning the defaults
            UrlOptions urlOptions = (UrlOptions)UrlOptions.DefaultOptions.Clone();
            //set the options to use
            urlOptions.SiteResolving = true;
            urlOptions.ShortenUrls = true;
            //retunr the link created with the options
            var computedLink = LinkManager.GetItemUrl(item, urlOptions);
            return computedLink;
        }
    }
}
