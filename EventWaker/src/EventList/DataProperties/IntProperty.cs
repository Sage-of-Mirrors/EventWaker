using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker.EventList
{
    public class IntProperty : DataProperty
    {
        public int IntData
        {
            get { return mIntData; }
            set
            {
                if (mIntData != value)
                {
                    mIntData = value;
                    OnPropertyChanged("IntData");
                }
            }
        }

        private int mIntIndex;
        private int mIntData;

        public IntProperty(EndianBinaryReader reader) : base(reader)
        {
            mIntIndex = reader.ReadInt32();
            reader.SkipInt32();
            mNextPropertyIndex = reader.ReadInt32();

            reader.Skip(12);
        }

        public void ReadIntData(List<int> intList)
        {
            mIntData = intList[mIntIndex];
        }
    }
}
