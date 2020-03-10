using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRTableBooking.Hubs
{
    public class TableBookingHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}