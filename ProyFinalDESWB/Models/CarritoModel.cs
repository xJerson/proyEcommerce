namespace ProyFinalDESWB.Models
{
    public class CarritoModel
    {

        
        public string Codigo { get; set; }

        public string fotopro { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }

        public int stock { get; set; }
        public decimal Importe
        {
            get
            {
                return Precio * Cantidad;
            }
        }



    }
}
