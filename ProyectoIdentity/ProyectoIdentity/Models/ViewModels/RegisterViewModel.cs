using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "¡El correo es obligatorio!")]
        [EmailAddress(ErrorMessage = "¡Ingrese una dirección de correo válida!")]
        [DisplayName("Correo")]
        public string Email { get; set; }

        [Required(ErrorMessage = "¡La clave es obligatoria!")]
        [StringLength(50, ErrorMessage = "La longitud máxima de la {0} es de {2} caracteres", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [DisplayName("Clave")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "¡La confirmación de la clave es obligatoria!")]
        [Compare("Password", ErrorMessage = "¡Las contraseñas no coinciden!")]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar Clave")]
        public string ConfirmedPassword { get; set; }

        [Required(ErrorMessage = "¡Ingrese su primer nombre!")]
        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Primer Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "¡Ingrese su segundo nombre!")]
        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Segundo Nombre")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "¡Ingrese su primer apellido!")]
        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z0-9\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Primer Apellido")]
        public string FirstLastName { get; set; }

        [Required(ErrorMessage = "¡Ingrese su segundo apellido!")]
        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Segundo Apellido")]
        public string SecondLastName { get; set; }

        [Required(ErrorMessage = "¡Ingrese su dirección!")]
        [MaxLength(50, ErrorMessage = "¡La longitud máxima permitida es de 50 caracteres!")]
        [DisplayName("Dirección")]
        public string Address { get; set; }

        [Required(ErrorMessage = "¡Ingrese su sexo Masculino o Femenino!")]
        [MaxLength(10, ErrorMessage = "¡La longitud máxima permitida es de 10 caracteres!")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "¡Solo se permiten letras!")]
        [DisplayName("Sexo")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "¡Ingrese su fecha de nacimiento!")]
        [DisplayName("Fecha de Nacimiento")]
        public DateTime BirthDate { get; set; }

    }
}
