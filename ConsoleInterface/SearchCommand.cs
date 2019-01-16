using DocumentSearchEngine;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Linq;

namespace ConsoleInterface
{
    internal class SearchCommand : ICommand
    {
        private readonly string documentDbPath;
        private readonly string keywordsPath;
        private readonly int numberOfResults;

        public SearchCommand(string documentDbPath, string keywordsPath, int numberOfResults)
        {
            this.documentDbPath = documentDbPath ?? throw new ArgumentNullException(nameof(documentDbPath));
            this.keywordsPath = keywordsPath ?? throw new ArgumentNullException(nameof(keywordsPath));
            this.numberOfResults = numberOfResults;
        }

        public static void Configure(CommandLineApplication app)
        {
            var documentDbArgument = app.Argument("[documentDbPath]", "Path to documents database.");
            var keywordsArgument = app.Argument("[keywordsPath]", "Path to file with keywords.");
            var nResultsOption = app.Option<int>(
                "-n <numberOfResults>",
                "Number of results to retrieve.",
                CommandOptionType.SingleValue);
            app.OnExecute(() =>
            {
                var max = nResultsOption.HasValue() ? nResultsOption.ParsedValue : 10;
                return new SearchCommand(documentDbArgument.Value, keywordsArgument.Value, max).Run();
            });
        }

        public int Run()
        {
            var keywords = File.ReadAllLines(this.keywordsPath);
            var searchEngine = new SearchEngine(keywords, new DocumentSanitizer());

            foreach (var rawDocument in File.ReadAllText(this.documentDbPath).Replace("\r", "").Split("\n\n"))
            {
                searchEngine.AddDocument(rawDocument);
            }
            while (true)
            {
                var query = Prompt.GetString("What are you looking for?", promptColor: ConsoleColor.Green);
                if (query?.Length == 0)
                {
                    break;
                }
                try
                {
                    var results = searchEngine.Search(query).Results.Take(this.numberOfResults).Where(x => x.similarity > 0).ToList();
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
                    return 1;
                }
            }
            return 0;
        }
    }
}
