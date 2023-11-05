
using Microsoft.AspNetCore.SignalR;
namespace FlightBooking.Hub
{ 
    public class ReservationHub: Microsoft.AspNetCore.SignalR.Hub
    {

        public async Task SendReservationUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveReservationUpdate", message);
        }




    }
}
