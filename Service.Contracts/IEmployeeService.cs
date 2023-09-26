using Entities.LinkModels;
using Entities.Models;
using Shared.DataTranasferObject;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IEmployeeService
    {
        Task<(LinkResponse linkResponse, MetaData MetaData)> GetEmployeesAsync(Guid companyId, LinkParameters linkParameters, bool trackChanges);
        Task<EmployeeDto> GetEmployeeAsync(Guid companyId,Guid id, bool trackChanges);
        Task<Employee> GetEmployeeById(Guid employeedId, bool trackChanges);
        Task<EmployeeDto> CreateEmployeeAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges);
        Task DeleteEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        Task UpdateEmployeeAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges);
        Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)>GetEmployeeForPatchAsync(Guid companyId,Guid id,  bool CompTrackChanges, bool empTrackChanges);
        Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);
       
    }
}
