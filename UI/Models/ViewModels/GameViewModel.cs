using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Models.ViewModels
{
    public class GameViewModel
    {
        private HubConnection connection;
        private string enlace= "http://localhost:5062/salajuego";
        private string groupName = "Partida1";



        //public static int NumeroJugadores { get; private set; }

        // lista de opciones
        public List<Eleccion> Opciones { get; }

        public static int Puntos {get; set;}


        //Jugadores ?? Cada uno tendra una eleccion
        private Jugador j1;
        private Jugador j2;




        private string nombre;
        private string grupo;

        public string Nombre
        {
            get => nombre;
            set { nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        public string Grupo
        {
            get => grupo;
            set { grupo = value; OnPropertyChanged(nameof(Grupo)); }
        }

        public ICommand UnirseCommand { get; }

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

            UnirseCommand = new Command(async () => await UnirseAlJuego());

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







            

            private async Task UnirseAlJuego()
            {
                if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Grupo))
                {
                    //await Application.Current.MainPage.DisplayAlert("Error", "Debes ingresar un nombre y un grupo", "OK");
                    //return;
                }

                await connection.StartAsync();
                await connection.InvokeAsync("JoinGroup", Grupo);
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        










    }
}
