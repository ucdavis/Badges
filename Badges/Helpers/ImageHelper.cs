using System.Web.Configuration;
using SoundInTheory.DynamicImage.Fluent;

namespace Badges.Helpers
{
    public static class ImageHelper
    {
        public static string GetImage(string url, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            var composition =
                new CompositionBuilder().WithLayer(
                    LayerBuilder.Image.SourceUrl(url).WithFilter(FilterBuilder.Resize.To(width, height)));

            return composition.Url;
        }

        public static string GetProfileImage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return GetImage(WebConfigurationManager.AppSettings["DefaultProfilePictureUrl"], 60, 60);
            }

            return GetImage(url, 60, 60);
        }
    }
}