using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace TiendaCompu.Pages
{
    public class BajaModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public BajaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class ProductoInfo
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public string CategoriaDesc { get; set; }
        }

        public List<ProductoInfo> ListaProductos = new List<ProductoInfo>();

        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }

        public void OnGet()
        {
            CargarDatos();
        }

        public void OnPostEliminar(int idProducto)
        {
            try
            {
                string stringConexion = _configuration.GetConnectionString("ConexionSQL");
                using (SqlConnection conexion = new SqlConnection(stringConexion))
                {
                    conexion.Open();

                    string sql = "DELETE FROM productos WHERE idProducto = @idProducto";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@idProducto", idProducto);
                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MensajeExito = "El producto ha sido eliminado del sistema de forma permanente.";
                        }
                        else
                        {
                            MensajeError = "No se encontró el producto a eliminar.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeError = "Ocurrió un error al intentar eliminar: " + ex.Message;
            }

            CargarDatos();
        }

        private void CargarDatos()
        {
            ListaProductos.Clear();
            string stringConexion = _configuration.GetConnectionString("ConexionSQL");

            using (SqlConnection conexion = new SqlConnection(stringConexion))
            {
                conexion.Open();
                string sql = @"SELECT p.idProducto, p.nombre, p.precio, c.descripcion as categoriaDesc 
                               FROM productos p 
                               INNER JOIN categorias c ON p.idCategoria = c.idCategoria
                               ORDER BY p.idProducto";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            ListaProductos.Add(new ProductoInfo
                            {
                                Id = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Precio = lector.GetDecimal(2),
                                CategoriaDesc = lector.GetString(3)
                            });
                        }
                    }
                }
            }
        }
    }
}