using LanAdeptCore.Service.Challonge.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptCore.Service.Challonge.Response
{
    public class MatchResponse : ChallongeResponse
    {
        public ChallongeMatch Match { get; private set; }

        public MatchResponse() { }

        public MatchResponse(HttpResponseMessage response)
        {
            Parse(response);
        }

        public override void SuccessResponse(JObject result)
        {
            //TODO la creation de l'objet
            throw new NotImplementedException();
        }
    }
}
