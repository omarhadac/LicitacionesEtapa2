//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Licitaciones.BaseDato
{
    using System;
    using System.Collections.Generic;
    
    public partial class LicConsultas
    {
        public int idConsulta { get; set; }
        public Nullable<int> idObra { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> idEmpresa { get; set; }
        public string razonSocial { get; set; }
        public string detalle { get; set; }
        public Nullable<int> idPregunta { get; set; }
        public Nullable<int> esFracasada { get; set; }
        public Nullable<int> idUsuario { get; set; }
    }
}