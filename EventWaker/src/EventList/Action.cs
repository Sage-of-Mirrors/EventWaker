using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;
using EventWaker.Nodes;
using Graph;

namespace EventWaker.EventList
{
    public class Action : INotifyPropertyChanged, IConditional
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return mDupeID == 0 ? mName : $"{ mName } ({ mDupeID })"; }
            set
            {
                if (mName != value)
                {
                    mName = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public IConditional[] Conditions
        {
            get { return mConditions; }
        }

        public int Flag
        {
            get { return mFlag; } set { mFlag = value; }
        }

        public int NextActionIndex
        {
            get { return mNextActionIndex; } set { mNextActionIndex = value; }
        }

        public BindingList<DataProperty> Properties
        {
            get { return mProperties; }
            set
            {
                if (mProperties != value)
                {
                    mProperties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }

        public Actor ParentActor
        {
            get { return mParentActor; }
            set
            {
                if (mParentActor != value)
                {
                    mParentActor = value;
                    OnPropertyChanged("ParentActor");
                }
            }
        }

        public Node NodeData { get; set; }

        private string mName;
        private int mDupeID;
        private int[] mConditionalFlags;
        private IConditional[] mConditions;
        private int mFlag;
        private int mFirstPropertyIndex;
        private int mNextActionIndex;
        private BindingList<DataProperty> mProperties;
        private Actor mParentActor;

        public Action()
        {
            Name = "Action";
            mProperties = new BindingList<DataProperty>();
            mConditions = new IConditional[3];
        }

        public Action(EndianBinaryReader reader)
        {
            Properties = new BindingList<DataProperty>();

            Name = new string(reader.ReadChars(32)).Trim('\0');
            mDupeID = reader.ReadInt32();
            reader.SkipInt32();

            mConditionalFlags = new int[3];
            mConditions = new IConditional[3];
            for (int i = 0; i < 3; i++)
                mConditionalFlags[i] = reader.ReadInt32();

            mFlag = reader.ReadInt32();
            mFirstPropertyIndex = reader.ReadInt32();
            mNextActionIndex = reader.ReadInt32();

            // The last 16 bytes of each entry is zero-initialized for runtime data
            reader.Skip(16);
        }

        public void ReadProperties(List<DataProperty> propList)
        {
            int nextProperty = mFirstPropertyIndex;

            while (nextProperty != -1)
            {
                propList[nextProperty].ParentAction = this;
                Properties.Add(propList[nextProperty]);
                nextProperty = propList[nextProperty].NextPropertyIndex;
            }
        }

        public void ReadConditionalFlags(List<IConditional> conditionalList)
        {
            for (int i = 0; i < 3; i++)
            {
                if (mConditionalFlags[i] == -1)
                    continue;

                Conditions[i] = conditionalList.Find(x => x.Flag == mConditionalFlags[i]);
            }
        }

        public void Write(EndianBinaryWriter writer, List<DataProperty> propList, int index)
        {
            writer.WriteFixedString(mName, 32);
            for (int i = 0; i < 32 - mName.Length; i++)
                writer.Write((byte)0);

            writer.Write(mDupeID);
            writer.Write(index);

            for (int i = 0; i < 3; i++)
            {
                if (Conditions[i] != null)
                    writer.Write(Conditions[i].Flag);
                else
                    writer.Write(-1);
            }

            writer.Write(Flag);
            if (Properties.Count > 0)
                writer.Write(propList.IndexOf(Properties[0]));
            else
                writer.Write(-1);
            writer.Write(NextActionIndex);

            writer.Write(new byte[16]);
        }

        public void SetFlag(ref int flag)
        {
            Flag = flag++;
            flag += Properties.Count;
        }

        public void SetPropertyLinks(List<DataProperty> propList)
        {
            for (int i = 0; i < Properties.Count; i++)
            {
                if (i + 1 >= Properties.Count)
                {
                    Properties[i].NextPropertyIndex = -1;
                    break;
                }

                Properties[i].NextPropertyIndex = propList.IndexOf(Properties[i + 1]);
            }
        }

        public string ToFullPathString()
        {
            return $"{ ParentActor.ParentEvent.Name }.{ ParentActor.Name }.{ Name }";
        }

        public override string ToString()
        {
            return $"{ mName } ({ Properties.Count } property(ies))";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddDataPropertyFromNodeRecursive(DataPropertyNode node)
        {
            Properties.Add(node.AttatchedDataProperty);
            node.AttatchedDataProperty.ParentAction = this;

            // This isn't the last node in the chain, so we need to move on
            if (node.LastNodeConnection.Output.HasConnection == true)
            {
                NodeConnection releventConnection = null;

                // We'll get the connection between this DataPropertyNode and the next DataPropertyNode
                foreach (NodeConnection connect in node.Connections)
                {
                    // This ignores a connection between the last DataPropertyNode and this one
                    if (connect.To.Node != node)
                        releventConnection = connect;
                }

                if (releventConnection == null)
                    return;

                // RECURSE!
                AddDataPropertyFromNodeRecursive(releventConnection.To.Node as DataPropertyNode);
            }
        }

        public void RemoveDataPropertyFromNodeRecursive(DataPropertyNode node)
        {
            Properties.Remove(node.AttatchedDataProperty);
            node.AttatchedDataProperty.ParentAction = null;

            // This isn't the last node in the chain, so we need to move on
            if (node.LastNodeConnection.Output.HasConnection == true)
            {
                NodeConnection releventConnection = null;

                // We'll get the connection between this DataPropertyNode and the next DataPropertyNode
                foreach (NodeConnection connect in node.Connections)
                {
                    // This ignores a connection between the last DataPropertyNode and this one
                    if (connect.To.Node != node)
                        releventConnection = connect;
                }

                if (releventConnection == null)
                    return;

                // RECURSE!
                RemoveDataPropertyFromNodeRecursive(releventConnection.To.Node as DataPropertyNode);
            }
        }

        public void AddConditionalPropertyNodeFromActorRecursive(ConditionalNode node)
        {
            if (node.AttachedAction == null)
                return;

            mParentActor.AddActionFromNodeRecursive(node.AttachedAction.NodeData as ActionNode);
        }

        public void AddConditionalPropertyNodeFromConditionals(IConditional node, Graph.Items.NodeLabelItem connector)
        {
            switch (connector.Text)
            {
                case "Condition 1":
                    Conditions[0] = node;
                    break;
                case "Condition 2":
                    Conditions[1] = node;
                    break;
                case "Condition 3":
                    Conditions[2] = node;
                    break;
            }
        }

        public void RemoveConditionalPropertyNodeFromActorRecursive(ConditionalNode condNode)
        {
            mParentActor.RemoveActionFromNodeRecursive(condNode.AttachedAction.NodeData as ActionNode);
        }

        public void RemoveConditionalPropertyNodeFromConditionals(ConditionalNode condNode, Graph.Items.NodeLabelItem connector)
        {
            switch (connector.Text)
            {
                case "Condition 1":
                    Conditions[0] = null;
                    break;
                case "Condition 2":
                    Conditions[1] = null;
                    break;
                case "Condition 3":
                    Conditions[2] = null;
                    break;
            }
        }
    }
}
