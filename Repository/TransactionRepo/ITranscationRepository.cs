using Microsoft.EntityFrameworkCore.Query;
using PayBridgeAPI.Models.Transcations;
using System.Linq.Expressions;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public interface ITranscationRepository<TransactionType> where TransactionType : class
    {
        Task<List<TransactionType>> GetAllTransactions(Expression<Func<TransactionType, bool>> predicate = null,  Func<IQueryable<TransactionType>, IIncludableQueryable<TransactionType, object>> include = null, Func<IQueryable<TransactionType>, IOrderedQueryable<TransactionType>> orderBy = null, bool isTracked = true);
        Task<TransactionType> GetSingleTransaction(Expression<Func<TransactionType, bool>> predicate = null, Func<IQueryable<TransactionType>, IIncludableQueryable<TransactionType, object>> include = null, Func<IQueryable<TransactionType>, IOrderedQueryable<TransactionType>> orderBy = null, bool isTracked = true);
        Task CreateTransaction(TransactionType transactionType);
    }
}
