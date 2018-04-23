using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace AnimateChangedRows
{
    public class ViewModel
    {

        public ObservableCollection<TestData> List
        {
            get;
            set;
        }
        DispatcherTimer timer = new DispatcherTimer();
        Random random = new Random();


        public ViewModel()
        {
            List = new ObservableCollection<TestData>();
            PopulateData();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.IsEnabled = true;
        }

        private void PopulateData()
        {
            for (int i = 0; i < 30; i++)
            {
                var t = new TestData(i, "Element" + i.ToString());
                List.Add(t);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int rowHandle = random.Next(List.Count);
            List[rowHandle].Number = random.Next(1000);
        }
    }

    public class TestData : INotifyPropertyChanged
    {
        int number;
        string text;

        public TestData(int number, string text)
        {
            this.number = number;
            this.text = text;
        }
        
        
        
        public int Number
        {
            get { return number; }
            set
            {
                if (number == value)
                    return;
                number = value;
                NotifyChanged("Number");
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;
                text = value;
                NotifyChanged("Text");
            }
        }
        void NotifyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
