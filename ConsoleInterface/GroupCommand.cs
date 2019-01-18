using DocumentSearchEngine;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleInterface
{
    public class GroupCommand : ICommand
    {
        private readonly string documentDbPath;
        private readonly string keywordsPath;

        public GroupCommand(string documentDbPath, string keywordsPath)
        {
            this.documentDbPath = documentDbPath ?? throw new ArgumentNullException(nameof(documentDbPath));
            this.keywordsPath = keywordsPath ?? throw new ArgumentNullException(nameof(keywordsPath));
        }

        public static void Configure(CommandLineApplication app)
        {
            var documentDbArgument = app.Argument("[documentDbPath]", "Path to documents database.");
            var keywordsArgument = app.Argument("[keywordsPath]", "Path to file with keywords.");

            app.OnExecute(() => new GroupCommand(documentDbArgument.Value, keywordsArgument.Value).Run());
        }

        public int Run()
        {
            var keywords = File.ReadAllLines(this.keywordsPath);
            var searchEngine = new SearchEngine(keywords, new DocumentWithGroupSanitizer());

            foreach (var rawDocument in File.ReadAllText(this.documentDbPath).Replace("\r", "").Split("\n\n"))
            {
                searchEngine.AddDocument(rawDocument);
            }
            try
            {
                var groups = searchEngine.Cluster();
                Console.WriteLine("Assigned group                  Original group");
                Console.WriteLine(new String('-', 50));
                foreach (var group in groups)
                {
                    foreach (var document in group.Value)
                    {
                        Console.WriteLine($"{group.Key, -30}  {document.Group}");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
