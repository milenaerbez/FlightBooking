namespace FlightBooking.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public AppUser? Customer { get; set; }
        public string CustomerId { get; set; }
        public Flight? Flight { get; set; }
        public int FlightId { get; set; }
        public string Status { get; set; } = "in progress";
        public int Seats { get; set; }
    }
}
