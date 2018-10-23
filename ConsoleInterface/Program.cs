using DocumentSearchEngine;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleInterface
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Provide at least two arguments: <documents_database> <keywords_list> <n_results(optional)>");
                return;
            }
            var documentDbPath = args[0];
            var keywordsPath = args[1];
            if (!File.Exists(documentDbPath))
            {
                Console.Write($"Document database does not exist: {documentDbPath}");
                return;
            }
            if (!File.Exists(keywordsPath))
            {
                Console.Write($"Keywords list does not exist: {keywordsPath}");
                return;
            }
            if (args.Length == 2 || !Int32.TryParse(args[2], out int max))
            {
                max = 10;
            }

            var keywords = File.ReadAllLines(keywordsPath);

            var searchEngine = new SearchEngine(keywords, new DocumentSanitizer());
            foreach (var rawDocument in File.ReadAllText(documentDbPath).Replace("\r", "").Split("\n\n"))
            {
                searchEngine.AddDocument(rawDocument);
            }
            while (true)
            {
                Console.WriteLine("What are you looking for?");
                var query = Console.ReadLine();
                try
                {
                    var results = searchEngine.Search(query).Results.Take(max).Where(x => x.similarity > 0).ToList();
                    if (results.Count == 0)
                    {
                        Console.WriteLine("No documents retrieved for your query :(");
                    }
                    for (int i = 0; i < results.Count; i++)
                    {
                        var (document, similarity) = results[i];
                        Console.WriteLine($"{i + 1}. similarity: {similarity:f2}");
                        Console.WriteLine(document.RawContents);
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
