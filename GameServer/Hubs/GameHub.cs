using Microsoft.AspNetCore.SignalR;
using Models;

namespace GameServer.Hubs
{
    //probar
    public class GameHub:Hub
    {

        #region Atributos

        //public hub

        public static Dictionary<string, int> datos;


        private static List<Jugador> jugadores = new List<Jugador>();






        #endregion





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

    }
}
