using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace DataProtection
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
            #region Save to File System
            //services.AddDataProtection().SetApplicationName("SharedApplicationName")
            //    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp-keys\"));

            #endregion

            #region Save to Databse            
            services.AddDbContext<DataProtectionKeysContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DataProtectionKeysConnection")));

            services.AddDataProtection().SetApplicationName("SharedApplicationName")
               .PersistKeysToDbContext<DataProtectionKeysContext>();
            #endregion

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }

    public class DataProtectionKeysContext : DbContext, IDataProtectionKeyContext
    {
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public DataProtectionKeysContext(DbContextOptions<DataProtectionKeysContext> options)
            : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
}
