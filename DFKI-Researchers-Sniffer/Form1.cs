using HtmlAgilityPack;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;

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


        public static void UpdateDB()
        {
            string dfkiLink = "/web/forschung/forschungsbereiche";
            DFKI_Page dfki = Crawl<DFKI_Page>(dfkiLink);

            //List<string> list = new List<string>();
            foreach (var department in dfki.Departments)
            {
                DFKI_Team teamPage = Crawl<DFKI_Team>(department.DepartmentLink);
                DFKI_Employees team = Crawl<DFKI_Employees>(teamPage.TeamLink);

                Console.WriteLine(department.DepartmentName);

                for (int i = 0; i < team.Groups.Count - 1; i++)
                {
                    //list.AddRange(team.Groups[i].ResearcherNames);

                    foreach (var groupMember in team.Groups[i].ResearcherNames)
                    {
                        string personName = RemoveTitle(groupMember);
                        context.Researchers.Add(new Researcher(Guid.NewGuid().ToString(), personName, department.DepartmentName));

                        context.SaveChanges();
                    }
                }

            }
            //return list;
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
    }

}