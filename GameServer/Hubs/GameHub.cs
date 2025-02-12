using Microsoft.AspNetCore.SignalR;

namespace GameServer.Hubs
{
    public class GameHub:Hub
    {


        public async Task JoinGroup(string grupo)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);
        }



    }
}
