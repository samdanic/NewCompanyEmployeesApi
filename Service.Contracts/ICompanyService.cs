
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ICompanyService
    {

        //IEnumerable<Company> GetAllCompanies(bool trackChanges);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges);
        Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges);
        Task<CompanyDto> CreateCompanyAsync(CompanyForCreatingDto company);
        Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync
            (IEnumerable<CompanyForCreatingDto> companyCollection);
        Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
        Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdateDto, bool trackchanges);

    }
}
