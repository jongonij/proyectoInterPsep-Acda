using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;

public class Administrador
{
    public string Correo { get; set; }
    public string Contraseña { get; set; }
    public string Nombre { get; set; }
    public string ContraseñaCifrada { get; set; }
    public string ClavePrivada { get; set; }
}

public class Pedido
{
    public Guid Id { get; set; }
    public string Estado { get; set; }
}

public enum EstadoPedido
{
    Pendiente,
    EnProceso,
    Completado,
    Cancelado
}

public class ActualizarEstadoRequest
{
    public string Correo { get; set; }
    public string Contraseña { get; set; }
    public EstadoPedido NuevoEstado { get; set; }
}

namespace ClienteGestorPedidos
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static bool usuarioAutenticado = false; // Bandera para controlar si el usuario está autenticado
        private static string token = ""; // Token o algún valor de sesión que usarás para validar el inicio de sesión

        // Variables para almacenar las credenciales del administrador autenticado
        private static string correoAutenticado = "";
        private static string contraseñaAutenticada = "";

        static async Task Main(string[] args)
        {
            client.BaseAddress = new Uri("https://localhost:5001/api/"); // Dirección de la API

            // Menú de opciones
            while (true)
            {
                // Si el usuario no está autenticado, solo mostrar opciones de inicio de sesión o salir
                if (!usuarioAutenticado)
                {
                    Console.WriteLine("Selecciona una opción:");
                    Console.WriteLine("1. Iniciar sesión");
                    Console.WriteLine("2. Registrarse");
                    Console.WriteLine("3. Salir");

                    if (!int.TryParse(Console.ReadLine(), out int opcion))
                    {
                        Console.WriteLine("Por favor, selecciona una opción válida.");
                        continue;
                    }

                    try
                    {
                        if (opcion == 1)
                        {
                            // Solicitar credenciales para el inicio de sesión
                            Console.WriteLine("Introduce el correo del administrador:");
                            string correo = Console.ReadLine();

                            Console.WriteLine("Introduce la contraseña del administrador:");
                            string contraseña = Console.ReadLine();

                            // Cifrar la contraseña antes de enviarla
                            string contraseñaCifrada = CifrarContraseña(contraseña);

                            // Realizar la solicitud para verificar las credenciales
                            var loginData = new { Correo = correo, Contraseña = contraseñaCifrada };
                            var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                            var response = await client.PostAsync("administradores/login", content); // Suponiendo que tienes una API que maneja el login

                            if (response.IsSuccessStatusCode)
                            {
                                // Obtener el token o alguna respuesta de autenticación (esto depende de tu API)
                                var responseBody = await response.Content.ReadAsStringAsync();
                                var responseJson = JsonConvert.DeserializeObject<dynamic>(responseBody);

                                // Si la respuesta contiene un token, guárdalo
                                if (responseJson.token != null)
                                {
                                    token = responseJson.token;
                                }

                                // Almacenar las credenciales del administrador autenticado
                                correoAutenticado = correo;
                                contraseñaAutenticada = CifrarContraseña(contraseña); // Cifrar la contraseña antes de almacenarla

                                usuarioAutenticado = true; // Cambiar el estado de la sesión a autenticado
                                Console.WriteLine(responseJson.message);
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                var errorJson = JsonConvert.DeserializeObject<dynamic>(errorContent);
                                Console.WriteLine($"Error al iniciar sesión. Código de error: {response.StatusCode}. Detalles: {errorJson.message}");
                            }
                        }
                        else if (opcion == 2)
                        {
                            // Solicitar datos del administrador para registrarse
                            Console.WriteLine("Introduce el correo del administrador:");
                            string correo = Console.ReadLine();

                            Console.WriteLine("Introduce la contraseña del administrador:");
                            string contraseña = Console.ReadLine();

                            Console.WriteLine("Introduce el nombre del administrador:");
                            string nombre = Console.ReadLine();

                            // Cifrar la contraseña antes de almacenarla
                            string contraseñaCifrada = CifrarContraseña(contraseña);

                            // Crear objeto administrador
                            var administrador = new Administrador
                            {
                                Correo = correo,
                                Contraseña = contraseñaCifrada,
                                Nombre = nombre,
                                ContraseñaCifrada = contraseñaCifrada,
                                ClavePrivada = "ClavePrivada123" // Valor predeterminado
                            };

                            var content = new StringContent(JsonConvert.SerializeObject(administrador), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("administradores/registrar", content);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Administrador registrado con éxito.");
                            }
                            else
                            {
                                Console.WriteLine($"Error al registrar el administrador. Código de error: {response.StatusCode}");
                            }
                        }
                        else if (opcion == 3)
                        {
                            Console.WriteLine("Saliendo del programa...");
                            break; // Salir del ciclo y terminar el programa
                        }
                        else
                        {
                            Console.WriteLine("Opción no válida.");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Hubo un error al realizar la solicitud: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
                    }
                }
                else
                {
                    // Mostrar el menú principal si el usuario está autenticado
                    Console.WriteLine("Selecciona una opción:");
                    Console.WriteLine("1. Registrar Administrador");
                    Console.WriteLine("2. Cambiar Estado de Pedido");
                    Console.WriteLine("3. Cerrar sesión");
                    Console.WriteLine("4. Salir");

                    if (!int.TryParse(Console.ReadLine(), out int opcion))
                    {
                        Console.WriteLine("Por favor, selecciona una opción válida.");
                        continue;
                    }

                    try
                    {
                        if (opcion == 1)
                        {
                            // Solicitar datos del administrador
                            Console.WriteLine("Introduce el correo del administrador:");
                            string correo = Console.ReadLine();

                            Console.WriteLine("Introduce la contraseña del administrador:");
                            string contraseña = Console.ReadLine();

                            Console.WriteLine("Introduce el nombre del administrador:");
                            string nombre = Console.ReadLine();

                            // Cifrar la contraseña antes de almacenarla
                            string contraseñaCifrada = CifrarContraseña(contraseña);

                            // Crear objeto administrador
                            var administrador = new Administrador
                            {
                                Correo = correo,
                                Contraseña = contraseñaCifrada,
                                Nombre = nombre,
                                ContraseñaCifrada = contraseñaCifrada,
                                ClavePrivada = "ClavePrivada123" // Valor predeterminado
                            };

                            var content = new StringContent(JsonConvert.SerializeObject(administrador), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("administradores/registrar", content);

                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Administrador registrado con éxito.");
                            }
                            else
                            {
                                Console.WriteLine($"Error al registrar el administrador. Código de error: {response.StatusCode}");
                            }
                        }
                        else if (opcion == 2)
                        {
                            // Verificar si el usuario está autenticado antes de permitir cambiar el estado de los pedidos
                            if (usuarioAutenticado)
                            {
                                // Obtener la lista de pedidos
                                var responsePedidos = await client.GetAsync("pedidos"); // Asegúrate de que esta URL sea correcta
                                if (responsePedidos.IsSuccessStatusCode)
                                {
                                    var pedidos = JsonConvert.DeserializeObject<List<Pedido>>(await responsePedidos.Content.ReadAsStringAsync());

                                    if (pedidos.Count > 0)
                                    {
                                        Console.WriteLine("Seleccione el número del pedido para cambiar su estado:");

                                        // Mostrar lista de pedidos numerada
                                        for (int i = 0; i < pedidos.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. Pedido ID: {pedidos[i].Id} - Estado: {pedidos[i].Estado}");
                                        }

                                        // Solicitar el número del pedido
                                        if (int.TryParse(Console.ReadLine(), out int pedidoIndex) && pedidoIndex >= 1 && pedidoIndex <= pedidos.Count)
                                        {
                                            var pedidoSeleccionado = pedidos[pedidoIndex - 1];

                                            // Mostrar opciones de estado
                                            Console.WriteLine("Seleccione el nuevo estado del pedido:");
                                            foreach (var estado in Enum.GetValues(typeof(EstadoPedido)))
                                            {
                                                Console.WriteLine($"{(int)estado}. {estado}");
                                            }

                                            if (int.TryParse(Console.ReadLine(), out int estadoIndex) && Enum.IsDefined(typeof(EstadoPedido), estadoIndex))
                                            {
                                                var nuevoEstado = (EstadoPedido)estadoIndex;

                                                // Crear objeto ActualizarEstadoRequest utilizando las credenciales almacenadas y el estado seleccionado
                                                var actualizarEstadoRequest = new ActualizarEstadoRequest
                                                {
                                                    Correo = correoAutenticado, // Utiliza el correo del administrador autenticado
                                                    Contraseña = contraseñaAutenticada, // Utiliza la contraseña cifrada del administrador autenticado
                                                    NuevoEstado = nuevoEstado // Cambiar el estado según corresponda
                                                };

                                                var content = new StringContent(JsonConvert.SerializeObject(actualizarEstadoRequest), Encoding.UTF8, "application/json");

                                                var response = await client.PutAsync($"pedidos/actualizar-estado/{pedidoSeleccionado.Id}", content);

                                                if (response.IsSuccessStatusCode)
                                                {
                                                    Console.WriteLine($"Estado del pedido {pedidoSeleccionado.Id} cambiado a {nuevoEstado}.");
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"Error al cambiar el estado del pedido. Código de error: {response.StatusCode}");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Selección inválida. Debes elegir un número de la lista.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Selección inválida. Debes elegir un número de la lista.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No hay pedidos disponibles.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Error al obtener los pedidos. Código de error: {responsePedidos.StatusCode}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Por favor, inicie sesión primero.");
                            }
                        }
                        else if (opcion == 3)
                        {
                            // Cerrar sesión
                            usuarioAutenticado = false;
                            token = "";
                            Console.WriteLine("Has cerrado sesión.");
                        }
                        else if (opcion == 4)
                        {
                            Console.WriteLine("Saliendo del programa...");
                            break; // Salir del ciclo y terminar el programa
                        }
                        else
                        {
                            Console.WriteLine("Opción no válida.");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Hubo un error al realizar la solicitud: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
                    }
                }
            }
        }

        public static string CifrarContraseña(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contraseña);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }

}

