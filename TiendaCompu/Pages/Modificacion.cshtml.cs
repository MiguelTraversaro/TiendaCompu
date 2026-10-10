using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace TiendaCompu.Pages
{
    public class ModificacionModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ModificacionModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class ProductoInfo
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public int IdCategoria { get; set; }
            public string CategoriaDesc { get; set; }
        }

        public List<ProductoInfo> ListaProductos = new List<ProductoInfo>();

        // Esta variable dice que fila debe mostrar los <input> en lugar de texto
        [BindProperty]
        public int IdEditando { get; set; }

        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }

        public void OnGet()
        {
            CargarDatos();
        }

        public void OnPostEditar(int idProducto)
        {
            IdEditando = idProducto;
            CargarDatos();
        }

        public void OnPostCancelar()
        {
            IdEditando = 0;
            CargarDatos();
        }

        public void OnPostGuardar(int idProducto, string nombreOriginal, decimal precioOriginal, int idCategoriaOriginal, string nombre, decimal precio, int idCategoria)
        {
            // Validar que un dato se haya modificado
            if (nombre == nombreOriginal && precio == precioOriginal && idCategoria == idCategoriaOriginal)
            {
                MensajeError = $"No se detectaron cambios en el producto '{nombreOriginal}'.";
                IdEditando = idProducto;
                CargarDatos();
                return;
            }

            try
            {
                string stringConexion = _configuration.GetConnectionString("ConexionSQL");
                using (SqlConnection conexion = new SqlConnection(stringConexion))
                {
                    conexion.Open();
                    string sql = "UPDATE productos SET nombre = @nombre, precio = @precio, idCategoria = @idCategoria WHERE idProducto = @idProducto";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@precio", precio);
                        comando.Parameters.AddWithValue("@idCategoria", idCategoria);
                        comando.Parameters.AddWithValue("@idProducto", idProducto);

                        comando.ExecuteNonQuery();
                    }
                }
                MensajeExito = "¡Producto actualizado correctamente!";
                IdEditando = 0;
            }
            catch (Exception ex)
            {
                MensajeError = "Error: " + ex.Message;
                IdEditando = idProducto;
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
                string sql = @"SELECT p.idProducto, p.nombre, p.precio, p.idCategoria, c.descripcion as categoriaDesc 
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
                                IdCategoria = lector.GetInt32(3),
                                CategoriaDesc = lector.GetString(4)
                            });
                        }
                    }
                }
            }
        }
    }
}