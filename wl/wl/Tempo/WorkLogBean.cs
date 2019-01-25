using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace wl.Tempo
{
    [DataContract]
    public class WorkLogBean
    {
        public TimeSpan TimeSpent { get; set; }

        [DataMember(Name = "dateStarted")]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "author")]
        public Author Author { get; set; }

        [DataMember(Name = "issue")]
        public Issue Issue { get; set; }

        [DataMember(Name = "timeSpentSeconds")]
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
        
    }

    [DataContract]
    public class Author
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Issue
    {
        [DataMember(Name = "key")]
        public string Key { get; set; }
    }
}
