using MicroServiceApp.Catalog.Api.Features.Courses;
using MicroServiceApp.Catalog.Api.Repositories;

namespace MicroServiceApp.Catalog.Api.Features.Categories
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public List<Course>? Courses { get; set; }
    }
}
