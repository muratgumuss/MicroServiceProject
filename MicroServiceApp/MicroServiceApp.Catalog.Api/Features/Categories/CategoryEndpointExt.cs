using MicroServiceApp.Catalog.Api.Features.Categories.Create;
using MicroServiceApp.Catalog.Api.Features.Categories.GetAll;
using MicroServiceApp.Catalog.Api.Features.Categories.GetById;

namespace MicroServiceApp.Catalog.Api.Features.Categories
{
    public static class CategoryEndpointExt
    {
        public static void AddCategoryGroupEndpointExt(this WebApplication app)
        {
            app.MapGroup("api/categories")
                .CreateCategoryGroupItemEndpoint()
                .GetAllCategoryGroupItemEndpoint()
                .GetByIdCategoryGroupItemEndpoint();
            //app.MapGroup("api/v{version:apiVersion}/categories").WithTags("Categories");
            //.WithApiVersionSet(apiVersionSet)
            //.CreateCategoryGroupItemEndpoint()
            //.GetAllCategoryGroupItemEndpoint()
            //.GetByIdCategoryGroupItemEndpoint();
        }
    }
}
