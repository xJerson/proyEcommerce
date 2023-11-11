using Microsoft.AspNetCore.Mvc;
using ProyFinalDESWB.Models;
using System.Diagnostics;

namespace ProyFinalDESWB.Controllers
{
    public class HomeController : Controller
    {
     

        public IActionResult Index()
        {
            return View();
        }

      
    }
}