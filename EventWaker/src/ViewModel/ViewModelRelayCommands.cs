using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace EventWaker.ViewModel
{
    public partial class DataViewModel : INotifyPropertyChanged
    {
        /// <summary> The user has requested to open a file. </summary>
        public ICommand OnRequestOpenFile
        {
            get { return new RelayCommand(x => OpenList()); }
        }

        /// <summary> The user has requested to save the currently open data back to the file they originally opened. </summary>
        public ICommand OnRequestSave
        {
            get { return new RelayCommand(x => SaveList(), x => mLoadedEventList != null); }
        }

        /// <summary> The user has requested to save the currently open data to a new file. </summary>
        public ICommand OnRequestSaveAs
        {
            get { return new RelayCommand(x => SaveListAs(), x => mLoadedEventList != null); }
        }

        /// <summary> The user has requested to unload the currently open data. </summary>
        public ICommand OnRequestClose
        {
            get { return new RelayCommand(x => CloseList(), x => mLoadedEventList != null); }
        }

        /// <summary> The user has pressed Alt + F4, chosen Exit from the File menu, or clicked the close button. </summary>
        public ICommand OnRequestApplicationExit
        {
            get { return new RelayCommand(x => ExitApplication()); }
        }

        /// <summary> The user has pressed View->Reset Viewport. </summary>
        public ICommand OnRequestResetViewport
        {
            get { return new RelayCommand(x => ResetViewport()); }
        }

        /// <summary> The user has pressed Edit->Add Event. </summary>
        public ICommand OnRequestAddEvent
        {
            get { return new RelayCommand(x => AddEvent(), x=> mLoadedEventList != null); }
        }

        /// <summary> The user has clicked Report a Bug... from the Help menu. </summary>
        public ICommand OnRequestReportBug
        {
            get { return new RelayCommand(x => ReportBug()); }
        }

        /// <summary> The user has clicked Report a Bug... from the Help menu. </summary>
        public ICommand OnRequestOpenWiki
        {
            get { return new RelayCommand(x => OpenWiki()); }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public virtual void ExitApplication()
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Opens the user's default browser to OpenGL_in_WPF_Framework's Issues page.
        /// </summary>
        public virtual void ReportBug()
        {
            System.Diagnostics.Process.Start("https://github.com/Sage-of-Mirrors/EventWaker/issues");
        }

        /// <summary>
        /// Opens the user's default browser to OpenGL_in_WPF_Framework's Wiki page.
        /// </summary>
        public virtual void OpenWiki()
        {
            System.Diagnostics.Process.Start("https://github.com/Sage-of-Mirrors/EventWaker/wiki");
        }
    }
}
