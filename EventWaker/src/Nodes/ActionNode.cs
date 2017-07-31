using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    public class ActionNode : Node
    {
        public NodeLabelItem LastConnector { get; set; }
        public NodeLabelItem PropertyConnector { get; set; }

        public NodeLabelItem Condition1Connector { get; set; }
        public NodeLabelItem Condition2Connector { get; set; }
        public NodeLabelItem Condition3Connector { get; set; }

        public Action AttachedAction { get { return mAction; } }

        private Action mAction;

        public ActionNode(string title) : base(title)
        {
        }

        public ActionNode(Action action) : base ("Action")
        {
            mAction = action;

            LastConnector = new NodeLabelItem("", true, true) { Tag = "ActionInOut" };
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

            PropertyConnector = new NodeLabelItem("", false, true) { Tag = 'p' };
            AddItem(PropertyConnector);
        }

        public void ProcessNodeConnect(Node node)
        {
            switch (node)
            {
                case ActionNode actionNode:
                    ProcessActionNodeConnect(actionNode);
                    break;
                case DataPropertyNode dataPropNode:
                    ProcessDataPropertyNodeConnect(dataPropNode);
                    break;
            }
        }

        private void ProcessActionNodeConnect(ActionNode actionNode)
        {
            if (mAction.ParentActor == null)
                return;

            mAction.ParentActor.AddActionFromNodeRecursive(actionNode);
        }

        private void ProcessDataPropertyNodeConnect(DataPropertyNode dataPropNode)
        {
            AttachedAction.AddDataPropertyFromNodeRecursive(dataPropNode);
        }

        public void ProcessNodeDisconnect(Node node)
        {
            switch (node)
            {
                case ActionNode actionNode:
                    ProcessActionNodeDisconnect(actionNode);
                    break;
                case DataPropertyNode dataPropNode:
                    ProcessDataPropertyNodeDisconnect(dataPropNode);
                    break;
            }
        }

        private void ProcessActionNodeDisconnect(ActionNode actionNode)
        {
            // No special processing needed, since we don't have a parent Actor
            if (AttachedAction.ParentActor == null)
                return;

            AttachedAction.ParentActor.RemoveActionFromNodeRecursive(actionNode);
        }

        private void ProcessDataPropertyNodeDisconnect(DataPropertyNode dataPropNode)
        {
            AttachedAction.RemoveDataPropertyFromNodeRecursive(dataPropNode);
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
