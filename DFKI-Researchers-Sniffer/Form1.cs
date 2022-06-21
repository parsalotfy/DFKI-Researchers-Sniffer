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
            var rlist = new BindingList<Researcher>(researchers);
            dataGridView1.DataSource = rlist.Where(x =>
                (x.Name).ToLower().Contains(textBox1.Text.ToLower()) ||
                (x.Department).ToLower().Contains(textBox1.Text.ToLower()))
                .ToList();


            List<HasPublication> publications = context.HasPublications.ToList();
            var plist = new BindingList<HasPublication>(publications);
            dataGridView2.DataSource = plist.Where(x =>
                (x.PublicationName).ToLower().Contains(textBox1.Text.ToLower()))
                .ToList();
        }

        public static string RemoveTitle(string name)
        {
            return Regex.Replace(name, @"(\w+\.|,|-)", "").Trim();
        }

        public void UpdateResearchers()
        {
            MessageBox.Show("Fetching Researchers.");

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

        public void UpdateHasPublications(int from_page, int to_page)
        {
            MessageBox.Show("Fetching Publications.");

            // last page is 1050
            List<Researcher> dbResearchers = context.Researchers.ToList();

            // 1 to 1051
            for (int i = from_page; i <= to_page; i++)
            {
                label2.Invoke(() => label2.Text = $"Page: {i.ToString()}");

                string pubsLink = $"/web/forschung/projekte-publikationen/publikationen?tx_solr%5Bpage%5D={i}";
                DFKI_Publication_Page pubsPage = Crawl<DFKI_Publication_Page>(pubsLink);

                foreach (var publication in pubsPage.Publications)
                {
                    string pid = Guid.NewGuid().ToString();

                    foreach (var personName in publication.Names)
                    {
                        // Add people who are not in Researchers Table
                        if (dbResearchers.Where(x => personName == x.Name).ToList().Count == 0)
                        {
                            // I should Clear name noise here
                            context.Researchers.Add(new Researcher(Guid.NewGuid().ToString(), personName, "N/A"));
                            context.SaveChanges();
                            dbResearchers = context.Researchers.ToList();
                        }

                        if (context.Publications.Where(x => x.Name == publication.PublicationName).ToList().Count() == 0)
                        {
                            context.Publications.Add(new Publication(publication.PublicationName));
                            context.SaveChanges();
                        }



                        // Add publications
                        List<Researcher> researchers = dbResearchers.Where(x => x.Name == personName).ToList();
                        foreach (var researcher in researchers)
                        {
                            if (context.HasPublications.Where(x => x.Email == researcher.Email && x.PublicationName == publication.PublicationName).ToList().Count() == 0)
                            {
                                context.HasPublications.Add(new HasPublication(Guid.NewGuid().ToString(), researcher.Email, publication.PublicationName));
                                context.SaveChanges();
                            }
                        }


                    }
                }
                double progress = ((double)(i - from_page) / (double)(to_page - from_page)) * 100;
                backgroundWorker1.ReportProgress((int)progress);

                int _ = 0;
                Thread.Sleep(500);
                if (pubsPage.Publications.Count < 10)
                {
                    textBox2.Invoke(()=> textBox2.Text = textBox2.Text + i.ToString() + "--" + pubsPage.Publications.Count + "\n");
                    i--;
                }
            }
            button2.Invoke(() => button2.Enabled = true);
        }

        public void UpdateDB()
        {
            UpdateResearchers();
            UpdateHasPublications(1,1051);
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
            backgroundWorker1.RunWorkerAsync();
            button2.Enabled = false;
        }


        public void TruncateAllTables()
        {
            foreach (var r in context.Researchers)
            {
                context.Researchers.Remove(r);
            }
            foreach (var link in context.HasPublications)
            {
                context.HasPublications.Remove(link);
            }

            context.SaveChanges();
        }


        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateDB();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TruncateAllTables();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
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