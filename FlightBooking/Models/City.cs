using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlightBooking.Models
{
    public class City
    {

      public static List<string> GetCities()
        {
            return new List<string>
            {
            "Beograd",
            "Nis",
            "Pristina",
            "Kraljevo"
            };
        }
    }
}
