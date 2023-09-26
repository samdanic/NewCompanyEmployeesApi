using CompanyEmployees.ActionFilters;
using CompanyEmployees.Presentation.ActionFilters;
using Entities.LinkModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTranasferObject;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager service)
        {
            _service = service;
        }
        [HttpGet]
        [HttpHead]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployees(Guid companyId,[FromQuery]EmployeeParameters employeeParameters, Guid id, bool trackChanges)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);
            //try
            //{
            //throw new Exception("Exception");
            var result = await _service.EmployeeService.GetEmployeesAsync(companyId,linkParams, trackChanges: false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.MetaData));
            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkEntities) :
                Ok(result.linkResponse.ShapedEntities);
            //}
            //catch (Exception)
            //{
            //    return StatusCode(500, "Internal server error");
            //    throw;
            //}
        }
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
       
        public async Task<IActionResult> GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            //try
            //{
            //throw new Exception("Exception");
            var employees = await _service.EmployeeService.GetEmployeeAsync(companyId, id, trackChanges: false);
            return Ok(employees);
            //}
            //catch (Exception)
            //{
            //    return StatusCode(500, "Internal server error");
            //    throw;
            //}
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployee(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            //if (employee is null)
            //{
            //    return BadRequest("EmployeeForCreationDto object is null");
            //}
            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);
            var employeeReturn = await _service.EmployeeService.CreateEmployeeAsync(companyId, employee, trackChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeReturn.Id }, employeeReturn);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid id)
        {
            await _service.EmployeeService.DeleteEmployeeAsync(companyId, id, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employeeForUpdate)
        {
            //if(employeeForUpdate is null)
            //    return BadRequest("EmployeeForUpdate is null");
            //if (!ModelState.IsValid)
            //{
            //    return UnprocessableEntity(ModelState);
            //}
            await _service.EmployeeService.UpdateEmployeeAsync(companyId, id, employeeForUpdate, compTrackChanges: false, empTrackChanges: true);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id}, employeeForUpdate);
        }
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc sent from client is null");

            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id, CompTrackChanges: false, empTrackChanges: true);

            patchDoc.ApplyTo(result.employeeToPatch);
            await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }
    }
}
