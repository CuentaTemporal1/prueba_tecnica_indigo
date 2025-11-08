using Microsoft.AspNetCore.Http;
using Prueba.Application.Dtos;

namespace Prueba.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);

        Task<UserDto> GetUserByEmailAsync(string email);

        Task<UserDto> GetUserByIdAsync(int id);

        Task<UserDto> RegisterAsync(string email, string password, string firstName, string lastName, IFormFile profilePicture);
    }
}