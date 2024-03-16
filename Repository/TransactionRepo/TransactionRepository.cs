using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models.Transcations;
using Stripe.Climate;
using System.Linq.Expressions;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public class TransactionRepository<TransactionType>: ITranscationRepository<TransactionType> where TransactionType : class
    {
        private readonly PayBridgeDbContext _context;
        internal DbSet<TransactionType> _dbSet { get; set; }

        public TransactionRepository(PayBridgeDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TransactionType>();   
        }

        public async Task<List<TransactionType>> GetAllTransactions(
            Expression<Func<TransactionType, bool>> predicate = null,
            Func<IQueryable<TransactionType>, IIncludableQueryable<TransactionType, object>> include = null,
            Func<IQueryable<TransactionType>, IOrderedQueryable<TransactionType>> orderBy = null,
            bool isTracked = true)
        {
            IQueryable<TransactionType> queryToDb = _dbSet.AsQueryable();

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

        public async Task<TransactionType> GetSingleTransaction(Expression<Func<TransactionType, bool>> predicate = null, Func<IQueryable<TransactionType>, IIncludableQueryable<TransactionType, object>> include = null, Func<IQueryable<TransactionType>, IOrderedQueryable<TransactionType>> orderBy = null, bool isTracked = true)
        {
            IQueryable<TransactionType> queryToDb = _dbSet.AsQueryable();

            if (predicate != null)
            {
                queryToDb = queryToDb.Where(predicate);
            }

            if(include != null)
            {
                queryToDb = include(queryToDb);
            }

            if(orderBy != null)
            {
                queryToDb = orderBy(queryToDb);
            }

            if(!isTracked)
            {
                queryToDb = queryToDb.AsNoTracking();
            }

            return await queryToDb.FirstOrDefaultAsync();
        }

        public async Task CreateTransaction(TransactionType transaction)
        {
            await _context.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
