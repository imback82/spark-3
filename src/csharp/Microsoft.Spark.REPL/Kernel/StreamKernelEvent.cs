using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Spark.REPL.Kernel
{
    public class StreamKernelEvent
    {
        private static readonly JsonSerializerSettings s_jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        };

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("event")]
        public object Event { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, s_jsonSerializerSettings);
        }

        public string GetValue()
        {
            if (EventType == "ReturnValueProduced" ||
             EventType == "DisplayedValueProduced" ||
             EventType == "DisplayedValueUpdated")
            {
                return ((JObject)Event)["value"].ToString();
            }
            else if (EventType == "CommandFailed")
            {
                return ((JObject)Event)["message"].ToString();
            }
            else
            {
                return "";
            }
        }

        public bool IsProcessingComplete()
        {
            return EventType == "CommandParseFailure" ||
             EventType == "CommandNotRecognized" ||
             EventType == "CommandHandled" ||
             EventType == "CommandFailed";
        }

        public bool IsError()
        {
            return EventType == "CommandNotRecognized" ||
                EventType == "CommandFailed";
        }
    }
}
