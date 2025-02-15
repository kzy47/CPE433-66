using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DNWS
{
    public class HTTPRequest
    {
        protected string _url;
        protected string _filename;
        protected Dictionary<string, string> _propertyListDictionary;
        protected Dictionary<string, string> _requestListDictionary;
        protected string _body;
        protected int _status;
        protected string _method;

        public string Url => _url;
        public string Filename => _filename;
        public string Body => _body;
        public int Status => _status;
        public string Method => _method;

        public HTTPRequest(string request)
        {
            _propertyListDictionary = new Dictionary<string, string>();
            _requestListDictionary = new Dictionary<string, string>();

            string[] lines = request.Split('\n');

            if (lines.Length == 0)
            {
                _status = 500;
                return;
            }

            string[] statusLine = lines[0].Trim().Split(' ');

            if (statusLine.Length < 2)
            {
                _status = 400; // Bad Request
                return;
            }

            _method = statusLine[0].ToUpper();
            _url = statusLine[1];

            if (_method != "GET" && _method != "POST")
            {
                _status = 501; // Not Implemented
                return;
            }

            _status = 200;
            _filename = _url.Split('/').Last();

            if (_filename.Contains("?"))
            {
                string[] parts = _filename.Split('?');
                _filename = parts[0];

                if (parts.Length > 1)
                {
                    _requestListDictionary = parts[1]
                        .Split('&')
                        .Where(p => p.Contains("="))
                        .Select(p => p.Split('='))
                        .ToDictionary(kv => kv[0].ToLower(), kv => kv.Length > 1 ? kv[1] : "");
                }
            }

            // Process headers and body
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                int index = line.IndexOf(": ");
                if (index > 0)
                {
                    string key = line.Substring(0, index).ToLower();
                    string value = line.Substring(index + 2);
                    _propertyListDictionary[key] = value;
                }
                else if (_method == "POST")
                {
                    // Body Parameters (for POST)
                    var bodyParams = line
                        .Split('&')
                        .Where(p => p.Contains("="))
                        .Select(p => p.Split('='))
                        .ToDictionary(kv => Uri.UnescapeDataString(kv[0].ToLower()), kv => kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : "");

                    _requestListDictionary = _requestListDictionary.Concat(bodyParams).ToDictionary(x => x.Key, x => x.Value);
                }
            }
        }

        public string getPropertyByKey(string key) => _propertyListDictionary.TryGetValue(key.ToLower(), out string value) ? value : null;

        public string getRequestByKey(string key) => _requestListDictionary.TryGetValue(key.ToLower(), out string value) ? value : null;

        public void AddProperty(string key, string value) => _propertyListDictionary[key.ToLower()] = value;

        public void AddRequest(string key, string value) => _requestListDictionary[key.ToLower()] = value;
    }
}
