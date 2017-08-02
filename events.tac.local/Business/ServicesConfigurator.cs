using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using events.tac.local.Business.Navigation;
using events.tac.local.Controllers;
using Sitecore.Mvc.Presentation;

namespace events.tac.local.Business
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<NavigationModelBuilder>();
            serviceCollection.AddTransient<NavigationController>();
            //add the rendering context service
            //serviceCollection.AddTransient<RenderingContext>((r) => RenderingContext.Current);
            serviceCollection.AddTransient((r) => RenderingContext.Current);
        }
    }
}