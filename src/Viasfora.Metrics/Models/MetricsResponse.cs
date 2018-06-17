using System;
using System.Collections.Generic;

namespace Viasfora.Metrics.Models {
  public class MetricsResponse {
    public int TotalUsers { get; set; }
    public IList<UserVersionCount> UsersPerHostVersion { get; set; }
    public IList<FeatureCount> FeatureUsage { get; set; }
  }

  public class UserVersionCount {
    public String HostVersion { get; set; }
    public int UserCount { get; set; }
  }

  public class FeatureCount {
    public String Feature { get; set; }
    public int UserCount { get; set; }
  }
}
