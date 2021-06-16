using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Poetry.Chinese
{
    /// <summary>
    /// The poetry pool.
    /// </summary>
    public static class PoetryPool
    {
        private readonly static SemaphoreSlim slim = new(1, 1);
        private static bool hasInit = false;
        internal static PoetryFileInfo tangPoetry300;

        /// <summary>
        /// Gets or sets the Tang Poetry.
        /// </summary>
        public static PoetryCollection TangPoetry { get; set; }

        /// <summary>
        /// Gets or sets the Southern Tang Poetry.
        /// </summary>
        public static PoetryCollection SouthernTangPoetry { get; set; }

        /// <summary>
        /// Gets or sets the Huajian Anthology.
        /// </summary>
        public static PoetryCollection HuajianAnthology { get; set; }

        /// <summary>
        /// Gets or sets the Song Poetry.
        /// </summary>
        public static PoetryCollection SongPoetry { get; set; }

        /// <summary>
        /// Gets or sets the Song Lyrics.
        /// </summary>
        public static PoetryCollection SongLyrics { get; set; }

        /// <summary>
        /// Gets or sets the Yuan Verses.
        /// </summary>
        public static PoetryCollection YuanVerses { get; set; }

        /// <summary>
        /// Gets or sets the Confucian Analects.
        /// </summary>
        public static PoetryCollection ConfucianAnalects { get; set; }

        /// <summary>
        /// Gets or sets the Book of Songs.
        /// </summary>
        public static PoetryCollection BookOfSongs { get; set; }

        /// <summary>
        /// Gets or sets the Songs of Chu.
        /// </summary>
        public static PoetryCollection ChuSongs { get; set; }

        /// <summary>
        /// Gets or sets the Four Books and the Five Classics.
        /// </summary>
        public static PoetryCollection FourBooksFiveClassics { get; set; }

        /// <summary>
        /// Gets or sets the primate school.
        /// </summary>
        public static PoetryCollection PrimateSchool { get; set; }

        /// <summary>
        /// Gets or sets the Caocao Anthology.
        /// </summary>
        public static PoetryCollection CaocaoAnthology { get; set; }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        public static DirectoryInfo RootPath { get; set; }

        /// <summary>
        /// Loads the Tang Poetry.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadTangPoetryAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (TangPoetry != null && !force) return TangPoetry;
            var rootPath = GetRootDirectory();
            await slim.WaitAsync(cancellationToken);
            try
            {
                TangPoetry = await ParseFileMenuAsync(rootPath, "json", "menu.tang.json", cancellationToken)
                    ?? await PoetryCollection.GenerateAsync(GetDirectoryInfo(rootPath, "json"), "poet.tang.*.json", "menu.song.json", cancellationToken);
                await TangPoetry.LoadAuthorsFileAsync(GetFileInfo(rootPath, "json", "authors.tang.json"), cancellationToken);
                return TangPoetry;
            }
            catch (JsonException)
            {
                return TangPoetry = new PoetryCollection();
            }
            finally
            {
                slim.Release();
            }
        }

        /// <summary>
        /// Loads the Huajian Anthology.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadHuajianAnthologyAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (HuajianAnthology != null && !force) return HuajianAnthology;
            var rootPath = GetRootDirectory();
            await slim.WaitAsync(cancellationToken);
            try
            {
                return HuajianAnthology = await ParseFileMenuAsync(rootPath, "wudai\\huajianji", "menu.huajianji.json", cancellationToken)
                    ?? await PoetryCollection.GenerateAsync(GetDirectoryInfo(rootPath, "wudai\\huajianji"), "huajianji-*.json", "menu.huajianji.json", cancellationToken);
            }
            catch (JsonException)
            {
                return HuajianAnthology = new PoetryCollection();
            }
            finally
            {
                slim.Release();
            }
        }

        /// <summary>
        /// Loads the Southern Tang Poetry.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadSouthernTangPoetryAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (SouthernTangPoetry != null && !force) return SouthernTangPoetry;
            SouthernTangPoetry = await LoadSingleFile("wudai\\nantang", "poetrys.json", null, cancellationToken);
            await SouthernTangPoetry.LoadAuthorsFileAsync(GetFileInfo(GetRootDirectory(), "wudai\\nantang", "authors.json"), cancellationToken);
            return SouthernTangPoetry;
        }

        /// <summary>
        /// Loads the Song Poetry.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadSongPoetryAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (SongPoetry != null && !force) return SongPoetry;
            var rootPath = GetRootDirectory();
            await slim.WaitAsync(cancellationToken);
            try
            {
                SongPoetry = await ParseFileMenuAsync(rootPath, "json", "menu.song.json", cancellationToken)
                    ?? await PoetryCollection.GenerateAsync(GetDirectoryInfo(rootPath, "json"), "poet.song.*.json", "menu.song.json", cancellationToken);
                await TangPoetry.LoadAuthorsFileAsync(GetFileInfo(rootPath, "json", "authors.song.json"), cancellationToken);
                return SongPoetry;
            }
            catch (JsonException)
            {
                return SongPoetry = new PoetryCollection();
            }
            finally
            {
                slim.Release();
            }
        }

        /// <summary>
        /// Loads the Song Lyrics.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadSongLyricsAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (SongLyrics != null && !force) return SongLyrics;
            var rootPath = GetRootDirectory();
            await slim.WaitAsync(cancellationToken);
            try
            {
                SongLyrics = await ParseFileMenuAsync(rootPath, "ci", "menu.song.json", cancellationToken)
                    ?? await PoetryCollection.GenerateAsync(GetDirectoryInfo(rootPath, "ci"), "ci.song.*.json", "menu.song.json", cancellationToken);
                await SongLyrics.LoadAuthorsFileAsync(GetFileInfo(rootPath, "ci", "author.song.json"), cancellationToken);
                return SongLyrics;
            }
            catch (JsonException)
            {
                return SongLyrics = new PoetryCollection();
            }
            finally
            {
                slim.Release();
            }
        }

        /// <summary>
        /// Loads the Yuan Verses.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadYuanVersesAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (YuanVerses != null && !force) return YuanVerses;
            return YuanVerses = await LoadSingleFile("yuanqu", "yuanqu.json", null, cancellationToken);
        }

        /// <summary>
        /// Loads the Confucian Analects.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadConfucianAnalectsAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (ConfucianAnalects != null && !force) return ConfucianAnalects;
            return ConfucianAnalects = await LoadSingleFile("lunyu", "lunyu.json", null, cancellationToken);
        }

        /// <summary>
        /// Loads the Book of Songs.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadBookOfSongsAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (BookOfSongs != null && !force) return BookOfSongs;
            return BookOfSongs = await LoadSingleFile("shijing", "shijing.json", null, cancellationToken);
        }

        /// <summary>
        /// Loads the Songs of Chu.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadChuSongsAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (ChuSongs != null && !force) return ChuSongs;
            return ChuSongs = await LoadSingleFile("chuci", "chuci.json", null, cancellationToken);
        }

        /// <summary>
        /// Loads the Four Books and the Five Classics.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadFourBooksFiveClassicsAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (FourBooksFiveClassics != null && !force) return FourBooksFiveClassics;
            return FourBooksFiveClassics = await LoadSingleFile("sishuwujing", new[]
            {
                "daxue.json",
                "mengzi.json",
                "zhongyong.json"
            }, null, cancellationToken);
        }

        /// <summary>
        /// Loads the primate school.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadPrimateSchoolAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (PrimateSchool != null && !force) return PrimateSchool;
            PrimateSchool = await LoadSingleFile("mengxue", new[]
            {
                "baijiaxing.json",
                "sanzijing-new.json",
                "qianziwen.json",
                "zhuzijiaxun.json"
            }, null, cancellationToken);
            await slim.WaitAsync(cancellationToken);
            try
            {
                var file = GetFileInfo(GetRootDirectory(), "mengxue", "tangshisanbaishou.json");
                if (file != null)
                {
                    using var stream = file.OpenRead();
                    var tangPoems300 = await JsonSerializer.DeserializeAsync<TitledContentContainer<IEnumerable<ContentContainer<IEnumerable<PoetryModel>>>>>(stream, null, cancellationToken);
                    var col = tangPoems300?.Content?.Where(ele => ele?.Content != null)?.SelectMany(ele => ele.Content);
                    if (col != null) tangPoetry300 = new PoetryFileInfo(file, col);
                }

                file = GetFileInfo(GetRootDirectory(), "mengxue", "dizigui.json");
                if (file != null)
                {
                    using var stream = file.OpenRead();
                    var dzg = await JsonSerializer.DeserializeAsync<TitledContentContainer<IEnumerable<PoetryModel>>>(stream, null, cancellationToken);
                    if (dzg?.Content != null)
                    {
                        var col = dzg.Content.Where(ele => ele != null).Select(ele =>
                        {
                            ele.Title = $"{dzg.Title}·{ele.Chapter}";
                            ele.Chapter = null;
                            ele.Reorganize();
                            return ele;
                        });
                        PrimateSchool.Add(new PoetryFileInfo(file, col));
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (System.Security.SecurityException)
            {
            }
            catch (JsonException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (ArgumentException)
            {
            }
            finally
            {
                slim.Release();
            }

            return PrimateSchool;
        }

        /// <summary>
        /// Loads the Caocao Anthology.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The poetry collection.</returns>
        public static async Task<PoetryCollection> LoadCaocaoAnthologyAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            if (CaocaoAnthology != null && !force) return CaocaoAnthology;
            return CaocaoAnthology = await LoadSingleFile("caocaoshiji", "caocao.json", ele => ele.Author = "曹操", cancellationToken);
        }

        /// <summary>
        /// Loads files.
        /// </summary>
        /// <param name="force">true if force to load (even if it exists); otherwise, false.</param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>The async task.</returns>
        public static async Task LoadFilesAsync(bool force = false, CancellationToken cancellationToken = default)
        {
            await LoadPrimateSchoolAsync(force, cancellationToken);
            await LoadTangPoetryAsync(force, cancellationToken);
            await LoadSongPoetryAsync(force, cancellationToken);
            await LoadSongLyricsAsync(force, cancellationToken);
            await LoadYuanVersesAsync(force, cancellationToken);
            await LoadBookOfSongsAsync(force, cancellationToken);
            await LoadFourBooksFiveClassicsAsync(force, cancellationToken);
            await LoadConfucianAnalectsAsync(force, cancellationToken);
            await LoadChuSongsAsync(force, cancellationToken);
            await LoadHuajianAnthologyAsync(force, cancellationToken);
            await LoadSouthernTangPoetryAsync(force, cancellationToken);
            await LoadCaocaoAnthologyAsync(force, cancellationToken);
        }

        internal static FileInfo GetFileInfo(DirectoryInfo rootPath, string folder, string fileName)
        {
            rootPath = GetDirectoryInfo(rootPath, folder);
            if (rootPath == null || !rootPath.Exists) return null;
            return rootPath.EnumerateFiles(fileName).FirstOrDefault();
        }

        internal static DirectoryInfo GetDirectoryInfo(DirectoryInfo rootPath, string folder)
        {
            if (rootPath == null) return null;
            if (string.IsNullOrWhiteSpace(folder)) return rootPath;
            return rootPath.EnumerateDirectories(folder).FirstOrDefault();
        }

        internal static DirectoryInfo GetRootDirectory()
        {
            if (RootPath != null) return RootPath;
            try
            {
                var dir = Directory.Exists("chinese-poetry")
                    ? new DirectoryInfo("chinese-poetry")
                    : Directory.Exists("../chinese-poetry")
                        ? new DirectoryInfo("../chinese-poetry")
                        : Directory.Exists("../../chinese-poetry")
                            ? new DirectoryInfo("../../chinese-poetry")
                            : Directory.Exists("../../../chinese-poetry")
                                ? new DirectoryInfo("../../../chinese-poetry")
                                : new DirectoryInfo("../../../../chinese-poetry");
                if (dir.Exists) return dir;
            }
            catch (IOException)
            {
            }
            catch (System.Security.SecurityException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (NotSupportedException)
            {
            }

            if (hasInit) return null;
            hasInit = true;
            if (File.Exists("chinese-poetry.zip"))
            {
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory("chinese-poetry.zip", "chinese-poetry");
                    if (Directory.Exists("chinese-poetry\\chinese-poetry")) return new DirectoryInfo("chinese-poetry\\chinese-poetry");
                    return new DirectoryInfo("chinese-poetry");
                }
                catch (IOException)
                {
                }
                catch (System.Security.SecurityException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (InvalidDataException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (NullReferenceException)
                {
                }
            }

            return null;
        }

        internal static bool Contains(string s, string q)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
#if NETCOREAPP3_1_OR_GREATER
            return s.Contains(q, StringComparison.CurrentCultureIgnoreCase);
#else
            return s.Contains(q);
#endif
        }

        internal static bool Contains(IEnumerable<string> col, string q)
        {
            return col.Any(ele => !string.IsNullOrWhiteSpace(ele) && ele.Equals(q, StringComparison.CurrentCultureIgnoreCase));
        }

        internal static bool Contains(string s, IEnumerable<string> q)
        {
            return !string.IsNullOrWhiteSpace(s) && q?.Any(ele => Contains(s, ele)) == true;
        }

        private static Task<PoetryCollection> ParseFileMenuAsync(DirectoryInfo rootPath, string folder, string fileName, CancellationToken cancellationToken = default)
        {
            return PoetryCollection.ParseAsync(GetFileInfo(rootPath, folder, fileName), cancellationToken);
        }

        private static async Task<PoetryCollection> LoadSingleFile(string folder, string fileName, Action<PoetryModel> handler, CancellationToken cancellationToken = default)
        {
            await slim.WaitAsync(cancellationToken);
            var r = new PoetryCollection();
            try
            {
                var rootPath = GetRootDirectory();
                await r.LoadFileAsync(GetFileInfo(rootPath, folder, fileName), handler, true, cancellationToken);
                return r;
            }
            catch (JsonException)
            {
                return r;
            }
            finally
            {
                slim.Release();
            }
        }

        private static async Task<PoetryCollection> LoadSingleFile(string folder, IEnumerable<string> fileNames, Action<PoetryModel> handler, CancellationToken cancellationToken = default)
        {
            await slim.WaitAsync(cancellationToken);
            try
            {
                var r = new PoetryCollection();
                var rootPath = GetRootDirectory();
                foreach (var fileName in fileNames)
                {
                    await r.LoadFileAsync(GetFileInfo(rootPath, folder, fileName), handler, true, cancellationToken);
                }

                return r;
            }
            finally
            {
                slim.Release();
            }
        }
    }
}
