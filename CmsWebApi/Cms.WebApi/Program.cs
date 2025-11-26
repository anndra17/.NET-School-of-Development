
using Cms.Repository.Repositories;
using Cms.WebApi.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Cms.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<ICmsRepository, InMemoryCmsRepository>();
            builder.Services.AddAutoMapper(cfg => { }, typeof(CmsMapper));

            builder.Services.AddControllers();

            // API Versioning
            builder.Services.AddApiVersioning(setupAction =>
            {
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.AssumeDefaultVersionWhenUnspecified = true;

                //setupAction.ApiVersionReader = new QueryStringApiVersionReader("v"); // Query versioning
                setupAction.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
