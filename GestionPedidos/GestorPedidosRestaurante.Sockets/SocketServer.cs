using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GestorPedidosRestaurante.Sockets
{
    public class SocketServer
    {
        private readonly List<SocketClient> _clientesConectados = new List<SocketClient>();

        public async Task Iniciar()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 5000));
            socket.Listen(10);
            Console.WriteLine("Servidor de sockets iniciado. Esperando conexiones en el puerto 5000...");

            while (true)
            {
                var clienteSocket = await socket.AcceptAsync();
                var cliente = new SocketClient(clienteSocket);
                _clientesConectados.Add(cliente);
                _ = cliente.EscucharMensajesAsync(); // Escuchar mensajes del cliente
            }
        }

        public async Task NotificarClientes(string mensaje)
        {
            foreach (var cliente in _clientesConectados)
            {
                await cliente.EnviarMensajeAsync(mensaje);
            }
        }
    }
}