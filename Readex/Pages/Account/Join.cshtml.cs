using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Readex.Servicos;

namespace Readex.Pages.Account;

[AllowAnonymous]
[EnableRateLimiting("account-post")]
public class JoinModel(CadastroLeitoresPersistido cadastro) : PageModel
{
    [BindProperty]
    public JoinInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [TempData]
    public string? AccountMessage { get; set; }

    public IActionResult OnGet()
    {
        ValidateReturnUrl();
        return User.Identity?.IsAuthenticated == true ? RedirectToPage("/Index") : Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");

        ValidateReturnUrl();
        if (!ModelState.IsValid)
            return Page();

        if (!await cadastro.CriarContaAsync(Input.Username, Input.Email, Input.Password, cancellationToken))
        {
            ModelState.AddModelError(string.Empty, "That username or email is already in use.");
            return Page();
        }

        AccountMessage = "Your account is ready. Sign in to continue.";
        return RedirectToPage("/Account/SignIn", new { ReturnUrl });
    }

    private void ValidateReturnUrl()
    {
        if (Url.IsLocalUrl(ReturnUrl))
            return;

        ReturnUrl = null;
        ModelState.Remove(nameof(ReturnUrl));
    }

    public sealed class JoinInput
    {
        [Required(ErrorMessage = "Enter a username.")]
        [StringLength(30, ErrorMessage = "Use 30 characters or fewer for your username.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Enter your email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(254, ErrorMessage = "Use 254 characters or fewer for your email address.")]
        [Display(Name = "Email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Enter a password.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Use 8 to 128 characters for your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Enter your password again.")]
        [Compare(nameof(Password), ErrorMessage = "The passwords must match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = "";
    }
}
