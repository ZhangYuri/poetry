using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Poetry.Chinese
{

    /// <summary>
    /// The basic information of the poetry item.
    /// </summary>
    public interface IPoetryBasicInfo
    {
        /// <summary>
        /// Gets or sets the full title.
        /// </summary>
        string FullTitle { get; }

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        string Author { get; set; }
    }

    /// <summary>
    /// The basic information of the poetry item.
    /// </summary>
    [DataContract]
    internal class PoetryBasicInfo : IPoetryBasicInfo
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonPropertyName("title")]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets the full title.
        /// </summary>
        [JsonIgnore]
        public string FullTitle { get; set; }

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        [JsonPropertyName("author")]
        [DataMember(Name = "author")]
        public string Author { get; set; }
    }

    /// <summary>
    /// The poetry model.
    /// </summary>
    [DataContract]
    public class PoetryModel : IPoetryBasicInfo
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
        /// Gets or sets the chapter name.
        /// </summary>
        [JsonPropertyName("chapter")]
#if !NETCOREAPP3_1
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        [DataMember(Name = "chapter", EmitDefaultValue = false)]
        public string Chapter { get; set; }


        /// <summary>
        /// Gets or sets the section name.
        /// </summary>
        [JsonPropertyName("section")]
#if !NETCOREAPP3_1
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        [DataMember(Name = "section", EmitDefaultValue = false)]
        public string Section { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonPropertyName("title")]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets the full title.
        /// </summary>
        [JsonIgnore]
        public string FullTitle
        {
            get
            {
                var title = Title?.Trim();
                var prefix = (Section ?? Chapter)?.Trim();
                if (string.IsNullOrEmpty(prefix)) return title;
                return string.IsNullOrEmpty(title) ? prefix : $"{prefix}·{title}";
            }
        }

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        [JsonPropertyName("author")]
        [DataMember(Name = "author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the dynasty, region or country name.
        /// </summary>
        [JsonPropertyName("dynasty")]
        [DataMember(Name = "dynasty")]
        public string Dynasty { get; set; }

        /// <summary>
        /// Gets or sets the paragraphs.
        /// </summary>
        [JsonPropertyName("paragraphs")]
        [DataMember(Name = "paragraphs")]
        public IEnumerable<string> Paragraphs { get; set; }

        /// <summary>
        /// Gets or sets the strains.
        /// </summary>
        [JsonPropertyName("strains")]
#if !NETCOREAPP3_1
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
#endif
        [DataMember(Name = "strains", EmitDefaultValue = false)]
        public IEnumerable<string> Strains { get; set; }

        /// <summary>
        /// Gets a value indicating whether it contains any paragraph.
        /// </summary>
        [JsonIgnore]
        public bool HasContent => Paragraphs?.Any() == true;

        /// <summary>
        /// Gets or sets the extension data for JSON serialization.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, JsonElement> ExtensionSerializationData { get; set; }

        /// <summary>
        /// Reorganizes data.
        /// </summary>
        public void Reorganize()
        {
            if (ExtensionSerializationData == null) return;
            if (Paragraphs == null && ExtensionSerializationData.TryGetValue("content", out var content))
            {
                if (content.ValueKind == JsonValueKind.String)
                {
                    Paragraphs = new List<string>
                    {
                        content.GetString()
                    };
                }
                else if (content.ValueKind == JsonValueKind.Array)
                {
                    Paragraphs = content.EnumerateArray()
                        .Where(ele => ele.ValueKind == JsonValueKind.String)
                        .Select(ele => ele.GetString())
                        .ToList();
                }
            }

            if (ExtensionSerializationData.TryGetValue("rhythmic", out var rhythmic))
            {
                if (rhythmic.ValueKind == JsonValueKind.String)
                {
                    var s = rhythmic.GetString();
                    if (string.IsNullOrWhiteSpace(Title)) Title = s;
                    else if (!string.IsNullOrWhiteSpace(s) && !Title.StartsWith(s))
                        Title = $"{s}·{Title}";
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current poetry.
        /// </summary>
        /// <returns>A string that represents the current poetry.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Chapter))
                sb.Append(Chapter);
            if (!string.IsNullOrWhiteSpace(Section))
            {
                if (sb.Length > 0) sb.Append('·');
                sb.Append(Section);
            }

            if (!string.IsNullOrWhiteSpace(Title))
            {
                if (sb.Length > 0) sb.Append('·');
                sb.AppendLine(Title);
            }

            if (string.IsNullOrWhiteSpace(Dynasty))
            {
                if (!string.IsNullOrWhiteSpace(Author))
                    sb.AppendLine(Author);
            }
            else
            {
                sb.Append('[');
                sb.Append(Dynasty);
                sb.Append("] ");
                if (!string.IsNullOrWhiteSpace(Author))
                    sb.AppendLine(Author);
            }

            var p = Paragraphs?.Where(ele => !string.IsNullOrWhiteSpace(ele))?.ToList();
            if (p != null && p.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine(string.Join(Environment.NewLine, p));
            }

            return sb.ToString();
        }
    }
}
