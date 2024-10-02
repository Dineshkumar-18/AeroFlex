namespace AeroFlex.Dtos
{
    public class SeatLayoutDto
    {
       
        public int FlightId { get; set; }
        public int TotalColumns { get; set; }
        public string LayoutPattern { get; set; }
        public string SeatTypePattern { get; set; }
        public string ClassType { get; set; }
        public int RowCount { get; set; }
    }
}
