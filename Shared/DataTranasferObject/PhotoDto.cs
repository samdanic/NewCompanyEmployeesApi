using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranasferObject
{
    public record PhotoDto
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        
    }
}
