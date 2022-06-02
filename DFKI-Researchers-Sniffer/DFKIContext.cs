using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DFKI_Researchers_Sniffer
{
    public class DFKIContext : DbContext
    {
        public DbSet<Researcher> Researchers { get; set; }
        public DbSet<HasPublication> HasPublications { get; set; }

        public string DbPath { get; }

        public DFKIContext()
        {
            DbPath = Application.StartupPath + "dfki.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
       => options.UseSqlite($"Data Source={DbPath}");
    }

    public class Researcher
    {
        public Researcher()
        {

        }

        public Researcher(string email,string rName, string depName)
        {
            Email = email;
            Name = rName;
            Department = depName;
        }

        [Key]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
    }

    public class HasPublication
    {
        public HasPublication()
        {

        }

        
        public HasPublication(string _id, string email, string pName)
        {
            id = _id;
            Email = email;
            PublicationName = pName;
        }

        [Key]
        public string id { get; set; }
        public string Email { get; set; }
        public string PublicationName { get; set; }
    }
}
