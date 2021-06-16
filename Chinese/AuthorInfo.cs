using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Text;
using Trivial.Data;

namespace Poetry.Chinese
{
    /// <summary>
    /// The author information model.
    /// </summary>
    public class AuthorInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [JsonPropertyName("id")]
#if !NETCOREAPP3_1
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonPropertyName("name")]
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the introduction.
        /// </summary>
        [JsonPropertyName("intro")]
        [DataMember(Name = "intro", EmitDefaultValue = false)]
        public string Introduction { get; set; }

        /// <summary>
        /// Converts to a JSON object.
        /// </summary>
        /// <param name="info">The instance to convert.</param>
        public static explicit operator JsonObject(AuthorInfo info)
        {
            if (info == null) return null;
            return new JsonObject
            {
                { "id", info.Id },
                { "name", info.Name },
                { "intro", info.Introduction }
            };
        }
    }
}
