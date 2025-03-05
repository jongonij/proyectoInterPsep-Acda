using System;
using System.Security.Cryptography;
using System.Text;

namespace GestorPedidosRestaurante_API.Models
{
    public class Administrador
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } // Encrypted with RSA
        public string Correo { get; set; } // Encrypted with RSA
        public string ContraseñaCifrada { get; set; } // Hashed with SHA-256
        public string ClavePrivada { get; set; }

        // Propiedad temporal para manejar la entrada de la contraseña en texto plano
        public string Contraseña { get; set; }

        // Método para cifrar la contraseña antes de almacenarla
        public void EstablecerContraseña(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contraseña);
                byte[] hash = sha256.ComputeHash(bytes);
                ContraseñaCifrada = Convert.ToBase64String(hash);
            }
        }

        // Método para verificar si una contraseña ingresada coincide con la almacenada
        public bool VerificarContraseña(string contraseñaIngresada)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contraseñaIngresada);
                byte[] hash = sha256.ComputeHash(bytes);
                string hashIngresado = Convert.ToBase64String(hash);

                return hashIngresado == ContraseñaCifrada;
            }
        }
    }
}