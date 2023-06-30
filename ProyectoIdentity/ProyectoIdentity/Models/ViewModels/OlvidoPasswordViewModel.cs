using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProyectoIdentity.Models.ViewModels
{
	public class OlvidoPasswordViewModel
	{
		[Required(ErrorMessage = "¡El correo es obligatorio!")]
		[EmailAddress(ErrorMessage = "¡Ingrese una dirección de correo válida!")]
		[DisplayName("Correo")]
		public string Email { get; set; }
	}
}
