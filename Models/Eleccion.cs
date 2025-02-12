using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Eleccion
    {
        //tendra una imagen en funcion del nombre
        public string Nombre { get; set; } // "Piedra", "Papel" o "Tijeras"
        public string Imagen => $"images/{Nombre.ToLower()}.png"; // Ruta de imagen
    }
}
