using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licitaciones.ViewModels
{
    public class UsuarioAudit
    {
        public int idUser { get; set; }
        public string cuil { get; set; }
        public string nombreApellido { get; set; }
        public string apeApoderado { get; set; }
        public string telefono { get; set; }
        public string mail { get; set; }
        public string pasword { get; set; }
        public string lastLogIn { get; set; }
        public int activo { get; set; }
        public string fechaAlta { get; set; }
        public string fechaBaja { get; set; }
        public int Rol { get; set; }
        public string nombreEmpresa { get; set; }
        public int IdEmpresaUser { get; set; }
        public int IdUserAccion { get; set; }
        public int IdEmpresa { get; set; }

    }
}
