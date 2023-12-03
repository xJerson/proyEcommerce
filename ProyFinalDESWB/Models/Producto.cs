namespace ProyFinalDESWB.Models
{
    public class Producto
    {
        public string codigo { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string disponibilidad { get; set; }
        public string animal { get; set; }
        public string tipocomi { get; set; }
        public string fotopro { get; set; }

        public bool ProductoAgregado { get; set; }
    }
}
