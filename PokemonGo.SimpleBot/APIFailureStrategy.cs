using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.SimpleBot.Utils;
using POGOProtos.Networking.Envelopes;

namespace PokemonGo.SimpleBot
{
    class APIFailureStrategy: IApiFailureStrategy
    {
        // TODO: move the error tracking logic into this method
        public Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            Randomization.RandomDelay(5000).Wait();
            return Task.Run(() => ApiOperation.Abort);
        }

        public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
            Randomization.RandomDelay(500).Wait();
        }
    }
}
