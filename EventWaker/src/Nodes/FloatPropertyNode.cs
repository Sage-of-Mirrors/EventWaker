using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class FloatPropertyNode : DataPropertyNode
    {
        public FloatPropertyNode(string title) : base(title)
        {

        }

        public FloatPropertyNode(FloatProperty prop) : base(prop)
        {
            NodeNumericSliderItem floatDataBox = new NodeNumericSliderItem("", 100, 0, 0, 1000, prop.FloatData, false, false);
            floatDataBox.ValueChanged += FloatDataBox_ValueChanged;
            AddItem(floatDataBox);
        }

        private void FloatDataBox_ValueChanged(object sender, NodeItemEventArgs e)
        {
            FloatProperty floatProp = mProperty as FloatProperty;

            if (sender.GetType() == typeof(NodeNumericSliderItem))
            {
                NodeNumericSliderItem num = sender as NodeNumericSliderItem;
                num.Value = num.Value;
                floatProp.FloatData = num.Value;
            }
        }
    }
}
