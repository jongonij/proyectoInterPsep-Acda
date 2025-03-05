using System;
using System.Threading.Tasks;

namespace GestorPedidosRestaurante.Client
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var cliente = new Cliente();

            while (true)
            {
                Console.WriteLine("Seleccione una opción:");
                Console.WriteLine("1. Crear Pedido");
                Console.WriteLine("2. Ver Pedido por ID");
                Console.WriteLine("3. Ver todos los pedidos");
                Console.WriteLine("4. Borrar Pedido");
                Console.WriteLine("5. Salir");

                string opcion = Console.ReadLine()?.Trim();

                switch (opcion)
                {
                    case "1":
                        await cliente.RealizarPedido();
                        break;

                    case "2":
                        Console.Write("Ingrese el ID del pedido a ver: ");
                        string inputId = Console.ReadLine();
                        if (Guid.TryParse(inputId, out Guid pedidoId))
                        {
                            await cliente.VerPedidoPorId(pedidoId);
                        }
                        else
                        {
                            Console.WriteLine("⚠️ Error: Ingrese un GUID válido.");
                        }
                        break;

                    case "3":
                        await cliente.VerPedidos();
                        break;

                    case "4":
                        await cliente.BorrarPedido();
                        break;

                    case "5":
                        Console.WriteLine("👋 Saliendo...");
                        return;

                    default:
                        Console.WriteLine("⚠️ Opción no válida. Intente de nuevo.");
                        break;
                }
            }
        }
    }
}
