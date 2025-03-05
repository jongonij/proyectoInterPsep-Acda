using System;

namespace GestorPedidosRestaurante_API.Models
{
    public class IniciarSesionRequest
    {
        public string Correo { get; set; } // Correo cifrado con RSA
        public string Contraseña { get; set; } // Contraseña en texto plano 
    }
}