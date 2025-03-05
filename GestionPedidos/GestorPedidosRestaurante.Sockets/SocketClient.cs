using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GestorPedidosRestaurante.Sockets
{
    public class SocketClient
    {
        private readonly Socket _socket;

        public SocketClient(Socket socket)
        {
            _socket = socket;
        }

        public async Task EscucharMensajesAsync()
        {
            var buffer = new byte[1024];
            while (true)
            {
                var bytesRecibidos = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRecibidos);
                Console.WriteLine($"Mensaje recibido: {mensaje}");
            }
        }

        public async Task EnviarMensajeAsync(string mensaje)
        {
            var buffer = Encoding.UTF8.GetBytes(mensaje);
            await _socket.SendAsync(buffer, SocketFlags.None);
        }
    }
}