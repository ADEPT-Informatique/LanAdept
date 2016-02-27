using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace LanAdeptCore.Service.Challonge.Response
{
    public class SimpleResponse : ChallongeResponse
    {
        public SimpleResponse() { }

        public SimpleResponse(HttpResponseMessage response)
        {
            Parse(response);
        }

        public override void SuccessResponse(JObject result) { }
    }
}
