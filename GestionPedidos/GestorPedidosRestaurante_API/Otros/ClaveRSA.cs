using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;

namespace GestorPedidosRestaurante_API.Otros
{
    public class ClaveRSA
    {
        public string PublicKeyBase64 { get; }
        public string PrivateKeyBase64 { get; }

        public ClaveRSA(string publicKeyBase64, string privateKeyBase64)
        {
            PublicKeyBase64 = publicKeyBase64;
            PrivateKeyBase64 = privateKeyBase64;
        }

        public static ClaveRSA GenerarClaveRSA()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string publicKeyBase64 = rsa.ToXmlString(false); // Clave pública
                string privateKeyBase64 = rsa.ToXmlString(true); // Clave privada
                return new ClaveRSA(publicKeyBase64, privateKeyBase64);
            }
        }

        public static string CifrarConRSA(string texto, string publicKeyBase64)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyBase64);
                byte[] datosCifrados = rsa.Encrypt(Encoding.UTF8.GetBytes(texto), true);
                return Convert.ToBase64String(datosCifrados);
            }
        }

        public static string DescifrarConRSA(string textoCifrado, string privateKeyXml)
        {
            try
            {
                // Convertir el texto cifrado desde Base64
                byte[] textoCifradoBytes = Convert.FromBase64String(textoCifrado);

                // Crear una nueva instancia de RSACryptoServiceProvider
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // Cargar la clave privada en formato XML
                    rsa.FromXmlString(privateKeyXml);

                    // Descifrar los datos con la clave privada
                    byte[] datosDescifrados = rsa.Decrypt(textoCifradoBytes, true);

                    // Convertir los datos descifrados de bytes a texto
                    return Encoding.UTF8.GetString(datosDescifrados);
                }
            }
            catch (FormatException ex)
            {
                throw new Exception("El texto cifrado no está en formato Base64 válido. " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al descifrar el texto: " + ex.Message);
            }
        }


    }
}