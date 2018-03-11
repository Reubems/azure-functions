using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    string valueTerm = "";
    string path = "";

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();
    valueTerm = data?.Value;
    path = data?.Path;
   
    HttpClient _httpClient = new HttpClient();
    string url = "http://---------------.azurewebsites.net/api/Terms";
    string str = "{\"Value\":\"" + valueTerm + "\",\"Path\":\"" + path + "\",\"Time\":\"" + DateTime.Now.TimeOfDay.ToString()+ "\"}";
            
    _httpClient.DefaultRequestHeaders
    .Accept
    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

    // Put method with error handling
    using (var content = new StringContent(str, Encoding.UTF8, "application/json"))
    {
        var result = await _httpClient.PostAsync($"{url}", content).ConfigureAwait(false);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return req.CreateResponse(HttpStatusCode.OK, "Term inserted");
        }
        else 
        {
            // Something wrong happened
            string resultContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            // ... post to Monitor
    
            string logstr = "{\"Origin\":\"LoadBalancer\",\"Time\":\""+ DateTime.Now.TimeOfDay.ToString()+ "\",\"Message\":\""+resultContent+"\"} ";
            string URL_MONITOR = "http://-----------------.azurewebsites.net/api/LogMonitors";

            // Put method with error handling
            using (var contentLog = new StringContent(logstr, Encoding.UTF8, "application/json"))
            {
                var resultLog = await _httpClient.PostAsync($"{URL_MONITOR}", contentLog).ConfigureAwait(false);
                
            }
        }
    }
    return valueTerm == null ? req.CreateResponse(HttpStatusCode.BadRequest, "Bad request") : req.CreateResponse(HttpStatusCode.OK, "Term inserted");
}
