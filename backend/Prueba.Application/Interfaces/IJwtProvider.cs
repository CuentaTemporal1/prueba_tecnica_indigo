using Prueba.Domain.Entities;
namespace Prueba.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}