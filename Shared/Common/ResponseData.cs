using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common
{
     public class ResponseData
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }


    public class ResponseData<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }


    public class TokenData
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }



}