﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Vonk.Core.Configuration;
using Vonk.Core.Operations.Crud;
using Vonk.Core.Operations.Search;
using Vonk.Core.Operations.Validation;
using Vonk.Core.Pluggability;
using Vonk.Fhir.R3;
using Vonk.Smart;

namespace Vonk.Facade.Starter
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFhirServices()
                .AddSmartServices()
                .AddVonkMinimalServices()
                .AddSearchServices()
                .AddReadServices()
                .AddViSiServices()
                .AllowResourceTypes("Patient", "Observation")
                .AddInstanceValidationServices()
                .AddValidationServices()
             ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseVonkMinimal()
                .UseSmartAuthorization()
                .UseSearch()
                .UseRead()
                .UseValidation()
                .UseInstanceValidation()
            ;
        }
    }
}
