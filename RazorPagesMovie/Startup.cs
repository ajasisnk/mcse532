using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using RazorPagesMovie.Models;

namespace RazorPagesMovie
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        #region snippet_ConfigureServices
        public void ConfigureServices(IServiceCollection services)
        {
            // requires 
            // using RazorPagesMovie.Models;
            // using Microsoft.EntityFrameworkCore;

            services.AddDbContext<MovieContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MovieContext")));
            services.AddMvc();


            // ====> EDITED!
            // DEPENDENCY INJECTION EXAMPLE

            // get the connection string - preferably keyvault
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.GetValue<string>("storage"));

            // =====> BLOB CLIENT
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            services.AddSingleton<CloudBlobClient>(blobClient);

            // =====> TABLE CLIENT AND TABLE CREATE

            CloudTableClient tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference("ajposter");
            table.CreateIfNotExistsAsync();
            services.AddSingleton<CloudTable>(table);

            // ====> EDITED!
        }
        #endregion

#if SQLite
        #region snippet_ConfigureServices2
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<MovieContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("MovieContext")));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        #endregion
#endif

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
