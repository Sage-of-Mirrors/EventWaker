using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class ActionNode : Node
    {
        public NodeLabelItem LastConnector { get; set; }
        public NodeLabelItem PropertyConnector { get; set; }

        public NodeLabelItem Condition1Connector { get; set; }
        public NodeLabelItem Condition2Connector { get; set; }
        public NodeLabelItem Condition3Connector { get; set; }

        private Action mAction;

        public ActionNode(string title) : base(title)
        {
        }

        public ActionNode(Action action) : base ("Action")
        {
            mAction = action;

            LastConnector = new NodeLabelItem("", true, true) { Tag = "ActorActionInput" };
            AddItem(LastConnector);

            NodeTextBoxItem nameBox = new NodeTextBoxItem(mAction.Name);
            nameBox.TextChanged += NameBox_TextChanged;
            AddItem(nameBox);

            Condition1Connector = new NodeLabelItem("Condition 1", true, false);
            AddItem(Condition1Connector);
            Condition2Connector = new NodeLabelItem("Condition 2", true, false);
            AddItem(Condition2Connector);
            Condition3Connector = new NodeLabelItem("Condition 3", true, false);
            AddItem(Condition3Connector);

            PropertyConnector = new NodeLabelItem("", false, true) { Tag = "ActionPropertyInput" };
            AddItem(PropertyConnector);
        }

        private void NameBox_TextChanged(object sender, AcceptNodeTextChangedEventArgs e)
        {
            // Names have a max of 31 characters + null terminator
            if (e.Text.Length <= 31)
                mAction.Name = e.Text;
            else
                e.Cancel = true;
        }
    }
}
