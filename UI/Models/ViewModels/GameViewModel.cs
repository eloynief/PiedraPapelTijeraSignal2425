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

        //public static int NumeroJugadores { get; private set; }

        // lista de opciones
        public List<Eleccion> Opciones { get; }

        public static int Puntos {get; set;}


        //Jugadores ??





        
        public GameViewModel()
        {
            //OPCIONES DEL JUEGO (LISTADO DEL JUEGO)
            Opciones = new List<Eleccion> {
                new Eleccion("piedra"),
                new Eleccion("papel"),
                new Eleccion("tijeras")
                };
        }


























    }
}
