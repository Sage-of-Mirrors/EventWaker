using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker
{
    class Actor : INotifyPropertyChanged
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
            get { return mFlag; }
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

        private string mName;
        private int mStaffID;
        private int mFlag;
        private int mStaffType;
        private int mFirstActionIndex;
        private BindingList<Action> mActions;

        public Actor(EndianBinaryReader reader)
        {
            Name = new string(reader.ReadChars(32));
            StaffID = reader.ReadInt32();
            reader.SkipInt32();
            mFlag = reader.ReadInt32();
            StaffType = reader.ReadInt32();
            mFirstActionIndex = reader.ReadInt32();

            reader.Skip(28);
        }

        public void GetActions(List<Action> actionList)
        {
            int nextAction = mFirstActionIndex;

            while (nextAction != -1)
            {
                Actions.Add(actionList[nextAction]);
                nextAction = actionList[nextAction].NextActionIndex;
            }
        }

        public void Write(EndianBinaryWriter writer, List<Action> actionList, int index)
        {
            writer.WriteFixedString(Name, 32);
            writer.Write(StaffID);
            writer.Write(index);
            writer.Write(Flag);
            writer.Write(StaffType);
            writer.Write(actionList.IndexOf(Actions[0]));

            // The last 28 bytes of each entry is zero-initialized for runtime data
            writer.Write(new byte[28]);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
