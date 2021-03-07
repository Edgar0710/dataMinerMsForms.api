using DataMinerBussiness.Bussiness;
using DataMinerBussiness.IBussiness;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dataMiner.Data.IRepository;
using dataMiner.Data.Repository;

namespace dataMinerMsForms.api.dependencies
{
    public static class ServicesDependency
    {
        public static void AddServiceDependency(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGeneralRepository<>),typeof(GeneralRepository<>));
            services.AddScoped<IUsuarioBussines, UssuarioBusiness>();
            services.AddScoped<IformBusiness, FormBusiness>();


                
       }
    }
}
