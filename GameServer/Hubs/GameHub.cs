using Microsoft.AspNetCore.SignalR;
using Models;

namespace GameServer.Hubs
{
    //probar
    public class GameHub:Hub
    {
        //este mensaje se enviara a todos los clientes
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }





        public async Task JoinGroup(string groupName, string playerName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("PlayerJoined", playerName);
        }



        /**
        #region Atributos

        //public hub

        public static Dictionary<string, int> datos;

        //lista de jugadores
        private static List<Jugador> jugadores = new List<Jugador>();
        #endregion

        //este metodo se invoca siempre que se conecta un cliente
        public override async Task OnConnectedAsync()
        {

            //envia quien se unio
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} c unio");

            await Groups.AddToGroupAsync(Context.ConnectionId, "New Connections");

            await Groups.AddToGroupAsync(Context.ConnectionId, "Special Group");

            //await Clients.Group("...").SendAsync;

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("RecieveMessage", $"{Context.ConnectionId}:{message}");

        }



        #region Functions

        public async Task JoinGroup(string grupo)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, grupo);

            await Clients.Group(grupo).SendAsync("Send", $"{Context.ConnectionId} se ha unido al grupo: {grupo}");
        }

        public async Task LeaveGroup(string grupo)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupo);

            await Clients.Group(grupo).SendAsync("Send", $"{Context.ConnectionId} se fue del grupo: {grupo}");
        }

        #endregion

        */



    }
}
