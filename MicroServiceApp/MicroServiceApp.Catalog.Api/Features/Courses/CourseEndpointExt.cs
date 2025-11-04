using MicroServiceApp.Catalog.Api.Features.Categories.Create;
using MicroServiceApp.Catalog.Api.Features.Courses.Create;
using MicroServiceApp.Catalog.Api.Features.Courses.Delete;
using MicroServiceApp.Catalog.Api.Features.Courses.GetAll;
using MicroServiceApp.Catalog.Api.Features.Courses.GetById;
using MicroServiceApp.Catalog.Api.Features.Courses.Update;

namespace MicroServiceApp.Catalog.Api.Features.Courses
{
    public static class CourseEndpointExt
    {
        public static void AddCourseGroupEndpointExt(this WebApplication app)
        {
            app.MapGroup("api/courses").WithTags("Courses")
                .CreateCourseGroupItemEndpoint()
                .GetAllCourseGroupItemEndpoint()
                .GetByIdCourseGroupItemEndpoint()
                .UpdateCourseGroupItemEndpoint()
                .DeleteCourseGroupItemEndpoint();

            //app.MapGroup("api/v{version:apiVersion}/courses").WithTags("Courses").WithApiVersionSet(apiVersionSet)
            //    .CreateCourseGroupItemEndpoint()
            //    .GetAllCourseGroupItemEndpoint()
            //    .GetByIdCourseGroupItemEndpoint()
            //    .UpdateCourseGroupItemEndpoint()
            //    .DeleteCourseGroupItemEndpoint()
            //    .GetByUserIdCourseGroupItemEndpoint();
        }
    }
}
