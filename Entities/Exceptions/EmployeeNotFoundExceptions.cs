using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class EmployeeNotFoundExceptions : NotFoundException
    {
        public EmployeeNotFoundExceptions(Guid employeeId) : base($"The employee with id:{employeeId} doesn't exist in the database.")
        {

        }
    }
}
