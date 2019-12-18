using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using OPPE.R.Models.DB;
using OPPE.R.Models.DB.Repository;
using OPPE.R.Services;

namespace OPPE.R
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("OPPEDBWork");
            services.AddDbContext<OPPEDbContext>(x => x.UseSqlServer(connection));
            services.AddControllers();
            services.AddScoped<IOppeDBService, OppeDBService>();
            services.AddScoped<IRService, RService>();
            services.AddScoped<IExtractToFile, ExtractToFile>();
            services.AddScoped<IRUtilService, RUtilService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
