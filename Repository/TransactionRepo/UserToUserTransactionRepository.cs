using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models.Transcations;
using Stripe.Climate;
using System.Linq.Expressions;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public class UserToUserTransactionRepository : ITranscationRepository<UserToUserTransaction>
    {
        private readonly PayBridgeDbContext _context;

        public UserToUserTransactionRepository(PayBridgeDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserToUserTransaction>> GetAllTransactions(
            Expression<Func<UserToUserTransaction, bool>> predicate = null,
            Func<IQueryable<UserToUserTransaction>, IIncludableQueryable<UserToUserTransaction, object>> include = null,
            Func<IQueryable<UserToUserTransaction>, IOrderedQueryable<UserToUserTransaction>> orderBy = null,
            bool isTracked = true)
        {
            IQueryable<UserToUserTransaction> queryToDb = _context.Set<UserToUserTransaction>();

            if (include != null)
            {
                queryToDb = include(queryToDb);
            }

            if(predicate != null)
            {
                queryToDb = queryToDb.Where(predicate);
            }

            if(orderBy != null)
            {
                queryToDb = orderBy(queryToDb);
            }

            if(!isTracked)
            {
                queryToDb = queryToDb.AsNoTracking();
            }

            return await queryToDb.ToListAsync();
        }

        //public Task<UserToUserTransaction> GetSingleTransaction(Expression<Func<UserToUserTransaction, bool>> filter = null, string includeProps = "", bool isTracked = true)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> MakeTransactionRequest(int transactionId)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateTransaction(int transactionId, UserToUserTransaction transactionEntity)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
