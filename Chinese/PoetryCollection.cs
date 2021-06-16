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

using Trivial.Collection;
using Trivial.Text;
using Trivial.Data;

namespace Poetry.Chinese
{
    /// <summary>
    /// The poetry collection.
    /// </summary>
    public class PoetryCollection
    {
        private readonly List<PoetryFileInfo> col = new();
        private readonly List<AuthorInfo> authors = new();

        /// <summary>
        /// Gets count.
        /// </summary>
        /// <returns>The count</returns>
        public int Count()
            => col.Sum(ele => ele?.Count() ?? 0);

        /// <summary>
        /// Gets all of author info.
        /// </summary>
        /// <returns>The author information collection.</returns>
        public IReadOnlyList<AuthorInfo> GetAuthors()
        {
            try
            {
                return authors.AsReadOnly();
            }
            catch (InvalidOperationException)
            {
                try
                {
                    return authors.AsReadOnly();
                }
                catch (InvalidOperationException)
                {
                    return new List<AuthorInfo>();
                }
            }
        }

        /// <summary>
        /// Gets the author info.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The author information.</returns>
        public AuthorInfo GetAuthorById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return GetAuthors().FirstOrDefault(ele => ele.Id == id);
        }

        /// <summary>
        /// Gets the author info.
        /// </summary>
        /// <param name="name">The author name.</param>
        /// <returns>The author information.</returns>
        public IEnumerable<AuthorInfo> GetAuthorByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return GetAuthors().Where(ele => name.Equals(ele.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the author info.
        /// </summary>
        /// <param name="name">The author name.</param>
        /// <returns>The author information.</returns>
        public IEnumerable<AuthorInfo> GetAuthorByName(IEnumerable<string> name)
        {
            if (name == null) return null;
            return GetAuthors().Where(ele => PoetryPool.Contains(name, ele.Name));
        }

        /// <summary>
        /// Gets the author info.
        /// </summary>
        /// <param name="name1">The name of author 1.</param>
        /// <param name="name2">The name of author 2.</param>
        /// <param name="name3">The name of author 3.</param>
        /// <param name="name4">The name of author 4.</param>
        /// <returns>The author information.</returns>
        public IEnumerable<AuthorInfo> GetAuthorByName(string name1, string name2, string name3 = null, string name4 = null)
        {
            var arr = new List<string> { name1, name2, name3, name4 };
            return GetAuthors().Where(ele => PoetryPool.Contains(arr, ele.Name));
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The list of poetry matched.</returns>
        public Task<IEnumerable<PoetryModel>> SearchAsync(Func<PoetryFileInfo, bool> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null)
                return Task.FromResult<IEnumerable<PoetryModel>>(new List<PoetryModel>());
            var list = col.Where(predicate);
            return LoadDataAsync(list, cancellationToken);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The list of poetry matched.</returns>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(AuthorInfo author, CancellationToken cancellationToken = default)
        {
            await SearchAsync(ele => ele.Any(author), cancellationToken);
            return col.SelectMany(ele => ele.SearchCache(author));
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="authors">The authors.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The list of poetry matched.</returns>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(IEnumerable<AuthorInfo> authors, CancellationToken cancellationToken = default)
        {
            await SearchAsync(ele => ele.Any(authors), cancellationToken);
            return col.SelectMany(ele => ele.SearchCache(authors));
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="q">The query string.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The list of poetry matched.</returns>
        /// <exception cref="ArgumentNullException">predicate was null.</exception>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(string q, CancellationToken cancellationToken = default)
        {
            await SearchAsync(ele => ele.Any(q), cancellationToken);
            var empty = new List<PoetryModel>();
            return col.Where(ele => ele != null).SelectMany(ele => ele.SearchCache(q) ?? empty);
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="q">The query string.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The list of poetry matched.</returns>
        /// <exception cref="ArgumentNullException">predicate was null.</exception>
        public async Task<IEnumerable<PoetryModel>> SearchAsync(IEnumerable<string> q, CancellationToken cancellationToken = default)
        {
            await SearchAsync(ele => ele.Any(q), cancellationToken);
            var empty = new List<PoetryModel>();
            return col.Where(ele => ele != null).SelectMany(ele => ele.SearchCache(q) ?? empty);
        }

        /// <summary>
        /// Get the collection of basic info.
        /// </summary>
        /// <returns>A collection of basic info.</returns>
        public IEnumerable<IPoetryBasicInfo> EnumerableBasicInfo()
        {
            return col.Where(ele => ele != null).SelectMany(ele => ele.EnumerableBasicInfo());
        }

        /// <summary>
        /// Loads a file.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="cache">true if save the content into cache; otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry file information instance.</returns>
        public Task<PoetryFileInfo> LoadFileAsync(FileInfo file, bool cache = false, CancellationToken cancellationToken = default)
        {
            return LoadFileAsync(file, null, cache, cancellationToken);
        }

        /// <summary>
        /// Adds a poetry file information instance.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Add(PoetryFileInfo data)
        {
            if (data is null) return;
            if (data.File != null) col.RemoveAll(ele => ele.File == data.File);
            col.Add(data);
        }

        /// <summary>
        /// Loads a file.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="handler">The poetry model handler.</param>
        /// <param name="cache">true if save the content into cache; otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry file information instance.</returns>
        public async Task<PoetryFileInfo> LoadFileAsync(FileInfo file, Action<PoetryModel> handler, bool cache = false, CancellationToken cancellationToken = default)
        {
            if (file == null || !file.Exists) return null;
            col.RemoveAll(ele => ele.File == file);
            var c = await PoetryFileInfo.LoadDataAsync(file, cancellationToken);
            var r = new PoetryFileInfo(file, cache ? c : null);
            foreach (var p in c)
            {
                if (p == null) continue;
                handler?.Invoke(p);
                if (string.IsNullOrWhiteSpace(p.Author))
                {
                    r.Add(null, p.FullTitle);
                    continue;
                }

                var author = GetAuthorByName(p.Author)?.FirstOrDefault();
                if (author == null)
                {
                    author = new AuthorInfo
                    {
                        Name = p.Author
                    };
                    try
                    {
                        authors.Add(author);
                    }
                    catch (InvalidOperationException)
                    {
                        try
                        {
                            authors.Add(author);
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    }
                }

                r.Add(author, p.FullTitle);
            }

            col.Add(r);
            return r;
        }

        /// <summary>
        /// Loads authors from the specific file.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The author collection.</returns>
        public async Task<IEnumerable<AuthorInfo>> LoadAuthorsFileAsync(FileInfo file, CancellationToken cancellationToken = default)
        {
            if (file == null || !file.Exists) return null;
            using var t = file.OpenRead();
            var c = await JsonArray.ParseAsync(t, cancellationToken);
            var col = new List<AuthorInfo>();
            foreach (var item in c)
            {
                if (item is not JsonObject json) continue;
                var name = json.TryGetStringValue("name")?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var id = json.TryGetStringValue("id")?.Trim();
                AuthorInfo author = null;
                if (!string.IsNullOrEmpty(id))
                    author = GetAuthors().FirstOrDefault(ele => ele.Id == id);
                if (author == null)
                    author = GetAuthors().FirstOrDefault(ele => ele.Name == name);
                if (author == null)
                {
                    author = new AuthorInfo();
                    try
                    {
                        authors.Add(author);
                    }
                    catch (InvalidOperationException)
                    {
                        try
                        {
                            authors.Add(author);
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    }
                }

                if (string.IsNullOrEmpty(author.Id) && !!string.IsNullOrEmpty(id))
                    author.Id = id;
                author.Name = name;
                author.Introduction = json.TryGetStringValue("intro") ?? json.TryGetStringValue("desc") ?? json.TryGetStringValue("introduction") ?? json.TryGetStringValue("description");
                col.Add(author);
            }

            return col;
        }

        /// <summary>
        /// Converts to a JSON array.
        /// </summary>
        /// <param name="menu">The instance to convert.</param>
        public static explicit operator JsonArray(PoetryCollection menu)
        {
            if (menu == null) return null;
            var arr = new JsonArray();
            foreach (var item in menu.col)
            {
                if (item == null) continue;
                arr.Add((JsonObject)item);
            }

            return arr;
        }

        /// <summary>
        /// Generates the menu.
        /// </summary>
        /// <param name="dir">The directory to load files.</param>
        /// <param name="searchPattern">The search string to match against the names of files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <param name="outputFileName">The optional output file name to save the result.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry file menu instance.</returns>
        public static async Task<PoetryCollection> GenerateAsync(DirectoryInfo dir, string searchPattern, string outputFileName = null, CancellationToken cancellationToken = default)
        {
            if (dir == null) return null;
            var files = dir.GetFiles(searchPattern ?? "*.json", SearchOption.TopDirectoryOnly);
            var menu = new PoetryCollection();
            foreach (var file in files)
            {
                await menu.LoadFileAsync(file, false, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(outputFileName))
            {
                var s = (JsonArray)menu;
                try
                {
                    var path = Path.Combine(dir.FullName, outputFileName);
#if NETOLDVER
                    File.WriteAllText(path, s.ToString(), Encoding.UTF8);
#else
                    await File.WriteAllTextAsync(path, s.ToString(), Encoding.UTF8, cancellationToken);
#endif
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (System.Security.SecurityException)
                {
                }
                catch (IOException)
                {
                }
                catch (NotSupportedException)
                {
                }
            }

            return menu;
        }

        /// <summary>
        /// Loads and parses the menu file.
        /// </summary>
        /// <param name="file">A UTF-8 JSON format file.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry file menu instance.</returns>
        public static async Task<PoetryCollection> ParseAsync(FileInfo file, CancellationToken cancellationToken = default)
        {
            if (file == null || !file.Exists) return null;
            var menu = new PoetryCollection();
            var s = file.OpenRead();
            var arr = await JsonArray.ParseAsync(s, cancellationToken);
            foreach (var json in arr)
            {
                var info = PoetryFileInfo.Create(json as JsonObject, menu.authors, file.Directory);
                menu.col.Add(info);
            }

            return menu;
        }

        /// <summary>
        /// Searches.
        /// </summary>
        /// <param name="list">The poetry file information collection.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The list of poetry matched.</returns>
        /// <exception cref="ArgumentNullException">predicate was null.</exception>
        public static async Task<IEnumerable<PoetryModel>> LoadDataAsync(IEnumerable<PoetryFileInfo> list, CancellationToken cancellationToken = default)
        {
            var result = new List<PoetryModel>();
            foreach (var item in list)
            {
                var poems = await item.LoadDataAsync(cancellationToken);
                result.AddRange(poems);
            }

            return result;
        }
    }
}

