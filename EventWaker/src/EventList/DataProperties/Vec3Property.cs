using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;
using OpenTK;

namespace EventWaker.EventList
{
    public class Vec3Property : DataProperty
    {
        public Vector3 Vec3Data
        {
            get { return mVec3Data; }
            set
            {
                if (mVec3Data != value)
                {
                    mVec3Data = value;
                    OnPropertyChanged("Vec3Data");
                }
            }
        }

        private int mVec3Index;
        private Vector3 mVec3Data;

        public Vec3Property(EndianBinaryReader reader) : base(reader)
        {
            mVec3Index = reader.ReadInt32();
            reader.SkipInt32();
            mNextPropertyIndex = reader.ReadInt32();

            reader.Skip(12);
        }

        public void ReadVec3Data(List<float> floatList)
        {
            mVec3Data = new Vector3(floatList[mVec3Index], floatList[mVec3Index + 1], floatList[mVec3Index + 2]);
        }
    }
}
