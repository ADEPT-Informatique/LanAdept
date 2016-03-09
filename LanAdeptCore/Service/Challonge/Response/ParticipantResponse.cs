using LanAdeptCore.Service.Challonge.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace LanAdeptCore.Service.Challonge.Response
{
    public class ParticipantResponse : ChallongeResponse
    {
        public ChallongeParticipant Participant { get; private set; }

        public ParticipantResponse() { }

        public ParticipantResponse(HttpResponseMessage response)
        {
            Parse(response);
        }

        public override void SuccessResponse(JArray result)
        {
            Participant = new ChallongeParticipant();
            var participant = result[0].SelectToken("participant");
            Participant.ParticipantId = (int)participant.SelectToken("id");
        }
    }
}
