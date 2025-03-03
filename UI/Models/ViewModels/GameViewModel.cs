using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Models.ViewModels
{
    public class GameViewModel
    {
        private HubConnection connection;
        private string enlace= "https://localhost:3000/salajuego";
        private string groupName = "Partida1";



        //public static int NumeroJugadores { get; private set; }

        // lista de opciones
        public List<Eleccion> Opciones { get; }

        public static int Puntos {get; set;}


        //Jugadores ?? Cada uno tendra una eleccion
        private Jugador j1;
        private Jugador j2;





        /////para testear/////
        






        /// <summary>
        /// constructor
        /// </summary>
        public GameViewModel()
        {

            //conexion
            connection = new HubConnectionBuilder()
            .WithUrl(enlace)
            .Build();



            //OPCIONES DEL JUEGO (LISTADO DEL JUEGO)
            Opciones = new List<Eleccion> {
                new Eleccion("piedra"),
                new Eleccion("papel"),
                new Eleccion("tijeras")
                };
        }


        /// <summary>
        /// funcion para comprobar quien ha ganado la eleccion
        /// </summary>
        public void compobacionJuego()
        {

            if(
                j1.eleccion.Nombre=="piedra"&&j2.eleccion.Nombre== "papel" ||
                j1.eleccion.Nombre == "papel" && j2.eleccion.Nombre == "tijeras" ||
                j1.eleccion.Nombre == "tijeras" && j2.eleccion.Nombre == "piedra"
            )
            {
                //gana j2
            }
            else if (
                j2.eleccion.Nombre == "piedra" && j1.eleccion.Nombre == "papel" ||
                j2.eleccion.Nombre == "papel" && j1.eleccion.Nombre == "tijeras" ||
                j2.eleccion.Nombre == "tijeras" && j1.eleccion.Nombre == "piedra"
                )
            {
                //gana j1
            }
            else if(j1.eleccion.Nombre==j2.eleccion.Nombre)
            {
                //empate
            }





        }

















    }
}
