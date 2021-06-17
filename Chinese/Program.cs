using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Net;
using Trivial.Text;

namespace Poetry.Chinese
{
    class Program
    {
#if NET5_0_OR_GREATER
        private static bool enableConvert = true;
#endif

        static async Task Main(string[] args)
        {
            Console.WriteLine("Chinese Poetry");
            if (PoetryPool.GetRootDirectory()?.Exists != true)
            {
                Console.WriteLine();
                Console.WriteLine("Oops, No data found!");
                Console.WriteLine();
                Console.WriteLine("You can follow these steps to get data.");
                Console.WriteLine("1.  Download the compress file from following URL.");
                Console.WriteLine("    https://github.com/chinese-poetry/chinese-poetry/archive/refs/heads/master.zip");
                Console.WriteLine("2.  Unzip its content into the directory that this app is in.");
                try
                {
                    var file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
                    if (file != null) Console.WriteLine("    " + file.DirectoryName);
                }
                catch (System.Security.SecurityException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (IOException)
                {
                }
                catch (ArgumentException)
                {
                }

                Console.WriteLine();
                return;
            }

            var task = PoetryPool.LoadFilesAsync();
            Console.WriteLine();

            var noArgs = args.Length < 1;
            if (noArgs)
            {
                GetHelp();
                Console.WriteLine();
                Console.Write("> ");
                var input = Console.ReadLine();
                args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }

            while (args.Length > 0)
            {
                var q = args.Length > 1 ? args[1] : null;
                switch (args[0].Replace("-", string.Empty).ToLowerInvariant())
                {
                    case "bye":
                    case "quit":
                    case "exit":
                    case "esc":
                    case "close":
                    case "off":
                    case "goodbye":
                    case "x":
                    case "退出":
                    case "关闭":
                        return;
                    case "ts":
                    case "tang":
                    case "tangshi":
                    case "tangpoetry":
                    case "唐诗":
                        if (q == "300" && PoetryPool.tangPoetry300 != null)
                        {
                            Console.WriteLine(string.Join(Environment.NewLine, PoetryPool.tangPoetry300.EnumerableBasicInfo().Select(ele => $"{ele.FullTitle} \t{ele.Author}")));
                            Console.WriteLine();
                            break;
                        }

                        await WriteAsync(PoetryPool.LoadTangPoetryAsync, q);
                        break;
                    case "ntc":
                    case "nantang":
                    case "nantangci":
                    case "southerntangpoetry":
                    case "南唐词":
                        await WriteAsync(PoetryPool.LoadSouthernTangPoetryAsync, q);
                        break;
                    case "hjj":
                    case "huajianji":
                    case "huajiananthology":
                    case "花间集":
                        await WriteAsync(PoetryPool.LoadHuajianAnthologyAsync, q);
                        break;
                    case "ss":
                    case "songshi":
                    case "songpoetry":
                    case "宋诗":
                        await WriteAsync(PoetryPool.LoadSongPoetryAsync, q);
                        break;
                    case "sc":
                    case "songci":
                    case "songlyrics":
                    case "宋词":
                        await WriteAsync(PoetryPool.LoadSongLyricsAsync, q);
                        break;
                    case "yq":
                    case "yuan":
                    case "yuanqu":
                    case "yuanverses":
                    case "元曲":
                        await WriteAsync(PoetryPool.LoadYuanVersesAsync, q);
                        break;
                    case "ly":
                    case "lunyu":
                    case "confuciananalects":
                    case "论语":
                        await WriteAsync(PoetryPool.LoadConfucianAnalectsAsync, q);
                        break;
                    case "sj":
                    case "shijing":
                    case "bookofsongs":
                    case "诗经":
                        await WriteAsync(PoetryPool.LoadBookOfSongsAsync, q);
                        break;
                    case "cc":
                    case "chuci":
                    case "songsofchu":
                    case "楚辞":
                        await WriteAsync(PoetryPool.LoadChuSongsAsync, q);
                        break;
                    case "sswj":
                    case "sishuwujing":
                    case "four-books-and-five-classics":
                    case "四书五经":
                        await WriteAsync(PoetryPool.LoadFourBooksFiveClassicsAsync, q);
                        break;
                    case "mx":
                    case "mengxue":
                    case "primateschool":
                    case "蒙学":
                        await WriteAsync(PoetryPool.LoadPrimateSchoolAsync, q);
                        break;
                    case "ccsj":
                    case "caocao":
                    case "caocaoshiji":
                    case "caocaoanthology":
                    case "曹操":
                    case "曹操诗集":
                        await WriteAsync(PoetryPool.LoadCaocaoAnthologyAsync, q);
                        break;
                    case "?":
                    case "help":
                    case "gethelp":
                    case "帮助":
                        GetHelp();
                        break;
                }

                if (!noArgs) return;
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) break;
                args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        static void GetHelp()
        {
            Console.WriteLine("USAGE:");
            Console.WriteLine("  Poetry.Chinese [type] [query]");
            Console.WriteLine();
            Console.WriteLine("where");
            Console.WriteLine("  type:");
            Console.WriteLine("    tangshi \t\tSearch for the Tang Poetry (唐诗).");
            Console.WriteLine("    nantangci \t\tSearch for the Southern Tang Poetry (南唐词).");
            Console.WriteLine("    huajianji \t\tSearch for the Huajian Anthology (花间集).");
            Console.WriteLine("    songshi \t\tSearch for the Song Poetry (宋诗).");
            Console.WriteLine("    songci  \t\tSearch for the Song Lyrics (宋词).");
            Console.WriteLine("    yuanqu  \t\tSearch for the Yuan Verses (元曲).");
            Console.WriteLine("    lunyu   \t\tSearch for the Confucian Analects (论语).");
            Console.WriteLine("    shijing \t\tSearch for the Book of Songs (诗经).");
            Console.WriteLine("    chuci   \t\tSearch for the Songs of Chu (楚辞).");
            Console.WriteLine("    sishuwujing \tSearch for the Four Books and the Five Classics (四书五经).");
            Console.WriteLine("    mengxue \t\tSearch for the Primate School (蒙学).");
            Console.WriteLine("    caocao \t\tSearch for the Caocao Anthology (曹操诗集).");
            Console.WriteLine();
            Console.WriteLine("  query \t\tThe query string of name and author to search.");
        }

        static async Task<string> WriteAsync(Func<bool, CancellationToken, Task<PoetryCollection>> load, string q, CancellationToken cancellationToken = default)
        {
            IEnumerable<PoetryModel> col;
            var task = load(false, cancellationToken);
            if (!task.Wait(1000, cancellationToken))
            {
                Console.WriteLine("Loading...");
            }

            var menu = await task;
            if (string.IsNullOrEmpty(q))
            {
                var authors = menu.GetAuthors().Where(ele => ele != null).ToList();
                if (authors.Count == 1 && !string.IsNullOrWhiteSpace(authors[0]?.Name))
                {
                    Console.WriteLine(authors[0].Name);
                    if (!string.IsNullOrWhiteSpace(authors[0].Introduction))
                        Console.WriteLine(authors[0].Introduction);
                    Console.WriteLine();
                }

                if (menu.Count() < 320)
                {
                    var titles = menu.EnumerableBasicInfo().Where(ele => !string.IsNullOrWhiteSpace(ele?.FullTitle)).Select(ele => $"{ele.FullTitle} \t{ele.Author}");
                    var str = string.Join(Environment.NewLine, titles);
                    Console.WriteLine(str);
                    Console.WriteLine();
                    return str;
                }
                else if (authors.Count > 1)
                {
                    Console.WriteLine("{0} authors", authors.Count);
                    var titles = authors.Select(ele => ele?.Name).Where(ele => !string.IsNullOrWhiteSpace(ele));
                    var j = 0;
                    foreach (var s in titles)
                    {
                        j++;
                        Console.Write(s);
                        Console.Write(s.Length > 8 ? " \t" : "\t\t");
                        if (j % 4 == 0) Console.WriteLine();
                    }

                    if (j % 4 > 0) Console.WriteLine();
                    Console.WriteLine();
                    return string.Join(Environment.NewLine, titles);
                }

                col = await menu.SearchAsync(e => true, cancellationToken);
            }
            else
            {
#if NET5_0_OR_GREATER
                if (enableConvert && OperatingSystem.IsWindows())
                {
                    var qc = new List<string> { q };
                    try
                    {
                        var q1 = Microsoft.VisualBasic.Strings.StrConv(q, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese);
                        var q2 = Microsoft.VisualBasic.Strings.StrConv(q, Microsoft.VisualBasic.VbStrConv.TraditionalChinese);
                        if (!string.IsNullOrWhiteSpace(q1) && !q1.Equals(q, StringComparison.OrdinalIgnoreCase)) qc.Add(q1);
                        if (!string.IsNullOrWhiteSpace(q2) && !q2.Equals(q, StringComparison.OrdinalIgnoreCase)) qc.Add(q2);
                    }
                    catch (ArgumentException)
                    {
                        enableConvert = false;
                    }

                    var author = menu.GetAuthorByName(qc).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(author?.Introduction))
                    {
                        Console.WriteLine(author.Introduction);
                        Console.WriteLine();
                    }

                    col = await menu.SearchAsync(qc, cancellationToken);
                }
                else
                {
                    col = await LoadModelsAsync(menu, q, cancellationToken);
                }
#else
                col = await LoadModelsAsync(menu, q, cancellationToken);
#endif
            }

            var i = 0;
            var list = col.ToList();
            if (list.Count > 200)
            {
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item.Author))
                        Console.WriteLine(item.FullTitle);
                    else
                        Console.WriteLine("{0} \t{1}", item.FullTitle, item.Author);
                    i++;
                }

                Console.WriteLine();
                Console.WriteLine("Total " + i);
                return string.Join(Environment.NewLine, list.Select(ele => ele.FullTitle));
            }

            if (list.Count == 0)
            {
                Console.WriteLine("Empty.");
                return string.Empty;
            }

            if (list.Count == 1)
            {
                var s = list[0]?.ToString() ?? string.Empty;
                Console.WriteLine(s);
                return s;
            }

            i = 0;
            foreach (var item in list)
            {
                if (item == null) continue;
                Console.WriteLine(item.ToString());
                Console.WriteLine();
                i++;
            }

            Console.WriteLine("Total " + i);
            return string.Join(Environment.NewLine + Environment.NewLine, list.Select(ele => ele.ToString()));
        }

        private static Task<IEnumerable<PoetryModel>> LoadModelsAsync(PoetryCollection menu, string q, CancellationToken cancellationToken = default)
        {
            var author = menu.GetAuthorByName(q).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(author?.Introduction))
            {
                Console.WriteLine(author.Introduction);
                Console.WriteLine();
            }

            return menu.SearchAsync(q, cancellationToken);
        }
    }
}
