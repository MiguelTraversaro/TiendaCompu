using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace TiendaCompu.Pages
{
    public class ConsultaModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ConsultaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class ProductoInfo
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public string Categoria { get; set; }
        }

        public List<ProductoInfo> ListaProductos = new List<ProductoInfo>();

        public void OnGet()
        {
            string stringConexion = _configuration.GetConnectionString("ConexionSQL");

            using (SqlConnection conexion = new SqlConnection(stringConexion))
            {
                conexion.Open();
                string sql = @"SELECT p.idProducto, p.nombre, p.precio, c.descripcion as categoria 
                               FROM productos p 
                               INNER JOIN categorias c ON p.idCategoria = c.idCategoria";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            ProductoInfo producto = new ProductoInfo
                            {
                                Id = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Precio = lector.GetDecimal(2),
                                Categoria = lector.GetString(3)
                            };
                            ListaProductos.Add(producto);
                        }
                    }
                }
            }
        }
    }
}