using Entities.Models;
using Shared.DataTranasferObject;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
        Task<Employee> GetEmployeeAsync(Guid employeeId, Guid id,bool trackChanges);
        Task<Employee>GetEmployeeById(Guid employeeId, bool trackChanges);
       void CreateEmployee(Guid companyId, Employee employee);
        void DeleteEmployee(Employee employee);
    }
}
