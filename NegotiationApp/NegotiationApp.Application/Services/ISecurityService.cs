namespace NegotiationApp.Application.Services
{
    public interface ISecurityService
    {
        string GenerateJwtToken(string username, string role);
    }
}