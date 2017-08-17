using events.tac.local.Areas.ATDW.Controllers;
using events.tac.local.Areas.Importer.Controllers;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using events.tac.local.Controllers;
using SATC.SC.Framework.Navigation;
using SATC.SC.Framework.SitecoreHelpers;
using Sitecore.Mvc.Presentation;

namespace events.tac.local.Business
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
             //Add the site controllers here, only the ones with extra fields declared in the constructor
            serviceCollection.AddTransient<NavigationController>();
            serviceCollection.AddTransient<PageHeaderController>();
            serviceCollection.AddTransient<BreadcrumbController>();
            serviceCollection.AddTransient<CarouselController>();
            serviceCollection.AddTransient<PageFooterController>();
            serviceCollection.AddTransient<EventsListController>();
            serviceCollection.AddTransient<CommentsController>();
            //events importer controller
            serviceCollection.AddTransient<EventsController>();
            //ATDW Controllers
            serviceCollection.AddTransient<AtdwImporterController>();


            // add a service to get the rendering context
            serviceCollection.AddTransient((r) => RenderingContext.Current);

             //SATC SC Framework service registration
            serviceCollection.AddTransient<StandardHelpers>();
            serviceCollection.AddTransient<NavigationHelpers>();
            serviceCollection.AddTransient<LanguageHelpers>();

        }
    }
}