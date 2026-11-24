using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Readex.Servicos;

namespace Readex.Pages.Account;

[AllowAnonymous]
[EnableRateLimiting("account-post")]
public class SignInModel(CadastroLeitoresPersistido cadastro) : PageModel
{
    [BindProperty]
    public SignInInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [TempData]
    public string? AccountMessage { get; set; }

    public IActionResult OnGet()
    {
        ValidateReturnUrl();
        return User.Identity?.IsAuthenticated == true ? RedirectAfterSignIn() : Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        ValidateReturnUrl();
        if (User.Identity?.IsAuthenticated == true)
            return RedirectAfterSignIn();

        if (!ModelState.IsValid)
            return Page();

        var reader = await cadastro.VerificarCredenciaisAsync(Input.Username, Input.Password, cancellationToken);
        if (reader is null)
        {
            ModelState.AddModelError(string.Empty, "The username or password is incorrect.");
            return Page();
        }

        const string scheme = CookieAuthenticationDefaults.AuthenticationScheme;
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, reader.Id.ToString()),
            new Claim(ClaimTypes.Name, reader.NomeUsuario)
        ], scheme);

        await HttpContext.SignInAsync(scheme, new ClaimsPrincipal(identity));
        return RedirectAfterSignIn();
    }

    private void ValidateReturnUrl()
    {
        if (Url.IsLocalUrl(ReturnUrl))
            return;

        ReturnUrl = null;
        ModelState.Remove(nameof(ReturnUrl));
    }

    private IActionResult RedirectAfterSignIn() => ReturnUrl is { } url
        ? LocalRedirect(url)
        : RedirectToPage("/Index");

    public sealed class SignInInput
    {
        [Required(ErrorMessage = "Enter your username.")]
        [StringLength(30, ErrorMessage = "Use 30 characters or fewer for your username.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Enter your password.")]
        [StringLength(128, ErrorMessage = "Use 128 characters or fewer for your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";
    }
}
