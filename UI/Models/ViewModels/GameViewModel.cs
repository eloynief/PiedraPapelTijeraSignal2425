using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Models.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        #region atributos

        private HubConnection connection;

        // URL del Hub
        private const string enlace = "http://localhost:5062/gamehub";

        // Jugadores
        private Jugador yo = new Jugador();
        //este lo cojo para mostrar el nombre/puntos
        private Jugador rival = new Jugador();

        private string nombre;

        private string grupo;

        private string resultado;

        private bool estaEsperando;
        #endregion

        #region Propiedades

        public Jugador Yo
        {
            get { return yo; }
            set { 
                yo = value;
                //desde aqui notifico la puntuacion
                OnPropertyChanged(nameof(Yo));
            }
        }

        public Jugador Rival
        {
            get { return rival; }
            set
            {
                rival = value;
                //notifico la puntuacion
                OnPropertyChanged(nameof(Rival));
            }
        }

        public string Nombre
        {
            get { return nombre; }
            set
            {
                nombre = value;
                //OnPropertyChanged(Nombre);
            }
        }

        public string Grupo
        {

            get { return grupo; }
            set {
                grupo = value;
                //OnPropertyChanged(Grupo);
            }
        }

        public List<Eleccion> Opciones
        {
            get
            {
                return new List<Eleccion>
                {
                    new Eleccion("piedra"),
                    new Eleccion("papel"),
                    new Eleccion("tijeras")
                };
            } 
        } 

        public string Resultado
        {
            get => resultado;
            set { resultado = value; OnPropertyChanged(nameof(Resultado)); }
        }

        public bool EstaEsperando
        {
            get => estaEsperando;
            set { estaEsperando = value; OnPropertyChanged(nameof(EstaEsperando)); }
        }

        #endregion


        // Comandos
        public ICommand UnirseCommand { get; set; }
        public ICommand ElegirOpcionCommand { get; set; }

        public GameViewModel()
        {

            //obtenemos la conexion con el servidor
            connection = new HubConnectionBuilder().WithUrl(enlace).Build();

            UnirseCommand = new Command(async () => await UnirseAlJuego());
            ElegirOpcionCommand = new Command<string>(async (opcion) => await ElegirOpcion(opcion));
            SignalR();
        }

        // Inicialización de SignalR
        private async void SignalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(enlace)
                .Build();

            // Métodos que el servidor puede invocar
            connection.On<string, string>("ReceiveChoice", (jugadorId, eleccion) => RecibirEleccion(jugadorId, eleccion));
            connection.On<string>("ReceiveResult", (resultado) => MostrarResultado(resultado));

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Resultado = $"Error al conectar: {ex.Message}";
            }
        }

        // Unirse al grupo
        private async Task UnirseAlJuego()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Grupo))
            {
                Resultado = "Debes ingresar un nombre y un grupo";
                return;
            }

            if (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
            }
            
            //llamariamos al metodo del gamehub
            await connection.InvokeAsync("JoinGroup", Grupo, Nombre);
            Resultado = $"Te has unido al grupo {Grupo}";
        }

        // Elegir una opción
        private async Task ElegirOpcion(string opcion)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                Resultado = "No estás conectado al servidor";
                return;
            }

            // Asignar la elección al jugador local (J1 o J2 según el contexto)
            var eleccion = Opciones.FirstOrDefault(o => o.Nombre == opcion);
            if (string.IsNullOrEmpty(yo.Nombre)) // Si J1 no tiene nombre, este cliente es J1
            {
                yo.Nombre = Nombre;
                yo.JugadorEleccion = eleccion;
            }
            else if (string.IsNullOrEmpty(rival.Nombre)) // Si J2 no tiene nombre, este cliente es J2
            {
                rival.Nombre = Nombre;
                rival.JugadorEleccion = eleccion;
            }

            // Enviar la elección al servidor
            await connection.InvokeAsync("SendChoice", Grupo, Nombre, opcion);
            EstaEsperando = true; // Indicamos que estamos esperando al otro jugador
            Resultado = "Esperando al otro jugador...";
        }

        // Recibir la elección del otro jugador
        private void RecibirEleccion(string jugadorId, string eleccion)
        {
            var opcion = Opciones.FirstOrDefault(o => o.Nombre == eleccion);
            if (jugadorId != Nombre) // Si no es mi elección
            {
                if (yo.Nombre == Nombre) // Soy J1, entonces esto es de J2
                {
                    rival.Nombre = jugadorId;
                    rival.JugadorEleccion = opcion;
                }
                else // Soy J2, entonces esto es de J1
                {
                    yo.Nombre = jugadorId;
                    yo.JugadorEleccion = opcion;
                }
            }
        }

        // Mostrar el resultado recibido del servidor
        private void MostrarResultado(string resultado)
        {
            Resultado = resultado;
            EstaEsperando = false;
            // Reiniciar elecciones para la próxima ronda
            yo.JugadorEleccion = null;
            rival.JugadorEleccion = null;
        }

        // Lógica del juego (puede moverse al servidor si prefieres)
        private string ComprobacionJuego()
        {
            if (yo.JugadorEleccion == null || rival.JugadorEleccion == null)
                return "Faltan elecciones";

            if (yo.JugadorEleccion.Nombre == rival.JugadorEleccion.Nombre)
                return "Empate";

            if ((yo.JugadorEleccion.Nombre == "piedra" && rival.JugadorEleccion.Nombre == "tijeras") ||
                (yo.JugadorEleccion.Nombre == "papel" && rival.JugadorEleccion.Nombre == "piedra") ||
                (yo.JugadorEleccion.Nombre == "tijeras" && rival.JugadorEleccion.Nombre == "papel"))
            {
                return $"{yo.Nombre} gana";
            }
            return $"{rival.Nombre} gana";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
            
    }
}