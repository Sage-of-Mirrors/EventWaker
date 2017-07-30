using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

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
            get { return mFlag; } set { }
        }

        public int NextActionIndex
        {
            get { return mNextActionIndex; }
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

        private string mName;
        private int mDupeID;
        private int[] mConditionalFlags;
        private IConditional[] mConditions;
        private int mFlag;
        private int mFirstPropertyIndex;
        private int mNextActionIndex;
        private BindingList<DataProperty> mProperties;
        private Actor mParentActor;

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

        public void Write(EndianBinaryWriter writer, List<DataProperty> propList, int index, int nextAction)
        {
            writer.WriteFixedString(Name, 32);
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
            writer.Write(nextAction);
            writer.Write(nextAction);

            writer.Write(new byte[16]);
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
    }
}
