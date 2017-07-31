using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph;

namespace EventWaker.EventList
{
    public interface IConditional
    {
        int Flag { get; set; }
        Node NodeData { get; set; }

        string ToFullPathString();
    }
}
