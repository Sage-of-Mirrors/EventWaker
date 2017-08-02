using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class IntPropertyNode : DataPropertyNode
    {
        public IntPropertyNode(string title) : base(title)
        {

        }

        public IntPropertyNode(IntProperty prop) : base(prop)
        {
            NodeNumericSliderItem intDataBox = new NodeNumericSliderItem("", 100, 0, 0, 1000000, prop.IntData, false, false);
            intDataBox.ValueChanged += IntDataBox_ValueChanged;
            AddItem(intDataBox);
        }

        private void IntDataBox_ValueChanged(object sender, NodeItemEventArgs e)
        {
            IntProperty intProp = mProperty as IntProperty;

            if (sender.GetType() == typeof(NodeNumericSliderItem))
            {
                NodeNumericSliderItem num = sender as NodeNumericSliderItem;
                num.Value = (int)num.Value;
                intProp.IntData = (int)num.Value;
            }
        }
    }
}
