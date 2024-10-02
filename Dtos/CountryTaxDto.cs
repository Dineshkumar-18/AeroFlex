using AeroFlex.Models;

namespace AeroFlex.Dtos
{
    public class CountryTaxDto
    {
        public int CountryId { get; set; }
        public string TravelType { get; set; }
        public decimal Rate { get; set; }
    }
}
