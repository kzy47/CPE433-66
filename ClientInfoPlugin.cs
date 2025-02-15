using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace DNWS
{
    class ClientInfoPlugin : IPlugin
    {
        protected static Dictionary<String, int> statDictionary = new Dictionary<String, int>();
        private static readonly object lockObject = new object();

        public void PreProcessing(HTTPRequest request)
        {
            Thread thread = new Thread(() => ProcessRequest(request));
            thread.Start();
        }

        private void ProcessRequest(HTTPRequest request)
        {
            lock (lockObject)
            {
                if (statDictionary.ContainsKey(request.Url))
                {
                    statDictionary[request.Url]++;
                }
                else
                {
                    statDictionary[request.Url] = 1;
                }
            }
        }

        public HTTPResponse GetResponse(HTTPRequest request)
        {
            StringBuilder sb = new StringBuilder();

            IPEndPoint endpoint;
            try
            {
                endpoint = IPEndPoint.Parse(request.getPropertyByKey("remoteendpoint"));
            }
            catch
            {
                endpoint = new IPEndPoint(IPAddress.Any, 0);
            }

            sb.Append("<html><head> <style> body {font-family: courier new; font-size: 14px;} h1 {font-size:30px; color: #4CAF50;} div {line-height: 1.5; margin: 20px;} </style></head>");
            sb.Append("<body><h1>Client Info:</h1>");
            sb.AppendFormat("<div><b>Client IP:</b> {0}<br/>", endpoint.Address);
            sb.AppendFormat("<b>Client Port:</b> {0}<br/>", endpoint.Port);
            sb.AppendFormat("<b>Browser Information:</b> {0}<br/>", request.getPropertyByKey("user-agent")?.Trim() ?? "Unknown");
            sb.AppendFormat("<b>Accept Language:</b> {0}<br/>", request.getPropertyByKey("accept-language")?.Trim() ?? "Unknown");
            sb.AppendFormat("<b>Accept Encoding:</b> {0}<br/></div>", request.getPropertyByKey("accept-encoding")?.Trim() ?? "Unknown");
            sb.Append("</body></html>");

            HTTPResponse response = new HTTPResponse(200);
            response.body = Encoding.UTF8.GetBytes(sb.ToString());
            return response;
        }

        public HTTPResponse PostProcessing(HTTPResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
