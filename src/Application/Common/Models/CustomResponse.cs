namespace Forum.Application.Common.Models;

public class CustomResponse
{
    public bool Succeed { get; set; }
    public string? Message { get; set; }
}

public class CustomResponse<T>
{
    public bool Succeed { get; set; }
    public string? Message { get; set; }
    public T? Payload { get; set; }
}
