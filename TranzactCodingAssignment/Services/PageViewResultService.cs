using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using TranzactCodingAssignment.Models;
using TranzactCodingAssignment.Models.Results;

namespace TranzactCodingAssignment.Services
{
    public class PageViewResultService
    {
        private const string m_FileName1 = "page-view-results-1";
        private const string m_FileName2 = "page-view-results-2";
        private Dictionary<string, PageViewResult1> m_PageViewResults1;
        private Dictionary<string, PageViewResult2> m_PageViewResults2;

        public PageViewResultService()
        {
            m_PageViewResults1 = new Dictionary<string, PageViewResult1>();
            m_PageViewResults2 = new Dictionary<string, PageViewResult2>();
        }

        public void SaveResults()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            //Serialize our dictionary (cached results) to json format
            string pageViewResults1String = JsonSerializer.Serialize(m_PageViewResults1, options);
            string pageViewResults2String = JsonSerializer.Serialize(m_PageViewResults2, options);

            File.WriteAllText(m_FileName1, pageViewResults1String);
            File.WriteAllText(m_FileName2, pageViewResults2String);
        }

        public void LoadResults()
        {
            //Try reading cached values from our json format files
            if (File.Exists(m_FileName1))
            {
                string pageViewResults1String = File.ReadAllText(m_FileName1);
                m_PageViewResults1 = JsonSerializer.Deserialize<Dictionary<string, PageViewResult1>>(pageViewResults1String);
            }

            if (File.Exists(m_FileName2))
            {
                string pageViewResults2String = File.ReadAllText(m_FileName2);
                m_PageViewResults2 = JsonSerializer.Deserialize<Dictionary<string, PageViewResult2>>(pageViewResults2String);
            }
        }

        public PageViewResult1 ComputeResult1(string key, DateTime dateTime, List<PageView> pageViews)
        {
            Console.WriteLine($"Computing {key} result 1");

            //Get hour in 1 digit AM/PM local time format
            string period = dateTime.ToLocalTime().ToString("htt", CultureInfo.InvariantCulture);
            //Language & Domain trailing part - display the max viewed count for language & domain combination
            PageViewResult1 result1 = (from pageView in pageViews
                                       group pageView by (pageView.Language, pageView.Domain)
                                       into pageViewGroup1
                                       select new PageViewResult1
                                       {
                                           Period = period,
                                           Language = pageViewGroup1.First().Language,
                                           Domain = pageViewGroup1.First().Domain,
                                           ViewCount = pageViewGroup1.Sum(x => x.CountViews)
                                       }
                                       into pageViewGroup2
                                       orderby pageViewGroup2.ViewCount descending
                                       select pageViewGroup2).First();

            m_PageViewResults1[key] = result1;

            Console.WriteLine($"Computed {key} result 1");
            Console.WriteLine();
            return result1;
        }

        public PageViewResult1 GetResult1(string key)
        {
            if (m_PageViewResults1.ContainsKey(key))
                return m_PageViewResults1[key];

            return null;
        }

        public PageViewResult2 ComputeResult2(string key, DateTime dateTime, List<PageView> pageViews)
        {
            Console.WriteLine($"Computing {key} result 2");

            //Get hour in 1 digit AM/PM local time format
            string period = dateTime.ToLocalTime().ToString("htt", CultureInfo.InvariantCulture);
            //Page title with max count of views per page (should include all languages)
            PageViewResult2 result2 = (from pageView in pageViews
                                       group pageView by pageView.Title
                                       into pageViewGroup1
                                       select new PageViewResult2
                                       {
                                           Period = period,
                                           Page = pageViewGroup1.First().Title,
                                           ViewCount = pageViewGroup1.Sum(x => x.CountViews)
                                       }
                                       into pageViewGroup2
                                       orderby pageViewGroup2.ViewCount descending
                                       select pageViewGroup2).First();

            m_PageViewResults2[key] = result2;

            Console.WriteLine($"Computed {key} result 2");
            Console.WriteLine();
            return result2;
        }

        public PageViewResult2 GetResult2(string key)
        {
            if (m_PageViewResults2.ContainsKey(key))
                return m_PageViewResults2[key];

            return null;
        }
    }
}
