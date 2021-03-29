using TranzactCodingAssignment.Services;

namespace TranzactCodingAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            PageViewFileService pageViewFileService = new PageViewFileService();
            PageViewResultService pageViewResultService = new PageViewResultService();
            PageViewService pageViewService = new PageViewService(pageViewFileService, pageViewResultService);

            pageViewService.PrintResults(numberOfHours: 5);
        }
    }
}
