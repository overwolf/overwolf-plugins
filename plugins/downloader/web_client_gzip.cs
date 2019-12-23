using System;
using System.Net;

namespace com.overwolf.dwnldr {
  class WebClientGzip : WebClient {
    protected override WebRequest GetWebRequest(Uri address) {
      HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
      request.AutomaticDecompression = DecompressionMethods.Deflate | 
                                       DecompressionMethods.GZip;
      return request;
    }
  }
}
