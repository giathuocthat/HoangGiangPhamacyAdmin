using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common
{
    public static class OrderNumberGenerator
    {
        private const string Prefix = "HGSG-";

        public static string GenerateOrderNumber()
        {
            var sb = new StringBuilder(10);

            // Tạo 10 chữ số random (0-9)
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[4];

                for (int i = 0; i < 10; i++)
                {
                    rng.GetBytes(data);
                    int digit = (int)(BitConverter.ToUInt32(data, 0) % 10);
                    sb.Append(digit);
                }
            }

            return Prefix + sb.ToString();
        }
    }
}
