using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventWaker.EventList
{
    public interface IConditional
    {
        int Flag { get; set; }

        string ToFullPathString();
    }
}
