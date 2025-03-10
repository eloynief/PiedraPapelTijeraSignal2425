using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using UI.Models.Notify;

namespace UI.Models.ViewModels
{
    public class SalaEsperaVM:MiNotifyChanged
    {
        #region atributos
        private HubConnection connection;
        private const string enlace = "http://localhost:5062/gamehub";

        private string nombre;
        private string grupo;
        private List<Jugador> jugadores;

        private bool botonJoinPulsado;

        private bool juegoListo;

        //evento para cambiar a la pantalla de juego
        public event Action onGameStart; 

        #endregion

        #region properties
        public string Nombre {
            get { return nombre; }
            set { nombre = value; }
        }
        public string Grupo { 
            get { return grupo; }
            set { grupo = value; }
        }

        public List<Jugador> Jugadores {
            get { return jugadores; }
            set
            {
                jugadores = value;
                OnPropertyChanged(nameof(Jugadores));

                // Si hay 2 jugadores, iniciar el juego
                if (jugadores.Count == 2)
                {
                    onGameStart.Invoke();
                    Shell.Current.GoToAsync("///GamePage");
                }

            }

        }

        public bool BotonJoinPulsado
        {
            get { return botonJoinPulsado; }
            set { 
                botonJoinPulsado = value;
                OnPropertyChanged(nameof(BotonJoinPulsado));
            }
        }

        public bool JuegoListo {
            get { return juegoListo; }
            set { juegoListo = value; }
        }
        #endregion

        #region commands
        public ICommand UnirseCommand { get; set; }
        
        public ICommand CancelarCommand { get; set; }
        #endregion



        #region constructors
        /// <summary>
        /// constructor para inicializar
        /// </summary>
        public SalaEsperaVM()
        {
            jugadores = new List<Jugador>();

            connection = new HubConnectionBuilder().WithUrl(enlace).Build();

            //la funcion de unirse se pondra en el comando de boton
            UnirseCommand = new Command(async () => await UnirseAlJuego());
            
            // Configuramos las funciones de SignalR
            ComprobarUnion();
            esperaConexion();

        }

        #endregion

        #region metodos con signalR


        private async void Unirme()
        {
            //creo mi conexion



        }

        private async void ComprobarUnion()
        {

            connection.On<Jugador>("PlayerJoined", (jugador) =>
            {
                if (!Jugadores.Contains(jugador))
                {
                    Jugadores.Add(jugador);
                    OnPropertyChanged(nameof(Jugadores));
                }
                //Si hay 2 jugadores, la partida empieza
                if (Jugadores.Count == 2) 
                {
                    JuegoListo = true;
                    //OnGameStart?.Invoke();
                }
            });


        }



        #endregion

        #region funciones asincronas

        /// <summary>
        /// metodo para la conexion
        /// </summary>
        private async void esperaConexion()
        {
            await connection.StartAsync();
        }

        /// <summary>
        /// Pre: nada
        /// Post: espera poder unirse al juego si el jugador tiene nombre y grupo puestos en la pagina
        /// funcion para unirse al juego
        /// </summary>
        /// <returns></returns>
        private async Task UnirseAlJuego()
        {
            //si se ha escrito un nombre y grupo
            if (!string.IsNullOrWhiteSpace(Nombre) || !string.IsNullOrWhiteSpace(Grupo))
            {
                //llamamos la funcion de signal
                await connection.InvokeAsync("JoinGroup", Grupo, Nombre);

                jugadores.Add(new Jugador(nombre,grupo));
                OnPropertyChanged(nameof(Jugadores));

                BotonJoinPulsado = true;
            }



        }

        #endregion


    }
}
