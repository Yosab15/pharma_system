using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public DateTime Expiration { get; set; }

        public string UserName { get; set; } = null!;

        public List<string> Roles { get; set; } = new();
    }
}
