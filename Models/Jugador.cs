namespace Models
{
    public class Jugador
    {

        private string nombre;

        private string grupo;

        private int puntos;

        private int victorias;

        private Eleccion eleccion;


        public string Nombre { 
            get{ return nombre; } 
            set{ nombre = value; }
        }

        public string Grupo
        {
            get { return grupo; }

            set { grupo = value; }

        }

        public int Puntos
        {
            get { return puntos; }
            set { puntos = value; }
        }

        public int Victorias
        {
            get { return victorias; }
            set { victorias = value; }
        }

        //herencia de la eleccion (tiene imagen y nombre)
        public Eleccion JugadorEleccion {
            get { return eleccion; }
            set { eleccion = value; }
        }



        /// <summary>
        /// constructor sin params
        /// </summary>
        public Jugador()
        {
            this.nombre = "";
            this.grupo = "";
            this.puntos = 0;
            this.victorias = 0;
            this.eleccion=new Eleccion("");
        }

        /// <summary>
        /// constructor con params
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="grupo"></param>
        /// <param name="puntos"></param>
        public Jugador(string nombre,string grupo, int puntos,int victorias)
        {
            this.nombre = nombre;
            this.grupo = grupo;
            this.puntos = puntos;
            this.victorias = victorias;
            this.eleccion = new Eleccion("");
        }

    }
}
