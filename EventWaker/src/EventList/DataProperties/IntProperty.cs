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

        public IntProperty() : base()
        {
            mType = PropertyType.Integer;
        }

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

        public void Write(EndianBinaryWriter writer, List<int> intList, int index)
        {
            writer.WriteFixedString(Name, 32);
            for (int i = 0; i < 32 - Name.Length; i++)
                writer.Write((byte)0);

            writer.Write(index);
            writer.Write((int)Type);

            if (!intList.Contains(IntData))
            {
                intList.Add(IntData);
            }
            writer.Write(intList.IndexOf(IntData));

            writer.Write((int)1);
            writer.Write(mNextPropertyIndex);

            writer.Write(new byte[12]);
        }
    }
}
