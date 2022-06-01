using HtmlAgilityPack;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DFKI_Researchers_Sniffer
{
    public partial class Form1 : Form
    {
        public static DFKIContext context = new DFKIContext();

        public static string baseurl = "https://www.dfki.de";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Researcher> researchers = context.Researchers.ToList();

            var list = new BindingList<Researcher>(researchers);

            dataGridView1.DataSource = list.Where(x =>
            (x.ResearcherName).ToLower().Contains(textBox1.Text.ToLower()) ||
            (x.ResearcherDepartment).ToLower().Contains(textBox1.Text.ToLower()))
            .ToList();
        }


        public static string RemoveTitle(string name)
        {
            return Regex.Replace(name, @"(\w+\.|,|-)", "").Trim();
        }


        public void UpdateDB()
        {
            string dfkiLink = "/web/forschung/forschungsbereiche";
            DFKI_Page dfki = Crawl<DFKI_Page>(dfkiLink);

            foreach (var department in dfki.Departments)
            {
                DFKI_Team teamPage = Crawl<DFKI_Team>(department.DepartmentLink);
                DFKI_Employees team = Crawl<DFKI_Employees>(teamPage.TeamLink);

                for (int i = 0; i < team.Groups.Count - 1; i++)
                {
                    for (int j = 0; j < team.Groups[i].ResearcherNames.Count; j++)
                    {
                        string personName = RemoveTitle(team.Groups[i].ResearcherNames[j]);
                        string email = team.Groups[i].ResearcherEmails[j].Replace("&#64;", "@").Replace("&#46;", ".");


                        if (context.Researchers.Where(x => x.Email == email).Count() == 0)
                        {
                                context.Researchers.Add(new Researcher(email, personName, department.DepartmentName));
                                context.SaveChanges();
          

                        }
                    }

                }

            }

        }

        public static T Crawl<T>(string link)
        {
            HtmlWeb site = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument htmlDocument = site.Load(baseurl + link);
            T page = htmlDocument.DocumentNode.GetEncapsulatedData<T>();
            return page;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateDB();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string pubsLink = "/web/forschung/projekte-publikationen/publikationen?tx_solr%5Bpage%5D=1050";
            DFKI_Publication_Page pubsPage = Crawl<DFKI_Publication_Page>(pubsLink);

            List<Researcher> dbResearchers = context.Researchers.ToList();

            foreach (var publication in pubsPage.Publications)
            {
                foreach (var personName in publication.Names)
                {
                    if (dbResearchers.Where(x => personName == x.ResearcherName).ToList().Count == 0)
                    {
                        context.Researchers.Add(new Researcher(Guid.NewGuid().ToString(), personName, "N/A"));
                        context.SaveChanges();
                    }

                }
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            //context.Database.ExecuteSqlRaw("delete from Researchers");
            //context.Database.ExecuteSqlRaw("delete from Publications");
            //context.Database.ExecuteSqlRaw("delete from HasPublication");


            foreach (var r in context.Researchers)
            {
                context.Researchers.Remove(r);
            }



            context.SaveChanges();
            MessageBox.Show("Deleted");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }


    [HasXPath]
    public class DFKI_Page
    {
        [XPath("//*[@class='manager-col-list']/li")]
        public List<DFKI_Departments> Departments { get; set; }
    }

    [HasXPath]
    public class DFKI_Departments
    {
        [XPath("/strong/a")]
        public string DepartmentName { get; set; }

        [XPath("/strong/a", "href")]
        public string DepartmentLink { get; set; }
    }

    [HasXPath]
    public class DFKI_Team
    {

        [XPath("/html/body/div[3]/div/div[2]/div[1]/div/p[1]/a", "href")]
        public string TeamLink { get; set; }
    }



    [HasXPath]
    public class DFKI_Employees
    {

        [XPath("//*[@class='person-list-view']/div")]
        public List<DFKI_Researchers> Groups { get; set; }
    }


    [HasXPath]
    public class DFKI_Researchers
    {
        [XPath("div[2]/ul/li/strong/a")]
        public List<string> ResearcherNames { get; set; }

        [XPath("div[2]/ul/li/a")]
        public List<string> ResearcherEmails { get; set; }

    }


    [HasXPath]
    public class DFKI_Publication_Page
    {
        [XPath("//*[@class='project-list-view manager-list-view']/div")]
        public List<DFKI_Publication> Publications { get; set; }
    }

    [HasXPath]
    public class DFKI_Publication
    {
        [XPath("div/div[1]/span/a")]
        public List<string> Names { get; set; }

        [XPath("div/div[2]/h3/a")]
        public string PublicationName { get; set; }
    }

}