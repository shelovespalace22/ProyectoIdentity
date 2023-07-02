using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProyectoIdentity.Models.ViewModels
{
	public class RecuperaPasswordViewModel
	{
		[Required(ErrorMessage = "¡El correo es obligatorio!")]
		[EmailAddress(ErrorMessage = "¡Ingrese una dirección de correo válida!")]
		[DisplayName("Correo")]
		public string Email { get; set; }

		[Required(ErrorMessage = "¡La clave es obligatoria!")]
		[DataType(DataType.Password)]
		[DisplayName("Clave")]
		public string Password { get; set; }

		[Required(ErrorMessage = "¡La confirmación de la clave es obligatoria!")]
		[Compare("Password", ErrorMessage = "¡Las contraseñas no coinciden!")]
		[DataType(DataType.Password)]
		[DisplayName("Confirmar Clave")]
		public string ConfirmedPassword { get; set; }

		public string Code { get; set; }
	}
}
