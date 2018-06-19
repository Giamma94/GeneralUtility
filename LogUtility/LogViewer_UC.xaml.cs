using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GeneralUtility.NumberUtility;

namespace GeneralUtility.LogUtility
{
    /// <summary>
    /// Interaction logic for LogViewer_UC.xaml
    /// </summary>
    public partial class LogViewer_UC : UserControl
    {
        private readonly string _filePath;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public LogViewer_UC(string filePath)
        {

            _filePath = filePath;

            
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            InitializeComponent();

            CreateCollumn();

        }

        private void CreateCollumn()
        {

            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Data";
            c1.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            c1.Binding = new Binding("data");
            LogDG.Columns.Add(c1);
            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "Type";
            c2.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            c2.Binding = new Binding("logType");
            LogDG.Columns.Add(c2);
            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "Message";
            c3.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
            c3.Binding = new Binding("message");
            LogDG.Columns.Add(c3);

            UpdateFIleLog();

        }

        private void UpdateFIleLog()
        {

            List<string> FileRow = new List<string>();

            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(_filePath);
            while ((line = file.ReadLine()) != null)
            {
                if (line != "")
                {

                    FileRow.Add(line);

                }
                

            }

            foreach (string fr in FileRow)
            {

                string[] app = fr.Split(';');

                CustomDataGridItem data = new CustomDataGridItem() { data = app[0], logType = app[1], message = app[2] };
                DataGridRow RowItem = new DataGridRow(){Item = data};

                if (data.logType == LogType.Error.ToString())
                {

                    RowItem.Background = Brushes.LightCoral;

                }
                else if (data.logType == LogType.Warning.ToString())
                {

                    RowItem.Background = Brushes.LightSalmon;

                }
                else if (data.logType == LogType.Info.ToString())
                {

                    RowItem.Background = Brushes.LightGreen;

                }
                

                LogDG.Items.Add(RowItem);
               
            }

            NumbOfRow_LB.Content = FileRow.Count;
        }

        private void Update_BTN_OnClick(object sender, RoutedEventArgs e)
        {

            LogDG.Items.Clear();
            UpdateFIleLog();

        }


        private void SecondToUpdate_TB_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            e.Handled = !Verify.IsValid(((TextBox)sender).Text + e.Text, 1, 1000);

        }

        private void AutoUpdate_CB_Checked(object sender, RoutedEventArgs e)
        {
                
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, Convert.ToInt32(SecondToUpdate_TB.Text), 0);
                SecondToUpdate_TB.IsEnabled = false;
                dispatcherTimer.Start();

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            Update_BTN_OnClick(null, null);

        }

        private void AutoUpdate_CB_OnUnchecked(object sender, RoutedEventArgs e)
        {

            dispatcherTimer.Stop();
            SecondToUpdate_TB.IsEnabled = true;
        
        }
    }

    public class CustomDataGridItem
    {

        public string data { get; set; }
        public string logType { get; set; }
        public string message { get; set; }

    }

}
