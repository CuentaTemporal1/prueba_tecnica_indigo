namespace Prueba.Application.Dtos
{
    public class PaginatedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}