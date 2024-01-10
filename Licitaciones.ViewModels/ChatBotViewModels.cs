using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licitaciones.ViewModels
{
    public class ChatBotViewModels
    {
        public int? idChat { get; set; }
        public int? idPadre { get; set; }
        public string TituloPpal { get; set; }
        public string Opcion { get; set; }
        public string MensajePpal { get; set; }
        public int IdLicChatbot { get; set; }
        public string TituloOpcion { get; set; }
        
        public int? HijoDe { get; set; }
        public int? IdEscenario { get; set; }
        public int? tipo { get; set; }
        public int? esFin { get; set; }
        public int? tipoPregunta { get; set; }
        public List<ChatBotViewModels> Hijos { get; set; } = new List<ChatBotViewModels>();

    }
    public class EscenariosChatBotViewModels
    {
        public int? idEscenario { get; set; }
        public string sector { get; set; }
        public string descripcion { get; set; }
        public string activo { get; set; }
        public string accion { get; set; }
        public string nombreEscenario { get; set; }
    }
    public class ComentariosViewModels
    {
        public string comentario { get; set; }
        public int? idComentario { get; set; }
        public int? idUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public DateTime? fechaHora { get; set; }
    }
}
