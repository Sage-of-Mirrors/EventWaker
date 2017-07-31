using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class StringPropertyNode : DataPropertyNode
    {
        public StringPropertyNode(string title) : base(title)
        {

        }

        public StringPropertyNode(StringProperty prop) : base(prop)
        {
            NodeTextBoxItem stringDataBox = new NodeTextBoxItem(prop.StringData);
            stringDataBox.TextChanged += StringDataBox_TextChanged;
            AddItem(stringDataBox);
        }

        private void StringDataBox_TextChanged(object sender, AcceptNodeTextChangedEventArgs e)
        {
            StringProperty stringProp = mProperty as StringProperty;
            stringProp.StringData = e.Text;
        }
    }
}
