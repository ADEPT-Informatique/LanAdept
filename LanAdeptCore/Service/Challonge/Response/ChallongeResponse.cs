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

        public abstract void SuccessResponse(JObject result);

        public async void Parse(HttpResponseMessage response)
        {
            JObject jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());

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
                        ResponseMessage = (string)jsonObject.SelectToken("errors");
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
