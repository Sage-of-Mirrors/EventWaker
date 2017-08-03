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
    class EndNode : Node
    {
        public NodeLabelItem EndCondition1 { get { return mEndCondition1; } set { mEndCondition1 = value; } }
        public NodeLabelItem EndCondition2 { get { return mEndCondition2; } set { mEndCondition2 = value; } }
        public NodeLabelItem EndCondition3 { get { return mEndCondition3; } set { mEndCondition3 = value; } }
        public Event attachedEvent { get { return mEvent; } }

        private NodeLabelItem mEndCondition1;
        private NodeLabelItem mEndCondition2;
        private NodeLabelItem mEndCondition3;

        private Event mEvent;

        public EndNode(string title) : base(title)
        {
        }

        public EndNode(Event ev) : base("End")
        {
            mEvent = ev;

            EndCondition1 = new NodeLabelItem("End Condition 1", true, false) { Tag = 1000 };
            AddItem(EndCondition1);

            EndCondition2 = new NodeLabelItem("End Condition 2", true, false) { Tag = 1000 };
            AddItem(EndCondition2);

            EndCondition3 = new NodeLabelItem("End Condition 3", true, false) { Tag = 1000 };
            AddItem(EndCondition3);
        }

        public void ProcessNodeConnection(NodeConnection connect)
        {
            if (!(connect.From.Node is IConditional))
            {
                throw new Exception("From node for EndNode connection wasn't IConditional!");
            }

            IConditional connectedCond = connect.From.Node as IConditional;

            if (connect.To.Item == EndCondition1)
                mEvent.AddEndConditionFromNode(connectedCond, 0);
            else if (connect.To.Item == EndCondition2)
                mEvent.AddEndConditionFromNode(connectedCond, 1);
            else if (connect.To.Item == EndCondition3)
                mEvent.AddEndConditionFromNode(connectedCond, 2);
        }

        public void ProcessNodeDisconnection(NodeConnection disconnect)
        {
            if (disconnect.To.Item == EndCondition1)
                mEvent.RemoveEndConditionFromNode(0);
            else if (disconnect.To.Item == EndCondition2)
                mEvent.RemoveEndConditionFromNode(1);
            else if (disconnect.To.Item == EndCondition3)
                mEvent.RemoveEndConditionFromNode(2);
        }
    }
}
