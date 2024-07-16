using Fuchibol.ChatService.SubscribeTableDependencies;

namespace Fuchibol.ChatService.MiddlewareExtensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseUserTableDependecy(this IApplicationBuilder applicationBuilder)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;
            var service = serviceProvider.GetService<SubscribeUserTableDependency>();
            service.SubscribeTableDependency();
        }
    }
}