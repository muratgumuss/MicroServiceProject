using MicroServiceApp.Catalog.Api.Features.Categories.Create;

namespace MicroServiceApp.Catalog.Api.Features.Categories
{
    public static class CategoryEndpointExt
    {
        public static void AddCategoryGroupEndpointExt(this WebApplication app)
        {
            app.MapGroup("api/categories").CreateCategoryGroupItemEndpoint();

            //app.MapGroup("api/v{version:apiVersion}/categories").WithTags("Categories");
            //.WithApiVersionSet(apiVersionSet)
            //.CreateCategoryGroupItemEndpoint()
            //.GetAllCategoryGroupItemEndpoint()
            //.GetByIdCategoryGroupItemEndpoint();
        }
    }
}
