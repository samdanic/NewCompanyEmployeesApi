using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTranasferObject;
using System.Dynamic;

namespace CompanyEmployees.Utility
{
    public class EmployeeLinks : IEmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDto> _dataShaper;
        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }
        public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeeDto, string fields, Guid companyId, HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employeeDto, fields);
            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLinkedEmployees(employeeDto, fields, companyId, httpContext, shapedEmployees);
                
            }
            return ReturnShapedEmployees(shapedEmployees);
        }
        private List<ExpandoObject> ShapeData(IEnumerable<EmployeeDto> employeeDto, string fields) =>
            _dataShaper.ShapeData(employeeDto, fields)
            .Select(e => e.ExpandoObject)
            .ToList();

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }
        private LinkResponse ReturnShapedEmployees(List<ExpandoObject> shapedEmployees) =>
            new LinkResponse { ShapedEntities = shapedEmployees };

        private LinkResponse ReturnLinkedEmployees(IEnumerable<EmployeeDto>employeeDto, string fields, Guid companyId, HttpContext httpContext,List<ExpandoObject>shapedEmployees)
        {
            var employeeDtoList = employeeDto.ToList();

            for (var index = 0; index < employeeDtoList.Count(); index++)
            {
                var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
                shapedEmployees[index].TryAdd("Links", employeeLinks);
            }
            var employeeCollection = new LinkCollectionWrapper<ExpandoObject>(shapedEmployees);
            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);
            return new LinkResponse { HasLinks = true, LinkEntities = linkedEmployees };
        }
        private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields = "")
        {
            var links = new List<Link>
            {
                new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployee", 
                values: new {companyId, id, fields}),
                "self",
                "Get"),
                new Link(_linkGenerator.GetUriByAction(httpContext,"DeleteEmployee",
                values: new {companyId, id}),
                "delete_employee",
                "DELETE"),

                new Link(_linkGenerator.GetUriByAction(httpContext,"UpdateEmployee",
                values: new {companyId, id}),
                "update_employee",
                "PUT"),

                 new Link(_linkGenerator.GetUriByAction(httpContext,"PartiallyUpdateEmployee",
                values: new {companyId, id}),
                "partially_update_employee",
                "PATCH")

            };
            return links;
        }
        private LinkCollectionWrapper<ExpandoObject> CreateLinksForEmployees(HttpContext httpContext, LinkCollectionWrapper<ExpandoObject>employeesWrapper)
        {
            employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployee", values: new { }),
                "self",
                "GET"));
            return employeesWrapper;
        }
    }
}
