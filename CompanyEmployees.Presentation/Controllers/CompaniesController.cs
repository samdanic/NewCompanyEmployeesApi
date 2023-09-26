using CompanyEmployees.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiExplorerSettings(GroupName ="v1")]
    [ApiController]
    //[ResponseCache(CacheProfileName ="120SecondsDuration")]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public CompaniesController(IServiceManager service)
        {
            _service = service;
        }
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,DELETE,PUT");
            return Ok();
        }
        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns>The companies list</returns>
        [HttpGet(Name ="GetCompanies")]
        //[Authorize(Roles ="Manager")]
        public async Task<IActionResult> GetCompanies()
        {
            //try
            //{
            //throw new Exception("Exception");
            var companies = await _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
            //}
            //catch (Exception)
            //{

            //    return StatusCode(500, "Internal server error");
            //}

        }
        [HttpGet("{id:guid}", Name ="CompanyById" )]
        //[ResponseCache(Duration = 60)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public,MaxAge =60)]
        [HttpCacheValidation(MustRevalidate =false)]
        public async Task<IActionResult> GetCompanyAsync(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }
        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        ///<response code="422">If the model is invalid</response>
        [HttpPost(Name ="CreateCompany")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompanyAsync([FromBody] CompanyForCreatingDto company)
        {
            //if (company is null)
            //{
            //    return BadRequest("CompanyForCreatingDto object is null");
            //}
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new {id = createdCompany.Id}, createdCompany);
        }

        [HttpGet("collection/({ids})", Name ="CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollectionAsync ([ModelBinder(binderType:typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollectionAsync([FromBody] IEnumerable<CompanyForCreatingDto>companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new {result.ids}, result.companies);

        }
        [HttpDelete("{id:Guid}", Name ="DeleteCompany")]
        public async Task<IActionResult> DeleteCompanyAsync(Guid id, bool trackChanges)
        {
           await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);

            return NoContent();
        }
        [HttpPut("{id:guid}", Name ="UpdateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompanyAsync(Guid id, CompanyForUpdateDto companyForUpdateDto, bool trackchanges)
        {
            //if (companyForUpdateDto is null)
            //{
            //    return BadRequest("CompanyForUpdateDto object is null");

            //}
           await _service.CompanyService.UpdateCompanyAsync(id, companyForUpdateDto, trackchanges:true);
            return NoContent();
        }
    }
}
