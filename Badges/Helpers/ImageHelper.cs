using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}