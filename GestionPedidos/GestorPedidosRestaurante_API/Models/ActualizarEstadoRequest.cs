// Clase para la solicitud de actualización de estado
using GestorPedidosRestaurante_API.Models;

public class ActualizarEstadoRequest
        {
            public string Correo { get; set; }
            public string Contraseña { get; set; }
            public EstadoPedido NuevoEstado { get; set; }
        }