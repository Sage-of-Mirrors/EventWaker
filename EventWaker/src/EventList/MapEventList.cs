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
            Events = new BindingList<Event>();

            List<Actor> actorList = new List<Actor>();
            List<Action> actionList = new List<Action>();
            List<DataProperty> propList = new List<DataProperty>();

            List<IConditional> conditionalList = new List<IConditional>();

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
                propList.Add(DataProperty.LoadProperty(reader));

            conditionalList.AddRange(actorList);
            conditionalList.AddRange(actionList);

            // Data banks
            for (int i = 0; i < floatBankCount; i++)
                floatBank.Add(reader.ReadSingle());

            for (int i = 0; i < integerBankCount; i++)
                integerBank.Add(reader.ReadInt32());

            // Strings are stored in a glob of bytes, so we'll encapsulate it with a stream reader
            stringBank = new EndianBinaryReader(reader.ReadBytes(stringBankLength), Endian.Big);

            foreach (DataProperty prop in propList)
            {
                switch (prop)
                {
                    case FloatProperty floatProp:
                        floatProp.ReadFloatData(floatBank);
                        break;
                    case Vec3Property vec3Prop:
                        vec3Prop.ReadVec3Data(floatBank);
                        break;
                    case IntProperty intProp:
                        intProp.ReadIntData(integerBank);
                        break;
                    case StringProperty stringProp:
                        stringProp.ReadStringData(stringBank);
                        break;
                }
            }

            foreach (Event ev in Events)
            {
                ev.ReadActors(actorList);
                ev.ReadLastCondition(conditionalList);
            }
            foreach (Actor act in actorList)
                act.ReadActions(actionList);
            foreach (Action action in actionList)
            {
                action.ReadProperties(propList);
                action.ReadConditionalFlags(conditionalList);
            }
        }

        private void DumpEventsToFile(string fileName)
        {
            StringWriter writer = new StringWriter();

            foreach (Event ev in Events)
            {
                writer.Write($"Event: { ev.Name } (Flags: { ev.Flags[0] }, { ev.Flags[1] })\n");

                foreach (Actor act in ev.Actors)
                {
                    writer.Write("\t");
                    writer.Write($"Actor: { act.Name } (Flag: { act.Flag })\n");
                    
                    foreach (Action action in act.Actions)
                    {
                        writer.Write("\t\t");
                        writer.Write($"Action: { action.Name } (Flag: { action.Flag })");

                        if (action.Conditions[0] != null)
                        {
                            writer.Write(" (waits for ");

                            for (int i = 0; i < 3; i++)
                            {
                                if (action.Conditions[i] != null)
                                {
                                    writer.Write($"{ action.Conditions[i].ToFullPathString() }, ");
                                }
                            }

                            writer.Write(")\n");
                        }
                        else
                            writer.Write('\n');

                        foreach (DataProperty prop in action.Properties)
                        {
                            string propDataVal = "";

                            switch (prop)
                            {
                                case FloatProperty floatProp:
                                    propDataVal = floatProp.FloatData.ToString();
                                    break;
                                case Vec3Property vec3Prop:
                                    propDataVal = vec3Prop.Vec3Data.ToString();
                                    break;
                                case IntProperty intProp:
                                    propDataVal = intProp.IntData.ToString();
                                    break;
                                case StringProperty stringProp:
                                    propDataVal = stringProp.StringData;
                                    break;
                            }

                            writer.Write("\t\t\t");
                            writer.Write($"{ prop.Type } Property: \t{ prop.Name }\t- Value: { propDataVal }\n");
                        }
                    }
                }

                writer.Write('\n');
            }

            using (FileStream strm = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                EndianBinaryWriter binWriter = new EndianBinaryWriter(strm, Endian.Big);
                binWriter.Write(writer.ToString().ToCharArray());
            }
        }

        public void Write(string fileName)
        {
            using (FileStream strm = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                EndianBinaryWriter writer = new EndianBinaryWriter(strm, Endian.Big);
                WriteEventList(writer);
            }
        }

        public void Write(MemoryStream strm)
        {
            EndianBinaryWriter writer = new EndianBinaryWriter(strm, Endian.Big);
            WriteEventList(writer);
        }

        public void Write(EndianBinaryWriter writer)
        {
            WriteEventList(writer);
        }

        private void WriteEventList(EndianBinaryWriter writer)
        {
            List<Actor> actorList = new List<Actor>();
            List<Action> actionList = new List<Action>();
            List<DataProperty> propList = new List<DataProperty>();

            List<float> floatBank = new List<float>();
            List<int> integerBank = new List<int>();
            EndianBinaryWriter stringBank;

            int initFlag = 0;

            foreach (Event ev in Events)
            {
                ev.SetFlags(ref initFlag);

                foreach (Actor act in ev.Actors)
                {
                    if (!actorList.Contains(act))
                        actorList.Add(act);
                }
            }

            foreach (Actor act in actorList)
            {
                foreach (Action action in act.Actions)
                    actionList.Add(action);
            }

            foreach (Action act in actionList)
            {
                foreach (DataProperty prop in act.Properties)
                    propList.Add(prop);
            }

            int runningOffset = 64;
            writer.Write(runningOffset); writer.Write(Events.Count);
            runningOffset += (176 * Events.Count);
            writer.Write(runningOffset); writer.Write(actorList.Count);
            runningOffset += (80 * actorList.Count);
            writer.Write(runningOffset); writer.Write(actionList.Count);
            runningOffset += (80 * actionList.Count);
            writer.Write(runningOffset); writer.Write(propList.Count);
            writer.Write(new byte[0x20]);

            for (int i = 0; i < Events.Count; i++)
                Events[i].Write(writer, actorList, i);

            for (int i = 0; i < actorList.Count; i++)
            {
                actorList[i].SetActionLinks(actionList);
                actorList[i].Write(writer, actionList, i);
            }

            for (int i = 0; i < actionList.Count; i++)
            {
                actionList[i].SetPropertyLinks(propList);
                actionList[i].Write(writer, propList, i);
            }

            using (MemoryStream stringStrm = new MemoryStream())
            {
                stringBank = new EndianBinaryWriter(stringStrm, Endian.Big);

                for (int i = 0; i < propList.Count; i++)
                {
                    switch (propList[i])
                    {
                        case FloatProperty floatProp:
                            floatProp.Write(writer, floatBank, i);
                            break;
                        case Vec3Property vec3Prop:
                            vec3Prop.Write(writer, floatBank, i);
                            break;
                        case IntProperty intProp:
                            intProp.Write(writer, integerBank, i);
                            break;
                        case StringProperty stringProp:
                            stringProp.Write(writer, stringBank, i);
                            break;
                    }
                }

                writer.Seek(32, SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length); writer.Write(floatBank.Count);
                writer.Seek(0, SeekOrigin.End);

                foreach (float fl in floatBank)
                    writer.Write(fl);

                writer.Seek(40, SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length); writer.Write(integerBank.Count);
                writer.Seek(0, SeekOrigin.End);

                foreach (int inte in integerBank)
                    writer.Write(inte);

                writer.Seek(48, SeekOrigin.Begin);
                writer.Write((int)writer.BaseStream.Length); writer.Write((int)stringBank.BaseStream.Length);
                writer.Seek(0, SeekOrigin.End);

                writer.Write(stringStrm.ToArray());
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
