using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class VerificarAutenticadorViewModel
    {
        [Required]
        [DisplayName("Código Autenticador")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [DisplayName("Recordar Datos")]
        public bool RecordarDatos { get; set; }

    }
}
