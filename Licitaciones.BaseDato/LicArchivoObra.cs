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
    
    public partial class LicArchivoObra
    {
        public int idLicArchivoObra { get; set; }
        public Nullable<int> idObra { get; set; }
        public string nombreArchivo { get; set; }
        public string rutaArchivo { get; set; }
        public Nullable<int> idCategoria { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> esFracasada { get; set; }
    }
}
