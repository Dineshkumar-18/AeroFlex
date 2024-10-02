using AeroFlex.Models;

namespace AeroFlex.Dtos
{
    public class PassengerDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public PassengerType PassengerType { get; set; }
    }
}
