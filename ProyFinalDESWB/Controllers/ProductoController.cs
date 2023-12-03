using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyFinalDESWB.DAO;
using ProyFinalDESWB.Models;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Transactions;
using System.Collections.Generic;

namespace ProyFinalDESWB.Controllers
{
    public class ProductoController : Controller
    {


        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ProductoDAO dao;

        public ProductoController(IWebHostEnvironment _webHostEnvironment, ProductoDAO _dao)
        {
            webHostEnvironment = _webHostEnvironment;
            dao = _dao;
        }


        Producto BuscarProducto(string codigo)
        {

            var query = dao.ListadoProductos()
                    .Find(pro => pro.codigo == codigo);

            // Verificar si query/consultar es null antes de intentar acceder a sus propiedades
            if (query != null)
            {
                Producto buscado = new Producto()
                {
                    codigo = query.codigo,
                    descripcion = query.descripcion,
                    precio = query.precio,
                    stock = query.stock,
                    disponibilidad = query.disponibilidad,
                    animal = query.animal,
                    tipocomi = query.tipocomi,
                    fotopro = query.fotopro,
                };

                return buscado;
            }
            else
            {
                // Manejar el caso donde no se encontró el producto con el código especificado
                return null; // O podrías lanzar una excepción, según tus necesidades
            }
        }



        // GET: ProductoController
        public ActionResult ListadoProductos()
        {
            //si la variable ssession no existe 
            //entonces la creamos
            if (HttpContext.Session.GetString("Carrito") == null)
            {
                //serializar la coleccion de tipo list el modelo
                //CarritoModel y guardarlo en la sesion
                HttpContext.Session.SetString("Carrito",
                    JsonConvert.SerializeObject(new List<CarritoModel>()));

            }

            // Obtenemos la lista de Producto del DAO
            var listado = dao.ListadoProductos();

            // Recuperamos la lista de la Carrito desde la sesión
            var lista_carrito= JsonConvert.DeserializeObject<List<CarritoModel>>(HttpContext.Session.GetString("Carrito"));

            // Iteramos sobre la lista de Producto
            foreach (var item in listado)
            {
                // Verificamos si el Producto ya está en la Carrito
                var encontrado = lista_carrito.Any(c => c.Codigo.Equals(item.codigo));

                // Actualizamos el modelo de Producto para reflejar si el producto ya se agregó al carrito
                item.ProductoAgregado = encontrado;
            }

            return View(listado);
        }

        //POST
        [HttpPost]
        public IActionResult ListadoProductos(string buscar)
        {



            if (string.IsNullOrEmpty(buscar))
            {
                // La variable buscar está vacía, recupera la lista completa
                var lista = dao.ListadoProductos();
                return View(lista);
            }
            else
            {
                // La variable buscar tiene un valor, realiza la búsqueda
                var lista = dao.BuscarProductos(buscar);
                return View(lista);
            }

        }


        public IActionResult AgregarAlCarrito(string id)
        {

            // Redirigir al usuario de vuelta a la página de listado de productos.
            var producto = BuscarProducto(id);

            return View(producto);

        }

        //post
        [HttpPost]
        public IActionResult AgregarAlCarrito(string id, int cantidad)
        {
            // recuperar los datos del producto a agregar al carrito
            Producto obj = BuscarProducto(id);

            //valida si hay stock (sin transacciones)
            if (obj.stock < cantidad)
            {
                ViewBag.Mensaje = string.Format("Error: Stock insuficiente. Cantidad disponible: {0}", obj.stock);

                //
                return View(obj);
            }


            // definimos la variable del carrito
            CarritoModel cm = new CarritoModel()
            {
                Codigo = obj.codigo,
                fotopro = obj.fotopro,
                Nombre = obj.descripcion,
                Precio = obj.precio,
                stock = obj.stock,
                Cantidad = cantidad

            };
            // recuperar la lista del carrito desde la session(desearializar)
            var lista_carrito = JsonConvert.DeserializeObject<List<CarritoModel>>(
                HttpContext.Session.GetString("Carrito"));

            //si el producto que se requiere agregar existe en el 
            // carrito de compra "lista_carrito"
            var encontrado = lista_carrito.Find(p => p.Codigo.Equals(id));

            obj.ProductoAgregado = true;

            // entonces lo agregamos al carrito de compra
            if (encontrado == null)
            {
                lista_carrito?.Add(cm);
                ViewBag.Mensaje = "Nuevo Producto Agregado al Carrito de Compra";

            }
            else
            {
                encontrado.Cantidad += cantidad;
                ViewBag.Mensaje = "Cantidad de Producto actualizada CORRECTAMENTE";
            }

            // guardar la nueva version de lista_carrito
            HttpContext.Session.SetString("Carrito",
                    JsonConvert.SerializeObject(lista_carrito));
            //
            return View(obj);
        }

