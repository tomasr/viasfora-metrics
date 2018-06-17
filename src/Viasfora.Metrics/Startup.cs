using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Viasfora.Metrics;

namespace Viasfora.Metrics {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services
        .Configure<AppInsightsOptions>(Configuration.GetSection("appInsightsQuery"))
        .AddSingleton<IAppInsightsQuery, AppInsightsQuery>()
        .AddMemoryCache()
        .AddResponseCaching()
        .AddMvc(options => {
          options.CacheProfiles.Add("12Hour", new CacheProfile {
            Duration = (int)TimeSpan.FromHours(12).TotalSeconds,
            Location = ResponseCacheLocation.Any,
          });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if ( env.IsDevelopment() ) {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc();
    }
  }
}
