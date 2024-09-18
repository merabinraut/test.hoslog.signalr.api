namespace hoslog.signalr.api.Models.Common.CommonAPIResponse;

public class CommonAPIResponse
{
    public string code { get; set; } = null!;
    public string message { get; set; } = null!;
    public object data { get; set; } = null!;
}