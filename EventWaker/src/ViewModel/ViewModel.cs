using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using EventWaker.EventList;
using Microsoft.Win32;
using Graph;

namespace EventWaker.ViewModel
{
    public partial class DataViewModel : INotifyPropertyChanged
    {
        public string MainWindowTitle
        {
            get { return mEventListPath == null ? "Event Waker" : $"Event Waker - { mEventListPath }"; }
            set
            {
                if (mEventListPath != value)
                {
                    mEventListPath = value;
                    OnPropertyChanged("MainWindowTitle");
                }
            }
        }

        public MapEventList LoadedEventList
        {
            get { return mLoadedEventList; }
            set
            {
                if (mLoadedEventList != value)
                {
                    mLoadedEventList = value;
                    OnPropertyChanged("LoadedEventList");
                }
            }
        }

        public GraphControl Graph
        {
            get { return mGraph; }
            set
            {
                if (mGraph != value)
                {
                    mGraph = value;
                    OnPropertyChanged("Graph");
                }
            }
        }

        public Event SelectedEvent
        {
            get { return mSelectedEvent; }
            set
            {
                if (mSelectedEvent != value)
                {
                    mSelectedEvent = value;
                    OnPropertyChanged("LoadedEventList");
                    if (mSelectedEvent != null)
                        UpdateNodeView();
                }
            }
        }

        private string mEventListPath;
        private bool mHasChanges;
        private GraphControl mGraph;

        private MapEventList mLoadedEventList;
        private Event mSelectedEvent;

        public DataViewModel()
        {
            Graph = new GraphControl();
            Graph.CompatibilityStrategy = new Graph.Compatibility.TagTypeCompatibility();
            Graph.HighlightCompatible = true;
            Graph.ConnectionAdded += Graph_ConnectionAdded;
            Graph.ConnectionRemoving += Graph_ConnectionRemoving;
            //Graph.ConnectionRemoved += Graph_ConnectionRemoved;

            Graph.MouseUp += Graph_MouseUp;
            Graph.MouseDown += Graph_MouseDown;

            Graph.KeyUp += Graph_KeyUp;

            mNodes = new List<Node>();
            mConditionalNodes = new List<Nodes.ConditionalNode>();
            CreateContextMenu();
        }

        public void OpenList()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "EventList files (*.dat) |*.dat| All files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                MainWindowTitle = openFile.FileName;
                LoadedEventList = new MapEventList(mEventListPath);
                SetContextMenuItemEnabled(true);
                SelectedEvent = mLoadedEventList.Events[0];
            }
        }

        public void SaveList()
        {
            LoadedEventList.Write(mEventListPath);
        }

        public void SaveListAs()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "EventList files (*.dat) |*.dat";

            if (saveFile.ShowDialog() == true)
            {
                mEventListPath = saveFile.FileName;
                SaveList();
            }
        }

        public void CloseList()
        {
            if (mHasChanges)
            {
                if (CheckSaveChanges())
                    SaveList();
            }

            LoadedEventList = null;

            mDisableConnectionUpdates = true;
            Graph.Clear();
            mNodes.Clear();
            mConditionalNodes.Clear();
            mEventNode = null;
            mDisableConnectionUpdates = false;
        }

        public void ResetViewport()
        {
            Graph.Translation = new System.Drawing.PointF();
            Graph.Zoom = 1.0f;
            Graph.Invalidate();
        }

        public void AddEvent()
        {
            Event newEv = new Event();
            LoadedEventList.Events.Add(newEv);
            SelectedEvent = newEv;
        }

        private bool CheckSaveChanges()
        {
            MessageBoxResult res = MessageBox.Show("You have unsaved changes. Would you like to save them?", "Save changes?", MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }
    }
}
