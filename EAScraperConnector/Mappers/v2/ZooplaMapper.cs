using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EAScraperConnector.Models;

namespace EAScraperConnector.Mappers.v2
{
    public static class ZooplaMapper
    {
        private const string PriceClass = "css-xz7r6w-Price";
        private const string AddressClass = "css-1uvt63a-Address";
        public static List<House> v2MapZ(this IHtmlCollection<IElement> propertiesDiv)
        {
            var lstReturn = new List<House>();

            foreach (var propertyCard in propertiesDiv[0].Children)
            {
                var pid = "0";
                var aTags = propertyCard.QuerySelector("a")?.OuterHtml.Split("=");
                if (aTags != null)
                {
                    pid = aTags[2]?.Split("/")[3];
                }

                var imgs = propertyCard.GetElementsByTagName("picture");

                if (imgs.Any())
                {
                    //cool we can use the existing mapper
                }
                else
                {
                    //lets look for the img tags
                }

                if (!propertyCard.InnerHtml.ToLower().Contains("hotel")
                    && !propertyCard.InnerHtml.ToLower().Contains("retirement")
                    && !propertyCard.InnerHtml.ToLower().Contains("investment only")
                    && !propertyCard.InnerHtml.ToLower().Contains("cash buyers only")
                    && !propertyCard.InnerHtml.ToLower().Contains("shared ownership")
                    && !propertyCard.InnerHtml.ToLower().Contains("share")) lstReturn.Add(new House()
                    {
                        Description = propertyCard.GetElementsByTagName("h2")[0].InnerHtml,
                        Price = propertyCard.GetElementsByClassName(PriceClass)[0].InnerHtml,
                        Area = propertyCard.GetElementsByClassName(AddressClass)[0].InnerHtml,
                        Images = propertyCard.GetElementsByTagName("picture").Map(),
                        Link = $"https://www.zoopla.co.uk/for-sale/details/{pid}",                        
                        MonthlyRepayments = 0,                        
                        Deposit = 0
                    });
            }
            return lstReturn;
        }

        private static List<string> Map(this IHtmlCollection<IElement> pics)
        {
            var images = new List<string>();

            foreach (var pic in pics)
            {
                foreach (IHtmlSourceElement img in pic.GetElementsByTagName("source"))
                {
                    string sourceSet = img.SourceSet;
                    foreach (string url in sourceSet.Split(" "))
                    {
                        if (url.Contains("jpg"))
                        {
                            images.Add(url);
                        }
                    }
                }
            }
            return images;
        }
    }
}
