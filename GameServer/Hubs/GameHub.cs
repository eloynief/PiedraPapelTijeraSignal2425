using Microsoft.AspNetCore.SignalR;
using Models;
using System.Text.RegularExpressions;

namespace GameServer.Hubs
{
    //probar
    public class GameHub:Hub
    {
        //atributo para el listado de grupos
        public List<Grupo> grupos=new List<Grupo>();

        /// <summary>
        /// Pre: nombre del jugador y del grupo
        /// Post: espera el resultado de union del grupo
        /// funcion para unirse al grupo
        /// </summary>
        /// <param name="grupoNombre">el nombre del grupo que nos queremos unir</param>
        /// <param name="jugadorNombre">el nombre del jugador</param>
        /// <returns></returns>
        public async Task JoinGroup(string grupoNombre, string jugadorNombre)
        {
            //variable para el grupo en el que queremos unirnos
            Grupo grupo= grupos.FirstOrDefault(g=>g.Nombre== grupoNombre);

            //comprobamos que el grupo no exista
            if (grupo == null)
            {
                //creamos el grupo y agregamos el primer jugador
                grupo = new Grupo(grupoNombre);
                Jugador jugador = new Jugador(jugadorNombre, grupoNombre, 0, 0);
                grupo.Jugadores = new List<Jugador> { jugador };
                grupos.Add(grupo);

                //lo ponemos en el grupo
                await Groups.AddToGroupAsync(Context.ConnectionId, grupoNombre);

                //ponemos los jugadores dentro del grupo
                await Clients.Group(grupoNombre).SendAsync("PlayerJoined", jugadorNombre);
            }
            else
            {
                // Si el grupo ya tiene 2 jugadores, no se puede unir nadie más
                if (grupo.Jugadores.Count >= 2)
                {
                    await Clients.Caller.SendAsync("GroupFull", "El grupo ya está completo.");
                    return;
                }

                // Agregar el segundo jugador
                Jugador jugador = new Jugador(jugadorNombre, grupoNombre, 0, 0);
                grupo.Jugadores.Add(jugador);

                await Groups.AddToGroupAsync(Context.ConnectionId, grupoNombre);
                await Clients.Group(grupoNombre).SendAsync("PlayerJoined", jugadorNombre);
            }

        }


        public async Task LeaveGroup(string grupoNombre)
        {
            //busco el grupo en la lista
            Grupo grupo = obtenerGrupo(grupoNombre);

            //si existe
            if (grupo != null)
            {
                //busco al jugador en el grupo
                Jugador jugador = grupo.Jugadores.FirstOrDefault(j => j.Grupo == grupoNombre && Context.ConnectionId == Context.ConnectionId);


                //si el jugador existe
                if (jugador != null)
                {
                    //se elimina al jugador del grupo
                    grupo.Jugadores.Remove(jugador);

                    //se notifica a los jugadores
                    //await Clients.Group(grupoNombre).SendAsync("PlayerLeft", jugador.Nombre);
                }

                //si el grupo esta vacio, se elimina de la lista
                if (grupo.Jugadores.Count == 0)
                {
                    grupos.Remove(grupo);
                }
            }

            //eliminar al usuario del grupo de SignalR
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, grupoNombre);
        }




        /// <summary>
        /// Pre: quiero obtener el resultado de la eleccion
        /// Post: obtendria ese resultado de la eleccion
        /// funcion en donde el jugador elige una opcion (piedra, papel o tijeras)
        /// </summary>
        public async Task ChooseOption(string grupoNombre, string jugadorNombre, string eleccion)
        {
            //obtengo grupo
            Grupo grupo = obtenerGrupo(grupoNombre);
            //si existe
            if (grupo != null) {
                //obtengo jugador
                Jugador jugador = obtenerJugadorGrupo(grupo, jugadorNombre);
                //si existe
                if (jugador != null) {

                    jugador.JugadorEleccion = new Eleccion(eleccion);

                    // Notificar que el jugador ha hecho su elección
                    await Clients.Group(grupoNombre).SendAsync("PlayerChose", jugadorNombre, eleccion);

                    // Si ambos jugadores han elegido, determinar el ganador
                    if (grupo.Jugadores.All(j => !string.IsNullOrEmpty(j.JugadorEleccion.Nombre)))
                    {
                        await DetermineWinner(grupo);
                    }
                }
            }
        }

        /// <summary>
        /// funcion que determina el ganador de la ronda
        /// </summary>
        private async Task DetermineWinner(Grupo grupo)
        {
            Jugador j1 = grupo.Jugadores[0];
            Jugador j2 = grupo.Jugadores[1];

            string e1 = j1.JugadorEleccion.Nombre;
            string e2 = j2.JugadorEleccion.Nombre;

            string resultado = "Empate";

            if ((e1 == "piedra" && e2 == "tijeras") ||
                (e1 == "papel" && e2 == "piedra") ||
                (e1 == "tijeras" && e2 == "papel"))
            {
                j1.Puntos++;
                resultado = $"{j1.Nombre} gana la ronda";
            }
            else if ((e2 == "piedra" && e1 == "tijeras") ||
                     (e2 == "papel" && e1 == "piedra") ||
                     (e2 == "tijeras" && e1 == "papel"))
            {
                j2.Puntos++;
                resultado = $"{j2.Nombre} gana la ronda";
            }

            await Clients.Group(grupo.Nombre).SendAsync("RoundResult", resultado, j1.Nombre, j1.Puntos, j2.Nombre, j2.Puntos);

            //si un jugador llega a 5 puntos, gana la partida
            if (j1.Puntos == 5)
            {
                j1.Victorias++;
                await Clients.Group(grupo.Nombre).SendAsync("GameWinner", j1.Nombre);
                ResetGame(grupo);
            }
            else if (j2.Puntos == 5)
            {
                j2.Victorias++;
                await Clients.Group(grupo.Nombre).SendAsync("GameWinner", j2.Nombre);
                ResetGame(grupo);
            }
        }


        #region funciones (Para usar aqui)

        /// <summary>
        /// Pre: quiero obetener el jugador en funcion del grupo y el nombre del jugador
        /// Post: jugador con ese nombre
        /// funcion que sirve para obtener un jugador del grupo en funcion del nombre del jugador
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        private Jugador obtenerJugadorGrupo(Grupo grupo,string jugadorNombre)
        {
            return grupo.Jugadores.FirstOrDefault(j => j.Nombre == jugadorNombre);
        }

        /// <summary>
        /// Pre: nombre del grupo
        /// Post: grupo en funcion del nombre
        /// funcion que sirve para obtener el grupo en funcion del nombre del grupo
        /// </summary>
        /// <param name="grupoNombre"></param>
        /// <returns></returns>
        private Grupo obtenerGrupo(string grupoNombre)
        {
            return grupos.FirstOrDefault(g => g.Nombre == grupoNombre);
        }


        /// <summary>
        /// funcion que reinicia el juego despues de una victoria
        /// </summary>
        private void ResetGame(Grupo grupo)
        {
            //recorro la lista de jugadores del grupo
            foreach (var jugador in grupo.Jugadores)
            {
                //reseteo la puntuacion y la eleccion
                jugador.Puntos = 0;
                jugador.JugadorEleccion = new Eleccion("");
            }
        }
        #endregion


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
