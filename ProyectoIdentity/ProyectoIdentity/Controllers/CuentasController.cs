using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models;
using ProyectoIdentity.Models.ViewModels;

namespace ProyectoIdentity.Controllers
{
    [Authorize]
    public class CuentasController : Controller
    {
        //Inyecciones de dependencia

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public readonly UrlEncoder _urlEncoder;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, UrlEncoder urlEncoder, RoleManager<IdentityRole> rolManager)
        {
            _userManager = userManager;
            _rolManager = rolManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(string returnurl = null)
        {
            //Creación de Roles 

            //Validar si el rol ya existe.

            if (!await _rolManager.RoleExistsAsync("Administrador"))
            {
                //Crear el rol Administrador

                await _rolManager.CreateAsync(new IdentityRole("Administrador"));
            }

            //Validar si el rol ya existe

            if (!await _rolManager.RoleExistsAsync("Registrado"))
            {
                //Crear el rol Registrado

                await _rolManager.CreateAsync(new IdentityRole("Registrado"));
            }

            ViewData["ReturnUrl"] = returnurl;
            RegisterViewModel registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public IActionResult Acceso(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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

                //Para autenticación de dos factores

                if (resultado.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(VerificarCodigoAutenticador), new { returnurl, accesoViewModel.RememberMe });
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
        [AllowAnonymous]
        public IActionResult OlvidoPassword()
        {
            return View();
        }

        /* METODO PARA RECUPERACIÓN DE CONTRASEÑA */

        [HttpPost]
        [AllowAnonymous]
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
        [AllowAnonymous]
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

            //Para autenticación de dos factores

            if (resultado.RequiresTwoFactor)
            {
                return RedirectToAction("VerificarCodigoAutenticador", new { returnurl = returnurl});
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

        /* METODO CONFIRMACION DE ACCESO EXTERNO */

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmacionAccesoExterno(ConfirmacionAccesoExternoViewModel caeVm, string returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //Obtener la información del usuario del proveedor externo

                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                {
                    return View("Error");
                }

                var usuario = new AppUsuario { UserName = caeVm.Email, Email = caeVm.Email, FirstName = caeVm.Name };

                var resultado = await _userManager.CreateAsync(usuario);

                if (resultado.Succeeded)
                {
                    resultado = await _userManager.AddLoginAsync(usuario, info);

                    if (resultado.Succeeded)
                    {
                        await _signInManager.SignInAsync(usuario, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                        return LocalRedirect(returnurl);
                    }
                }

                ValidarErrores(resultado);

            }

            ViewData["ReturnUrl"] = returnurl;

            return View(caeVm);
        }

        /* METODO AUTENTICACIÓN DE DOS FACTORES */

        [HttpGet]
        public async Task<IActionResult> ActivarAutenticador()
        {
            string formatoUrlAutenticador = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            var usuario = await _userManager.GetUserAsync(User);

            await _userManager.ResetAuthenticatorKeyAsync(usuario);

            var token = await _userManager.GetAuthenticatorKeyAsync(usuario);

            //Habilitar código QR

            string urlAutenticador = string.Format(formatoUrlAutenticador, _urlEncoder.Encode("ProyectoIdentity"), _urlEncoder.Encode(usuario.Email), token);

            var adfModel = new Autenticacion2FViewModel() { Token = token, UrlCódigoQR = urlAutenticador };

            return View(adfModel);

		}

        /* METODO ELIMINACIÓN DE AUTENTICACION 2F */

        [HttpGet]
        public async Task<IActionResult> EliminarAutenticador()
        {
            var usuario = await _userManager.GetUserAsync(User);

            await _userManager.ResetAuthenticatorKeyAsync(usuario);

            await _userManager.SetTwoFactorEnabledAsync(usuario, false);

            return RedirectToAction(nameof(Index), "Home");

        }

        /* METODO AUTENTICADOR DE DOS FACTORES */

        [HttpPost]
        public async Task<IActionResult> ActivarAutenticador(Autenticacion2FViewModel autenticacion2FView)
        {
            var usuario = await _userManager.GetUserAsync(User);

            var succeeded = await _userManager.VerifyTwoFactorTokenAsync(usuario, _userManager.Options.Tokens.AuthenticatorTokenProvider, autenticacion2FView.Code);

            if (succeeded)
            {
                await _userManager.SetTwoFactorEnabledAsync(usuario, true);
            }
            else
            {
                ModelState.AddModelError("Error", $"Su autenticación de dos factores no ha sido válidada");
                return View(autenticacion2FView);
            }

            return RedirectToAction(nameof(ConfirmacionAutenticador));
		}

        /* METODO CONFIRMACION AUTENTICACION DOS FACTORES */

        [HttpGet]
        public IActionResult ConfirmacionAutenticador()
        {
            return View();
        }

        /* METODO VERIFICACION DE CODIGO ACCESO */

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarCodigoAutenticador(bool recordarDatos, string returnurl = null)
        {

            var usuario = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if(usuario == null)
            {
                return View("Error");
            }

            ViewData["ReturnUrl"] = returnurl;

            return View(new VerificarAutenticadorViewModel { ReturnUrl = returnurl, RecordarDatos = recordarDatos }); 
        }

        /* METODO VERIFICACION DE CODIGO */
        
		[HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> VerificarCodigoAutenticador(VerificarAutenticadorViewModel viewModel)
		{
            viewModel.ReturnUrl = viewModel.ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                return View(viewModel);
            }

            var resultado = await _signInManager.TwoFactorAuthenticatorSignInAsync(viewModel.Code, viewModel.RecordarDatos, rememberClient: true);

            if (resultado.IsLockedOut)
            {
                return View("Bloqueado");
            }

            if (resultado.Succeeded)
            {
                return LocalRedirect(viewModel.ReturnUrl);
            }

            else
            {
                ModelState.AddModelError(string.Empty, "Código invalido.");
                return View(viewModel);
            }
		}

        /* MANEJADOR DE ERRORES */

        [AllowAnonymous]
		private void ValidarErrores(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
        }


    }
}
