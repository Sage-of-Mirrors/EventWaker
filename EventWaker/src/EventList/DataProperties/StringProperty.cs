using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker.EventList
{
    public class StringProperty : DataProperty
    {
        public string StringData
        {
            get { return mStringData; }
            set
            {
                if (mStringData != value)
                {
                    mStringData = value;
                    OnPropertyChanged("StringData");
                }
            }
        }

        private int mStringStartingIndex;
        private int mStringLength;
        private string mStringData;

        public StringProperty(EndianBinaryReader reader) : base(reader)
        {
            mStringStartingIndex = reader.ReadInt32();
            mStringLength = reader.ReadInt32();
            mNextPropertyIndex = reader.ReadInt32();

            reader.Skip(12);
        }

        public void ReadStringData(EndianBinaryReader stringData)
        {
            stringData.BaseStream.Seek(mStringStartingIndex, System.IO.SeekOrigin.Begin);
            mStringData = new string(stringData.ReadChars(mStringLength)).Trim('\0');
        }
    }
}
