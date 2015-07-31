using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wl
{
    public enum WorkLogType
    {
        Empty = 0,
        Features = 1,
        Defects = 2,
        Tasks = 3,
        Incidents = 4
    }

    public class WorkLog
    {
        private static Regex pattern = new Regex(@"(\d{1,2}:\d{1,2} (?:AM|PM) \d{1,2}\/\d{1,2}\/\d{4}) ?(\[axo[dft]: ?\d+\])? ?(.*)");
        private static Regex taskPattern = new Regex(@"\[(\w+) ?: ?(\d+)\]");

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public int TaskId { get; set; }
        public string Message { get; set; }
        public WorkLogType Type { get; set; }

        public double Minutes
        {
            get
            {
                if (Begin == null || End == null) return 0;

                return End.Subtract(Begin).TotalMinutes;
            }
        }

        public override string ToString()
        {

            return string.Format("{0,10:t} ({4,3} min) {2,4}:{1,4} {3}",
                Begin,
                TaskId > 0 ? TaskId.ToString() : "----",
                TaskTypeString(Type),
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
            var message = match.Groups[3].Value;

            DateTime begin;
            if (!string.IsNullOrEmpty(time) && DateTime.TryParse(time, out begin))
            {
                wl.Begin = begin;
            }

            WorkLogType type;
            int taskId;
            if (!string.IsNullOrEmpty(task) && TryParseTask(task, out taskId, out type))
            {
                wl.TaskId = taskId;
                wl.Type = type;
            }

            wl.Message = message;

            return wl;
        }

        private static bool TryParseTask(string task, out int taskId, out WorkLogType type)
        {
            taskId = 0;
            type = WorkLogType.Empty;

            if (!taskPattern.IsMatch(task)) return false;

            var match = taskPattern.Match(task);

            var typeText = match.Groups[1].Value;
            type = new WorkLog().TaskTypeEnum(typeText);

            if (!int.TryParse(match.Groups[2].Value, out taskId))
                return false;

            return true;
        }

        private string TaskTypeString(WorkLogType type)
        {
            switch (type)
            {
                case WorkLogType.Defects: return "AXOD";
                case WorkLogType.Features: return "AXOF";
                case WorkLogType.Incidents: return "AXOI";
                case WorkLogType.Tasks: return "AXOT";
                default: return "----";
            }
        }

        private WorkLogType TaskTypeEnum(string type)
        {
            type = type.ToUpper();
            switch (type)
            {
                case "AXOD": return WorkLogType.Defects;
                case "AXOF": return WorkLogType.Features;
                case "AXOI": return WorkLogType.Incidents;
                case "AXOT": return WorkLogType.Tasks;
                default: return WorkLogType.Empty;
            }
        }
    }
}
