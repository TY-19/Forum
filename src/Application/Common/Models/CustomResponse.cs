namespace Forum.Application.Common.Models;

public class CustomResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string> Errors {get; set; } = [];

    public CustomResponse()
    { }

    public CustomResponse(Exception exception)
    {
        Succeeded = false;
        Message = exception.Message;
    }
}

public class CustomResponse<T> : CustomResponse
{
    public T? Payload { get; set; }
    public CustomResponse() : base()
    { }

    public CustomResponse(Exception exception) : base(exception)
    { }
}
