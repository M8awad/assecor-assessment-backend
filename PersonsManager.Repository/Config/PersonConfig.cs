using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonsManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsManager.Repository.Config
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons");
            builder.Property(e => e.Id).UseIdentityColumn();  // This makes it auto-increment

            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.ZipCode).HasMaxLength(20);
            builder.Property(e => e.City).HasMaxLength(100);
            builder.Property(e => e.Color).HasMaxLength(50);
        }
    }
}
