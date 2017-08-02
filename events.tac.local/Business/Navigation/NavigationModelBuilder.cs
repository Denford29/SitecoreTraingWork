using TAC.Utils.Abstractions;
using Sitecore.Links;
using System.Linq;
using events.tac.local.Models;
using TAC.Utils.TestModels;
using Sitecore.Data.Items;
using TAC.Utils.SitecoreModels;

namespace events.tac.local.Business.Navigation
{
    public class NavigationModelBuilder 
    {

        public NavigationMenu CreateNavigationMenu(SitecoreItem root, SitecoreItem current)
        {
            NavigationMenu menu = new NavigationMenu()
            {
                Title = root.DisplayName,
                //Url = LinkManager.GetItemUrl(root),
                Url= root.GetUrl(),
                //Children = root.Axes.IsAncestorOf(current) ?
                //                  root.GetChildren().Select
                //                  (item => CreateNavigationMenu(item., current))
                //                : null
                Children = root.GetAxes().IsAncestorOf(current.CurrentItem()) ?
                                  root.GetChildren().Select
                                  (item => CreateNavigationMenu((SitecoreItem)item, current))
                                : null
            };

            return menu;
        }
    }
}