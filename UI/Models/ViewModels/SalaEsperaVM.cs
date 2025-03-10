using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using UI.Models.Notify;

namespace UI.Models.ViewModels
{
    public class SalaEsperaVM : MiNotifyChanged
    {
        #region atributos
        private HubConnection connection;
        private const string enlace = "http://localhost:5062/gamehub";
        private string nombre;
        private string grupo;
        private List<Jugador> jugadores;
        private bool botonJoinPulsado;
        private bool juegoListo;
        public event Action onGameStart;
        #endregion

        #region properties
        public string Nombre
        {
            get => nombre;
            set => nombre = value;
        }

        public string Grupo
        {
            get => grupo;
            set => grupo = value;
        }

        public List<Jugador> Jugadores
        {
            get => jugadores;
            set
            {
                jugadores = value;
                OnPropertyChanged(nameof(Jugadores));
                if (jugadores.Count == 2)
                {
                    JuegoListo = true;
                    onGameStart?.Invoke();
                    Shell.Current.GoToAsync("///GamePage");
                }
            }
        }

        public bool BotonJoinPulsado
        {
            get => botonJoinPulsado;
            set
            {
                botonJoinPulsado = value;
                OnPropertyChanged(nameof(BotonJoinPulsado));
            }
        }

        public bool JuegoListo
        {
            get => juegoListo;
            set => juegoListo = value;
        }
        #endregion

        #region commands
        public ICommand UnirseCommand { get; set; }
        public ICommand CancelarCommand { get; set; }
        #endregion

        #region constructors
        public SalaEsperaVM()
        {
            jugadores = new List<Jugador>();
            connection = new HubConnectionBuilder().WithUrl(enlace).Build();

            UnirseCommand = new Command(async () => await UnirseAlJuego());
            ComprobarUnion();
            esperaConexion();
        }
        #endregion

        #region metodos con signalR
        private void ComprobarUnion()
        {
            connection.On<Jugador>("PlayerJoined", (jugador) =>
            {
                if (!jugadores.Any(j => j.Nombre == jugador.Nombre))
                {
                    jugadores.Add(jugador);
                    OnPropertyChanged(nameof(Jugadores));
                }
                if (jugadores.Count == 2)
                {
                    JuegoListo = true;
                    onGameStart?.Invoke();
                }
            });
        }
        #endregion

        #region funciones asincronas
        private async void esperaConexion()
        {
            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar a SignalR: {ex.Message}");
            }
        }

        private async Task UnirseAlJuego()
        {
            if (!string.IsNullOrWhiteSpace(Nombre) && !string.IsNullOrWhiteSpace(Grupo))
            {
                await connection.InvokeAsync("JoinGroup", Grupo, Nombre);
                BotonJoinPulsado = true;
            }
        }
        #endregion
    }
}
