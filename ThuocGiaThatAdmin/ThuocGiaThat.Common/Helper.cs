using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Common
{
    public static class NumberGenerator
    {
        private const string Prefix = "HGSG-";

        public static string GenerateOrderNumber()
        {
            var sb = GenernateNumbersString(10);

            return Prefix + sb.ToString();
        }

        public static string GenernateNumbersString(int numberOfNumbers)
        {
            var sb = new StringBuilder(numberOfNumbers);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[4];

                for (int i = 0; i < numberOfNumbers; i++)
                {
                    rng.GetBytes(data);
                    int digit = (int)(BitConverter.ToUInt32(data, 0) % 10);
                    sb.Append(digit);
                }
            }

            return sb.ToString();
        }  
    }
}
