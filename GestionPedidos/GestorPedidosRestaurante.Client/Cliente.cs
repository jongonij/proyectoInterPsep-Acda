using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using GestorPedidosRestaurante_API.Models;

namespace GestorPedidosRestaurante.Client
{
    public class Cliente
    {
        private readonly HttpClient _httpClient;
        private readonly Socket _socket;

        public Cliente()
        {
            _httpClient = new HttpClient();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task ConectarAlServidorDeSockets()
        {
            // Cambiar al puerto correcto para el servidor de sockets (5000)
            await _socket.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));  // Cambiado a puerto 5000
            _ = EscucharNotificaciones();
        }

        private async Task EscucharNotificaciones()
        {
            var buffer = new byte[1024];
            while (true)
            {
                var bytesRecibidos = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRecibidos);
                Console.WriteLine($"Notificación: {mensaje}");
            }
        }

        public async Task RealizarPedido()
        {
            var clienteId = Guid.NewGuid(); // Se genera automáticamente
            Console.WriteLine($"🆕 Se ha generado un nuevo ClienteId: {clienteId}");

            var items = new List<ItemPedido>();

            while (true)
            {
                var productoId = Guid.NewGuid(); // Se autogenera el ProductoId
                Console.WriteLine($"🆕 Se ha generado un nuevo ProductoId: {productoId}");

                Console.Write("Ingrese el nombre del producto: ");
                string nombreProducto = Console.ReadLine()?.Trim();

                Console.Write("Ingrese la cantidad para este producto: ");
                if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
                {
                    Console.WriteLine("⚠️ Error: Ingrese un número válido para la cantidad.");
                    continue;
                }

                items.Add(new ItemPedido { ProductoId = productoId, Nombre = nombreProducto, Cantidad = cantidad });

                Console.Write("¿Desea agregar otro producto? (s/n): ");
                string respuesta = Console.ReadLine()?.Trim().ToLower();
                if (respuesta != "s") break;
            }

            var pedido = new Pedido
            {
                Id = Guid.NewGuid(), // Este ID será ignorado por el servidor
                ClienteId = clienteId,
                Items = items,
                Estado = new EstadoPedido()
            };

            var json = JsonSerializer.Serialize(pedido);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:5001/api/pedidos", content);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoPedido = await response.Content.ReadFromJsonAsync<Pedido>();
                    Console.WriteLine($"🆕 El ID del nuevo pedido es: {nuevoPedido.Id}");
                    Console.WriteLine("✅ Pedido realizado con éxito.");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error: {response.StatusCode}");
                    Console.WriteLine($"📄 Respuesta: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción: {ex.Message}");
            }
        }


        public async Task VerPedidoPorId(Guid pedidoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:5001/api/pedidos/{pedidoId}");

                if (response.IsSuccessStatusCode)
                {
                    var pedido = await response.Content.ReadFromJsonAsync<Pedido>();

                    if (pedido != null)
                    {
                        Console.WriteLine($"📦 Pedido ID: {pedido.Id}");
                        Console.WriteLine($"👤 Cliente ID: {pedido.ClienteId}");
                        Console.WriteLine($"📌 Estado: {pedido.Estado}");
                        Console.WriteLine("🛒 Productos:");
                        foreach (var item in pedido.Items)
                        {
                            Console.WriteLine($"  - {item.Nombre} (ID: {item.ProductoId}) x {item.Cantidad}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Error: No se pudo obtener el pedido.");
                    }
                }
                else
                {
                    Console.WriteLine($"❌ Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción: {ex.Message}");
            }
        }



        public async Task VerPedidos()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:5001/api/pedidos");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("📋 Pedidos:");
                    Console.WriteLine(data);
                }
                else
                {
                    Console.WriteLine($"❌ Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción: {ex.Message}");
            }
        }

        public async Task BorrarPedido()
        {
            Console.Write("Ingrese el ID del pedido a eliminar: ");
            string input = Console.ReadLine();

            if (!Guid.TryParse(input, out Guid pedidoId))
            {
                Console.WriteLine("⚠️ Error: Ingrese un GUID válido.");
                return;
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"https://localhost:5001/api/pedidos/{pedidoId}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Pedido eliminado con éxito.");
                }
                else
                {
                    Console.WriteLine($"❌ Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción: {ex.Message}");
            }
        }
    }

}
