using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Interfaces;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            #region AutoMapper configuração
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });
            #endregion

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);


            //add mvc como serviço
            services.AddDbContext<MimicContext>(opt =>
            {
                //Database  = nossa pasta criada "Database"
                opt.UseSqlite("Data Source=Database\\Mimic.db");
            });
            services.AddMvc();
            services.AddScoped<IPalavraRepository, PalavraRepository>();
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;  // essa opção vai mostra no HEADERS as versiões suportada ex:(spi-suported  = 1.0 ,2.0)

              // cfg.ApiVersionReader= new HeaderApiVersionReader("api-version");

                /*trabalha juntas*/
                // cfg.AssumeDefaultVersionWhenUnspecified=true ;  assumi a versao padrao quando nao especificamos. 
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);

            });



            services.AddMvc(options => options.EnableEndpointRouting = false);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseRouting();
            app.UseDeveloperExceptionPage();
            app.UseMvc();

        }
    }
}
