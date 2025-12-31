using MicroServiceApp.Web.Pages.Auth.SignUp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MicroServiceApp.Web.Pages.Auth
{
    public class SignUpModel(SignUpService signUpService) : PageModel
    {
        [BindProperty] public required SignUpViewModel SignUpViewModel { get; set; } = SignUpViewModel.GetExampleModel;


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signUpService.CreateAccount(SignUpViewModel);


            if (result.IsFail)
            {
                ModelState.AddModelError(string.Empty, result.Fail.Title);

                if (!string.IsNullOrEmpty(result.Fail.Detail)) ModelState.AddModelError(string.Empty, result.Fail.Detail);

                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}
