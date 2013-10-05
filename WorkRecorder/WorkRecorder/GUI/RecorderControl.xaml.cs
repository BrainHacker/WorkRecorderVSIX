using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Community.WorkRecorder
{
    public class RecordStateChangedArgs : EventArgs
    {
        public bool Recording { get; set; }
    }

    internal enum RecorderState
    {
        Idle = 0,

        Recording,
        RecordingPaused,

        Playing,
        PlayingPaused,
    };

    /// <summary>
    /// Interaction logic for RecorderControl.xaml
    /// </summary>
    public partial class RecorderControl : UserControl
    {
        public static event EventHandler<RecordStateChangedArgs> RecordStateChanged;

        private RecorderState state = RecorderState.Idle;

        private const string strStartRecording = "Start recording";
        private const string strRecording = "Recording...";

        public RecorderControl()
        {
            // generated initialization
            InitializeComponent();

            // main initialization
            Initialize();
        }

        private void Initialize()
        {
            recordButton.Content = strStartRecording;
            state = RecorderState.Idle;
        }

        private void onRecordButtonClick(object sender, RoutedEventArgs e)
        {
            if (state == RecorderState.Idle)
            {
                recordButton.Content = strRecording;
                state = RecorderState.Recording;
            }
            else if (state == RecorderState.Recording)
            {
                recordButton.Content = strStartRecording;
                state = RecorderState.Idle;
            }

            OnRecordStateChanged();
        }

        private void OnRecordStateChanged()
        {
            EventHandler<RecordStateChangedArgs> handler = RecordStateChanged;
            if (handler != null)
            {
                bool recording = (state == RecorderState.Recording);

                RecordStateChangedArgs args = new RecordStateChangedArgs();
                args.Recording = recording;

                handler(this, args);
            }
        }
    }
}