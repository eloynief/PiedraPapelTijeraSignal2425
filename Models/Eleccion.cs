using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Eleccion
    {

        private string nombre;

        //tendra una imagen en funcion del nombre
        public string Nombre {
            get {
                return nombre;
            }
            set{
                nombre = value;
            }

        } // "Piedra", "Papel" o "Tijeras"

        // Ruta de imagen
        public string Imagen {
            get
            {
                if (!String.IsNullOrEmpty(Nombre))
                {
                    //devuelve
                    return $"{Nombre.ToLower()}.png";
                }
                return "";
            }
        }  

        public Eleccion(string nombre)
        {
            Nombre = nombre;
        }
    }
}
