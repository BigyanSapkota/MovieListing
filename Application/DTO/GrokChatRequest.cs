using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
     public class GrokChatRequest
    {
        public string Message { get; set; } 
    }

    public class GrokChatResponse
    {
        public string Reply { get; set; }
    }

}
