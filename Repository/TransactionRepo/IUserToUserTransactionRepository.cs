using PayBridgeAPI.Models.Transcations;

namespace PayBridgeAPI.Repository.TransactionRepo
{
    public  interface IUserToUserTransactionRepository : ITranscationRepository<UserToUserTransaction>
    {
        Task UpdateTransaction(UserToUserTransaction transaction);
        Task SaveChanges();
    }
}
