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

            PropertyConnector = new NodeLabelItem("", false, true) { Tag = 'p' };
            AddItem(PropertyConnector);
        }

        public void ProcessNodeConnect(NodeConnection connection)
        {
            switch (connection.To.Node)
            {
                case ActionNode actionNode:
                    ProcessActionNodeConnect(actionNode);
                    break;
                case DataPropertyNode dataPropNode:
                    ProcessDataPropertyNodeConnect(dataPropNode);
                    break;
                case ConditionalNode condNode:
                    ProcessConditionalNodeConnect(condNode, connection);
                    break;
                case EndNode endNode:
                    endNode.ProcessNodeConnection(connection);
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

        private void ProcessConditionalNodeConnect(ConditionalNode condNode, NodeConnection connection)
        {
            if (connection.To.Item.GetType() == typeof(NodeLabelItem))
            {
                NodeLabelItem item = connection.To.Item as NodeLabelItem;

                if (item.Text == "")
                {
                    AttachedAction.AddConditionalPropertyNodeFromActorRecursive(condNode);
                }
                else if (item.Text.Contains("Condition"))
                {
                    if (condNode.AttachedAction == null)
                        return;

                    IConditional newConditional = null;

                    switch (connection.From.Node)
                    {
                        case ActorNode actorNode:
                            newConditional = actorNode.AttatchedActor as IConditional;
                            break;
                        case ActionNode actionNode:
                            newConditional = actionNode.AttachedAction as IConditional;
                            break;
                    }

                    condNode.AttachedAction.AddConditionalPropertyNodeFromConditionals(newConditional, item);
                    connection.RenderColor = System.Drawing.Color.Firebrick;
                }
            }
        }

        public void ProcessNodeDisconnect(NodeConnection disconnection)
        {
            Node node = disconnection.To.Node;

            switch (node)
            {
                case ActionNode actionNode:
                    ProcessActionNodeDisconnect(actionNode);
                    break;
                case DataPropertyNode dataPropNode:
                    ProcessDataPropertyNodeDisconnect(dataPropNode);
                    break;
                case ConditionalNode condNode:
                    ProcessConditionalNodeDisconnect(condNode, disconnection);
                    break;
                case EndNode endNode:
                    endNode.ProcessNodeDisconnection(disconnection);
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

        private void ProcessConditionalNodeDisconnect(ConditionalNode condNode, NodeConnection disconnection)
        {
            if (condNode.AttachedAction == null)
                return;

            if (disconnection.To.Item.GetType() == typeof(NodeLabelItem))
            {
                NodeLabelItem item = disconnection.To.Item as NodeLabelItem;

                if (item.Text == "")
                    AttachedAction.RemoveConditionalPropertyNodeFromActorRecursive(condNode);
                else if (item.Text.Contains("Condition"))
                    condNode.AttachedAction.RemoveConditionalPropertyNodeFromConditionals(condNode, item);
            }
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
