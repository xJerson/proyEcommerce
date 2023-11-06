namespace ProyFinalDESWB.Models
{
    public class Boleta
    {

        public string codigo_venta { get; set; }
        public DateTime fec_venta { get; set; }
        public int tipo_ven { get; set; }
        public string cod_cliente { get; set; }
        public string cod_empleado { get; set; }
        public string cod_consultores { get; set; }
        public decimal total { get; set; }
       
    }
}
