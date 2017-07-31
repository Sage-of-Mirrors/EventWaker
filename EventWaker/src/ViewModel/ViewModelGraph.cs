using System.ComponentModel;
using System.Collections.Generic;
using Graph;
using Graph.Compatibility;
using Graph.Items;
using EventWaker.EventList;
using EventWaker.Nodes;
using System.Drawing;

namespace EventWaker.ViewModel
{
    public partial class DataViewModel : INotifyPropertyChanged
    {
        private EventNode mEventNode;
        private List<Node> mNodes;
        private List<ConditionalNode> mConditionalNodes;
        private bool mDisableConnectionUpdates;

        public void UpdateNodeView()
        {
            mDisableConnectionUpdates = true;

            mEventNode = new EventNode(SelectedEvent);
            BuildGraph();

            mDisableConnectionUpdates = false;
        }

        private void BuildGraph()
        {
            mNodes.Clear();
            Graph.Clear();

            Graph.AddNode(mEventNode);

            PointF nodeLocation = new PointF(0, 150);

            foreach (Actor act in SelectedEvent.Actors)
            {
                ActorNode actorNode = new ActorNode(act);
                act.NodeData = actorNode;

                actorNode.Location = nodeLocation;
                nodeLocation.X = 250 + actorNode.Bounds.Width;

                List<ActionNode> currentActionNodes = new List<ActionNode>();

                if (act.Actions[0].Conditions[0] != null)
                {
                    ConditionalNode condNode = new ConditionalNode(act.Actions[0]);
                    mConditionalNodes.Add(condNode);
                    condNode.Location = nodeLocation;
                    Graph.AddNode(condNode);

                    nodeLocation.X += 150 + condNode.Bounds.Width;
                }

                for (int i = 0; i < act.Actions.Count; i++)
                {
                    ActionNode actionNode = new ActionNode(act.Actions[i]);
                    act.Actions[i].NodeData = actionNode;
                    currentActionNodes.Add(actionNode);
                    actionNode.Location = nodeLocation;
                    nodeLocation.X += 150 + actionNode.Bounds.Width;

                    if (i + 1 < act.Actions.Count)
                    {
                        if (act.Actions[i + 1].Conditions[0] != null)
                        {
                            ConditionalNode condNode = new ConditionalNode(act.Actions[i + 1]);
                            mConditionalNodes.Add(condNode);
                            condNode.Location = nodeLocation;
                            Graph.AddNode(condNode);

                            nodeLocation.X += 150 + condNode.Bounds.Width;
                        }
                    }

                    List<DataPropertyNode> currentPropNodes = new List<DataPropertyNode>();
                    float curY = nodeLocation.Y;
                    nodeLocation.Y += 150;

                    for (int j = 0; j < act.Actions[i].Properties.Count; j++)
                    {
                        DataPropertyNode propNode = DataPropertyNode.GetPropertyNode(act.Actions[i].Properties[j]);
                        currentPropNodes.Add(propNode);
                        propNode.Location = nodeLocation;
                        nodeLocation.X += 150;

                        if (j == 0)
                            Graph.Connect(actionNode.PropertyConnector, propNode.LastNodeConnection);
                        else if (j >= 1)
                        {
                            Graph.Connect(currentPropNodes[j - 1].LastNodeConnection, currentPropNodes[j].LastNodeConnection);
                        }

                        mNodes.Add(propNode);
                    }

                    if (i == 0)
                    {
                        Graph.Connect(actorNode.ActionNodeConnector, actionNode.LastConnector);
                    }
                    else if (i >= 1)
                    {
                        Graph.Connect(currentActionNodes[i - 1].LastConnector, currentActionNodes[i].LastConnector);
                    }

                    mNodes.Add(actionNode);
                    nodeLocation.Y = curY;
                }

                mNodes.Add(actorNode);
                nodeLocation.X = 0;
                nodeLocation.Y += 280 + actorNode.Bounds.Height;
            }

            ConnectConditionalNodes();

            Graph.AddNodes(mNodes);
        }

