using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class ActorNode : Node
    {
        public NodeLabelItem ActionNodeConnector { get; set; }
        private Actor mActor;

        public ActorNode(string title) : base(title)
        {
        }

        public ActorNode(Actor act) : base("Actor")
        {
            mActor = act;

            NodeTextBoxItem nameBox = new NodeTextBoxItem(mActor.Name);
            nameBox.TextChanged += NameBox_TextChanged;
            AddItem(nameBox);

            NodeNumericSliderItem staffIDBox = new NodeNumericSliderItem("Staff ID:", 100, 0, 0, 100, mActor.StaffID, false, false);
            staffIDBox.ValueChanged += StaffIDBox_ValueChanged;
            AddItem(staffIDBox);

            NodeNumericSliderItem StaffTypeBox = new NodeNumericSliderItem("Staff Type:", 100, 0, 0, 100, mActor.StaffType, false, false);
            StaffTypeBox.ValueChanged += StaffTypeBox_ValueChanged;
            AddItem(StaffTypeBox);

            ActionNodeConnector = new NodeLabelItem("Actions", false, true) { Tag = "ActorOut" };
            
            AddItem(ActionNodeConnector);
        }

        public void ProcessActionNodeConnect(ActionNode actionNode)
        {
            mActor.AddActionFromNodeRecursive(actionNode);
        }

        public void ProcessActionNodeDisconnect(ActionNode actionNode)
        {
            mActor.RemoveActionFromNodeRecursive(actionNode);
        }

        private void NameBox_TextChanged(object sender, AcceptNodeTextChangedEventArgs e)
        {
            // Names have a max of 31 characters + null terminator
            if (e.Text.Length <= 31)
                mActor.Name = e.Text;
            else
                e.Cancel = true;
        }

        private void StaffIDBox_ValueChanged(object sender, NodeItemEventArgs e)
        {
            if (sender.GetType() == typeof(NodeNumericSliderItem))
            {
                NodeNumericSliderItem num = sender as NodeNumericSliderItem;
                num.Value = (int)num.Value;
                mActor.StaffID = (int)num.Value;
            }
        }

        private void StaffTypeBox_ValueChanged(object sender, NodeItemEventArgs e)
        {
            if (sender.GetType() == typeof(NodeNumericSliderItem))
            {
                NodeNumericSliderItem num = sender as NodeNumericSliderItem;
                num.Value = (int)num.Value;
                mActor.StaffType = (int)num.Value;
            }
        }
    }
}
