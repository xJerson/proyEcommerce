using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ProyFinalDESWB.DAO;
using ProyFinalDESWB.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace ProyFinalDESWB.Controllers
{

         
    public class ClienteController : Controller
    {

        private readonly IConfiguration configuracion;

        private string cad_conex = "";
        public ClienteController(IConfiguration _config)
        {
            configuracion = _config;
            cad_conex = configuracion.GetConnectionString("cn1");
        }

        // GET: ClienteController

        public ActionResult ListadoClientes()
        {
           var listado = listClientes();
            return View(listado);
        }
        public List<Cliente> listClientes()

        {
            List<Cliente> lista = new List<Cliente>();

            SqlDataReader dr = SqlHelper.ExecuteReader(

                        cad_conex, "listCliente");

            while (dr.Read())

            {
                lista.Add(new Cliente

                {
                    cod_cliente = dr.GetString(0),

                    nombres_completo = dr.GetString(1),

                    dniruc = dr.GetString(2),

                    direccion = dr.GetString(3),
                    correo = dr.GetString(4),
                    tipocli = dr.GetInt32(5)
                });

            }

            dr.Close();

            return lista;

        }

    



    // GET: ClienteController/Details/5
    public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClienteController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
