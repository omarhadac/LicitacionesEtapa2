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
    
    public partial class SegUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SegUser()
        {
            this.AudLogAccion = new HashSet<AudLogAccion>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public string Email { get; set; }
        public short SessionOpen { get; set; }
        public Nullable<long> State_Type_Id { get; set; }
        public short FailLoginCount { get; set; }
        public byte[] Photo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> SegOffice_Id { get; set; }
        public Nullable<int> SegVisualiza_Id { get; set; }
        public int InfoVersion { get; set; }
        public string Company { get; set; }
        public string numeroContacto { get; set; }
        public string motivoRegistro { get; set; }
        public string tokenSecurity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AudLogAccion> AudLogAccion { get; set; }
    }
}
