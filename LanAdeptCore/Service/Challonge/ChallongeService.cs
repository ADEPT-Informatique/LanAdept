using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using LanAdeptCore.Service.Challonge.Response;
using LanAdeptCore.Service.Challonge.Request;

namespace LanAdeptCore.Service.Challonge
{
    public enum TounamentType { Single, Double, RoundRobin, Swiss }

    public enum ChallongeState { All, Pending, Open, Complete }

    public static class ChallongeService
    {
        #region Tournament

        public static async Task<TournamentResponse> CreateTournament(TournamentRequest tournament)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("name", "[LanAdept] " + tournament.Name);
            string tournamentType = "single elimination";
            switch (tournament.Type)
            {
                case TounamentType.Double:
                    tournamentType = "double elimination";
                    break;
                case TounamentType.RoundRobin:
                    tournamentType = "round robin";
                    break;
                case TounamentType.Swiss:
                    tournamentType = "swiss";
                    break;
            }
            jsonObject.Add("tournament_type", tournamentType);
            jsonObject.Add("description", tournament.Description);
            jsonObject.Add("private", true);
            //^http:\/\/challonge\.com\/[a-zA-Z0-9]{1,20}$
            string url = "LA" + DateTime.Now.Year + Guid.NewGuid().ToString().Substring(0, 8);
            jsonObject.Add("url", url);

            return new TournamentResponse(await HttpService.Post("tournaments.json", jsonObject));
        }

        public static async Task<SimpleResponse> UpdateTournament(string tournamentUrl, TournamentRequest tournament)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("name", tournament.Name);
            string tournamentType = "single elimination";
            switch (tournament.Type)
            {
                case TounamentType.Double:
                    tournamentType = "double elimination";
                    break;
                case TounamentType.RoundRobin:
                    tournamentType = "round robin";
                    break;
                case TounamentType.Swiss:
                    tournamentType = "swiss";
                    break;
            }
            jsonObject.Add("tournament_type", tournamentType);
            jsonObject.Add("description", tournament.Description);

            return new SimpleResponse(await HttpService.Put("tournaments/" + tournamentUrl + ".json", jsonObject));
        }

        public static async Task<SimpleResponse> DeleteTournament(string tournamentUrl)
        {
            return new SimpleResponse(await HttpService.Delete("tournaments/" + tournamentUrl + ".json"));
        }

        public static async Task<SimpleResponse> StartTournament(string tournamentUrl)
        {
            return new SimpleResponse(await HttpService.Post("tournaments/" + tournamentUrl + "/start.json", new JObject()));
        }

        public static async Task<SimpleResponse> FinalizeTournament(string tournamentUrl)
        {
            return new SimpleResponse(await HttpService.Post("tournaments/" + tournamentUrl + "/finalize.json", new JObject()));
        }

        public static async Task<SimpleResponse> ResetTournament(string tournamentUrl)
        {
            return new SimpleResponse(await HttpService.Post("tournaments/" + tournamentUrl + "/reset.json", new JObject()));
        }

        public static async Task<SimpleResponse> RandomizeTournament(string tournamentUrl)
        {
            return new SimpleResponse(await HttpService.Post("tournaments/" + tournamentUrl + "/participants/randomize.json", new JObject()));
        }

        #endregion

        #region Participant

        public static async Task<ParticipantResponse> CreateParticipant(ParticipantRequest request)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("name", request.Name);
            jsonObject.Add("misc", request.Misc);

            string requestUrl = "tournaments/" + request.TournamentUrl + "/participants.json";
            return new ParticipantResponse(await HttpService.Post(requestUrl, jsonObject));
        }

        public static async Task<SimpleResponse> UpdateParticipant(int participantID, ParticipantRequest request)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("name", request.Name);

            string requestUrl = "tournaments/" + request.TournamentUrl + "/participants/" + participantID + ".json";
            return new SimpleResponse(await HttpService.Put(requestUrl, jsonObject));
        }

        public static async Task<SimpleResponse> DeleteParticipant(string tournamentUrl, int participantID)
        {
            string request = "tournaments/" + tournamentUrl + "/participants/" + participantID + ".json";
            return new SimpleResponse(await HttpService.Delete(request));
        }

        #endregion

        #region Match

        public static async Task<ListMatchResponse> IndexMatch(string tournamentUrl)
        {
            return await IndexMatch(tournamentUrl, ChallongeState.All, null);
        }

        public static async Task<ListMatchResponse> IndexMatch(string tournamentUrl, int participantID)
        {
            return await IndexMatch(tournamentUrl, ChallongeState.All, participantID);
        }

        public static async Task<ListMatchResponse> IndexMatch(string tournamentUrl, ChallongeState state)
        {
            return await IndexMatch(tournamentUrl, state, null);
        }

        public static async Task<ListMatchResponse> IndexMatch(string tournamentUrl, ChallongeState state, int? participantID)
        {
            string requestUrl = "tournaments/" + tournamentUrl + "/matches.json";

            string filter = string.Empty;
            if (state != ChallongeState.All)
            {
                string matchState = "pending";
                switch (state)
                {
                    case ChallongeState.Open:
                        matchState = "open";
                        break;
                    case ChallongeState.Complete:
                        matchState = "complete";
                        break;
                }
                filter += "&state=" + matchState;
            }

            if (participantID != null)
                filter += "&participant_id=" + participantID;

            return new ListMatchResponse(await HttpService.Get(requestUrl, filter));
        }

        #endregion
    }
}
