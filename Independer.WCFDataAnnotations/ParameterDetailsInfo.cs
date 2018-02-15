using System.Collections.Generic;

namespace Independer.WCFDataAnnotations {
  public class ParameterDetailsInfo {
    public List<ParameterDetails> ParameterDetails { get; set; }

    public ParameterDetailsInfo() {
      ParameterDetails = new List<ParameterDetails>();
    }
  }
}
