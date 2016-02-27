using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using LanAdeptCore.Service.Challonge.Data;

namespace LanAdeptCore.Service.Challonge.Response
{
    public class ListMatchResponse : ChallongeResponse
    {
        public List<ChallongeMatch> Matchs { get; private set; }

        public ListMatchResponse()
        {
            Matchs = new List<ChallongeMatch>();
        }

        public ListMatchResponse(HttpResponseMessage response)
        {
            Matchs = new List<ChallongeMatch>();
            Parse(response);
        }

        public override void SuccessResponse(JObject result)
        {
            //TODO la creation de l'objet
            throw new NotImplementedException();
        }
    }
}
