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
            switch (tournament.Type)
            {
                case TounamentType.Single:
                    jsonObject.Add("tournament_type", "single elimination");
                    jsonObject.Add("show_rounds", true);
                    break;
                case TounamentType.Double:
                    jsonObject.Add("tournament_type", "double elimination");
                    jsonObject.Add("show_rounds", true);
                    break;
                case TounamentType.RoundRobin:
                    jsonObject.Add("tournament_type", "round robin");
                    break;
                case TounamentType.Swiss:
                    jsonObject.Add("tournament_type", "swiss");
                    break;
            }
            jsonObject.Add("description", tournament.Description);
            jsonObject.Add("private", true);

            string url = "LA" + DateTime.Now.Year + Guid.NewGuid().ToString().Substring(0, 8);
            jsonObject.Add("url", url);

            return new TournamentResponse(await HttpService.Post("tournaments.json", jsonObject));
        }

        public static async Task<SimpleResponse> UpdateTournament(string url, string name)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("name", name);

            return new SimpleResponse(await HttpService.Put("tournaments/" + url + ".json", jsonObject));
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

        public static async Task<SimpleResponse> UpdateParticipant(int participantID, string tournamentUrl, string name)
        {
            JObject jsonObject = new JObject();
            jsonObject.Add("name", name);

            string requestUrl = "tournaments/" + tournamentUrl + "/participants/" + participantID + ".json";
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
