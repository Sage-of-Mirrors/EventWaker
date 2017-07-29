using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace EventWaker.EventList
{
    public class MapEventList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BindingList<Event> Events
        {
            get { return mEvents; }
            set
            {
                if (mEvents != value)
                {
                    mEvents = value;
                    OnPropertyChanged("Events");
                }
            }
        }

        private BindingList<Event> mEvents;

        public MapEventList(string fileName)
        {
            using (FileStream strm = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                EndianBinaryReader reader = new EndianBinaryReader(strm, Endian.Big);
                LoadEventList(reader);
            }
        }

        public MapEventList(EndianBinaryReader reader)
        {
            LoadEventList(reader);
        }

        public MapEventList(byte[] fileData)
        {
            EndianBinaryReader reader = new EndianBinaryReader(fileData, Endian.Big);
            LoadEventList(reader);
        }

        private void LoadEventList(EndianBinaryReader reader)
        {
            List<Actor> actorList = new List<Actor>();
            List<Action> actionList = new List<Action>();
            List<Property> propList = new List<Property>();

            List<float> floatBank = new List<float>();
            List<int> integerBank = new List<int>();
            EndianBinaryReader stringBank;

            int evOffset = reader.ReadInt32(); int evCount = reader.ReadInt32();
            int actorOffset = reader.ReadInt32(); int actorCount = reader.ReadInt32();
            int actionOffset = reader.ReadInt32(); int actionCount = reader.ReadInt32();
            int propOffset = reader.ReadInt32(); int propCount = reader.ReadInt32();
            int floatBankOffset = reader.ReadInt32(); int floatBankCount = reader.ReadInt32();
            int integerBankOffset = reader.ReadInt32(); int integerBankCount = reader.ReadInt32();
            int stringBankOffset = reader.ReadInt32(); int stringBankLength = reader.ReadInt32();

            reader.SkipInt64();

            // Main event data
            for (int i = 0; i < evCount; i++)
                Events.Add(new Event(reader));

            for (int i = 0; i < actorCount; i++)
                actorList.Add(new Actor(reader));

            for (int i = 0; i < actionCount; i++)
                actionList.Add(new Action(reader));

            for (int i = 0; i < propCount; i++)
                propList.Add(new Property(reader));

            // Data banks
            for (int i = 0; i < floatBankCount; i++)
                floatBank.Add(reader.ReadSingle());

            for (int i = 0; i < integerBankCount; i++)
                integerBank.Add(reader.ReadInt32());

            // Strings are stored in a glob of bytes, so we'll encapsulate it with a stream reader
            stringBank = new EndianBinaryReader(reader.ReadBytes(stringBankLength), Endian.Big);

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
