﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using LoggerService;
using Service.Contracts;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        private async Task<Company>GetCompanyAndCheckIfItExistsAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            return company;
        }
        //replacing the classes with the DTOs 
        public async Task<IEnumerable<CompanyDto>>GetAllCompaniesAsync(bool trackChanges)
        {
            //try
            //{
                var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
           

                    var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
                    return companiesDto;

                //var companiesDto = companies.Select(c =>
                //                    new CompanyDto(c.Id, c.Name ?? "", string.Join(' ', c.Address,",",c.Country)))
                //                    .ToList();

                //return companiesDto;
            //}
            //catch (Exception ex)
            //{
            
            //    _logger.LogError($"Something went wrong in the {nameof(GetAllCompanies)} service method{ex}");
            //    throw;
            //}



        }
        public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExistsAsync(id, trackChanges);
            //var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
            //if (company is null)
            
            //    throw new CompanyNotFoundException(id);
            
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }
        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreatingDto company)
        {

            var companyEntity = _mapper.Map<Company>(company);
          _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return companyToReturn;
        }
        public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if (ids is null)
            {
                throw new IdParametersBadRequestException();

            }
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges);
            if (ids.Count() != companyEntities.Count()) 

            {
                throw new CollectionByIdsBadRequestException();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return companiesToReturn;
        }
        public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync
            (IEnumerable<CompanyForCreatingDto>companyCollection)
        {
            if (companyCollection is null)
            {
                throw new CompanyCollectionBadRequest();
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
           await _repository.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return (companies: companyCollectionToReturn, ids: ids);
        }
        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await GetCompanyAndCheckIfItExistsAsync(companyId, trackChanges);
            //var company = await _repository.Company.GetCompanyAsync(companyId,trackChanges);
            //if (company is null)
            //{
            //    throw new CompanyNotFoundException(companyId);
            //}
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();

        }
        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdateDto, bool trackChanges)
        {
            var companyEntity = await GetCompanyAndCheckIfItExistsAsync(companyId, trackChanges);
           //var companyEntity = await _repository.Company.GetCompanyAsync(companyId,trackChanges);
           // if (companyEntity is null)
           // {
           //    throw new CompanyNotFoundException(companyId);
           // }
            _mapper.Map(companyForUpdateDto,companyEntity);
            await _repository.SaveAsync();
        }
    }
}
