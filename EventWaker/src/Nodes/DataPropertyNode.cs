using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    public class DataPropertyNode : Node
    {
        public NodeLabelItem LastNodeConnection;
        public DataProperty AttatchedDataProperty { get { return mProperty; } }

        protected DataProperty mProperty;

        public DataPropertyNode(string title) : base(title)
        {
        }

        public DataPropertyNode(DataProperty property) : base("Property")
        {
            mProperty = property;

            LastNodeConnection = new NodeLabelItem("", true, true) { Tag = 'i' };
            AddItem(LastNodeConnection);

            NodeTextBoxItem nameBox = new NodeTextBoxItem(property.Name);
            nameBox.TextChanged += NameBox_TextChanged;
            AddItem(nameBox);
        }

        public void ProcessPropertyNodeConnect(DataPropertyNode node)
        {
            if (mProperty.ParentAction == null)
                return;

            mProperty.ParentAction.AddDataPropertyFromNodeRecursive(node);
        }

        public void ProcessPropertyNodeDisconnect(DataPropertyNode node)
        {
            // There's no parent action, so we do nothing
            if (node.AttatchedDataProperty.ParentAction == null)
                return;

            node.AttatchedDataProperty.ParentAction.RemoveDataPropertyFromNodeRecursive(node);
        }

        private void NameBox_TextChanged(object sender, AcceptNodeTextChangedEventArgs e)
        {
            // Names have a max of 31 characters + null terminator
            if (e.Text.Length <= 31)
                mProperty.Name = e.Text;
            else
                e.Cancel = true;
        }

        public static DataPropertyNode GetPropertyNode(DataProperty property)
        {
            switch (property)
            {
                case StringProperty stringProperty:
                    return new StringPropertyNode(stringProperty);
                case IntProperty intProperty:
                    return new IntPropertyNode(intProperty);
                case FloatProperty floatProperty:
                    return new FloatPropertyNode(floatProperty);
                case Vec3Property vec3Property:
                    return new Vec3PropertyNode(vec3Property);
                default:
                    throw new System.Exception("Unknown property type!");
            }
        }
    }
}
