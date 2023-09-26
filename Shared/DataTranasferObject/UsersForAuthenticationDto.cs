using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTranasferObject
{
    public record UsersForAuthenticationDto
    {
        [Required(ErrorMessage="User Name is required")]
        public string? UserName { get; init; }
        [Required(ErrorMessage="Password is required")]
        public string? Password { get; init; }
    }
}
