using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;

namespace AnimateChangedRows {
    public partial class Window1 : Window {
        BindingList<TestData> list = new BindingList<TestData>();

        Dictionary<int, AnimationElement> animationElements =
            new Dictionary<int, AnimationElement>();

        DispatcherTimer timer = new DispatcherTimer();

        public Window1() {
            InitializeComponent();

            for (int i = 0; i < 100; i++)
                list.Add(new TestData() { Text = "Element" + i.ToString(), Number = i });
            grid.ItemsSource = list;

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.IsEnabled = true;
        }

        Random randow = new Random();

        void timer_Tick(object sender, EventArgs e) {
            int rowHandle = grid.GetRowHandleByListIndex(randow.Next(list.Count));
            grid.SetCellValue(rowHandle, grid.Columns[1], randow.Next(1000));
        }

        private void grid_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
            if (e.Column.FieldName == "AnimationElement") {
                e.Value = GetAnimationElement(e.ListSourceRowIndex);
            }
        }

        AnimationElement GetAnimationElement(int listIndex) {
            AnimationElement element;
            if (!animationElements.TryGetValue(listIndex, out element)) {
                element = new AnimationElement();
                animationElements[listIndex] = element;
            }
            return element;
        }

        private void view_CellValueChanged(object sender, CellValueEventArgs e) {
            GetAnimationElement(grid.GetListIndexByRowHandle(e.RowHandle)).StartAnimation();
        }
    }

    public class TestData : INotifyPropertyChanged {
        int number;
        string text;
        public int Number {
            get { return number; }
            set {
                if (number == value)
                    return;
                number = value;
                NotifyChanged("Number");
            }
        }
        public string Text {
            get { return text; }
            set {
                if (text == value)
                    return;
                text = value;
                NotifyChanged("Text");
            }
        }
        void NotifyChanged(string propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }

    public class AnimationElement : FrameworkContentElement {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(AnimationElement),
            new PropertyMetadata(Colors.White, OnChanged));
        static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        }
        public Color Color {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public void StartAnimation() {
            ColorAnimationUsingKeyFrames animation =
                new ColorAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new DiscreteColorKeyFrame(Color.FromArgb(255, 255, 255, 255),
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            animation.KeyFrames.Add(new LinearColorKeyFrame(Color.FromArgb(255, 200, 200, 200),
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            animation.KeyFrames.Add(new LinearColorKeyFrame(Color.FromArgb(255, 255, 255, 255),
                KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))));

            this.BeginAnimation(AnimationElement.ColorProperty, animation,
                HandoffBehavior.SnapshotAndReplace);
        }
    }

    public class ColorToBrushConverter : MarkupExtension, IValueConverter {
        object IValueConverter.Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            return new SolidColorBrush((Color)value);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture) {
            return value;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
}
