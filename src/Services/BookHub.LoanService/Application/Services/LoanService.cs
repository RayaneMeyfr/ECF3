using BookHub.LoanService.Domain.Entities;
using BookHub.LoanService.Domain.Ports;
using BookHub.Shared.DTOs;

namespace BookHub.LoanService.Application.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default);
    Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default);
}

public class LoanService : ILoanService
{
    private readonly ILoanRepository _repository;
    private readonly ICatalogServiceClient _catalogClient;
    private readonly IUserServiceClient _userClient;
    private readonly ILogger<LoanService> _logger;

    public LoanService(
        ILoanRepository repository,
        ICatalogServiceClient catalogClient,
        IUserServiceClient userClient,
        ILogger<LoanService> logger)
    {
        _repository = repository;
        _catalogClient = catalogClient;
        _userClient = userClient;
        _logger = logger;
    }

    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetAllAsync(cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        return loan == null ? null : MapToDto(loan);
    }

    public async Task<IEnumerable<LoanDto>> GetLoansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var loans = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var overdueLoans = await _repository.GetOverdueLoansAsync(cancellationToken);

        var dtos = overdueLoans.Select(loan =>
        {
            var daysOverdue = (DateTime.UtcNow - loan.DueDate).Days;
            var penalty = daysOverdue > 0 ? daysOverdue * 0.50m : 0m;

            return new LoanDto(
                loan.Id,
                loan.UserId,
                loan.BookId,
                loan.BookTitle,
                loan.UserEmail,
                loan.LoanDate,
                loan.DueDate,
                loan.ReturnDate,
                loan.Status == Domain.Entities.LoanStatus.Active && daysOverdue > 0 ? Shared.DTOs.LoanStatus.Overdue
                : (Shared.DTOs.LoanStatus)loan.Status, penalty
            );
        });

        return dtos;
    }

    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var userLoans = await _repository.GetByUserIdAsync(dto.UserId, cancellationToken);
        var activeLoansCount = userLoans.Count(l => l.Status == Domain.Entities.LoanStatus.Active);
        if (activeLoansCount >= Loan.MaxActiveLoansPerUser)
        {
            throw new InvalidOperationException("L'utilisateur a déjà 5 emprunts actifs.");
        }

        var bookAlreadyLoaned = userLoans.Any(l => l.BookId == dto.BookId && l.Status == Domain.Entities.LoanStatus.Active);
        if (bookAlreadyLoaned)
        {
            throw new InvalidOperationException("Ce livre est déjà emprunté.");
        }

        var book = await _catalogClient.GetBookAsync(dto.BookId, cancellationToken);
        if (book == null) {
            throw new InvalidOperationException("Livre introuvable.");
        }

        var user = await _userClient.GetUserAsync(dto.UserId, cancellationToken);
        if (user == null){
            throw new InvalidOperationException("Utilisateur introuvable.");

        }

        var decremented = await _catalogClient.DecrementAvailabilityAsync(dto.BookId, cancellationToken);
        if (!decremented)
        {
            throw new InvalidOperationException("Impossible de réserver ce livre pour le moment.");
        }

        var loan = Loan.Create(dto.UserId, dto.BookId, book.Title, user.Email);

        await _repository.AddAsync(loan, cancellationToken);

        return new LoanDto(
            loan.Id,
            loan.UserId,
            loan.BookId,
            loan.BookTitle,
            loan.UserEmail,
            loan.LoanDate,
            loan.DueDate,
            loan.ReturnDate,
            (Shared.DTOs.LoanStatus)loan.Status,
            0m
        );
    }

    public async Task<LoanDto?> ReturnLoanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _repository.GetByIdAsync(id, cancellationToken);
        if (loan == null)
        {
            throw new InvalidOperationException("Emprunt introuvable.");
        }

        if (loan.Status == Domain.Entities.LoanStatus.Returned)
        {
            throw new InvalidOperationException("Ce livre a déjà été retourné.");
        }

        loan.Return();

        await _repository.UpdateAsync(loan, cancellationToken);

        var incremented = await _catalogClient.IncrementAvailabilityAsync(loan.BookId, cancellationToken);
        if (!incremented)
        {
            _logger.LogWarning("Impossible de mettre à jour la disponibilité du livre {BookId}", loan.BookId);
        }

        var loanDto = new LoanDto(
            loan.Id,
            loan.UserId,
            loan.BookId,
            loan.BookTitle,
            loan.UserEmail,
            loan.LoanDate,
            loan.DueDate,
            loan.ReturnDate,
            (Shared.DTOs.LoanStatus)loan.Status,               
            loan.PenaltyAmount
        );

        return loanDto;
    }

    private static LoanDto MapToDto(Loan loan) => new(
        loan.Id,
        loan.UserId,
        loan.BookId,
        loan.BookTitle,
        loan.UserEmail,
        loan.LoanDate,
        loan.DueDate,
        loan.ReturnDate,
        (Shared.DTOs.LoanStatus)(int)loan.Status,
        loan.IsOverdue ? loan.CalculatePenalty() : loan.PenaltyAmount
    );
}
