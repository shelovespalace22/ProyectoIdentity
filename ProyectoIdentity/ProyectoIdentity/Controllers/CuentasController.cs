using System.Security.Claims;
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
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
					var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new { userId = newUser.Id, code = code }, protocol: HttpContext.Request.Scheme);

					await _emailSender.SendEmailAsync(registerViewModel.Email, "Confirmación de Cuenta - Proyecto Identity",
					" Por favor confirme su cuenta dando click aquí: <a href=\"" + urlRetorno + "\">enlace</a>");

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
		[AllowAnonymous]
		public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        /* METODO PARA REESTABLECIMIENTO DE CONTRASEÑA */

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task <IActionResult> ResetPassword(RecuperaPasswordViewModel recuperaPasswordView)
		{
			if (ModelState.IsValid)
			{
				var usuario = await _userManager.FindByEmailAsync(recuperaPasswordView.Email);

				if (usuario == null)
				{
					return RedirectToAction("ConfirmacionRecuperaPassword");
				}

                var resultado = await _userManager.ResetPasswordAsync(usuario, recuperaPasswordView.Code, recuperaPasswordView.Password);

                if (resultado.Succeeded)
                {
					return RedirectToAction("ConfirmacionRecuperaPassword");
				}

                ValidarErrores(resultado);
				
			}

			return View(recuperaPasswordView);
		}

		/* METODO PARA MOSTRAR CONFIRMACION DE REESTABLECIMIENTO DE CONTRASEÑA */

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ConfirmacionRecuperaPassword()
		{
            return View();
		}


		/* METODO PARA CONFIRMACIÓN DE EMAIL */

		[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail(string userId, string code)
        {
            if(userId == null || code == null)
            {
				return View("Error");
			}

            var usuario = await _userManager.FindByIdAsync(userId);

            if(usuario == null)
            {
                return View("Error");
            }

            var resultado = await _userManager.ConfirmEmailAsync(usuario, code);

            return View(resultado.Succeeded ? "ConfirmarEmail" : "Error");
        }

        /* METODO PARA ACCESO CON APPS EXTERNAS */

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult AccesoExterno(string proveedor, string returnurl = null)
        {
            var urlRedireccion = Url.Action("AccesoExternoCallback", "Cuentas", new { ReturnUrl = returnurl });

            var propiedades = _signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);

            return Challenge(propiedades, proveedor);
		}

		/* METODO CALLBACK */

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> AccesoExternoCallback(string returnurl = null, string error = null)
		{
			returnurl = returnurl ?? Url.Content("~/");

            if(error != null)
            {
                ModelState.AddModelError(string.Empty, $"Error en el acceso externo {error}");
                return View(nameof(Acceso)); 
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if(info == null)
            {
                return RedirectToAction(nameof(Acceso));
            }

            //Acceder con el usuario en el proveedor externo

            var resultado = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (resultado.Succeeded)
            {
                //Actualizar tokens de acceso

                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnurl);
            }
            else
            {
                //Si el usuario no tiene cuenta, crear una

                ViewData["ReturnUrl"] = returnurl;
                ViewData["NombreMostrarProveedor"] = info.ProviderDisplayName;

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nombre = info.Principal.FindFirstValue(ClaimTypes.Name);

                return View("ConfirmacionAccesoExterno", new ConfirmacionAccesoExternoViewModel { Email = email, Name = nombre});
            }
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
