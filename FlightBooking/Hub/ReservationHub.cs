
using FlightBooking.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace FlightBooking.Hub
{ 
    public class ReservationHub: Microsoft.AspNetCore.SignalR.Hub
    {

        public async Task SendReservationUpdate(string message)
        {
            //var message=JsonConvert.SerializeObject(reservation);
            await Clients.All.SendAsync("ReceiveReservationUpdate", message);
        }




    }
}
