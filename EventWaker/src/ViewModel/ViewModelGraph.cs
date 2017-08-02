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
            mConditionalNodes.Clear();
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

                if (act.Actions.Count != 0)
                {
                    if (act.Actions[0].Conditions[0] != null)
                    {
                        ConditionalNode condNode = new ConditionalNode(act.Actions[0]);
                        mConditionalNodes.Add(condNode);
                        condNode.Location = nodeLocation;
                        Graph.AddNode(condNode);

                        nodeLocation.X += 150 + condNode.Bounds.Width;
                    }
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
            RelocateConditionalNodes();

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

        private void RelocateConditionalNodes()
        {
            foreach (ConditionalNode cond in mConditionalNodes)
            {
                if (cond.Condition1.Input.HasConnection)
                {
                    foreach (NodeConnection connection in cond.Connections)
                    {
                        if (connection.To.Item == cond.Condition1)
                        {
                            SetConditionalNodeLocation(connection);
                        }
                    }
                }
                if (cond.Condition2.Input.HasConnection)
                {
                    foreach (NodeConnection connection in cond.Connections)
                    {
                        if (connection.To.Item == cond.Condition2)
                        {
                            SetConditionalNodeLocation(connection);
                        }
                    }
                }
                if (cond.Condition3.Input.HasConnection)
                {
                    foreach (NodeConnection connection in cond.Connections)
                    {
                        if (connection.To.Item == cond.Condition3)
                        {
                            SetConditionalNodeLocation(connection);
                        }
                    }
                }
            }
        }

        private void SetConditionalNodeLocation(NodeConnection connection)
        {
            Node condition = connection.From.Node;
            ConditionalNode thisConditional = connection.To.Node as ConditionalNode;
            float highestx = GetLastConditionHighestXCoord(condition, condition.Location.X);
            float minDistance = 50f;
            float shiftAmount = 200f;

            if (thisConditional.Location.X - highestx < 0f)
            {
                PointF thisConditionalOriginal = thisConditional.Location;
                //condition.Location = new PointF(thisConditional.Location.X - 200, condition.Location.Y);
                thisConditional.Location = new PointF(highestx + condition.Bounds.Width + shiftAmount, thisConditional.Location.Y);

                float deltaX = System.Math.Abs(thisConditionalOriginal.X - thisConditional.Location.X);

                foreach (NodeConnection nodeConnect in condition.Connections)
                {
                    if (nodeConnect.From.Node == condition && nodeConnect.To.Node != thisConditional)
                    {
                        //PropogateNodeLocationShift(nodeConnect.To.Node, condition.Location, shiftAmount);
                    }
                }

                foreach (NodeConnection nodeConnect in thisConditional.Connections)
                {
                    if (nodeConnect.From.Node == thisConditional)
                    {
                        PropogateNodeLocationShift(nodeConnect.To.Node, thisConditional.Location, deltaX);
                    }
                }
            }

            /*
            float highestx = GetLastConditionHighestXCoord(condition, condition.Location.X);
            ConditionalNode thisConditional = connection.To.Node as ConditionalNode;
            float shiftAmount = 200f;

            //condition.Location = new PointF(thisConditional.Location.X - 200, condition.Location.Y);
            thisConditional.Location = new PointF(highestx + condition.Bounds.Width + shiftAmount, thisConditional.Location.Y);

            foreach (NodeConnection nodeConnect in condition.Connections)
            {
                if (nodeConnect.From.Node == condition && nodeConnect.To.Node != thisConditional)
                {
                    //PropogateNodeLocationShift(nodeConnect.To.Node, condition.Location, shiftAmount);
                }
            }

            foreach (NodeConnection nodeConnect in thisConditional.Connections)
            {
                if (nodeConnect.From.Node == thisConditional)
                {
                    PropogateNodeLocationShift(nodeConnect.To.Node, thisConditional.Location, shiftAmount);
                }
            }*/
        }

        private void PropogateNodeLocationShift(Node node, PointF location, float shiftAmount)
        {
            //float highestX = GetLastConditionHighestXCoord(node, node.Location.X);
            node.Location = new PointF(node.Location.X + shiftAmount, node.Location.Y);

            foreach (NodeConnection nodeConnect in node.Connections)
            {
                if (nodeConnect.From.Node == node)
                {
                    PropogateNodeLocationShift(nodeConnect.To.Node, node.Location, shiftAmount);
                }
            }
        }

        private float GetLastConditionHighestXCoord(Node node, float lastX)
        {
            foreach (NodeConnection connection in node.Connections)
            {
                if (connection.From.Node == node && connection.To.Node is DataPropertyNode)
                {
                    lastX = node.Location.X;
                    return GetLastConditionHighestXCoord(connection.To.Node, lastX);
                }
            }

            return lastX;
        }

        private void ConnectActionNodeToCondition(ActionNode node, ConditionalNode cond, int index)
        {
            NodeConnection result = null;

            switch (index)
            {
                case 0:
                    result = Graph.Connect(node.LastConnector, cond.Condition1);
                    break;
                case 1:
                    result = Graph.Connect(node.LastConnector, cond.Condition2);
                    break;
                case 2:
                    result = Graph.Connect(node.LastConnector, cond.Condition3);
                    break;
            }

            result.RenderColor = Color.Firebrick;
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
                    actionNode.ProcessNodeConnect(e.Connection);
                    break;
                case DataPropertyNode dataPropNode:
                    dataPropNode.ProcessPropertyNodeConnect(e.Connection.To.Node as DataPropertyNode);
                    break;
                case ConditionalNode condNode:
                    condNode.ProcessActionNodeConnect(e.Connection.To.Node as ActionNode);
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
                    actionNode.ProcessNodeDisconnect(e.Connection);
                    break;
                case DataPropertyNode dataPropNode:
                    dataPropNode.ProcessPropertyNodeDisconnect(e.To.Node as DataPropertyNode);
                    break;
                case ConditionalNode condNode:
                    condNode.ProcessActionNodeDisconnect(e.To.Node as ActionNode);
                    break;
            }
        }

        private void Graph_ConnectionRemoving(object sender, AcceptNodeConnectionEventArgs e)
        {
            if (mDisableConnectionUpdates)
                return;

            switch (e.Connection.From.Node)
            {
                case ActorNode actNode:
                    actNode.ProcessActionNodeDisconnect(e.Connection.To.Node as ActionNode);
                    break;
                case ActionNode actionNode:
                    actionNode.ProcessNodeDisconnect(e.Connection);
                    break;
                case DataPropertyNode dataPropNode:
                    dataPropNode.ProcessPropertyNodeDisconnect(e.Connection.To.Node as DataPropertyNode);
                    break;
                case ConditionalNode condNode:
                    condNode.ProcessActionNodeDisconnect(e.Connection.To.Node as ActionNode);
                    break;
            }
        }
    }
}
