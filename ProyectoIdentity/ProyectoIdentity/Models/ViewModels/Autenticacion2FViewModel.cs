using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
	public class Autenticacion2FViewModel
	{
		// Para el acceso (Login)

		[Required]
		[DisplayName("Código autenticador")]
        public string Code { get; set; }

        //Para registro 

        public string Token { get; set; }

        //Para código QR

        public string UrlCódigoQR { get; set; }
    }
}
