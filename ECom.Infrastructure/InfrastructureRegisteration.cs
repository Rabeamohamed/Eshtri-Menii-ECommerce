using ECom.Core.Interfaces;
using ECom.Core.Services;
using ECom.Infrastructure.Data;
using ECom.Infrastructure.Repositories;
using ECom.Infrastructure.Repositories.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Applying Unit of Work Pattern
            service.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register IFileProvider for ImageManagementService
            service.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine( Directory.GetCurrentDirectory(),"wwwroot")));

            service.AddSingleton<IImageManagementService, ImgeManagementService>();

            // Apply DbContext Registration
            service.AddDbContext<AppDbContext>(options =>{
                options.UseSqlServer(configuration.GetConnectionString("EComConnection"));
                });

            return service;
        }
    }
}
