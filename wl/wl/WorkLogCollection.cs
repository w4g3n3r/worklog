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
    }
}
