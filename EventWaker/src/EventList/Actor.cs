using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;
using EventWaker.Nodes;
using Graph;

namespace EventWaker.EventList
{
    public class Actor : INotifyPropertyChanged, IConditional
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get { return mName; }
            set
            {
                if (mName != value)
                {
                    mName = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public int StaffID
        {
            get { return mStaffID; }
            set
            {
                if (mStaffID != value)
                {
                    mStaffID = value;
                    OnPropertyChanged("StaffID");
                }
            }
        }

        public int Flag
        {
            get { return mFlag; } set { mFlag = value; }
        }

        public int StaffType
        {
            get { return mStaffType; }
            set
            {
                if (mStaffType != value)
                {
                    mStaffType = value;
                    OnPropertyChanged("StaffType");
                }
            }
        }

        public BindingList<Action> Actions
        {
            get { return mActions; }
            set
            {
                if (mActions != value)
                {
                    mActions = value;
                    OnPropertyChanged("Actors");
                }
            }
        }

        public Event ParentEvent
        {
            get { return mParentEvent; }
            set
            {
                if (mParentEvent != value)
                {
                    mParentEvent = value;
                    OnPropertyChanged("ParentEvent");
                }
            }
        }

        public Node NodeData { get; set; }

        private string mName;
        private int mStaffID;
        private int mFlag;
        private int mStaffType;
        private int mFirstActionIndex;
        private BindingList<Action> mActions;
        private Event mParentEvent;

        public Actor(Event parent)
        {
            ParentEvent = parent;
            Name = "Actor";
            Actions = new BindingList<Action>();
        }

        public Actor(EndianBinaryReader reader)
        {
            Actions = new BindingList<Action>();

            Name = new string(reader.ReadChars(32)).Trim('\0');
            StaffID = reader.ReadInt32();
            reader.SkipInt32();
            mFlag = reader.ReadInt32();
            StaffType = reader.ReadInt32();
            mFirstActionIndex = reader.ReadInt32();

            reader.Skip(28);
        }

        public void ReadActions(List<Action> actionList)
        {
            int nextAction = mFirstActionIndex;

            while (nextAction != -1)
            {
                actionList[nextAction].ParentActor = this;
                Actions.Add(actionList[nextAction]);
                nextAction = actionList[nextAction].NextActionIndex;
            }
        }

        public void Write(EndianBinaryWriter writer, List<Action> actionList, int index)
        {
            writer.WriteFixedString(Name, 32);
            for (int i = 0; i < 32 - Name.Length; i++)
                writer.Write((byte)0);

            writer.Write(StaffID);
            writer.Write(index);
            writer.Write(Flag);
            writer.Write(StaffType);
            writer.Write(actionList.IndexOf(Actions[0]));

            // The last 28 bytes of each entry is zero-initialized for runtime data
            writer.Write(new byte[28]);
        }

        public void SetFlag(ref int flag)
        {
            Flag = flag++;

            foreach (Action action in Actions)
                action.SetFlag(ref flag);
        }

        public void SetActionLinks(List<Action> actionList)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                if (i + 1 >= Actions.Count)
                {
                    Actions[i].NextActionIndex = -1;
                    break;
                }

                Actions[i].NextActionIndex = actionList.IndexOf(Actions[i + 1]);
            }
        }

        public string ToFullPathString()
        {
            return $"{ ParentEvent.Name }.{ Name }";
        }

        public override string ToString()
        {
            return $"{ mName } ({ Actions.Count } action(s))";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddActionFromNodeRecursive(ActionNode node)
        {
            Actions.Add(node.AttachedAction);
            node.AttachedAction.ParentActor = this;

            // This isn't the last node in the chain, so we need to move on
            if (node.LastConnector.Output.HasConnection == true)
            {
                NodeConnection releventConnection = null;

                // We'll get the connection between this ActionNode and the next ActionNode
                foreach (NodeConnection connect in node.Connections)
                {
                    // This ignores a connection between the last ActionNode and this one
                    if (connect.To.Node.GetType() == typeof(ActionNode) && connect.To.Node != node)
                        releventConnection = connect;
                }

                if (releventConnection == null)
                    return;

                // RECURSE!
                AddActionFromNodeRecursive(releventConnection.To.Node as ActionNode);
            }
        }

        public void RemoveActionFromNodeRecursive(ActionNode node)
        {
            Actions.Remove(node.AttachedAction);
            node.AttachedAction.ParentActor = null;

            // This isn't the last node in the chain, so we need to move on
            if (node.LastConnector.Output.HasConnection == true)
            {
                NodeConnection releventConnection = null;

                // We'll get the connection between this ActionNode and the next ActionNode
                foreach (NodeConnection connect in node.Connections)
                {
                    // This ignores a connection between the last ActionNode and this one
                    if (connect.To.Node.GetType() == typeof(ActionNode) && connect.To.Node != node)
                        releventConnection = connect;
                }

                if (releventConnection == null)
                    return;

                // RECURSE!
                RemoveActionFromNodeRecursive(releventConnection.To.Node as ActionNode);
            }
        }
    }
}
