namespace NexoMovil.Core.CustomEntities
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public object? Errors { get; set; }
        public string? TraceId { get; set; } = null!;
    }
}
