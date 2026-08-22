using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Exceptions;
using FinanceHub.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinanceHub.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction =
            await _context.Database
                .BeginTransactionAsync();
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (EhViolacaoDeIdempotencyKey(ex))
        {
            throw new IdempotencyKeyJaProcessadaException();
        }
    }

    public async Task CommitAsync()
    {
        await SaveChangesAsync();

        if (_transaction is not null)
        {
            await _transaction.CommitAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }

    private static bool EhViolacaoDeIdempotencyKey(
        DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlException
            && sqlException.Number is 2601 or 2627
            && sqlException.Message.Contains(
                "UX_Compras_IdempotencyKey",
                StringComparison.OrdinalIgnoreCase);
    }
}