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
    
    public partial class SegAccess
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SegAccess()
        {
            this.AudLogAccion = new HashSet<AudLogAccion>();
        }
    
        public int Id { get; set; }
        public short Type_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public short Posicion { get; set; }
        public Nullable<int> Parent_Access_Id { get; set; }
        public int IsDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AudLogAccion> AudLogAccion { get; set; }
    }
}