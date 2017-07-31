using System.ComponentModel;
using EventWaker.EventList;
using EventWaker.Nodes;

namespace EventWaker.ViewModel
{
    public partial class DataViewModel : INotifyPropertyChanged
    {
        private System.Windows.Forms.ContextMenu mNodegraphContextMenu;
        private System.Drawing.Point lastMouseLocation;

        private void CreateContextMenu()
        {
            mNodegraphContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Add Actor"),
                new System.Windows.Forms.MenuItem("Add Action"),
                new System.Windows.Forms.MenuItem("Add Property...", new System.Windows.Forms.MenuItem[]
                {
                    new System.Windows.Forms.MenuItem("Float Property"),
                    new System.Windows.Forms.MenuItem("Vector 3 Property"),
                    new System.Windows.Forms.MenuItem("Integer Property"),
                    new System.Windows.Forms.MenuItem("String Property")
                })
            });

            mNodegraphContextMenu.MenuItems[0].Click += AddActorItem_Click;
            mNodegraphContextMenu.MenuItems[1].Click += AddActionItem_Click;
            mNodegraphContextMenu.MenuItems[2].MenuItems[0].Click += AddFloatPropertyItem_Click;
            mNodegraphContextMenu.MenuItems[2].MenuItems[1].Click += AddVec3PropertyItem_Click;
            mNodegraphContextMenu.MenuItems[2].MenuItems[2].Click += AddIntPropertyItem_Click;
            mNodegraphContextMenu.MenuItems[2].MenuItems[3].Click += AddStringPropertyItem_Click;

            SetContextMenuItemEnabled(false);
        }

        private void SetContextMenuItemEnabled(bool enabled)
        {
            mNodegraphContextMenu.MenuItems[0].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[1].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[2].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[2].MenuItems[0].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[2].MenuItems[1].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[2].MenuItems[2].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[2].MenuItems[3].Enabled = enabled;
        }

        private void Graph_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lastMouseLocation = e.Location;
        }

        private void Graph_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.Location == lastMouseLocation)
                {
                    mNodegraphContextMenu.Show(Graph, e.Location);
                }
            }
        }

        private void AddActorItem_Click(object sender, System.EventArgs e)
        {
            SelectedEvent.Actors.Add(new Actor(SelectedEvent));
            UpdateNodeView();
        }

        private void AddActionItem_Click(object sender, System.EventArgs e)
        {
            Action newAction = new Action();
            ActionNode actionNode = new ActionNode(newAction);
            AddNodeToGraph(actionNode);
        }

        private void AddFloatPropertyItem_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void AddVec3PropertyItem_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void AddIntPropertyItem_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void AddStringPropertyItem_Click(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
