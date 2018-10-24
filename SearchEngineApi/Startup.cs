using DocumentSearchEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SearchEngineApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<ISearchEngine, SearchEngine>(_ => this.CreateSearchEngine());
        }

        private SearchEngine CreateSearchEngine()
        {
            var keywords = this.Configuration["keywords.txt"].Replace("\r", "").Split("\n");
            var documents = this.Configuration["documents.txt"].Replace("\r", "").Split("\n\n");

            var searchEngine = new SearchEngine(keywords, new DocumentSanitizer());
            foreach (var rawDocument in documents)
            {
                searchEngine.AddDocument(rawDocument);
            }
            return searchEngine;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}