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
                new System.Windows.Forms.MenuItem("Add Conditional"),
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
            mNodegraphContextMenu.MenuItems[2].Click += AddConditional_Click;
            mNodegraphContextMenu.MenuItems[3].MenuItems[0].Click += AddFloatPropertyItem_Click;
            mNodegraphContextMenu.MenuItems[3].MenuItems[1].Click += AddVec3PropertyItem_Click;
            mNodegraphContextMenu.MenuItems[3].MenuItems[2].Click += AddIntPropertyItem_Click;
            mNodegraphContextMenu.MenuItems[3].MenuItems[3].Click += AddStringPropertyItem_Click;

            SetContextMenuItemEnabled(false);
        }

        private void SetContextMenuItemEnabled(bool enabled)
        {
            mNodegraphContextMenu.MenuItems[0].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[1].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[3].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[3].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[3].MenuItems[0].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[3].MenuItems[1].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[3].MenuItems[2].Enabled = enabled;
            mNodegraphContextMenu.MenuItems[3].MenuItems[3].Enabled = enabled;
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

        private void AddConditional_Click(object sender, System.EventArgs e)
        {
            ConditionalNode condNode = new ConditionalNode("Conditions");
            AddNodeToGraph(condNode);
        }

        private void AddFloatPropertyItem_Click(object sender, System.EventArgs e)
        {
            FloatProperty floatProp = new FloatProperty();
            FloatPropertyNode floatPropNode = new FloatPropertyNode(floatProp);
            floatPropNode.Location = lastMouseLocation;
            AddNodeToGraph(floatPropNode);
        }

        private void AddVec3PropertyItem_Click(object sender, System.EventArgs e)
        {
            Vec3Property vec3Prop = new Vec3Property();
            Vec3PropertyNode vec3PropNode = new Vec3PropertyNode(vec3Prop);
            vec3PropNode.Location = lastMouseLocation;
            AddNodeToGraph(vec3PropNode);
        }

        private void AddIntPropertyItem_Click(object sender, System.EventArgs e)
        {
            IntProperty intProp = new IntProperty();
            IntPropertyNode intPropNode = new IntPropertyNode(intProp);
            intPropNode.Location = lastMouseLocation;
            AddNodeToGraph(intPropNode);
        }

        private void AddStringPropertyItem_Click(object sender, System.EventArgs e)
        {
            StringProperty stringProp = new StringProperty();
            StringPropertyNode stringPropNode = new StringPropertyNode(stringProp);
            stringPropNode.Location = lastMouseLocation;
            AddNodeToGraph(stringPropNode);
        }
    }
}
