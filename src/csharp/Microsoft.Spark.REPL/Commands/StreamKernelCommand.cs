using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Spark.REPL.Commands
{
    public class Command
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    [JsonConverter(typeof(StreamKernelCommandConverter))]
    public class StreamKernelCommand
    {
        private static readonly JsonSerializerSettings s_jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None
        };

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("commandType")]
        public string CommandType { get; set; }

        [JsonProperty("command")]
        public Command Command { get; set; }

        public static StreamKernelCommand Deserialize(string source)
        {
            return JsonConvert.DeserializeObject<StreamKernelCommand>(source, s_jsonSerializerSettings);
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, s_jsonSerializerSettings);
        }
    }
}
