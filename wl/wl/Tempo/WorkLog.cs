using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace wl.Tempo
{
    [DataContract]
    public class WorkLog
    {
        [DataMember(Name = "issueKey", IsRequired = true)]
        public string IssueKey { get; set; }

        public DateTime Start { get; set; }
        public TimeSpan TimeSpent { get; set; }

        [DataMember(Name = "startDate", IsRequired = true)]
        public string StartDate
        {
            get
            {
                return Start.ToString("yyyy-MM-dd");
            }
            set
            {
                Start = DateTime.ParseExact(value + " " + StartTime, "yyyy-MM-dd HH:mm:ss", System.Threading.Thread.CurrentThread.CurrentCulture);
            }
        }

        [DataMember(Name = "startTime", IsRequired = true)]
        public string StartTime
        {
            get
            {
                return Start.ToString("HH:mm:ss");
            }
            set
            {
                Start = DateTime.ParseExact(StartDate + " " + value, "yyyy-MM-dd HH:mm:ss", System.Threading.Thread.CurrentThread.CurrentCulture);
            }
        }

        [DataMember(Name = "timeSpentSeconds", IsRequired = true)]
        public int TimeSpentSeconds
        {
            get
            {
                return (int)TimeSpent.TotalSeconds;
            }
            set
            {
                TimeSpent = new TimeSpan(0, 0, value);
            }
        }


        [DataMember(Name = "description", IsRequired = true)]
        public string Description { get; set; }
        [DataMember(Name = "authorAccountId", IsRequired = true)]
        public string AuthorAccountId { get; set; }
        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        public Dictionary<string, string> Attributes { get; set; }
    }
}
