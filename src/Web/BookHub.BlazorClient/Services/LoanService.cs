using BookHub.Shared.DTOs;
using System.Net.Http.Json;

namespace BookHub.BlazorClient.Services
{
    public class LoanService : ILoanService
    {
        private readonly HttpClient _httpClient;

        public LoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(Guid userId)
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>($"api/loans/user/{userId}");
            return response ?? Enumerable.Empty<LoanDto>();
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<LoanDto>>("api/loans/overdue");
            return response ?? Enumerable.Empty<LoanDto>();
        }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto, string authToken, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/loans")
            {
                Content = JsonContent.Create(createLoanDto)
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<LoanDto>(cancellationToken: cancellationToken)
                   ?? throw new Exception("Erreur lors de la création de l'emprunt");
        }

        public async Task<LoanDto?> ReturnLoanAsync(Guid loanId)
        {
            var response = await _httpClient.PutAsync($"api/loans/{loanId}/return", null);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<LoanDto>();
        }
    }
}
