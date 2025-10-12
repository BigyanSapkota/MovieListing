using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Shared.Common;

namespace Application.Interface.Services
{
     public interface IGoogleAuthService
    {
        public Task<TokenDto> AuthenticateWithGoogleAsync(string accessToken,string idToken);
        public Task<TokenDto> RefreshTokenAsync(string refreshToken);
    }
}