        List<CarritoModel> traerCarrito()
        {
            var lista = JsonConvert.DeserializeObject<List<CarritoModel>>(
                HttpContext.Session.GetString("Carrito"));

            return lista!;
        }

        public IActionResult VerCarritoCompra()
        {
            List<CarritoModel> lista_carrito = null;

            if (HttpContext.Session.GetString("Carrito") != null)
            {
                lista_carrito = traerCarrito();
                //si no hay producto en el carrito de compra, entonces 
                //nos dirigimos al listado de producto
                if (lista_carrito.Count == 0)
                    
                return RedirectToAction("ListadoProductos");
            }

            ViewBag.Cantidad = lista_carrito?.Count;
            // enviar en un viewbag el importe total del carrito 
            ViewBag.total = lista_carrito?.Sum(p => p.Importe);
            

            return View(lista_carrito);
        }




        //POST
        [HttpPost]
        public IActionResult EliminarProductoCarrito(string codigo)
        {
            //recuperar la lista de producto desde el carrito de compra
            var lista_carrito = traerCarrito();

            //buscamos el producto en base a su codigo
            CarritoModel? buscar = lista_carrito.Find(a => a.Codigo.Equals(codigo));

            //eliminamos del carrito
            lista_carrito.Remove(buscar!);

            // grabando el carrito en la session
            HttpContext.Session.SetString("Carrito",
                    JsonConvert.SerializeObject(lista_carrito));

            //nos redirigimos a la lista de producto
            return RedirectToAction("VerCarritoCompra");
        }

        public IActionResult ActualizarAgregarAlCarrito(string id)
        {
           var producto = BuscarProducto(id);
            return View(producto);
        }


        //post
        [HttpPost]
        public IActionResult ActualizarAgregarAlCarrito(string id, int cantidad)
        {
            // recuperar los datos del producto a agregar al carrito
            Producto obj = BuscarProducto(id);

            // valida si hay stock (sin transacciones)
            if (obj.stock < cantidad)
            {
                ViewData["Mensaje"] = string.Format("Error: Stock insuficiente. Cantidad disponible: {0}", obj.stock);
                return View("VerCarritoCompra", traerCarrito());
            }

            // definimos la variable del carrito
            CarritoModel cm = new CarritoModel()
            {
                Codigo = obj.codigo,
                fotopro = obj.fotopro,
                Nombre = obj.descripcion,
                Precio = obj.precio,
                stock = obj.stock,
                Cantidad = cantidad
            };

            // recuperar la lista del carrito desde la session (deserializar)
            var lista_carrito = JsonConvert.DeserializeObject<List<CarritoModel>>(
                HttpContext.Session.GetString("Carrito"));

            // si el producto que se requiere agregar existe en el 
            // carrito de compra "lista_carrito"
            var encontrado = lista_carrito.Find(p => p.Codigo.Equals(id));

            // entonces lo agregamos al carrito de compra
            if (encontrado == null)
            {
                lista_carrito?.Add(cm);
                ViewBag.Mensaje = "Nuevo Producto Agregado al Carrito de Compra";
            }
            else
            {
                encontrado.Cantidad = cantidad;
                ViewBag.Mensaje = "Cantidad de Producto actualizada CORRECTAMENTE";
            }

            // guardar la nueva version de lista_carrito
            HttpContext.Session.SetString("Carrito",
                    JsonConvert.SerializeObject(lista_carrito));

            // redirigir a la vista VerCarritoCompra
            return RedirectToAction("VerCarritoCompra");
        }



        public ActionResult MantenimientoProducto()
        {
            var listado = dao.ListadoProductos();

            return View(listado);
        }

