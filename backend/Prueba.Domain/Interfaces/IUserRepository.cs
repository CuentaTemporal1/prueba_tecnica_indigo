using Prueba.Domain.Entities;
using System.Threading.Tasks;

namespace Prueba.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}