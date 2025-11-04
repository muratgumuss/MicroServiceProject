using MicroServiceApp.Catalog.Api.Features.Courses.Create;
using MicroServiceApp.Catalog.Api.Features.Courses.Dtos;

namespace MicroServiceApp.Catalog.Api.Features.Courses
{
    public class CourseMapping : Profile
    {
        public CourseMapping()
        {
            CreateMap<CreateCourseCommand, Course>();
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<Feature, FeatureDto>().ReverseMap();
        }
    }
}
