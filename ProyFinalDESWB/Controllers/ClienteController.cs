using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ProyFinalDESWB.DAO;
using ProyFinalDESWB.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyFinalDESWB.Controllers
{

         
    public class ClienteController : Controller
    {
        private readonly ClienteDAO dao;
        

        public ClienteController(ClienteDAO _dao)
        {
            dao = _dao;
        }


        // GET: ClienteController

        public ActionResult ListadoClientes()
        {
            

            var listado = dao.ListadoClientes();
            return View(listado);
        }
   

    



    // GET: ClienteController/Details/5
    public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClienteController/Create
        public ActionResult GrabarCliente()
        {
            GrabarCliente nuevo = new GrabarCliente();
           
            ViewBag.Tipos = new SelectList(
                dao.ListadoTipos(),
                "cod_tipocli",
                "nom_tipocli"

                );
            

            return View(nuevo);
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GrabarCliente(GrabarCliente nuevo)
        {
            try
            {
                if (ModelState.IsValid == true)
                    ViewBag.Mensaje = dao.GrabarCliente(nuevo);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
            }

            ViewBag.Tipos = new SelectList(
             dao.ListadoTipos(),
             "cod_tipocli",
             "nom_tipocli"

             );

            return View(nuevo);
        }

        // GET: ClienteController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
