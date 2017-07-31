using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class Vec3PropertyNode : DataPropertyNode
    {
        public Vec3PropertyNode(string title) : base(title)
        {

        }

        public Vec3PropertyNode(Vec3Property prop) : base(prop)
        {
            NodeNumericSliderItem xCoordBox = new NodeNumericSliderItem("X:", 100, 0, 0, 1000, prop.Vec3Data.X, false, false);
            xCoordBox.ValueChanged += XCoordBox_ValueChanged;
            AddItem(xCoordBox);
        }

        private void XCoordBox_ValueChanged(object sender, NodeItemEventArgs e)
        {
            Vec3Property vec3Prop = mProperty as Vec3Property;

            if (sender.GetType() == typeof(NodeNumericSliderItem))
            {
                NodeNumericSliderItem num = sender as NodeNumericSliderItem;
                num.Value = num.Value;
                //vec3Prop.Vec3Data.X = num.Value;
            }
        }
    }
}