        private void ConnectConditionalNodes()
        {
            foreach (ConditionalNode cond in mConditionalNodes)
            {
                Actor parentActor = cond.AttachedAction.ParentActor;
                ActionNode actionNodeData = cond.AttachedAction.NodeData as ActionNode;
                int condIndex = parentActor.Actions.IndexOf(cond.AttachedAction);

                if (condIndex - 1 < 0)
                {
                    ActorNode actorNodeData = parentActor.NodeData as ActorNode;
                    Graph.Disconnect(actionNodeData.LastConnector.Input.Connectors.GetEnumerator().Current);
                    Graph.Connect(actorNodeData.ActionNodeConnector, cond.InputOutput);
                    Graph.Connect(cond.InputOutput, actionNodeData.LastConnector);
                }
                else
                {
                    ActionNode lastActionNode = parentActor.Actions[condIndex - 1].NodeData as ActionNode;
                    NodeConnection relevantConnection = null;

                    foreach (NodeConnection connection in lastActionNode.Connections)
                    {
                        if (connection.From.Node == lastActionNode && connection.To.Node == actionNodeData)
                            relevantConnection = connection;
                    }

                    if (relevantConnection == null)
                        throw new System.Exception("Conditional connection was null!");

                    Graph.Disconnect(relevantConnection);
                    Graph.Connect(lastActionNode.LastConnector, cond.InputOutput);
                    Graph.Connect(cond.InputOutput, actionNodeData.LastConnector);
                }

                for (int i = 0; i < 3; i++)
                {
                    if (actionNodeData.AttachedAction.Conditions[i] == null)
                        continue;

                    switch (actionNodeData.AttachedAction.Conditions[i].NodeData)
                    {
                        case ActorNode actorNode:
                            break;
                        case ActionNode actionNode:
                            ConnectActionNodeToCondition(actionNode, cond, i);
                            break;
                    }
                }
            }
        }

        private void ConnectActionNodeToCondition(ActionNode node, ConditionalNode cond, int index)
        {
            switch (index)
            {
                case 0:
                    Graph.Connect(node.LastConnector, cond.Condition1);
                    break;
                case 1:
                    Graph.Connect(node.LastConnector, cond.Condition2);
                    break;
                case 2:
                    Graph.Connect(node.LastConnector, cond.Condition3);
                    break;
            }
        }

        private void AddNodeToGraph(Node node)
        {
            mNodes.Add(node);
            Graph.AddNode(node);
        }

        private void Graph_ConnectionAdded(object sender, AcceptNodeConnectionEventArgs e)
        {
            if (mDisableConnectionUpdates)
                return;

            switch (e.Connection.From.Node)
            {
                case ActorNode actNode:
                    actNode.ProcessActionNodeConnect(e.Connection.To.Node as ActionNode);
                    break;
                case ActionNode actionNode:
                    actionNode.ProcessNodeConnect(e.Connection.To.Node);
                    break;
                case DataPropertyNode dataPropNode:
                    dataPropNode.ProcessPropertyNodeConnect(e.Connection.To.Node as DataPropertyNode);
                    break;
            }
        }

        private void Graph_ConnectionRemoved(object sender, NodeConnectionEventArgs e)
        {
            if (mDisableConnectionUpdates)
                return;

            switch (e.From.Node)
            {
                case ActorNode actNode:
                    actNode.ProcessActionNodeDisconnect(e.To.Node as ActionNode);
                    break;
                case ActionNode actionNode:
                    actionNode.ProcessNodeDisconnect(e.To.Node);
                    break;
                case DataPropertyNode dataPropNode:
                    dataPropNode.ProcessPropertyNodeDisconnect(e.To.Node as DataPropertyNode);
                    break;
            }
        }
    }
}
