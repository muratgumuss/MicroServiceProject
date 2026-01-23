using MicroServiceApp.Web.PageModels;
using MicroServiceApp.Web.Services;
using MicroServiceApp.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MicroServiceApp.Web.Pages
{
    public class IndexModel(CatalogService catalogService, ILogger<IndexModel> logger) : BasePageModel
    {
        public List<CourseViewModel>? Courses { get; set; } = [];


        public async Task<IActionResult> OnGet()
        {
            var coursesAsResult = await catalogService.GetAllCoursesAsync();

            if (coursesAsResult.IsFail) return ErrorPage(coursesAsResult);

            Courses = coursesAsResult.Data!;

            return Page();
        }
    }
}
