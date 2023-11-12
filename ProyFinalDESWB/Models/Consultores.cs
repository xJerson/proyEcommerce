namespace ProyFinalDESWB.Models
{
    public class Consultores
    {
        public string cod_consultores { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string dni { get; set; }
        public string correo { get; set; }
        public int codespecialidad { get; set; }

        //NOMBRE DE LA ESPECIALIDAD TIPO STRING
        public string nomespecialidad { get; set; }
    }
}
