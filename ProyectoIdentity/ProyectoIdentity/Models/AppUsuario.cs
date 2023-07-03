using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProyectoIdentity.Models
{
    [Table("Usuarios")]
    public class AppUsuario : IdentityUser
    {
        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Primer Nombre")]
        public string? FirstName { get; set; }

        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Segundo Nombre")]   
        public string? SecondName { get; set; }

        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z0-9\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Primer Apellido")]
        public string? FirstLastName { get; set; }

        [MaxLength(100, ErrorMessage = "¡La longitud máxima permitida es de 100 caracteres!")]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "¡Solo se permiten letras y espacios en blanco!")]
        [DisplayName("Segundo Apellido")]
        public string? SecondLastName { get; set; }

        [MaxLength(50, ErrorMessage = "¡La longitud máxima permitida es de 50 caracteres!")]
        [DisplayName("Dirección")]
        public string? Address { get; set; }

        [MaxLength(10, ErrorMessage = "¡La longitud máxima permitida es de 10 caracteres!")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "¡Solo se permiten letras!")]
        [DisplayName("Sexo")]
        public string? Sex { get; set; }

        [DisplayName("Fecha de Nacimiento")]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Estado")]
        public bool? State { get; set; }

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
