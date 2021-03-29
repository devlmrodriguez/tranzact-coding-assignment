namespace TranzactCodingAssignment.Models
{
    public class PageView
    {
        public string Language { get; set; }
        public string Domain { get; set; }
        public string Title { get; set; }
        public int CountViews { get; set; }

        public PageView(string language, string domain, string title, int countViews)
        {
            Language = language;
            Domain = domain;
            Title = title;
            CountViews = countViews;
        }
    }
}
