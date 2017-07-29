using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker.EventList
{
    public class Action : INotifyPropertyChanged
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

        public int[] Conditions
        {
            get { return mConditions; }
        }

        public int Flag
        {
            get { return mFlag; }
        }

        public int NextActionIndex
        {
            get { return mNextActionIndex; }
        }

        public BindingList<Property> Properties
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

        private string mName;
        private int mDupeID;
        private int[] mConditions;
        private int mFlag;
        private int mFirstPropertyIndex;
        private int mNextActionIndex;
        private BindingList<Property> mProperties;

        public Action(EndianBinaryReader reader)
        {
            Name = new string(reader.ReadChars(32));
            mDupeID = reader.ReadInt32();
            reader.SkipInt32();

            mConditions = new int[3];
            for (int i = 0; i < 3; i++)
                mConditions[i] = reader.ReadInt32();

            mFlag = reader.ReadInt32();
            mFirstPropertyIndex = reader.ReadInt32();
            mNextActionIndex = reader.ReadInt32();

            // The last 16 bytes of each entry is zero-initialized for runtime data
            reader.Skip(16);
        }

        public void GetProperties(List<Property> propList)
        {

        }

        public void Write(EndianBinaryWriter writer, List<Property> propList, int index, int nextAction)
        {
            writer.WriteFixedString(Name, 32);
            writer.Write(mDupeID);
            writer.Write(index);

            foreach (int i in mConditions)
                writer.Write(i);

            writer.Write(Flag);
            writer.Write(nextAction);
            writer.Write(nextAction);

            writer.Write(new byte[16]);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
