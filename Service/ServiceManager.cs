using AutoMapper;
using Contracts;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Shared.DataTranasferObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Entities.ConfigurationModel;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        //private readonly Lazy<IPhotoService> _photoService;

        public ServiceManager(IRepositoryManager repository, ILoggerManager logger,  IMapper mapper,UserManager<Users>userManager, IEmployeeLinks employeeLinks,IOptions<JwtConfiguration> configuration )
        {
            _companyService = new Lazy<ICompanyService>((new CompanyService(repository, logger,mapper)));
            _employeeService = new Lazy<IEmployeeService>((new EmployeeService(repository, logger,mapper,  employeeLinks)));
            _authenticationService = new Lazy<IAuthenticationService>(() =>
                  new AuthenticationService(logger, mapper, userManager, configuration));

            //_photoService = new Lazy<IPhotoService>(() =>
            //    new PhotoService(repository,logger,mapper, hostEnvironment,formFile));
        }
        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;
        //public IAuthenticationService AuthenticationServce => _authenticationService.Value;

        //public IPhotoService PhotoService => _photoService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
    }
}
