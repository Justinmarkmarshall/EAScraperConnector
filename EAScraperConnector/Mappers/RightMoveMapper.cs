using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EAScraperConnector.Models;

namespace EAScraperConnector.Mappers
{
    public static class RightMoveMapper
    {
        public static List<House> MapRM(this IHtmlCollection<IElement> propertiesDiv)
        {
            var houses = new List<House>();

           try
            {
                foreach (var propertyCard in propertiesDiv[0].Children)
                {
                    //var description = propertyCard.GetElementsByClassName("property-information");

                    //if (description.Any())
                    //{
                    //    if (description[0].GetElementsByTagName("title").Any())
                    //    {
                    //        description = description.GetElementsByTagName("title")[0];
                    //    }
                    //}
                    

                    var link = propertyCard.QuerySelector("a").Id;
                    if (!propertyCard.InnerHtml.ToLower().Contains("hotel")
                        && !propertyCard.InnerHtml.ToLower().Contains("retirement")
                        && !propertyCard.InnerHtml.ToLower().Contains("investment only")
                        && !propertyCard.InnerHtml.ToLower().Contains("cash buyers only")
                        && !propertyCard.InnerHtml.ToLower().Contains("shared ownership")
                        && !propertyCard.InnerHtml.ToLower().Contains("share")) houses.Add(new House()
                        {
                            Description = "",
                            Price = propertyCard.GetElementsByClassName("propertyCard-priceValue")[0].InnerHtml,
                            Area = propertyCard.GetElementsByClassName("propertyCard-address")[0].GetElementsByTagName("span")[0].InnerHtml,
                            Images = propertyCard.GetElementsByTagName("img").Map(),
                            Link = $"https://www.rightmove.co.uk/properties/{link.Replace("prop", "")}#/?channel=RES_BUY",

                        });
                }

                return houses;
            }
            catch (Exception ex)
            {
                return houses;
            }
        }

        private static List<string> Map(this IHtmlCollection<IElement> picsHtml)
        {
            List<string> images = new List<string>();
            foreach (IHtmlImageElement pic in picsHtml)
            {
                images.Add(pic.Source);
            }
            return images;
        }
    }
}
