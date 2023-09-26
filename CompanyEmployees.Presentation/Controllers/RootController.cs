using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        public RootController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        [HttpGet(Name ="GetRoot")]
        public IActionResult GetRoot([FromHeader(Name ="Accept")]string mediaType)
        {
            if (mediaType.Contains("application/vnd.samdan.apiroot"))
            {
                var list = new List<Link>
                {
                    new Link
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new {}),
                        Rel="Self",
                        Method="GET",
                    },
                    new Link
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext,"GetCompanies", new{}),
                        Rel = "companies",
                        Method="GET",
                    },
                    new Link
                    {
                       Href= _linkGenerator.GetUriByName(HttpContext, "CreateCompany", new{}),
                       Rel="create_company",
                       Method= "POST",
                    },
                    new Link
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, "DeleteCompany", new{}),
                        Rel = "delete_company",
                        Method="DELETE"
                    },
                    new Link
                    {
                        Href= _linkGenerator.GetUriByName(HttpContext, "UpdateCompany", new{}),
                        Rel = "update_company",
                        Method = "UPDATE"
                        
                    }

                };
                return Ok(list);
            }
            return NoContent();
        }
    }
}
