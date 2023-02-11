using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Monitor.Helpers
{
    public class Requesters
    {
        public static Task<HttpStatusCode> GetStatusFromUrl(string url){
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return Task.FromResult(response.StatusCode);
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    return Task.FromResult(response.StatusCode);
                }
                else
                {
                    return Task.FromResult(HttpStatusCode.NotFound);
                }
            }
        }
    }
}