using Entities.Models;
using Microsoft.AspNetCore.Http;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeLinks
    {
        LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeeDto, string fields, Guid companyId, HttpContext httpContext);
    }
}
