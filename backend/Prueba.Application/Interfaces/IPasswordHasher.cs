namespace Prueba.Application.Interfaces
{
    // Esta interfaz será implementada por Infrastructure
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string hash, string password);
    }
}