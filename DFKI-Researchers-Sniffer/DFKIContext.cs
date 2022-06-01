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

        public Researcher(string _id, string rName, string depName)
        {
            rid = _id;
            ResearcherName = rName;
            ResearcherDepartment = depName;
        }


        [Key]
        public string rid { get; set; }
        public string ResearcherName { get; set; }
        public string ResearcherDepartment { get; set; }
        public string Email { get; set; }

    }

}
