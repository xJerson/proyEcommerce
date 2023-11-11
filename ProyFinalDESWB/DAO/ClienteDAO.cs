using Microsoft.Extensions.Configuration;
using ProyFinalDESWB.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace ProyFinalDESWB.DAO
{
    


    public class ClienteDAO
    {
        private readonly string cad_conex;

 
        public ClienteDAO(IConfiguration config)
        {
            cad_conex = config.GetConnectionString("cn1");
        }

        public List<Cliente> ListadoClientes()

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



    }
}
