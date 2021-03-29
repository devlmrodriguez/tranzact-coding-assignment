using System;
using System.Collections.Generic;
using System.IO;
using TranzactCodingAssignment.Models.Results;

namespace TranzactCodingAssignment.Services
{
    public class PageViewService
    {
        private PageViewFileService m_PageViewFileService;
        private PageViewResultService m_PageViewResultService;
        private Dictionary<string, string> m_DomainToName;

        public PageViewService(PageViewFileService pageViewFileService, PageViewResultService pageViewResultService)
        {
            //Inject our dependencies manually
            m_PageViewFileService = pageViewFileService;
            m_PageViewResultService = pageViewResultService;

            //Try loading cached results
            m_PageViewResultService.LoadResults();

            //Build dictionary for easy translation from domain to name
            m_DomainToName = new Dictionary<string, string>
            {
                {"", "wikipedia" },
                {"b", "wikibooks" },
                {"d", "wiktionary" },
                {"f", "wikimediafoundation" },
                {"m", "wikimedia" },
                {"n", "wikinews" },
                {"q", "wikiquote" },
                {"s", "wikisource" },
                {"v", "wikiversity" },
                {"voy", "wikivoyage" },
                {"w", "mediawiki" },
                {"wd", "wikidata" }
            };
        }

        public void PrintResults(int numberOfHours)
        {
            //Convert to UTC and compute until previous hour, since current hour data is not available yet
            DateTime currentDateTime = DateTime.Now.ToUniversalTime().AddHours(-1);
            //Remove minutes and seconds from previous hour
            DateTime flooredDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, 00, 00);

            List<PageViewResult1> results1 = new List<PageViewResult1>();
            List<PageViewResult2> results2 = new List<PageViewResult2>();

            //Start from earliest hour (current hour - number of hours)
            flooredDateTime = flooredDateTime.AddHours(-numberOfHours);
            for (int i = 0; i < numberOfHours; i++)
            {
                //Keep incrementing hour by 1
                flooredDateTime = flooredDateTime.AddHours(1);

                //Get important values from formatted date directly
                string year = flooredDateTime.ToString("yyyy");
                string month = flooredDateTime.ToString("MM");
                string day = flooredDateTime.ToString("dd");
                string hour = flooredDateTime.ToString("HH");

                //Set page view name and Uri
                string pageViewName = $"pageviews-{year}{month}{day}-{hour}0000.gz";
                string pageViewUri = $"https://dumps.wikimedia.org/other/pageviews/{year}/{year}-{month}/{pageViewName}";

                //Get file names for later use
                FileInfo fileInfo = new FileInfo(pageViewName);
                string fileName = fileInfo.Name;
                string decompressedFileName = fileName.Remove(fileName.Length - fileInfo.Extension.Length);

                //Try getting cached results, if they exist
                PageViewResult1 result1 = m_PageViewResultService.GetResult1(pageViewName);
                PageViewResult2 result2 = m_PageViewResultService.GetResult2(pageViewName);

                //If there aren't cached results, compute them
                if (result1 == null || result2 == null)
                {
                    m_PageViewFileService.DownloadFile(pageViewUri, pageViewName);
                    m_PageViewFileService.DecompressFile(fileName, decompressedFileName);
                    m_PageViewFileService.ReadFile(decompressedFileName);

                    result1 = m_PageViewResultService.ComputeResult1(pageViewName, flooredDateTime, m_PageViewFileService.PageViews);
                    result2 = m_PageViewResultService.ComputeResult2(pageViewName, flooredDateTime, m_PageViewFileService.PageViews);

                    m_PageViewFileService.CleanFiles(fileName, decompressedFileName);
                }

                results1.Add(result1);
                results2.Add(result2);
            }

            //Don't forget to cache results (save them in json format)
            m_PageViewResultService.SaveResults();

            //Print results to screen
            Console.WriteLine(string.Format("{0,-10}{1,-15}{2,-20}{3,-10}", "Period", "Language", "Domain", "ViewCount"));
            for (int i = 0; i < results1.Count; i++)
                Console.WriteLine(string.Format("{0,-10}{1,-15}{2,-20}{3,-10}", results1[i].Period, results1[i].Language, m_DomainToName[results1[i].Domain], results1[i].ViewCount));
            Console.WriteLine();

            Console.WriteLine(string.Format("{0,-10}{1,-20}{2,-10}", "Period", "Page", "ViewCount"));
            for (int i = 0; i < results1.Count; i++)
                Console.WriteLine(string.Format("{0,-10}{1,-20}{2,-10}", results2[i].Period, results2[i].Page, results2[i].ViewCount));
            Console.WriteLine();
        }
    }
}
