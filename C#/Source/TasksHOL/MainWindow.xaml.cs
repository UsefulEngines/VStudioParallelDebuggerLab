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

using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace HOL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker w;
        int numTasks = 5;
        public static List<Task> tasks = new List<Task>();

        public MainWindow()
        {
            InitializeComponent();

            w = new BackgroundWorker();
            w.DoWork += new DoWorkEventHandler(w_DoWork);
            w.WorkerReportsProgress = true;
            w.ProgressChanged += new ProgressChangedEventHandler(w_ProgressChanged);
            w.RunWorkerCompleted += new RunWorkerCompletedEventHandler(w_RunWorkerCompleted);
        }

        void w_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonStart.IsEnabled = true;
            buttonStop.IsEnabled = false;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;
            buttonStop.IsEnabled = true;

            w.RunWorkerAsync();
        }

        int prog = 0;

        void w_DoWork(object sender, DoWorkEventArgs e)
        {
            prog = 0;

            w.ReportProgress(prog++);

            CreateTasks();

            w.ReportProgress(prog++);

            try
            {
                while (!Task.WaitAll(tasks.ToArray(), 300))
                {
                    w.ReportProgress(prog++);
                }
            }
            catch { } // Cancelling causes an exception to be througn

            w.ReportProgress(-1);
        }

        private void CreateTasks()
        {
            tasks.Clear();

            for (int i = 0; i < numTasks; i++)
            {
                OrderForm f = new OrderForm() { Name = "Test #" + i, PostalCode = "00000" };

                tasks.Add(new Task(Work.ProcessForm, f, Work.CancelToken.Token));
                w.ReportProgress(prog++);
            }

            foreach (Task t in tasks)
            {
                t.Start();
                w.ReportProgress(prog++);
            }
        }


        void w_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            listBoxStatus.ItemsSource = (from ct in tasks select new { ct.Id, Status = ct.Status.ToString(), OrderStatus = ct.AsyncState ?? "<Unassigned>" }).ToList();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            Work.CancelToken.Cancel();
        }
    }
}
