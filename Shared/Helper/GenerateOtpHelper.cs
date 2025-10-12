using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper
{
     public class GenerateOtpHelper
    {
        public static string GenerateRandomOtp()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var otp = new char[6];

            for (int i = 0; i< otp.Length;i++)
            {
                otp[i] = chars[random.Next(chars.Length)];
            }

            return new string(otp);

        }

       

    }
}
