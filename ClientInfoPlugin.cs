using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DNWS
{
  class ClientInfoPlugin : IPlugin
  {
    protected static Dictionary<String, int> statDictionary = null;
    public ClientInfoPlugin()
    {
      if (statDictionary == null)
      {
        statDictionary = new Dictionary<String, int>();

      }
    }

    public void PreProcessing(HTTPRequest request)
    {
      if (statDictionary.ContainsKey(request.Url))
      {
        statDictionary[request.Url] = (int)statDictionary[request.Url] + 1;
      }
      else
      {
        statDictionary[request.Url] = 1;
      }
    }

    public HTTPResponse GetResponse(HTTPRequest request)
    {
      HTTPResponse response = null;
      StringBuilder sb = new StringBuilder();

      IPEndPoint endpoint = IPEndPoint.Parse(request.getPropertyByKey("remoteendpoint"));
      // sb.Append("<html><body><pre>");
      // sb.AppendFormat("Client IP: {0}<br/>\n", endpoint.Address);
      // sb.AppendFormat("Client Port: {0}<br/>\n", endpoint.Port);
      // sb.AppendFormat("Browser Information: {0}<br/>\n", request.getPropertyByKey("user-agent").Trim());
      // sb.AppendFormat("Accept Language: {0}<br/>\n", request.getPropertyByKey("accept-language").Trim());
      // sb.AppendFormat("Accept Encoding: {0}<br/>\n", request.getPropertyByKey("accept-encoding").Trim());

      // sb.Append("</pre></body></html>");

      sb.Append("<html><head> <style> body {font-family: courier new: 10px;} h1 {font-size:30px; color: #4CAF50;} div {line-height: 1.5; margin: 20px;} </style></head></html>");
      sb.Append("<html><h1>Client Info:</h1>");
      sb.AppendFormat("<div><b>Client IP:</b> " + endpoint.Address + "<br/>" );
      sb.AppendFormat("<b>Client Port:</b> " + endpoint.Port + "<br/>");
      sb.AppendFormat("<b>Browser Information:</b> " + request.getPropertyByKey("user-agent").Trim() + "<br/>");
      sb.AppendFormat("<b>Accept Language:</b> " + request.getPropertyByKey("accept-language").Trim() + "<br/>");
      sb.AppendFormat("<b>Accept Encoding:</b> " + request.getPropertyByKey("accept-encoding").Trim() + "<br/><div>");
      
      sb.Append("</body></html>");

      response = new HTTPResponse(200);
      response.body = Encoding.UTF8.GetBytes(sb.ToString());
      return response;
    }


    public HTTPResponse PostProcessing(HTTPResponse response)
    {
      throw new NotImplementedException();
    }
  }
}
