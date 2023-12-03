using ProyFinalDESWB.Models;
using System;

using System.Data.SqlClient;

namespace ProyFinalDESWB.DAO
{
    public class ProductoDAO
    {

        private readonly string cad_conex;
        public ProductoDAO(IConfiguration config)
        {
            cad_conex = config.GetConnectionString("cn1");
        }


        public List<Producto> ListadoProductos()
        {
            List<Producto> lista = new List<Producto>();

            SqlDataReader dr = SqlHelper.ExecuteReader(cad_conex, "SP_LISTAR_PRODUCTO");
            {
                while (dr.Read())
                {
                    lista.Add(new Producto
                    {
                        codigo = dr.GetString(0),
                        descripcion = dr.GetString(1),
                        precio=dr.GetDecimal(2),
                        stock = dr.GetInt32(3),
                        disponibilidad = dr.GetString(4),
                        animal = dr.IsDBNull(5) ? null : dr.GetString(5),
                        tipocomi = dr.IsDBNull(6) ? null : dr.GetString(6),
                        fotopro = dr.IsDBNull(7) ? null : dr.GetString(7)
                    });
                }
            }

            return lista;
        }

        public List<Producto> BuscarProductos(string buscar)
        {
            List<Producto> lista = new List<Producto>();

            SqlDataReader dr = SqlHelper.ExecuteReader(cad_conex, "buscarProducto", buscar);
            {
                while (dr.Read())
                {
                    lista.Add(new Producto
                    {
                        codigo = dr.GetString(0),
                        animal = dr.IsDBNull(1) ? null : dr.GetString(1),
                        descripcion = dr.GetString(2),      
                        precio = dr.GetDecimal(3),
                        stock  = dr.GetInt32(4),
                        fotopro = dr.IsDBNull(5) ? null : dr.GetString(5)
                    });
                }
            }

            return lista;
        }


        public List<Animales> ListadoAnimales()

        {
            List<Animales> lista = new List<Animales>();

            SqlDataReader dr = SqlHelper.ExecuteReader(

                        cad_conex, "SP_LISTAR_ANIMALES");

            while (dr.Read())

            {
                lista.Add(new Animales

                {
                    cod_animal = dr.GetInt32(0),

                    nom_animal = dr.GetString(1),

                });

            }

            dr.Close();

            return lista;

        }
        public List<Tipopro> ListadoTipos()

        {
            List<Tipopro> lista = new List<Tipopro>();

            SqlDataReader dr = SqlHelper.ExecuteReader(

                        cad_conex, "SP_LISTAR_TIPO_PRODUCTO");

            while (dr.Read())

            {
                lista.Add(new Tipopro

                {
                    cod_tipo = dr.GetInt32(0),

                    nom_tipo = dr.GetString(1),

                });

            }

            dr.Close();

            return lista;

        }

        public string GrabarProducto(GrabarProducto obj)
        {
            string mensaje = "";
            try
            {
                SqlHelper.ExecuteNonQuery(cad_conex, "SP_REGISTRAR_PRODUCTO",
                   obj.descripcion,obj.precio ,obj.stock,
                    obj.animal, obj.tipocomi, string.IsNullOrEmpty(obj.fotopro) ? null : obj.fotopro);
                //
                mensaje = $"El Producto {obj.descripcion} " +
                           "fue registrado correctamente";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            //
            return mensaje;
        }

        public string EditProducto(Producto obj)
        {
            string mensaje = "";
            try
            {
                SqlHelper.ExecuteNonQuery(cad_conex, "SP_ACTUALIZAR_PRODUCTO",
                        obj.descripcion,obj.precio, obj.stock,
                        obj.animal, obj.tipocomi, obj.fotopro ,obj.codigo);
                //
                mensaje = $"El Producto {obj.descripcion} " +
                           "fue Editado correctamente";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            //
            return mensaje;
        }

        public string EliProducto(string codigo)
        {
            string mensaje = "";
            try
            {
                SqlHelper.ExecuteNonQuery(cad_conex, "SP_ELIMINAR_PRODUCTO", codigo);
                //
                mensaje = $"El Producto {codigo} se elimino correctamente";
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            //
            return mensaje;
        }

        public string GrabarVentas(string tipo ,
            string nomcard ,string nrocard,int cvv,string fechexp,decimal total, List<CarritoModel> lista_car)
        {
            string? numero = SqlHelper.ExecuteScalar(cad_conex,
                      "GRABAR_BOL_VENTA", tipo, nomcard, nrocard, cvv
                      , fechexp, total).ToString();

            foreach (var item in lista_car)
            {
                //grabar cada fila del detalle 
                SqlHelper.ExecuteNonQuery(cad_conex,
                            "GRABAR_DETA",
                            numero!, item.Codigo, item.Cantidad, item.Precio);
            }

            return numero;
        }


    }
}
