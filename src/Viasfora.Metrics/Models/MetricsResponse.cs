using System;
using System.Collections.Generic;

namespace Viasfora.Metrics.Models {
  public class MetricsResponse {
    public int TotalUsers { get; set; }
    public Series UsersPerHostVersion { get; set; }
    public Series FeatureUsage { get; set; }
  }

  public class Series {
    public IList<String> Labels { get; set; }
    public IList<int> Values { get; set; }

    public Series() {
      Labels = new List<String>();
      Values = new List<int>();
    }
  }
}
