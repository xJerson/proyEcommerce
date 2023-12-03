using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;
namespace ProyFinalDESWB.Models
{
    public class GrabarProducto
    {
        
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public int stock { get; set; }
        public string disponibilidad { get; set; }
        public int animal { get; set; }
        public int tipocomi { get; set; }

        
        public string fotopro { get; set; }
    }
}
