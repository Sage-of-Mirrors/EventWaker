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
                mNodes.Add(actorNode);
                actorNode.Location = nodeLocation;
                nodeLocation.X = 250 + actorNode.Bounds.Width;

                List<ActionNode> currentActionNodes = new List<ActionNode>();

                for (int i = 0; i < act.Actions.Count; i++)
                {
                    ActionNode actionNode = new ActionNode(act.Actions[i]);
                    act.Actions[i].NodeData = actionNode;
                    mNodes.Add(actionNode);
                    currentActionNodes.Add(actionNode);
                    actionNode.Location = nodeLocation;
                    nodeLocation.X += 150 + actionNode.Bounds.Width;

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

                    nodeLocation.Y = curY;
                }

                nodeLocation.X = 0;
                nodeLocation.Y += 280 + actorNode.Bounds.Height;
            }

            Graph.AddNodes(mNodes);
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
