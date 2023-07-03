using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
	public class ConfirmacionAccesoExternoViewModel
	{
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
