using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Viasfora.Metrics {
  public class AppInsightsQuery : IAppInsightsQuery {
    private HttpClient httpClient;
    private String queryUrl;

    public AppInsightsQuery(IOptions<AppInsightsOptions> optionsAccessor) {
      httpClient = new HttpClient();
      var opts = optionsAccessor.Value;
      queryUrl = $"https://api.applicationinsights.io/v1/apps/{opts.ApplicationId}/query?timespan=P30D";
      httpClient.DefaultRequestHeaders.Add("x-api-key", opts.ApplicationKey);
    }

    public async Task<IEnumerable<Tuple<TKey, TValue>>> Execute<TKey, TValue>(String query) {
      JObject req = new JObject {
        { "query", query }
      };

      var jsonBody = new StringContent(req.ToString(), Encoding.UTF8, "application/json");

      var resp = await httpClient.PostAsync(queryUrl, jsonBody);
      if ( resp.Content != null ) {
        var jsonResp = JObject.Parse(await resp.Content.ReadAsStringAsync());
        return ParseRows2<TKey, TValue>(jsonResp);
      }
      return Enumerable.Empty<Tuple<TKey, TValue>>();
    }

    private IEnumerable<Tuple<TKey, TValue>> ParseRows2<TKey, TValue>(JObject json) {
      var rows = (JArray)(json["tables"][0]["rows"]);
      foreach ( var r in rows ) {
        var jr = (JArray)r;
        yield return new Tuple<TKey, TValue>(jr[0].Value<TKey>(), jr[1].Value<TValue>());
      }
    }
  }


  public class AppInsightsOptions {
    public String ApplicationId { get; set; }
    public String ApplicationKey { get; set; }
  }
}
