using System.Web;
namespace Badges.Models.Student
{
    public class SupportingWorkModel
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string Url { get; set; }

        public HttpPostedFileBase WorkFile { get; set; }
    }
}