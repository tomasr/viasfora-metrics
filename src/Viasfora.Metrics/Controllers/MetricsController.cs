using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Viasfora.Metrics;
using Viasfora.Metrics.Models;

namespace viasfora_metrics.Controllers {
  [Route("api/[controller]")]
  public class MetricsController : Controller {
    private IAppInsightsQuery queryRunner;
    private IMemoryCache cache;
    private const String CACHE_KEY = "metrics";

    public MetricsController(IAppInsightsQuery query, IMemoryCache cache) {
      this.queryRunner = query;
      this.cache = cache;
    }

    [HttpGet]
    [ResponseCache(CacheProfileName="12Hour")]
    public async Task<IActionResult> Get() {
      MetricsResponse metrics = await this.cache.GetOrCreateAsync<MetricsResponse>(CACHE_KEY, cacheEntry => {
        cacheEntry.SetAbsoluteExpiration(DateTimeOffset.Now.AddHours(24));
        return GetMetrics();
      });
      return Ok(metrics);
    }

    private async Task<MetricsResponse> GetMetrics() {
      var response = new MetricsResponse {
        FeatureUsage = await GetFeatureUsage(),
        UsersPerHostVersion = await GetUsersPerHostVersion(),
      };
      response.TotalUsers = response.UsersPerHostVersion.Values.Sum();
      return response;
    }

    private async Task<Series> GetUsersPerHostVersion() {
      String query = 
@"let version=(props : dynamic) {
    let hostVersion=tostring(props['HostVersion']);
    let vsVersion=tostring(props['VsVersion']);
    coalesce(hostVersion, vsVersion)
};
customEvents
    | extend HostVersion=version(customDimensions) 
    | distinct user_Id, HostVersion
    | summarize UserCount=count() by HostVersion";
      var results = await queryRunner.Execute<String, int>(query);
      return results.Aggregate(new Series(), (ac, value) => {
        ac.Labels.Add(value.Item1);
        ac.Values.Add(value.Item2);
        return ac;
      });
    }

    private async Task<Series> GetFeatureUsage() {
      String query = @"customEvents
        | where name startswith 'Feature' and customDimensions['enabled'] == 'True'
        | distinct name, user_Id
        | summarize UserCount=count(name) by name";
      var results = await queryRunner.Execute<String, int>(query);
      return results.Aggregate(new Series(), (ac, value) => {
        ac.Labels.Add(value.Item1);
        ac.Values.Add(value.Item2);
        return ac;
      });
    }
  }
}
