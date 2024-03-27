using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.CompanyBankAssetRepository
{
    public interface ICompanyBankAssetRepository : IRepository<CompanyBankAsset>
    {
        Task UpdateAsync(CompanyBankAsset companyBankAsset);
        Task SaveChangesAsync();
    }
}
