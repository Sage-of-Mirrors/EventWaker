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

        public FloatProperty() : base()
        {
            mType = PropertyType.Single;
        }

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

        public void Write(EndianBinaryWriter writer, List<float> floatList, int index)
        {
            writer.WriteFixedString(Name, 32);
            for (int i = 0; i < 32 - Name.Length; i++)
                writer.Write((byte)0);

            writer.Write(index);
            writer.Write((int)Type);
            
            if (!floatList.Contains(FloatData))
            {
                floatList.Add(FloatData);
            }
            writer.Write(floatList.IndexOf(FloatData));

            writer.Write((int)1);
            writer.Write(mNextPropertyIndex);

            writer.Write(new byte[12]);
        }
    }
}
