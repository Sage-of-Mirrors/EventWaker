using Graph;
using Graph.Items;
using EventWaker.EventList;

namespace EventWaker.Nodes
{
    class DataPropertyNode : Node
    {
        public NodeLabelItem LastNodeConnection;

        protected DataProperty mProperty;

        public DataPropertyNode(string title) : base(title)
        {
        }

        public DataPropertyNode(DataProperty property) : base("Property")
        {
            mProperty = property;

            LastNodeConnection = new NodeLabelItem("", true, true) { Tag = "ActionPropertyInput" };
            AddItem(LastNodeConnection);

            NodeTextBoxItem nameBox = new NodeTextBoxItem(property.Name);
            nameBox.TextChanged += NameBox_TextChanged;
            AddItem(nameBox);
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
