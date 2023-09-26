using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranasferObject
{
    public abstract record EmployeeManipulatingPhotoDto
    {

        public Guid Id { get; set; }
        [Required(ErrorMessage = "Employee photo is a required field")]
        public string? FileName { get; set; }
        
    }
}
