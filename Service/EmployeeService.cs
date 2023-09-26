using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using LoggerService;
using Service.Contracts;
using Shared.DataTranasferObject;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        //private readonly IDataShaper<EmployeeDto> _dataShaper;
        private readonly IEmployeeLinks _employeeLinks;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,IEmployeeLinks employeeLinks)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            //_dataShaper = dataShaper;
            _employeeLinks = employeeLinks;

        }

        private async Task<Company> GetCompanyAndCheckIfItExistsAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            return company;
        }


        private async Task<Employee> GetEmployeeAndCheckIfItExistsAsync(Guid companyId,  Guid id, bool trackChanges)
        {
            var employee = await _repository.Employee.GetEmployeeAsync(companyId,id, trackChanges);
            if (employee is null)
            {
                throw new EmployeeNotFoundExceptions(id);
            }
            return employee;
        }
        public async Task<Employee>GetEmployeeById(Guid employeeId, bool trackChanges)
        {
            var employee = await _repository.Employee.GetEmployeeById(employeeId,trackChanges);
            if (employee is null)
            {
                throw new EmployeeNotFoundExceptions(employeeId);
            }
            return employee;
        }
        public async Task<(LinkResponse linkResponse, MetaData MetaData)>GetEmployeesAsync(Guid companyId,LinkParameters linkParameters, bool trackChanges)
        {

            //try
            //{
            if (!linkParameters.EmployeeParameters.ValidAgeRange)
            {
                throw new MaxAgeRangeBadRequestException();
            }
                await GetCompanyAndCheckIfItExistsAsync(companyId, trackChanges);
                    var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId,linkParameters.EmployeeParameters, trackChanges);

                            if (employeesWithMetaData is null)
                            {
                                throw new EmployeeNotFoundExceptions(companyId);
                            }

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
            var links = _employeeLinks.TryGenerateLinks(employeesDto, linkParameters.EmployeeParameters.fields, companyId, linkParameters.Context);
            //var shapedData = _dataShaper.ShapeData(employeesDto, employeeParameters.fields);
            //return (employees: shapedData, MetaData: employeesWithMetaData.MetaData);
            return (linkResponse: links, MetaData: employeesWithMetaData.MetaData);

                //var employeesDto = employees.Select(e =>
                //                    new EmployeeDto(e.Id, e.Name, e.Age,e.Position, e.CompanyId))
                //                    .ToList();
                
                //return employeesDto;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"somthing went wrong with in the {nameof(GetAllEmployees)} service method {ex}");
            //    throw;
            //}
          
        }
      public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
             await GetCompanyAndCheckIfItExistsAsync(companyId, trackChanges);
            var employeeDb = await GetEmployeeAndCheckIfItExistsAsync(companyId, id, trackChanges);
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }
        public async Task<EmployeeDto> CreateEmployeeAsync(Guid companyId,EmployeeForCreationDto employeeforcreation, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExistsAsync(companyId, trackChanges);
            var employeeEntity = _mapper.Map<Employee>(employeeforcreation);
             _repository.Employee.CreateEmployee(companyId,employeeEntity);
           await _repository.SaveAsync();

            var employeeReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeReturn;
        }
        public async Task DeleteEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
           await GetCompanyAndCheckIfItExistsAsync(companyId, trackChanges);
            var employee = await GetEmployeeAndCheckIfItExistsAsync(companyId,id, trackChanges);
             _repository.Employee.DeleteEmployee(employee);
           await _repository.SaveAsync();
        }
        public async Task UpdateEmployeeAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            await GetCompanyAndCheckIfItExistsAsync(companyId, compTrackChanges);
            var employeeEntity = await GetEmployeeAndCheckIfItExistsAsync(companyId, id, empTrackChanges);
            _mapper.Map(employeeForUpdate, employeeEntity);
           await _repository.SaveAsync();

        }
        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) >GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {

            await GetCompanyAndCheckIfItExistsAsync(companyId, compTrackChanges);

            var employeeEntity = await GetEmployeeAndCheckIfItExistsAsync(companyId, id,  empTrackChanges);

            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            return (employeeToPatch, employeeEntity);
        }
        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch,employeeEntity);
            await _repository.SaveAsync();
            
        }
       
    }
}
