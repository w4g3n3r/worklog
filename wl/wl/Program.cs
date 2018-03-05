using Mono.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wl
{
    class Program
    {
        enum Status
        {
            Posted,
            Failed,
            Skipped
        }

        static void Main(string[] args)
        {
            var logFilePaths = new List<string>();
            bool calculateOnly = false;
            bool showHelp = false;
            string ontimeUrl = ConfigurationManager.AppSettings["Url"];
            string ontimeClientId = ConfigurationManager.AppSettings["ClientId"];
            string ontimeClientSecret = ConfigurationManager.AppSettings["Secret"];
            string ontimeUserName = ConfigurationManager.AppSettings["Username"];
            string ontimePassword = ConfigurationManager.AppSettings["Password"];

            Console.OutputEncoding = Encoding.UTF8;

            var p = new OptionSet()
            {
                { "l|log=", "The path to a log file to parse. Multiple -l options can be specified on the command line.",
                    v => logFilePaths.Add(v) },

                { "c|calculate", "Calculate hours only. Do not post work logs to OnTime.",
                    v => calculateOnly = (v != null) },

                { "u|username=", "Your OnTime user name.",
                    v => ontimeUserName = v },

                { "p|password=", "Your OnTime password.",
                    v => ontimePassword = v },

                { "d|url=", "The url to your OnTime install.",
                    v => ontimeUrl = v },

                { "i|clientId=", "Your ontime client ID.",
                    v => ontimeClientId = v },

                { "s|secret=", "Your ontime client secret.",
                    v => ontimeClientSecret = v },

                { "h|help", "Show this message.",
                    v => showHelp = (v != null) }
            };

            try
            {
                p.Parse(args);
                if (!showHelp && !logFilePaths.Any())
                    throw new OptionException("At least one log is required.", "log");
            }
            catch (OptionException ex)
            {
                Console.Write("wl: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine();
                ShowHelp(p);
                return;
            }

            if (showHelp)
            {
                ShowHelp(p);
                return;
            }

            OnTime ot = new OnTime(ontimeUrl, ontimeClientId, ontimeClientSecret);
            if (!calculateOnly)
            {
                ot.Login(ontimeUserName, ontimePassword);
            }

            foreach (var logFilePath in logFilePaths)
            {
                if (File.Exists(logFilePath))
                {
                    var logs = GetWorklogs(logFilePath);
                    PostWorklogs(ot, logs, calculateOnly);
                }
            }
        }

        static WorkLogCollection GetWorklogs(string logFilePath)
        {
            var logs = new WorkLogCollection();

            using (var wr = new WorkLogReader(logFilePath))
            {
                while (!wr.EndOfStream)
                {
                    var log = wr.ReadWorkLog();

                    if (log != null) logs.Add(log);
                }
            }

            logs.Remove(logs.Last()); // The last worklog is just an end time.
            return logs;
        }

        static void PostWorklogs(OnTime service, WorkLogCollection logs, bool calculateOnly)
        {
            Console.WriteLine("Worklogs:");
            foreach (var log in logs)
            {
                var logText = log.ToString();
                var width = 80;
                try
                {
                    width = Console.BufferWidth - 1;
                }
                catch { }
                var status = Status.Skipped;

                if (logText.Length > width) logText = string.Concat(logText.Substring(0, width - 3), "...");

                Console.Write(logText);

                if (!calculateOnly)
                {
                    status = service.CreateWorkLog(log) ? Status.Posted : Status.Failed;
                }

                try
                {
                    WriteStatus(status);
                }
                catch 
                {
                    Console.WriteLine();
                }
            }

            ShowSummary(logs);
        }

        static void WriteStatus(Status status)
        {
            Console.CursorLeft = 0;
            switch (status)
            {
                case Status.Failed: Console.Write('X'); break;
                case Status.Posted: Console.Write('√'); break;
                case Status.Skipped: Console.Write('-'); break;
            }
            Console.CursorLeft = Console.BufferWidth - 1;
            Console.WriteLine();
        }

        static void ShowSummary(WorkLogCollection logs)
        {
            var totalCount = logs.Count;
            var totalDuration = TimeSpan.FromMinutes(logs.Where(l => l.Type != WorkLogType.Empty).Sum(l => l.Minutes));

            var groups = logs
                .GroupBy(l => l.Type)
                .Select(g => new 
                { 
                    Type = g.Key, 
                    Count = g.Count(),
                    Percentage = (double)(g.Sum(l => l.Minutes) / totalDuration.TotalMinutes),
                    Duration = TimeSpan.FromMinutes(g.Sum(l => l.Minutes))
                });

            Console.WriteLine("Summary:");
            groups.ToList().ForEach(t => Console.WriteLine("{0,10} {2,3}: {1:g} {3,7:p1}",
                Enum.GetName(typeof(WorkLogType), t.Type),
                t.Duration,
                t.Count,
                t.Percentage));

            Console.WriteLine("{0,10} {2,3}: {1:g}",
                "Total",
                totalDuration,
                totalCount);
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: wl [OPTIONS] -l=<path to work log>");
            Console.WriteLine("Parses a work log file and posts worklogs to OnTime for the tasks contained within.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
