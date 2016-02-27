using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using LanAdeptCore.Service.Challonge.Data;

namespace LanAdeptCore.Service.Challonge.Response
{
    public class TournamentResponse : ChallongeResponse
    {
        public ChallongeTournament Tournament { get; private set; }

        public TournamentResponse() { }

        public TournamentResponse(HttpResponseMessage response)
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
