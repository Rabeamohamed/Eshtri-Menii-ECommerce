using ECom.Core.Interfaces;
using ECom.Infrastructure.Data;
using ECom.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

            // Apply DbContext Registration
            service.AddDbContext<AppDbContext>(options =>{
                options.UseSqlServer(configuration.GetConnectionString("EComConnection"));
                });

            return service;
        }
    }
}
