using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
     public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        
    }


    public class GoogleUserProfileDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
    }

    public class GoogleSignInRequest
    {
        public string AccessToken { get; set; } = default!;
        public string IdToken { get; set; } = default!;
    }

}
