using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wl
{
    public class WorkLog
    {
        private static Regex pattern = new Regex(@"^(\d{4}(?:-\d{2}){2}\s\d{2}(?::\d{2}){2}) ?(\[[A-Z]+(:|\-)\s?\d+\])?\s?(.*)");
        private static Regex taskPattern = new Regex(@"\[(\w+)(:|\-) {0,1}(\d+)\]");

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public int TaskId { get; set; }
        public string Message { get; set; }
        public string Project { get; set; }

        public double Minutes
        {
            get
            {
                if (Begin == null || End == null) return 0;

                return Math.Round(End.Subtract(Begin).TotalMinutes, 2);
            }
        }

        public override string ToString()
        {
            return string.Format("{0,10:t} ({3,3} min) {1,8} {2}",
                Begin,
                TaskId > 0 ? string.Join("-", Project, TaskId.ToString()) : "        ",
                Message,
                Minutes);
        }

        public static WorkLog FromString(string value)
        {
            if (!pattern.IsMatch(value)) return null;

            WorkLog wl = new WorkLog();

            var match = pattern.Match(value);

            var time = match.Groups[1].Value;
            var task = match.Groups[2].Value;
            var message = match.Groups[4].Value;

            DateTime begin;
            if (!string.IsNullOrEmpty(time) && DateTime.TryParse(time, out begin))
            {
                wl.Begin = begin;
            }
            
            int taskId;
            if (!string.IsNullOrEmpty(task) && TryParseTask(task, out taskId, out string project))
            {
                wl.TaskId = taskId;
                wl.Project = project;
            }

            wl.Message = $"{task} {message}";

            return wl;
        }

        private static bool TryParseTask(string task, out int taskId, out string project)
        {
            taskId = 0;
            project = string.Empty;

            if (!taskPattern.IsMatch(task)) return false;

            var match = taskPattern.Match(task);

            project = match.Groups[1].Value;

            if (!int.TryParse(match.Groups[3].Value, out taskId))
                return false;

            return true;
        }
    }
}
