using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    int numLoadBalancers = 2;

    return req.CreateResponse(HttpStatusCode.OK ,numLoadBalancers);
} 
