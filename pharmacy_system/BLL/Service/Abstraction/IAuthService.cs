using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Abstraction
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto?> LoginAsync(LoginDto dto);

        Task<object?> GetCurrentUserAsync(string userId);
    }
}
