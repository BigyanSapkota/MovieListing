using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public record SignInDto(
                string Email,
                string Password,
                SignUpMethod SignUpMethod
                  );

    public enum SignUpMethod
    {
        Internal,
        External
    }

}
