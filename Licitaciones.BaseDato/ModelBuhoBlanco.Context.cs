﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ModelBuhoBlancoEntities : DbContext
    {
        public ModelBuhoBlancoEntities()
            : base("name=ModelBuhoBlancoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SegAccess> SegAccess { get; set; }
        public virtual DbSet<SegUser> SegUser { get; set; }
        public virtual DbSet<SegUserProfile> SegUserProfile { get; set; }
        public virtual DbSet<SegProfileAccess> SegProfileAccess { get; set; }
    }
}
