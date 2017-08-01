using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    public class ConditionalNode : Node
    {
        public Action AttachedAction { get { return mAction; } }
        public NodeLabelItem InputOutput { get; set; }
        public NodeLabelItem Condition1 { get; set; }
        public NodeLabelItem Condition2 { get; set; }
        public NodeLabelItem Condition3 { get; set; }

        private Action mAction;

        public ConditionalNode(string title) : base(title)
        {
            InputOutput = new NodeLabelItem("", true, true) { Tag = "ActionInOut" };
            AddItem(InputOutput);

            Condition1 = new NodeLabelItem("Condition 1", true, false) { Tag = "ActionInOut" };
            AddItem(Condition1);

            Condition2 = new NodeLabelItem("Condition 2", true, false) { Tag = "ActionInOut" };
            AddItem(Condition2);

            Condition3 = new NodeLabelItem("Condition 3", true, false) { Tag = "ActionInOut" };
            AddItem(Condition3);
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

        public void ProcessActionNodeConnect(ActionNode actionNode)
        {
            mAction = actionNode.AttachedAction;
            Actor newParent = null;

            foreach (NodeConnection connection in this.Connections)
            {
                if (connection.From.Node == this)
                    continue;

                if (connection.To.Item.GetType() == typeof(NodeLabelItem))
                {
                    NodeLabelItem label = connection.To.Item as NodeLabelItem;

                    IConditional conditional = null;

                    switch (connection.From.Node)
                    {
                        case ActorNode actorNode:
                            conditional = actorNode.AttatchedActor;
                            break;
                        case ActionNode actNode:
                            conditional = actNode.AttachedAction;
                            break;
                        case ConditionalNode condNode:
                            break;
                        default:
                            throw new System.Exception("Unknown node type in ProcessActionNodeConnect for ConditionalNode!");
                    }

                    switch(label.Text)
                    {
                        case "Condition 1":
                            mAction.Conditions[0] = conditional;
                            break;
                        case "Condition 2":
                            mAction.Conditions[1] = conditional;
                            break;
                        case "Condition 3":
                            mAction.Conditions[2] = conditional;
                            break;
                        case "":
                            Action parentSource = conditional as Action;
                            newParent = parentSource.ParentActor;
                            break;
                    }
                }
            }

            mAction.ParentActor = newParent;
            mAction.ParentActor.AddActionFromNodeRecursive(actionNode);
        }

        public void ProcessActionNodeDisconnect(ActionNode actionNode)
        {
            for (int i = 0; i < 3; i++)
            {
                mAction.Conditions[i] = null;
            }

            mAction.ParentActor.RemoveActionFromNodeRecursive(actionNode);
            mAction = null;
        }
    }
}
