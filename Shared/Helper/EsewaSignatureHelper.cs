using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Helper
{
     public class EsewaSignatureHelper
    {

        public static string GenerateSignature(string signedFields, IDictionary<string, string> data, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(signedFields))
                throw new ArgumentException("signedFields cannot be empty.", nameof(signedFields));

            if (data == null || data.Count == 0)
                throw new ArgumentException("data cannot be null or empty.", nameof(data));

            var fieldNames = signedFields.Split(',');
            var rawData = string.Join(",", fieldNames.Select(f => $"{f}={data[f]}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifySignature(string signedFields, IDictionary<string, string> data, string providedSignature, string secretKey)
        {
            if (string.IsNullOrWhiteSpace(providedSignature))
                return false;

            var generatedSignature = GenerateSignature(signedFields, data, secretKey);
            //return generatedSignature == providedSignature;
            return CryptographicOperations.FixedTimeEquals(
                                         Encoding.UTF8.GetBytes(generatedSignature),
                                          Encoding.UTF8.GetBytes(providedSignature));
        }


        public static string GenerateDV(string MerchantCode,string MD,string refrenceNumber,decimal amount,string currencyCode, string date, string r1, string r2, string returnUrl,string SecretKey)
        {
            var dataToHash = $"{MerchantCode},{MD},{refrenceNumber},{amount},{currencyCode},{date},{r1},{r2},{returnUrl},{SecretKey}";
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(SecretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            return Convert.ToBase64String(hash);
        }
            



    }
}
