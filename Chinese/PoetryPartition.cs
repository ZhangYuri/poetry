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
    /// The poetry file information.
    /// </summary>
    public class PoetryFileInfo
    {
        /// <summary>
        /// The poetry information collection.
        /// </summary>
        private readonly List<AuthorPoetry> poems = new();

        /// <summary>
        /// The handler to load data.
        /// </summary>
        private readonly Func<List<AuthorPoetry>, FileInfo, Task<IEnumerable<PoetryModel>>> load;

        /// <summary>
        /// The ` models cache.
        /// </summary>
        private List<PoetryModel> cache;

        /// <summary>
        /// Initializes a new instance of the PoetryFileInfo class.
        /// </summary>
        /// <param name="file">The file information instance.</param>
        /// <param name="allData">All data.</param>
        internal PoetryFileInfo(FileInfo file, IEnumerable<PoetryModel> allData = null)
        {
            File = file;
            cache = allData?.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the PoetryFileInfo class.
        /// </summary>
        /// <param name="load">The handler to load data.</param>
        /// <param name="file">The file information instance.</param>
        internal PoetryFileInfo(Func<List<AuthorPoetry>, FileInfo, Task<IEnumerable<PoetryModel>>> load, FileInfo file = null)
        {
            this.load = load;
            File = file;
        }

        /// <summary>
        /// Gets the file information.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Gets count.
        /// </summary>
        /// <returns>The count</returns>
        public int Count()
            => cache != null ? cache.Count : poems.Sum(ele => ele?.Poetry?.Count ?? 0);

        /// <summary>
        /// Add a poetry.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <param name="title">The poetry title.</param>
        internal void Add(AuthorInfo author, string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return;
            var item = poems.FirstOrDefault(ele => ele.Author == author);
            if (item == null)
            {
                item = new AuthorPoetry
                {
                    Author = author,
                    Poetry = new List<string>()
                };
                poems.Add(item);
            }

            if (item.Poetry == null)
                item.Poetry = new List<string>();
            if (!item.Poetry.Contains(title))
                item.Poetry.Add(title);
        }

        /// <summary>
        /// Gets all of author info.
        /// </summary>
        /// <returns>The author information collection.</returns>
        public IEnumerable<AuthorInfo> GetAuthors()
        {
            return poems.Select(ele => ele.Author).Distinct();
        }

        /// <summary>
        /// Gets the author info.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The author information.</returns>
        public AuthorInfo GetAuthorById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return poems.FirstOrDefault(ele => ele.Author?.Id == id)?.Author;
        }

        /// <summary>
        /// Gets the author info.
        /// </summary>
        /// <param name="name">The author name.</param>
        /// <returns>The author information.</returns>
        public IEnumerable<AuthorInfo> GetAuthorByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return poems.Where(ele => ele.Author?.Name == name).Select(ele => ele.Author);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="author">The author to test.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool Any(AuthorInfo author)
        {
            return poems.Any(ele => ele.Author == author);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="authors">The authors to test.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool Any(IEnumerable<AuthorInfo> authors)
        {
            if (authors is null) return false;
            var col = authors.ToList();
            return poems.Any(ele => col.Contains(ele.Author));
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="q">The query.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool Any(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return false;
            return poems.Any(ele => q.Equals(ele.Author?.Name, StringComparison.CurrentCultureIgnoreCase)
                || ele.Poetry?.Any(s => PoetryPool.Contains(s, q)) == true);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="q">The query.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool Any(IEnumerable<string> q)
        {
            if (q is null) return false;
            return poems.Any(ele => PoetryPool.Contains(q, ele.Author?.Name)
                || ele.Poetry?.Any(s => PoetryPool.Contains(s, q)) == true);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>true if contains; otherwise, false.</returns>
        public bool Any(Func<string, bool> predicate)
        {
            return poems.Any(ele => ele.Poetry?.Any(predicate) == true);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="author">The author to test.</param>
        /// <returns>The collection of poetry.</returns>
        public IEnumerable<PoetryModel> SearchCache(AuthorInfo author)
        {
            if (cache == null) return null;
            return cache.Where(ele => ele.Author == author?.Name);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="authors">The authors to test.</param>
        /// <returns>The collection of poetry.</returns>
        public IEnumerable<PoetryModel> SearchCache(IEnumerable<AuthorInfo> authors)
        {
            if (cache == null || authors is null) return null;
            var col = authors.ToList();
            return cache.Where(ele => col.Any(a => ele.Author == a?.Name));
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="q">The query.</param>
        /// <returns>The collection of poetry.</returns>
        public IEnumerable<PoetryModel> SearchCache(string q)
        {
            if (cache == null) return null;
            if (string.IsNullOrWhiteSpace(q)) return new List<PoetryModel>();
            return cache.Where(ele => q.Equals(ele.Author, StringComparison.CurrentCultureIgnoreCase)
                || (!string.IsNullOrWhiteSpace(ele.FullTitle) && PoetryPool.Contains(ele.FullTitle, q)) == true);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="q">The query.</param>
        /// <returns>The collection of poetry.</returns>
        public IEnumerable<PoetryModel> SearchCache(IEnumerable<string> q)
        {
            if (cache == null) return null;
            if (q == null) return new List<PoetryModel>();
            return cache.Where(ele => PoetryPool.Contains(q, ele.Author)
                || (!string.IsNullOrWhiteSpace(ele.FullTitle) && PoetryPool.Contains(ele.FullTitle, q)) == true);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The collection of poetry.</returns>
        public IEnumerable<PoetryModel> SearchCache(Func<PoetryModel, bool> predicate)
        {
            return cache?.Where(predicate);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="author">The author to test.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The collection of poetry.</returns>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(AuthorInfo author, CancellationToken cancellationToken = default)
        {
            await LoadDataAsync(cancellationToken);
            return SearchCache(author);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="q">The query.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The collection of poetry.</returns>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(string q, CancellationToken cancellationToken = default)
        {
            await LoadDataAsync(cancellationToken);
            return SearchCache(q);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="q">The query.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The collection of poetry.</returns>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(IEnumerable<string> q, CancellationToken cancellationToken = default)
        {
            await LoadDataAsync(cancellationToken);
            return SearchCache(q);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The collection of poetry.</returns>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(Func<PoetryModel, bool> predicate, CancellationToken cancellationToken = default)
        {
            await LoadDataAsync(cancellationToken);
            return SearchCache(predicate);
        }

        /// <summary>
        /// Gets all titles of poetry.
        /// </summary>
        /// <returns>The list of poetry title.</returns>
        public IEnumerable<string> GetPoetryTitles()
        {
            return poems.SelectMany(ele => ele.Poetry).Where(ele => !string.IsNullOrWhiteSpace(ele));
        }

        /// <summary>
        /// Gets all titles of poetry by its author.
        /// </summary>
        /// <param name="author">The author as a filter condition.</param>
        /// <returns>The list of poetry title.</returns>
        public IEnumerable<string> GetPoetryTitles(AuthorInfo author)
        {
            return poems.Where(ele => ele.Author == author).SelectMany(ele => ele.Poetry).Where(ele => !string.IsNullOrWhiteSpace(ele));
        }

        /// <summary>
        /// Clears cache.
        /// </summary>
        public void ClearCache()
        {
            cache = null;
        }

        /// <summary>
        /// Get the collection of basic info.
        /// </summary>
        /// <returns>A collection of basic info.</returns>
        public IEnumerable<IPoetryBasicInfo> EnumerableBasicInfo()
        {
            if (cache != null)
            {
                foreach (var item in cache)
                {
                    if (item == null) continue;
                    yield return item;
                }

                yield break;
            }

            foreach (var item in poems)
            {
                if (item?.Poetry == null) continue;
                var author = item.Author?.Name;
                foreach (var name in item.Poetry)
                {
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    yield return new PoetryBasicInfo
                    {
                        Author = author,
                        Title = name,
                        FullTitle = name
                    };
                }
            }
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry models.</returns>
        public async Task<IReadOnlyList<PoetryModel>> LoadDataAsync(CancellationToken cancellationToken = default)
        {
            if (cache != null) return cache.AsReadOnly();
            if (load != null) return cache = (await load(poems, File))?.ToList() ?? new List<PoetryModel>();
            if (File == null || !File.Exists) return cache = new List<PoetryModel>();
            cache = await LoadDataAsync(File, cancellationToken);
            return cache.AsReadOnly();
        }

        /// <summary>
        /// Converts from a JSON object.
        /// </summary>
        /// <param name="json">The JSON object to convert.</param>
        /// <param name="authors">The author list.</param>
        /// <param name="folder">The diretory.</param>
        public static PoetryFileInfo Create(JsonObject json, IList<AuthorInfo> authors, DirectoryInfo folder)
        {
            if (json == null) return null;
            var fileName = json.TryGetStringValue("file");
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            var file = new FileInfo(folder != null ? Path.Combine(folder.FullName, fileName) : fileName);
            var info = new PoetryFileInfo(file);
            var arr = json.TryGetArrayValue("poetry");
            if (arr == null) return info;
            if (authors is null) authors = new List<AuthorInfo>();
            foreach (var item in arr)
            {
                if (item == null || item is not JsonArray a || a.Count < 1) continue;
                var authorName = a.TryGetStringValue(0);
                AuthorInfo author = null;
                if (!string.IsNullOrEmpty(authorName))
                {
                    author = authors.FirstOrDefault(ele => ele?.Name == authorName);
                    if (author == null)
                    {
                        author = new AuthorInfo
                        {
                            Name = authorName
                        };
                        authors.Add(author);
                    }
                }

                for (var i = 1; i < a.Count; i++)
                {
                    info.Add(author, a.TryGetStringValue(i));
                }
            }

            return info;
        }

        /// <summary>
        /// Converts to a JSON object.
        /// </summary>
        /// <param name="info">The instance to convert.</param>
        public static explicit operator JsonObject(PoetryFileInfo info)
        {
            if (info == null) return null;
            var json = new JsonArray();
            foreach (var item in info.poems)
            {
                if (item == null) continue;
                var arr = new JsonArray
                {
                    item.Author?.Name
                };
                arr.AddRange(item.Poetry);
                json.Add(arr);
            }

            return new JsonObject
            {
                { "file", info.File?.Name },
                { "poetry", json }
            };
        }

        /// <summary>
        /// Loads data.
        /// </summary>
        /// <param name="file">The file to load.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry models.</returns>
        internal static async Task<List<PoetryModel>> LoadDataAsync(FileInfo file, CancellationToken cancellationToken = default)
        {
            if (file == null || !file.Exists) return new List<PoetryModel>();
            using var stream = file.OpenRead();
            try
            {
                var col = await JsonSerializer.DeserializeAsync<List<PoetryModel>>(stream, null, cancellationToken);
                if (col == null) return new List<PoetryModel>();
                foreach (var item in col)
                {
                    item.Reorganize();
                }
                return col;
            }
            catch (JsonException)
            {
                var col = new List<PoetryModel>();
                try
                {
                    stream.Position = 0;
                    var item = await JsonSerializer.DeserializeAsync<PoetryModel>(stream, null, cancellationToken);
                    if (item != null)
                    {
                        item.Reorganize();
                        col.Add(item);
                    }

                    return col;
                }
                catch (JsonException)
                {
                    stream.Dispose();
                    using var reader = file.OpenText();
                    var l = reader.ReadLine()?.Trim();
                    while (l != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (l.Length < 10)
                        {
                            l = reader.ReadLine();
                            continue;
                        }

                        var m = JsonSerializer.Deserialize<PoetryModel>(l);
                        if (m != null)
                        {
                            m.Reorganize();
                            col.Add(m);
                        }

                        l = reader.ReadLine()?.Trim();
                    }

                    return col;
                }
            }
        }
    }

    /// <summary>
    /// Content container.
    /// </summary>
    [DataContract]
    public class ContentContainer<T>
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        [JsonPropertyName("content")]
        [DataMember(Name = "content")]
        public T Content { get; set; }
    }

    /// <summary>
    /// Content container.
    /// </summary>
    [DataContract]
    public class TitledContentContainer<T> : ContentContainer<T>
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [JsonPropertyName("title")]
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }

    internal class AuthorPoetry
    {
        public AuthorInfo Author { get; set; }

        public IList<string> Poetry { get; set; }

        /// <summary>
        /// Converts to a JSON object.
        /// </summary>
        /// <param name="info">The instance to convert.</param>
        public static explicit operator JsonObject(AuthorPoetry info)
        {
            if (info == null) return null;
            return new JsonObject
            {
                { "author", (JsonObject)info.Author },
                { "poetry", info.Poetry }
            };
        }
    }
}
