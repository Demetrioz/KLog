using KLog.Api.Core;
using KLog.Api.Services;
using KLog.DataModel.Context;
using KLog.DataModel.DTOs.GitHub;
using KLog.DataModel.DTOs.GitHub.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Api.Controllers
{
    [Route("api/github")]
    public class GitHubController : KLogController
    {
        private readonly IAuthenticationService AuthService;
        private readonly IGitHubEventHandler EventHandler;

        private readonly Dictionary<string, Type> GithubEventTypeMap = new Dictionary<string, Type>()
        {
            { "ping", typeof(Ping) }
        };

        public GitHubController(
            IAuthenticationService authService,
            IGitHubEventHandler eventHandler,
            KLogContext context
        ) : base(context)
        {
            AuthService = authService;
            EventHandler = eventHandler;
        }

        // https://docs.github.com/en/developers/webhooks-and-events/webhooks/webhook-events-and-payloads#create

        /// <summary>
        /// Receives a post from GitHub and sends the event to a handler to log appropriately
        /// </summary>
        /// <returns>A boolean reflecting that the request was received</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ParseGithubEvent()
        {
            string signature;
            string key;

            try
            {
                signature = Request.Headers
                    .Where(h => h.Key == "X-Hub-Signature-256")
                    .Select(h => h.Value.ToString())
                    .FirstOrDefault();

                key = signature.Split("=")[1];
            }
            catch(Exception ex)
            {
                return ApiResponse.Unauthorized();
            }

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string body = await reader.ReadToEndAsync();
                int githubAppId = await AuthService.ValidateGithubSignature(body, key);
                if(githubAppId > 0)
                {
                    string eventType = Request.Headers
                        .Where(h => h.Key == "X-Github-Event")
                        .Select(h => h.Value.ToString())
                        .FirstOrDefault();

                    if(GithubEventTypeMap.TryGetValue(eventType, out Type type))
                    {
                        MethodInfo deserializer = typeof(GitHubController)
                            .GetMethod(nameof(GetGenericType));
                        MethodInfo genericDeserializer = deserializer.MakeGenericMethod(type);
                        object typedObject = genericDeserializer.Invoke(null, new[] { body });
                        await EventHandler.HandleEvent(githubAppId, typedObject);
                    }
                    else
                    {
                        GitHubEvent gitHubEvent = JsonConvert.DeserializeObject<GitHubEvent>(body);
                        await EventHandler.HandleEvent(githubAppId, gitHubEvent, eventType);
                    }
                }
            }

            return ApiResponse.Success(true);
        }

        public static T GetGenericType<T>(string json) where T : IGithubEvent
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
