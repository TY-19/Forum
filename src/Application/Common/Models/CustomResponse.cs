namespace Forum.Application.Common.Models;

public class CustomResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Payload { get; set; }
}
