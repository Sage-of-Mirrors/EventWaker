using GameFormatReader.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace EventWaker.EventList
{
    public class Event : INotifyPropertyChanged
    {
        public enum EventFlags : int
        {
            Unknown1,
            Unknown2,
            EventEnded,
            Unknown3,
            Unknown4
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

        public int Unknown1
        {
            get { return mUnknown1; }
            set
            {
                if (mUnknown1 != value)
                {
                    mUnknown1 = value;
                    OnPropertyChanged("Unknown1");
                }
            }
        }

        public int Priority
        {
            get { return mPriority; }
            set
            {
                if (mPriority != value)
                {
                    mPriority = value;
                    OnPropertyChanged("Priority");
                }
            }
        }

        public BindingList<Actor> Actors
        {
            get { return mActors; }
            set
            {
                if (mActors != value)
                {
                    mActors = value;
                    OnPropertyChanged("Actors");
                }
            }
        }

        public bool PlayJingle
        {
            get { return mPlayJingle; }
            set
            {
                if (mPlayJingle != value)
                {
                    mPlayJingle = value;
                    OnPropertyChanged("PlayJingle");
                }
            }
        }

        public int[] Flags { get { return mFlags; } set { mFlags = value; } }

        private string mName;
        private int mUnknown1;
        private int mPriority;
        private int[] mActorIndices;
        private BindingList<Actor> mActors;
        private int[] mFlags;
        private bool mPlayJingle;

        private IConditional mLastCondition;

        public Event(EndianBinaryReader reader)
        {
            mActors = new BindingList<Actor>();
            mFlags = new int[5];

            Name = new string(reader.ReadChars(32)).Trim('\0');
            reader.SkipInt32();
            Unknown1 = reader.ReadInt32();
            Priority = reader.ReadInt32();

            long flagOffset = reader.BaseStream.Position + 84;

            uint actorCount = reader.ReadUInt32At(reader.BaseStream.Position + 80);
            mActorIndices = new int[actorCount];

            for (int i = 0; i < actorCount; i++)
                mActorIndices[i] = reader.ReadInt32();

            reader.BaseStream.Seek(flagOffset, System.IO.SeekOrigin.Begin);

            for (int i = 0; i < 5; i++)
                Flags[i] = reader.ReadInt32();

            PlayJingle = reader.ReadBoolean();

            reader.Skip(27);
        }

        public void ReadActors(List<Actor> actorList)
        {
            foreach (int i in mActorIndices)
            {
                actorList[i].ParentEvent = this;
                Actors.Add(actorList[i]);
            }
        }

        public void ReadLastCondition(List<IConditional> conditionalList)
        {
            mLastCondition = conditionalList.Find(x => x.Flag == Flags[2]);
        }

        public void Write(EndianBinaryWriter writer, List<Actor> actorList, int index)
        {
            writer.WriteFixedString(Name, 32);
            for (int i = 0; i < 32 - Name.Length; i++)
                writer.Write((byte)0);

            writer.Write(index);
            writer.Write(Unknown1);
            writer.Write(Priority);

            foreach (Actor act in Actors)
            {
                writer.Write(actorList.IndexOf(act));
            }

            for (int i = 0; i < 20 - Actors.Count; i++)
                writer.Write(-1);

            writer.Write(Actors.Count);

            for (int i = 0; i < 5; i++)
                writer.Write(Flags[i]);

            writer.Write(PlayJingle);

            // The last 27 bytes of each entry is zero-initialized for runtime data
            writer.Write(new byte[27]);
        }

        public void SetFlags(ref int flag)
        {
            Flags[0] = flag++;
            Flags[1] = flag++;

            foreach (Actor act in Actors)
                act.SetFlag(ref flag);

            Flags[2] = mLastCondition.Flag;
        }

        public override string ToString()
        {
            return $"{ mName } ({ Actors.Count } actor(s))";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}