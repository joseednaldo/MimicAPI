using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Interfaces;
using System.IO;
using System.Linq;

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
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddScoped<IPalavraRepository, PalavraRepository>();

            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;  // essa opção vai mostra no HEADERS as versiões suportada ex:(spi-suported  = 1.0 ,2.0)  //// ativar a disponibilização do versionamento da API
                cfg.AssumeDefaultVersionWhenUnspecified = true; // parâmetro complementar ao ".DefaultApiVersion"
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0); // versão default - padrão ou sugerida.

            });


            services.AddSwaggerGen(cfg => {

                cfg.ResolveConflictingActions(apiDescription => apiDescription.First()); // para resolver conflitos de versões da API no Swagger, com mesmo nome.

                cfg.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API"

                });


                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;   //recupera o caminho do projeto...
                var NomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml"; //recupera o nome do projeto...
                var CaminhoArquivoXMLComentario = Path.Combine(CaminhoProjeto,NomeProjeto);
                cfg.IncludeXmlComments(CaminhoArquivoXMLComentario);


            });

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


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MimicAPI");
                    c.RoutePrefix = string.Empty;
                });

            }
        }
}
