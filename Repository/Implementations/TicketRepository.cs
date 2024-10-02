using AeroFlex.Data;
using AeroFlex.Dtos;
using AeroFlex.Models;
using AeroFlex.Repository.Contracts;
using AeroFlex.Response;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Security;

namespace AeroFlex.Repository.Implementations
{
    public class TicketRepository(ApplicationDbContext _context,IConfiguration _configuration) : ITicketRepository
    {
        public async Task<List<TicketDto>> GenerateTicket(int flightScheduleId,int bookingId)
        {
                
                var book=await _context.Bookings.Include(b=>b.User).FirstOrDefaultAsync(b=>b.BookingId==bookingId);
               

                var tickets = await (
                from ticket in _context.Tickets
                join passenger in _context.Passengers
                on ticket.PassengerId equals passenger.PassengerId
                join seat in _context.Seats
                    on ticket.SeatId equals seat.SeatId
                join flightScheduleClass in _context.FlightScheduleClasses
                    on seat.FlightScheduleClassId equals flightScheduleClass.FlightclassId
                join flightSchedule in _context.FlightsSchedules
                    on flightScheduleClass.FlightScheduleId equals flightSchedule.FlightScheduleId
                join flight in _context.Flights
                    on flightSchedule.FlightId equals flight.FlightId
                join flightClass in _context.Classes
                    on flightScheduleClass.ClassId equals flightClass.ClassId
                join departureAirport in _context.Airports
                    on flightSchedule.DepartureAirportId equals departureAirport.AirportId
                join arrivalAirport in _context.Airports
                    on flightSchedule.ArrivalAirportId equals arrivalAirport.AirportId
                join airline in _context.Airlines
                on flight.AirlineId equals airline.AirlineId
                where flightSchedule.FlightScheduleId == flightScheduleId && ticket.TicketStatus==Bookingstatus.CONFIRMED && ticket.BookingId==bookingId
                select new TicketDto
                {
                    TicketId=ticket.TicketId,
                    BookingId=bookingId,
                    CustomerName=$"{book.User.FirstName} {book.User.LastName}",
                    CustomerEmail=book.User.Email,
                    PassengerName = $"{passenger.Firstname} {passenger.Lastname}",
                    AirlineName=airline.AirlineName,
                    FlightNumber = flight.FlightNumber,
                    ClassName = flightClass.ClassName,
                    SeatNumber = seat.SeatNumber,
                    From = $"{departureAirport.City}-{departureAirport.IataCode}",
                    To = $"{arrivalAirport.City}-{arrivalAirport.IataCode}",
                    DepartureDate = DateOnly.FromDateTime(flightSchedule.DepartureTime), 
                    DepartureTime = flightSchedule.DepartureTime.ToString("HH:mm"), 
                    ArrivalDate= DateOnly.FromDateTime(flightSchedule.ArrivalTime), 
                    ArrivalTime = flightSchedule.ArrivalTime.ToString("HH:mm"),
                    TicketPrice=seat.SeatPrice
                }
            ).ToListAsync();
            return tickets;

        }

        public async Task<GeneralResponse> UploadTickets(List<TicketDto> ticketDetails,IFormFile[] tickets)
        {
            var commonDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedTickets");

            if (!Directory.Exists(commonDirectory))
            {
                Directory.CreateDirectory(commonDirectory);
            }


            var filePaths = new List<string>();

            foreach (var ticket in tickets)
            {
                if (ticket.Length == 0)
                    continue;

                var originalFileName = Path.GetFileNameWithoutExtension(ticket.FileName);
                var fileExtension = Path.GetExtension(ticket.FileName);
                var uniqueFileName = $"{originalFileName}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(commonDirectory, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ticket.CopyToAsync(stream);
                }

                filePaths.Add(filePath);
            }

            // Call the method to send the email with multiple attachments
            await SendEmailWithAttachments(ticketDetails[0],filePaths);

            //// Clean up temporary files
            //foreach (var filePath in filePaths)
            //{
            //    System.IO.File.Delete(filePath);
            //}

            return new GeneralResponse(true, "Tickets sent successfully");
        }


        private async Task SendEmailWithAttachments(TicketDto ticketDTO,IEnumerable<string> filePaths)
        {

            var smtpSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = smtpSettings["SmtpServer"];
            var port = int.Parse(smtpSettings["Port"]);
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("AeroFlex", username));
            message.To.Add(new MailboxAddress(ticketDTO.CustomerName,ticketDTO.CustomerEmail));
            message.Subject = $"Your AeroFlex Booking Confirmation - Booking ID: {ticketDTO.BookingId}";


            var emailBody = $@"
Dear {ticketDTO.CustomerName},

Thank you for choosing AeroFlex for your upcoming journey. We're pleased to confirm your booking with us and have attached your e-tickets below.

Booking Details:
        - Booking Reference: {ticketDTO.BookingId}
        - Flight Number: {ticketDTO.FlightNumber}
        - Departure Airport: {ticketDTO.From}
        - Arrival Airport: {ticketDTO.To}
        - Departure Date & Time: {ticketDTO.DepartureTime}
        - Arrival Date & Time: {ticketDTO.ArrivalTime}

Important Notes:
        - Please arrive at the airport at least 2 hours before your scheduled departure for domestic flights and 3 hours for international flights.
        - For any changes or cancellations, please visit our website or contact our customer service team.

We hope you have a wonderful trip with AeroFlex!

Best regards,
The AeroFlex Team";


            var body = new TextPart("plain")
            {
                Text = emailBody
            };

            var multipart = new MimeKit.Multipart("mixed");
            multipart.Add(body);

            foreach (var filePath in filePaths)
            {
                var attachment = new MimePart("images", "jpeg")
                {
                    Content = new MimeContent(File.OpenRead(filePath)),
                    ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(filePath)
                };

                multipart.Add(attachment);
            }

            message.Body = multipart;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(smtpServer, port, SecureSocketOptions.StartTls);
                client.Authenticate(username,password);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }
    }
}
