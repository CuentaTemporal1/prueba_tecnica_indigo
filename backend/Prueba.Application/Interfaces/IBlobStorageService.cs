namespace Prueba.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream stream, string path, string contentType);
        string GetFileUrl(string path);
        Task DeleteAsync(string path);
    }
}