using EAScraperConnector.Dtos;
using EAScraperConnector.Models;

namespace EAScraperConnector.Mappers
{
    public static class EFCoreMapper
    {
        public static List<Property> Map(this IList<House> houses)
        {
            var properties = new List<Property>();

            foreach (var house in houses)
            {
                try
                { 
                    var prx = Double.TryParse(house.Price.Replace("£", "").Replace(",", ""), out var price) ? price : 0;

                    //price POA causes the price to break
                    properties.Add(new Property()
                    {
                        Description = $"{ DateTime.Now.ToString()}{ house.Description}",
                        Price = prx,
                        Area = String.IsNullOrEmpty(house.Area) ? "" : house.Area,
                        Link = house.Link,
                        Deposit = house.Deposit
                    });
                }
                catch (Exception ex)
                {

                }
            }

            return properties;
        }

        public static Audit Map(this int postCodeCounter, string postcode, 
            string price, Enums.EstateAgent site)
        {
            return new Audit()
            {
                Site = (int)site,
                Postcode = postcode,
                UniqueResultsCount = postCodeCounter,
                Price = Convert.ToInt32(price),
                Date = DateTime.Now
            };
        }
    }
}
