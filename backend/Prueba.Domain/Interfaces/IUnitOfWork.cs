using System.Threading.Tasks;

namespace Prueba.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        Task<int> CompleteAsync();
    }
}