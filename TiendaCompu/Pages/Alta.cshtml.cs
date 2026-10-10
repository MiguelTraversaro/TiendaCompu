using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace TiendaCompu.Pages
{
    public class AltaModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AltaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Estas variables se conectarán con el formulario HTML
        [BindProperty]
        public string Nombre { get; set; }
        [BindProperty]
        public decimal Precio { get; set; }
        [BindProperty]
        public int IdCategoria { get; set; }

        public string MensajeExito { get; set; }
        public string MensajeError { get; set; }

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            try
            {
                string stringConexion = _configuration.GetConnectionString("ConexionSQL");

                using (SqlConnection conexion = new SqlConnection(stringConexion))
                {
                    conexion.Open();
                    string sql = "INSERT INTO productos (nombre, precio, idCategoria) VALUES (@nombre, @precio, @idCategoria)";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", Nombre);
                        comando.Parameters.AddWithValue("@precio", Precio);
                        comando.Parameters.AddWithValue("@idCategoria", IdCategoria);

                        comando.ExecuteNonQuery();
                    }
                }

                // para guardar la categoria
                string nombreCategoria = "";
                switch (IdCategoria)
                {
                    case 1: nombreCategoria = "Procesador"; break;
                    case 2: nombreCategoria = "Placa de video"; break;
                    case 3: nombreCategoria = "Memoria"; break;
                    case 4: nombreCategoria = "Motherboard"; break;
                    case 5: nombreCategoria = "Almacenamiento"; break;
                    default: nombreCategoria = "Desconocida"; break;
                }

                // se guarda el txt en el directorio
                string rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "log_altas.txt");

                // la linea del log a guardar
                string lineaLog = $"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Se agregó el producto: {Nombre} - Precio: ${Precio} - Categoría: {nombreCategoria} - IdCategoría: {IdCategoria}";

                using (StreamWriter writer = new StreamWriter(rutaArchivo, true))
                {
                    writer.WriteLine(lineaLog);
                }

                MensajeExito = "¡Producto guardado exitosamente en BD y registrado en el archivo de texto!";

                Nombre = "";
                Precio = 0;
                IdCategoria = 0;
            }
            catch (Exception ex)
            {
                MensajeError = "Ocurrió un error: " + ex.Message;
            }
        }
    }
}