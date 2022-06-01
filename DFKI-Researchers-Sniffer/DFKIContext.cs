using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DFKI_Researchers_Sniffer
{
    public class DFKIContext : DbContext
    {
        public DbSet<Researcher>? Researchers { get; set; }

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
            ResearcherName = rName;
            ResearcherDepartment = depName;
        }


        [Key]
        public string Email { get; set; }

        public string ResearcherName { get; set; }
        public string ResearcherDepartment { get; set; }


    }

}
