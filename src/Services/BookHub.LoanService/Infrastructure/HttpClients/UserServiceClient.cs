using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookHub.LoanService.Infrastructure.HttpClients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;

    public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId, string authToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{userId}", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Impossible de récupérer l'utilisateur {UserId}", userId);
            return null;
        }
    }
}
