using System;
using System.Windows;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private Grid mainGrid;
        private DispatcherTimer timer;   //  Generation timer
        private int genCounter;
        private AdWindow adWindow1;
        private AdWindow adWindow2;

        public MainWindow()
        {
            InitializeComponent();
            mainGrid = new Grid(MainCanvas);

            timer = new DispatcherTimer();
            timer.Tick += OnTimer;
            timer.Interval = TimeSpan.FromMilliseconds(200);
        }

        private void StartAd()
        {
            adWindow1 = CreateAdWindow(0);
            adWindow2 = CreateAdWindow(1);
        }

        private AdWindow CreateAdWindow(int index)
        {
            var adWindow = new AdWindow(this);
            adWindow.Closed += AdWindowOnClosed;
            adWindow.Top = this.Top + (330 * index) + 70;
            adWindow.Left = this.Left + 240;
            adWindow.Show();
            return adWindow;
        }

        private void AdWindowOnClosed(object sender, EventArgs eventArgs)
        {
            var closedWindow = sender as AdWindow;
            closedWindow.Closed -= AdWindowOnClosed;

            if (closedWindow == adWindow1)
            {
                adWindow1 = null;
            }
            else if (closedWindow == adWindow2)
            {
                adWindow2 = null;
            }
        }

        private void Button_OnClick(object sender, EventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
                ButtonStart.Content = "Stop";
                StartAd();
            }
            else
            {
                timer.Stop();
                ButtonStart.Content = "Start";
                CloseAdWindows();
            }
        }

        private void CloseAdWindows()
        {
            adWindow1?.Close();
            adWindow2?.Close();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            mainGrid.Update();
            genCounter++;
            lblGenCount.Content = "Generations: " + genCounter;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Clear();
        }

        protected override void OnClosed(EventArgs e)
        {
            timer.Stop();
            timer.Tick -= OnTimer;
            CloseAdWindows();
            base.OnClosed(e);
        }
    }
}