        // GET: ProductoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductoController/Create
        public ActionResult GrabarProducto()
        {
            GrabarProducto nuevo = new GrabarProducto();

            ViewBag.Animales = new SelectList(
            dao.ListadoAnimales(),
            "cod_animal",
            "nom_animal"
            );

            ViewBag.Tipos = new SelectList(
                dao.ListadoTipos(),
                "cod_tipo",
                "nom_tipo"

                );


            return View(nuevo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GrabarProducto(GrabarProducto nuevo, IFormFile fotopro)
        {

            ViewBag.Animales = new SelectList(
                dao.ListadoAnimales(),
                 "cod_animal",
                "nom_animal"
                 );

            ViewBag.Tipos = new SelectList(
                dao.ListadoTipos(),
                "cod_tipo",
                "nom_tipo"

                );

            try
            {
                if (fotopro == null)
                {
                    ViewBag.MENSAJE = "Debe seleccionar una imagen";
                    return View(nuevo);

                }

                // Obtén la extensión del archivo
                string extension = Path.GetExtension(fotopro.FileName)?.ToLower();

                // Lista de extensiones permitidas
                List<string> extensionesPermitidas = new List<string> { ".jpg", ".jpeg", ".png", ".gif" }; // Agrega más extensiones si es necesario

                // Verifica si la extensión está en la lista de extensiones permitidas
                if (!extensionesPermitidas.Contains(extension))
                {
                    ViewBag.MENSAJE = "La imagen debe ser de tipo JPG, JPEG, PNG o GIF";
                    return View(nuevo);
                }

                // Asigna el nombre del archivo con el código de la persona
                string nombreArchivo = $"{nuevo.descripcion}{extension}";
                nuevo.fotopro = $"~/Fotos/{nombreArchivo}";

                // Obtener la ruta donde se guardará la imagen
                string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "Fotos");

                // Asegurarse de que la carpeta exista, si no, crearla
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Construir la ruta completa donde se guardará la imagen
                string filePath = Path.Combine(uploadPath, nombreArchivo);
                // Verifica si la imagen existe en la ruta
             

                // Guardar la imagen en el servidor
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                   fotopro.CopyTo(fs);
                }

   
                nuevo.disponibilidad = "SI";

               

                // Luego del código de validación, llamas a tu método GrabarProducto
                ViewBag.MENSAJE = dao.GrabarProducto(nuevo);
            }

            catch (Exception ex)
            {
                ViewBag.MENSAJE = ex.Message;
            }

         
            //
            return View(nuevo);

        }

        public IActionResult PagarCarritoCompra()
        {
 
            List<CarritoModel> lista_carrito = null;

            if (HttpContext.Session.GetString("Carrito") != null)
            {
                lista_carrito = traerCarrito();
                //si no hay producto en el carrito de compra, entonces 
                //nos dirigimos al listado de producto
                if (lista_carrito.Count == 0)

                    return RedirectToAction("ListadoProductos");
            }

            ViewBag.Cantidad = lista_carrito?.Count;
            // enviar en un viewbag el importe total del carrito 
            ViewBag.total = lista_carrito?.Sum(p => p.Importe);



            return View(lista_carrito);
        }

        [HttpPost]
        public IActionResult PagarCarritoCompra(string tipo,
            string nomcard, string nrocard, int cvv, string fechexp)
        {

            // recuperar los productos del carrito de compra 
            var lista_carrito = traerCarrito();

            // generar una transaccion implicita 
            using (TransactionScope trx = new TransactionScope())
            {
                try
                {
                    decimal total = lista_carrito.Sum(p => p.Importe);
                    string numero = dao.GrabarVentas(tipo,
             nomcard,nrocard,cvv,fechexp,total, lista_carrito);
                    
                           
                    TempData["Mensaje"] = $"La Venta :{numero} fue registrada correctamente";

                    // si llegamos hasta aqui, sin producirse un error , entonces
                    // la transaccion se realizo con exito

                    trx.Complete();
                    // destrui una sola session
                    //HttpContext.Session.Remove("Carrito");
                    //destruimos todas las sesiones 
                    HttpContext.Session.Clear();
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = ex.Message;
                }

            }


            return RedirectToAction("PageSuccess");
        }




        // GET: ProductoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductoController/Edit/5
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProducto(string codigo)
        {
            try
            {
                // Obtén el producto antes de eliminarlo para obtener la ruta de la imagen
                var producto = BuscarProducto(codigo);

                if (producto != null)
                {
                    // Verifica si fotopro es null antes de construir la ruta
                    if (producto.fotopro != null)
                    {
                        string rutaFoto = Path.Combine(webHostEnvironment.WebRootPath, producto.fotopro.TrimStart('~', '/'));

                        // Utiliza tu método en el dao para eliminar el producto
                        TempData["Mensaje"] = dao.EliProducto(codigo);

                        // Elimina la foto del sistema de archivos si la ruta no está vacía
                        if (!string.IsNullOrEmpty(rutaFoto))
                        {
                            string rutaCompleta = Path.Combine(webHostEnvironment.WebRootPath, rutaFoto);

                            if (System.IO.File.Exists(rutaCompleta))
                            {
                                System.IO.File.Delete(rutaCompleta);
                            }
                        }
                    }
                    else
                    {
                        TempData["Mensaje"] = dao.EliProducto(codigo);
                    }
                }
                else
                {
                    TempData["Mensaje"] = $"El Producto con código {codigo} no fue encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = ex.Message;
            }

            // Redirige a la misma acción para refrescar el listado
            return RedirectToAction("MantenimientoProducto");
        }

        public IActionResult PageSuccess()
        {
            //
            return View();
        }

    }
}
