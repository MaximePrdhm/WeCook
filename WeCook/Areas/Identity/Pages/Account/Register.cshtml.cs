// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using WeCook.Models.Users;

namespace WeCook.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _env = env;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel : IValidatableObject
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Champ requis")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mot de Passe")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmation MDP")]
            [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Champ requis")]
            [StringLength(50)]
            [Display(Name = "Prénom")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Champ requis")]
            [StringLength(75)]
            [Display(Name = "Nom")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Champ requis")]
            [Display(Name = "Jour")]
            public int BirthDay { get; set; }

            [Required(ErrorMessage = "Champ requis")]
            [Display(Name = "Mois")]
            public int BirthMonth { get; set; }

            [Required(ErrorMessage = "Champ requis")]
            [Display(Name = "Année")]
            public int BirthYear { get; set; }

            [Display(Name = "Photo de profil")]
            public IFormFile? ImageFile { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                bool notExistingDate = false;
                bool futureDate = false;
                try
                {
                    var date = new DateTime(BirthYear, BirthMonth, BirthDay);

                    if (date > DateTime.Today)
                        futureDate = true;

                }
                catch (ArgumentOutOfRangeException)
                {
                    notExistingDate = true;
                }
                if(notExistingDate)
                    yield return new ValidationResult("Date inexistante", new List<string>() { nameof(BirthDay), nameof(BirthMonth) });
                if (futureDate)
                    yield return new ValidationResult("Date invalide", new List<string>() { nameof(BirthDay), nameof(BirthMonth) });

                if (ImageFile == null)
                {
                    yield return new ValidationResult("Champ requis", new List<string>() { nameof(ImageFile) });
                }
            }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            SetViewDataValues();
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FirstName = Input.FirstName;
                user.Name = Input.LastName;
                user.RegisteredAt = DateTime.Today;
                user.BirthDate = new DateTime(Input.BirthYear, Input.BirthMonth, Input.BirthDay);
                
                var fileName = $"${Guid.NewGuid()}{Path.GetExtension(Input.ImageFile.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", "users", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(fileStream);
                }
                user.AvatarName = fileName;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    /*var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirmation de votre adresse email",
                        $"Pour pouvoir accéder à votre compte, veuillez confirmer votre adresse email en <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliquant ici</a>.");*/

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            SetViewDataValues();
            return Page();
        }

        private void SetViewDataValues()
        {
            var days = new List<SelectListItem>();
            for (int i = 1; i <= 31; i++)
            {
                days.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewData["Days"] = days;

            var months = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new SelectListItem() { Text = new DateTime(2022,i,1).ToString("MMMM"), Value = i.ToString() });
            }
            ViewData["Months"] = months;

            var years = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
            {
                years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewData["Years"] = years;
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
