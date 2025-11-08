namespace Prueba.Application.Dtos
{
    public class PaginatedRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }
        public string SortOrder { get; set; } = "ASC";
    }
}