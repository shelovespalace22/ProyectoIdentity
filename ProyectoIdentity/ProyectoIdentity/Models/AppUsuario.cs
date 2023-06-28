using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProyectoIdentity.Models
{
    [Table("Usuarios")]
    public class AppUsuario : IdentityUser
    {
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

        [Required(ErrorMessage = "¡Seleccione un estado!")]
        [DisplayName("Estado")]
        public bool State { get; set; }

        [Required]
        [DisplayName("Creación del Registro")]
        public DateTime CreationDate { get; set; }

        [Required]
        [DisplayName("Modificación del Registro")]
        public DateTime ModificationDate { get; set; }

        public AppUsuario()
        {
            State = true;
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
        }
    }
}
