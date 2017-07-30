using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker.EventList
{
    public class FloatProperty : DataProperty
    {
        public float FloatData
        {
            get { return mFloatData; }
            set
            {
                if (mFloatData != value)
                {
                    mFloatData = value;
                    OnPropertyChanged("FloatData");
                }
            }
        }

        private int mFloatIndex;
        private float mFloatData;

        public FloatProperty(EndianBinaryReader reader) : base(reader)
        {
            mFloatIndex = reader.ReadInt32();
            reader.SkipInt32();
            mNextPropertyIndex = reader.ReadInt32();

            reader.Skip(12);
        }

        public void ReadFloatData(List<float> floatList)
        {
            mFloatData = floatList[mFloatIndex];
        }
    }
}
