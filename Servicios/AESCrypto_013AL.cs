using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Servicios_013AL
{
    public static class AESCrypto_013AL
    {
        private static readonly string Key = "EstaEsMiClaveAES1234567890123456"; // 32 caracteres (256 bits)
        private static readonly string IV = "EstaEsMiIV123456"; // 16 caracteres (128 bits)

        public static string Encriptar_013AL(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return string.Empty;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] bytesTexto = Encoding.UTF8.GetBytes(textoPlano);
                    byte[] bytesEncriptados = encryptor.TransformFinalBlock(bytesTexto, 0, bytesTexto.Length);
                    return Convert.ToBase64String(bytesEncriptados);
                }
            }
        }

        public static string Desencriptar_013AL(string textoEncriptado)
        {
            if (string.IsNullOrEmpty(textoEncriptado)) return string.Empty;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] bytesEncriptados = Convert.FromBase64String(textoEncriptado);
                    byte[] bytesDesencriptados = decryptor.TransformFinalBlock(bytesEncriptados, 0, bytesEncriptados.Length);
                    return Encoding.UTF8.GetString(bytesDesencriptados);
                }
            }
        }
    }
}
