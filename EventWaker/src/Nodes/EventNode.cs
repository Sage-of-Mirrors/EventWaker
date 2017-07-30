using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    public class EventNode : Node
    {
        private Event mEvent;

        public EventNode(string title) : base(title)
        {
        }

        public EventNode(Event ev) : base(ev.Name)
        {
            mEvent = ev;

            NodeTextBoxItem nameBox = new NodeTextBoxItem(mEvent.Name);
            nameBox.TextChanged += NameBox_TextChanged;
            AddItem(nameBox);
            NodeNumericSliderItem priorityBox = new NodeNumericSliderItem("Priority: ", 100, 0, 0, 100, mEvent.Priority, false, false);
            priorityBox.ValueChanged += PriorityBox_ValueChanged;
            AddItem(priorityBox);
            NodeCheckboxItem jingleBox = new NodeCheckboxItem("Play Jingle", true, false);
            AddItem(jingleBox);
        }

        private void PriorityBox_ValueChanged(object sender, NodeItemEventArgs e)
        {
            if (sender.GetType() == typeof(NodeNumericSliderItem))
            {
                NodeNumericSliderItem num = sender as NodeNumericSliderItem;
                num.Value = (int)num.Value;
                mEvent.Priority = (int)num.Value;
            }
        }

        private void NameBox_TextChanged(object sender, AcceptNodeTextChangedEventArgs e)
        {
            mEvent.Name = e.Text;
        }
    }
}
