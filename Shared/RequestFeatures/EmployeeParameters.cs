
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public class EmployeeParameters : RequestParameters
    {
        public EmployeeParameters() => OrderBy = "name";
        
        public int MinAge { get; set; } = 0;
        public int MaxAge { get; set; } = int.MaxValue;
        public bool ValidAgeRange => MaxAge > MinAge;
        public string? SearchTerm { get; set; }
    }

    
}
