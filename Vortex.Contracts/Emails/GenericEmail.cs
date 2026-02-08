namespace Vortex.Contracts.Emails;

public class GenericEmail
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Props { get; set; }
}