namespace AeroFlex.Dtos
{
    public class PaymentDetailsByFlightSchduleDto
    {
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int TotalSeatsAvailable { get; set; }
        public List<FlightClassDto> FlightClasses { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal CancellatonChargesRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountsApplied { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal RefundsIssued { get; set; }
        public decimal FinalAmountPayable { get; set; }
    }

    public class FlightClassDto
    {
        public string ClassName { get; set; }
        public int SeatsSold { get; set; }
        public decimal ClassRevenue { get; set; }
    }
}
