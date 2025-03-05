using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GestorPedidosRestaurante.Sockets
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var servidor = new SocketServer();
            await servidor.Iniciar();

            // Conectar un cliente de prueba
            var cliente = new TcpClient("localhost", 5000);
            var stream = cliente.GetStream();
            var buffer = new byte[1024];

            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Mensaje del servidor: {mensaje}");
            }
        }
    }
}