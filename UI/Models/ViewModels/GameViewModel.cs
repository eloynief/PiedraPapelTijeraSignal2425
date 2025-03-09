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
        private HubConnection _connection;
        private const string _enlace = "http://localhost:5062/gamehub"; // URL del Hub
        private string _groupName = "Partida1"; // Grupo por defecto

        // Jugadores
        private Jugador _j1 = new Jugador();
        private Jugador _j2 = new Jugador();

        // Propiedades públicas
        public List<Eleccion> Opciones { get; } = new List<Eleccion>
        {
            new Eleccion("piedra"),
            new Eleccion("papel"),
            new Eleccion("tijeras")
        };

        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        private string _grupo;
        public string Grupo
        {
            get => _grupo;
            set { _grupo = value; OnPropertyChanged(nameof(Grupo)); }
        }

        private string _resultado;
        public string Resultado
        {
            get => _resultado;
            set { _resultado = value; OnPropertyChanged(nameof(Resultado)); }
        }

        private bool _isWaiting;
        public bool IsWaiting
        {
            get => _isWaiting;
            set { _isWaiting = value; OnPropertyChanged(nameof(IsWaiting)); }
        }

        // Comandos
        public ICommand UnirseCommand { get; set; }
        public ICommand ElegirOpcionCommand { get; set; }

        public GameViewModel()
        {
            UnirseCommand = new Command(async () => await UnirseAlJuego());
            ElegirOpcionCommand = new Command<string>(async (opcion) => await ElegirOpcion(opcion));
            SignalR();
        }

        // Inicialización de SignalR
        private async void SignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(_enlace)
                .Build();

            // Métodos que el servidor puede invocar
            _connection.On<string, string>("ReceiveChoice", (jugadorId, eleccion) => RecibirEleccion(jugadorId, eleccion));
            _connection.On<string>("ReceiveResult", (resultado) => MostrarResultado(resultado));

            try
            {
                await _connection.StartAsync();
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

            if (_connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
            }
            
            //llamariamos al metodo del gamehub
            await _connection.InvokeAsync("JoinGroup", Grupo, Nombre);
            Resultado = $"Te has unido al grupo {Grupo}";
        }

        // Elegir una opción
        private async Task ElegirOpcion(string opcion)
        {
            if (_connection.State != HubConnectionState.Connected)
            {
                Resultado = "No estás conectado al servidor";
                return;
            }

            // Asignar la elección al jugador local (J1 o J2 según el contexto)
            var eleccion = Opciones.FirstOrDefault(o => o.Nombre == opcion);
            if (string.IsNullOrEmpty(_j1.Nombre)) // Si J1 no tiene nombre, este cliente es J1
            {
                _j1.Nombre = Nombre;
                _j1.eleccion = eleccion;
            }
            else if (string.IsNullOrEmpty(_j2.Nombre)) // Si J2 no tiene nombre, este cliente es J2
            {
                _j2.Nombre = Nombre;
                _j2.eleccion = eleccion;
            }

            // Enviar la elección al servidor
            await _connection.InvokeAsync("SendChoice", Grupo, Nombre, opcion);
            IsWaiting = true; // Indicamos que estamos esperando al otro jugador
            Resultado = "Esperando al otro jugador...";
        }

        // Recibir la elección del otro jugador
        private void RecibirEleccion(string jugadorId, string eleccion)
        {
            var opcion = Opciones.FirstOrDefault(o => o.Nombre == eleccion);
            if (jugadorId != Nombre) // Si no es mi elección
            {
                if (_j1.Nombre == Nombre) // Soy J1, entonces esto es de J2
                {
                    _j2.Nombre = jugadorId;
                    _j2.eleccion = opcion;
                }
                else // Soy J2, entonces esto es de J1
                {
                    _j1.Nombre = jugadorId;
                    _j1.eleccion = opcion;
                }
            }
        }

        // Mostrar el resultado recibido del servidor
        private void MostrarResultado(string resultado)
        {
            Resultado = resultado;
            IsWaiting = false;
            // Reiniciar elecciones para la próxima ronda
            _j1.eleccion = null;
            _j2.eleccion = null;
        }

        // Lógica del juego (puede moverse al servidor si prefieres)
        private string ComprobacionJuego()
        {
            if (_j1.eleccion == null || _j2.eleccion == null)
                return "Faltan elecciones";

            if (_j1.eleccion.Nombre == _j2.eleccion.Nombre)
                return "Empate";

            if ((_j1.eleccion.Nombre == "piedra" && _j2.eleccion.Nombre == "tijeras") ||
                (_j1.eleccion.Nombre == "papel" && _j2.eleccion.Nombre == "piedra") ||
                (_j1.eleccion.Nombre == "tijeras" && _j2.eleccion.Nombre == "papel"))
            {
                return $"{_j1.Nombre} gana";
            }
            return $"{_j2.Nombre} gana";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}