using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    //clase grupo que sirve para los grupos de los jugadores
    public class Grupo
    {
        #region atributos

        private List<Jugador> jugadores;

        private string nombre;

        #endregion

        #region properties
        public List<Jugador> Jugadores
        {
            get { return jugadores; }
            set { jugadores = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }


        #endregion

        #region constructors

        public Grupo()
        {
            nombre = "";
        }

        public Grupo(string nombre)
        {
            this.Nombre = nombre;
        }

        #endregion

    }
}
