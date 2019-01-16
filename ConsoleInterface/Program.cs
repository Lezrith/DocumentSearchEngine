using McMaster.Extensions.CommandLineUtils;

namespace ConsoleInterface
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "DocumentSearchEngine";
            app.HelpOption("-?|-h|--help");

            app.Command("search", SearchCommand.Configure);

            return app.Execute(args);
        }
    }
}
