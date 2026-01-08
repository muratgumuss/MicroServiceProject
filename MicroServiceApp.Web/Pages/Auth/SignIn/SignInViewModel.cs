using System.ComponentModel.DataAnnotations;

namespace MicroServiceApp.Web.Pages.Auth.SignIn
{
    public record SignInViewModel
    {
        [Display(Name = "Email :")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; init; }

        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; init; }


        public static SignInViewModel Empty => new()
        {
            Email = string.Empty,
            Password = string.Empty
        };

        public static SignInViewModel GetExampleModel => new()
        {
            Email = "muratgumus05@gmail.com",
            Password = "Password123."
        };
    }
}
