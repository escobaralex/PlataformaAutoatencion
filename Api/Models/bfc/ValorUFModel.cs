namespace Api.Models.bfc
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ValorUFModel : DbContext
    {
        public ValorUFModel()
            : base("name=ValorUFModel")
        {
        }

        public virtual DbSet<ValorUF> ValorUF { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValorUF>()
                .Property(e => e.Valor)
                .HasPrecision(10, 2);
        }
    }
}
