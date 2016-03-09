using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LanAdeptCore.Service.Challonge.Response
{
    public abstract class ChallongeResponse
    {
        public bool HasError { get; private set; }

        public string ResponseMessage { get; private set; }

        public abstract void SuccessResponse(JArray result);

        public async void Parse(HttpResponseMessage response)
        {
            string responseData = await response.Content.ReadAsStringAsync();
            JArray jsonObject = responseData[0] == '[' ? JArray.Parse(responseData) : new JArray(JObject.Parse(responseData));

            HasError = !response.IsSuccessStatusCode;

            if (response.IsSuccessStatusCode)
            {
                SuccessResponse(jsonObject);
            }
            else
            {
                switch (response.StatusCode)
                {
                    case (HttpStatusCode)422:
                        var errors = jsonObject[0].SelectToken("errors");
                        string message = string.Empty;
                        foreach (var error in errors)
                            message += error + "\n";

                        ResponseMessage = message;
                        break;
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.NotAcceptable:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.InternalServerError:
                        ResponseMessage = "Une erreur est survenue. Veuillez réessayer de nouveau ou contactez un administrateur";
                        break;
                    default:
                        ResponseMessage = "Une erreur inattendue est survenue. Veuillez réessayer de nouveau ou contactez un administrateur";
                        break;
                }
            }
        }
    }
}
