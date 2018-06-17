using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Viasfora.Metrics {
  public interface IAppInsightsQuery {
    Task<IEnumerable<Tuple<TKey, TValue>>> Execute<TKey, TValue>(String query);
  }
}
