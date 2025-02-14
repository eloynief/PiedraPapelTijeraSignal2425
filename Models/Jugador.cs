namespace Models
{
    public class Jugador
    {
        //
        private int id;
        private string nombre;

        public int Id { get; }
        public string Nombre { get; set; }

        public Jugador(int id, string nombre)
        {
            this.id = id;
            this.nombre = nombre;
        }

    }
}
