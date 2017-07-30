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

        public void Write(EndianBinaryWriter writer, EndianBinaryWriter stringBank, int index)
        {
            writer.WriteFixedString(Name, 32);
            for (int i = 0; i < 32 - Name.Length; i++)
                writer.Write((byte)0);

            writer.Write(index);
            writer.Write((int)Type);
            writer.Write((int)stringBank.BaseStream.Position);

            int paddedStringDataLength = GetPaddedStringDataLength();
            stringBank.WriteFixedString(StringData, paddedStringDataLength);
            for (int i = 0; i < paddedStringDataLength - StringData.Length; i++)
                stringBank.Write((byte)0);

            writer.Write(paddedStringDataLength);
            writer.Write(mNextPropertyIndex);

            writer.Write(new byte[12]);
        }

        private int GetPaddedStringDataLength()
        {
            if (StringData.Length < 8)
                return 8;
            else if (StringData.Length >= 8 && StringData.Length < 16)
                return 16;
            else if (StringData.Length >= 16 && StringData.Length < 24)
                return 24;
            else
                return 32;
        }
    }
}
