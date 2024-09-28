using System.Security.Cryptography.Pkcs;

namespace AeroFlex.Dtos
{
    public class TicketDto
    {
        public int TicketId { get; set; }
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string PassengerName { get; set; }
        public string AirlineName { get; set; }
        public string FlightNumber  { get; set; }
        public string ClassName { get; set; }
        public string SeatNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateOnly DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public DateOnly ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public decimal TicketPrice { get; set; }
    }
}
