using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker.EventList
{
    public abstract class DataProperty : INotifyPropertyChanged
    {
        public enum PropertyType : int
        {
            Single = 0,
            Vector3 = 1,
            Integer = 3,
            String = 4
        }

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

        public int NextPropertyIndex { get { return mNextPropertyIndex; } }
        public PropertyType Type { get { return mType; } }

        protected string mName;
        protected PropertyType mType;
        protected int mNextPropertyIndex;

        public DataProperty(EndianBinaryReader reader)
        {
            mName = new string(reader.ReadChars(32)).Trim('\0');
            reader.SkipInt32();
            mType = (PropertyType)reader.ReadInt32();

            // The rest will be handled by the derived types' constructors
        }

        public static DataProperty LoadProperty(EndianBinaryReader reader)
        {
            PropertyType propType = (PropertyType)reader.ReadInt32At(reader.BaseStream.Position + 36);

            switch (propType)
            {
                case PropertyType.Single:
                    return new FloatProperty(reader);
                case PropertyType.Vector3:
                    return new Vec3Property(reader);
                case PropertyType.Integer:
                    return new IntProperty(reader);
                case PropertyType.String:
                    return new StringProperty(reader);
                default:
                    throw new System.Exception($"Unknown property data type { propType } at offset { reader.BaseStream.Position + 36 }!");
            }
        }

        public override string ToString()
        {
            return $"{ mName } ({ mType })";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
