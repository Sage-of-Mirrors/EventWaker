using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class ConditionalNode : Node
    {
        public Action AttachedAction { get { return mAction; } }
        public NodeLabelItem InputOutput { get; set; }
        public NodeLabelItem Condition1 { get; set; }
        public NodeLabelItem Condition2 { get; set; }
        public NodeLabelItem Condition3 { get; set; }

        private Action mAction;

        public ConditionalNode(string title) : base(title)
        {
        }

        public ConditionalNode(Action action) : base("Conditions")
        {
            mAction = action;

            InputOutput = new NodeLabelItem("", true, true) { Tag = "ActionInOut" };
            AddItem(InputOutput);

            Condition1 = new NodeLabelItem("Condition 1", true, false) { Tag = "ActionInOut" };
            AddItem(Condition1);

            Condition2 = new NodeLabelItem("Condition 2", true, false) { Tag = "ActionInOut" };
            AddItem(Condition2);

            Condition3 = new NodeLabelItem("Condition 3", true, false) { Tag = "ActionInOut" };
            AddItem(Condition3);
        }
    }
}
