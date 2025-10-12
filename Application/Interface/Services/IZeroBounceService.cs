using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Microsoft.Extensions.Configuration;

namespace Application.Interface.Services
{
     public interface IZeroBounceService
    {
       public Task<ZeroBounceResponse> ValidateEmailAsync(string email);

    }
}
