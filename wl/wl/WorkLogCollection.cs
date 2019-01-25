using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wl
{
    public class WorkLogCollection : Collection<WorkLog>
    {
        protected override void InsertItem(int index, WorkLog item)
        {
            if (index > 0)
            {
                Items[index - 1].End = item.Begin;
            }

            base.InsertItem(index, item);
        }

        public IEnumerable<string> Errors
        {
            get
            {
                foreach(var wl in this)
                {
                    var time = wl.Begin.ToString("t");
                    var tag = string.Join("-", wl.Project, wl.TaskId);

                    var errorStart = $"Error for log {time} ({tag}): ";

                    if (wl.Minutes < 0)
                        yield return errorStart + "negative time";

                    if (wl.TaskId > 0 && string.IsNullOrEmpty(wl.Message))
                        yield return errorStart + "no comment";

                    if (wl.Minutes > 60 * 24)
                        yield return errorStart + "over a day";
                }
            }
        }
    }
}
