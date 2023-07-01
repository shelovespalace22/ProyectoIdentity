using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models;
using ProyectoIdentity.Models.ViewModels;

namespace ProyectoIdentity.Controllers
{
    public class CuentasController : Controller
    {
        //Inyecciones de dependencia

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Registro(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            RegisterViewModel registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegisterViewModel registerViewModel, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var newUser = new AppUsuario
                {
                    UserName = registerViewModel.Email,
                    FirstName = registerViewModel.FirstName,
                    SecondName = registerViewModel.SecondName,
                    FirstLastName = registerViewModel.FirstLastName,
                    SecondLastName = registerViewModel.SecondLastName,
                    Address = registerViewModel.Address,
                    Sex = registerViewModel.Sex,
                    BirthDate = registerViewModel.BirthDate,
                    Email = registerViewModel.Email,
                    State = true,
                    CreationDate = DateTime.Now,
                    ModificationDate = DateTime.Now
                };

                var resultado = await _userManager.CreateAsync(newUser, registerViewModel.Password);

                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);

                    return LocalRedirect(returnurl);
                }
                ValidarErrores(resultado);
            }

            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult Acceso(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Acceso(AccesoViewModel accesoViewModel, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(accesoViewModel.Email, accesoViewModel.Password, accesoViewModel.RememberMe, lockoutOnFailure: true);

                if (resultado.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }

                else if (resultado.IsLockedOut)
                {
                    return View("Bloqueado");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Acceso inválido.");
                    return View(accesoViewModel);
                }
            }
            return View(accesoViewModel);
        }

        /* METODO DE LOGOUT */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalirAplicacion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /* METODO DE OLVIDO PASSWORD */

        [HttpGet]
        public IActionResult OlvidoPassword()
        {
            return View();
        }

        /* METODO PARA RECUPERACIÓN DE CONTRASEÑA */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvidoPassword(OlvidoPasswordViewModel olvidoPasswordView)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(olvidoPasswordView.Email);

                if (usuario == null)
                {
                    return RedirectToAction("Confirmacion");
                }

                var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);

                var urlRetorno = Url.Action("ResetPassword", "Cuentas", new { userId = usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(olvidoPasswordView.Email, "Recuperación de su Password - Proyecto Identity", 
                    " Por favor recupere su password dando click aquí: <a href=\"" + urlRetorno + "\">enlace</a>");

                return RedirectToAction("Confirmacion");
            }

            return View(olvidoPasswordView);
        }

        /* METODO PARA MOSTRAR VISTA DE CONFIRMACION DE PASSWORD */

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Confirmacion()
        {
            return View();
        }

        /* METODO PARA MOSTRAR FORMULARIO DE REESTABLECIMIENTO DE CONTRASEÑA */

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        /* MANEJADOR DE ERRORES */

        private void ValidarErrores(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }


    }
}
