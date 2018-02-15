using System.Collections.Generic;
using System.Linq;

namespace Independer.WCFDataAnnotations {
  public class ParameterDetailsInfo {
    public List<ParameterDetails> ParameterDetails { get; set; }

    public bool HasAnyParameterSkipNullCheck {
      get { return ParameterDetails.Any(x => x.SkipNullcheck); }
    }

    public ParameterDetailsInfo() {
      ParameterDetails = new List<ParameterDetails>();
    }
  }
}
