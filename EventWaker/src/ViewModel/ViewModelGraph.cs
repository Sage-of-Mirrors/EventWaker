using System.ComponentModel;
using Graph;
using Graph.Items;
using EventWaker.EventList;
using EventWaker.Nodes;

namespace EventWaker.ViewModel
{
    public partial class DataViewModel : INotifyPropertyChanged
    {
        private EventNode mEventNode;

        public void UpdateNodeView()
        {
            mEventNode = new EventNode(SelectedEvent);
            BuildGraph();
        }

        private void BuildGraph()
        {
            Graph.Clear();

            Graph.AddNode(mEventNode);
        }

        private void Graph_ConnectionAdded(object sender, AcceptNodeConnectionEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void Graph_ConnectionRemoved(object sender, NodeConnectionEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
