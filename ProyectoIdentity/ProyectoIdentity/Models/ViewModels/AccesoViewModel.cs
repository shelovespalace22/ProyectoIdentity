using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProyectoIdentity.Models.ViewModels
{
    public class AccesoViewModel
    {
        [Required(ErrorMessage = "¡El correo es obligatorio!")]
        [EmailAddress(ErrorMessage = "¡Ingrese una dirección de correo válida!")]
        [DisplayName("Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "¡La clave es obligatoria!")]
        [DataType(DataType.Password)]
        [DisplayName("Clave")]
        public string Password { get; set; }

        [DisplayName("Recordar Datos")]
        public bool RememberMe { get; set; }
    }
}
