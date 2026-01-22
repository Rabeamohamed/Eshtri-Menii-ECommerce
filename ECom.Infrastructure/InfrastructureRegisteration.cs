using ECom.Core.Interfaces;
using ECom.Infrastructure.Repositories;
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
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection service)
        {
            service.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            //service.AddScoped<ICategoryRepository,CategoryRepository>();
            //service.AddScoped<IProductRepository, ProductRepository>();
            //service.AddScoped<IPhotoRepository, PhotoRepository>();

            service.AddScoped<IUnitOfWork, UnitOfWork>();
            return service;
        }
    }
}
