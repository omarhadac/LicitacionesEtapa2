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
    
    public partial class PryLicitacion
    {
        public int Id { get; set; }
        public Nullable<int> Plazo { get; set; }
        public Nullable<decimal> ValorPliego { get; set; }
        public Nullable<System.DateTime> FechaPublicacionDesde { get; set; }
        public Nullable<System.DateTime> FechaPublicacionHasta { get; set; }
        public string Radicacion { get; set; }
        public Nullable<System.DateTime> FechaApertura { get; set; }
        public Nullable<System.DateTime> FechaContrato { get; set; }
        public Nullable<decimal> MontoOficial { get; set; }
        public Nullable<decimal> MontoContratado { get; set; }
        public Nullable<int> IdPryProyecto { get; set; }
        public Nullable<int> TipoDeContratacion_Id { get; set; }
        public Nullable<decimal> Diferencial { get; set; }
        public string CUG { get; set; }
        public Nullable<System.DateTime> FechaLic1 { get; set; }
        public Nullable<System.DateTime> FechaLic2 { get; set; }
        public Nullable<System.DateTime> FechaLic3 { get; set; }
        public Nullable<int> TipoPlazo { get; set; }
        public Nullable<System.DateTime> FechaProrroga { get; set; }
        public string Hora { get; set; }
        public Nullable<System.DateTime> NuevaFechaApertura { get; set; }
        public Nullable<decimal> NuevoMonto { get; set; }
    }
}
