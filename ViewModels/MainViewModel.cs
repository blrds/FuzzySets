using Fuzzy.Infrastructure.Commands;
using Fuzzy.Models;
using Fuzzy.ViewModels.Base;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Fuzzy.ViewModels
{
    class MainViewModel : ViewModel
    {
        public ObservableCollection<Alpha> A { get; set; } = new ObservableCollection<Alpha>();
        public ObservableCollection<Alpha> B { get; set; } = new ObservableCollection<Alpha>();
        public ObservableCollection<Alpha> C { get; set; } = new ObservableCollection<Alpha>();

        private string oM;
        public string OutputMessage { get => oM; set { oM = value; OnPropertyChanged("OutputMessage"); } }
        public PlotModel model { get; private set; } = new PlotModel();

        public SolidColorBrush Greater { get; set; }    
        public SolidColorBrush GreaterEqual { get; set; }    
        public SolidColorBrush Less { get; set; }    
        public SolidColorBrush LessEqual { get; set; }    
        public SolidColorBrush Equal { get; set; }    
        public SolidColorBrush NonEqual { get; set; }    
        public ICommand DrawACommand { get; }
        private bool canDrawACommnadExecute(object p)
        {
            if (A.GroupBy(s => new { s.Slice }).Where(g => g.Count() > 1).Select(g => g.Key).Any()) return false;//duplictes
            if (A.Where(x => x.Less >= x.Greater).Any()) return false; //first point bigger than second
            List<Alpha> a = A.OrderBy(x => x.Slice).ToList();
            for (int i = 0; i < A.Count - 1; i++)//Convex
            {
                if (a[i].Less > a[i + 1].Less || a[i].Greater < a[i + 1].Greater)
                    return false;

            }

            if (A.Count() < 2) return false;
            return true;
        }
        private void DrawACommandExecuted(object p)
        {
            LineSeries line = new LineSeries();
            line.SeriesGroupName = "a";
            foreach (var a in A)
                line.Points.Add(new DataPoint(a.Less, a.Slice));
            foreach (var a in A.Reverse())
                line.Points.Add(new DataPoint(a.Greater, a.Slice));
            model.Series.Remove(model.Series.Where(x => x.SeriesGroupName == "a").First());
            model.Series.Add(line);
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
            OutputMessage = "";
        }

        public ICommand DrawBCommand { get; }
        private bool canDrawBCommnadExecute(object p)
        {
            if (B.GroupBy(s => new { s.Slice }).Where(g => g.Count() > 1).Select(g => g.Key).Any()) return false;//duplictes
            if (B.Where(x => x.Less >= x.Greater).Any()) return false;//first point bigger than second
            List<Alpha> a = B.OrderBy(x => x.Slice).ToList();
            for (int i = 0; i < A.Count - 1; i++)//Convex
            {
                if (a[i].Less > a[i + 1].Less || a[i].Greater < a[i + 1].Greater)
                    return false;
            }

            if (B.Count() < 2) return false;
            return true;
        }
        private void DrawBCommandExecuted(object p)
        {
            LineSeries line = new LineSeries();

            line.SeriesGroupName = "b";
            foreach (var a in B)
                line.Points.Add(new DataPoint(a.Less, a.Slice));
            foreach (var a in B.Reverse())
                line.Points.Add(new DataPoint(a.Greater, a.Slice));

            model.Series.Remove(model.Series.Where(x => x.SeriesGroupName == "b").First());
            model.Series.Add(line);
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
            OutputMessage = "";
        }

        public ICommand DrawCCommand { get; }
        private bool canDrawCCommnadExecute(object p) => (C.Count >= 2);
        private void DrawCCommandExecuted(object p)
        {
            LineSeries line = new LineSeries();

            line.SeriesGroupName = "c";
            foreach (var a in C)
                line.Points.Add(new DataPoint(a.Less, a.Slice));
            foreach (var a in C.Reverse())
                line.Points.Add(new DataPoint(a.Greater, a.Slice));

            model.Series.Remove(model.Series.Where(x => x.SeriesGroupName == "c").First());
            model.Series.Add(line);
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
            OutputMessage = "";
        }

        public void OnAChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null)
                foreach (INotifyPropertyChanged oldItem in e.OldItems)
                    oldItem.PropertyChanged -= OnAItemChanged;

            if (e.NewItems != null)
                foreach (INotifyPropertyChanged newItem in e.NewItems)
                    newItem.PropertyChanged += OnAItemChanged;
        }
        public void OnBChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (INotifyPropertyChanged oldItem in e.OldItems)
                    oldItem.PropertyChanged -= OnBItemChanged;

            if (e.NewItems != null)
                foreach (INotifyPropertyChanged newItem in e.NewItems)
                    newItem.PropertyChanged += OnBItemChanged;
        }
        public void OnAItemChanged(object sender, PropertyChangedEventArgs e)
        {
            OutputMessage = "";
            if (A.GroupBy(s => new { s.Slice }).Where(g => g.Count() > 1).Select(g => g.Key).Any())
            {
                OutputMessage += "Дублирующийся срез в A\n";
            } //duplictes
            if (A.Where(x => x.Less >= x.Greater).Any())
            {
                OutputMessage += "Начальная граница одного из срезов A больше или равен конечному\n";
            } //first point bigger than second
            if (A.Count() >= 2)
            {
                List<Alpha> a = A.OrderBy(x => x.Slice).ToList();
                for (int i = 0; i < A.Count - 1; i++)//Convex
                {
                    if (a[i].Less > a[i + 1].Less || a[i].Greater < a[i + 1].Greater)
                    {
                        OutputMessage += "В A присутсвует выпуклость\n";
                    }
                }
            }
            if (A.Count()<2) {
                OutputMessage += "В A недостаточно срезов\n";
            }
        }
        private void OnBItemChanged(object sender, PropertyChangedEventArgs e)
        {

            OutputMessage = "";
            if (B.GroupBy(s => new { s.Slice }).Where(g => g.Count() > 1).Select(g => g.Key).Any())
            {
                OutputMessage += "Дублирующийся срез в B\n";
            } //duplictes
            if (B.Where(x => x.Less >= x.Greater).Any())
            {
                OutputMessage += "Начальная граница одного из срезов B больше или равен конечному\n";
            } //first point bigger than second
            if (B.Count() >= 2)
            {
                List<Alpha> a = B.OrderBy(x => x.Slice).ToList();
                for (int i = 0; i < B.Count - 1; i++)//Convex
                {
                    if (a[i].Less > a[i + 1].Less || a[i].Greater < a[i + 1].Greater)
                    {
                        OutputMessage += "В B присутсвует выпуклость\n";
                    }
                }
            }

            if (B.Count() < 2)
            {
                OutputMessage += "В B недостаточно срезов\n";
            }
        }
        public MainViewModel()
        {
            model.Axes.Add(new LinearAxis());
            model.Axes.Last().Position = AxisPosition.Bottom;
            model.Axes.Add(new LinearAxis());
            model.Axes.Last().Position = AxisPosition.Left;
            A.CollectionChanged += OnAChanged;
            B.CollectionChanged += OnBChanged;
            A.Add(new Alpha(0, 0, 0));
            A.Add(new Alpha(1, 0, 0));
            B.Add(new Alpha(0, 0, 0));
            B.Add(new Alpha(1, 0, 0));
            Greater = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            GreaterEqual = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Less = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            LessEqual = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Equal = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            NonEqual = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            OutputMessage = "";
            DrawACommand = new LambdaCommand(DrawACommandExecuted, canDrawACommnadExecute);
            DrawBCommand = new LambdaCommand(DrawBCommandExecuted, canDrawBCommnadExecute);
            DrawCCommand = new LambdaCommand(DrawCCommandExecuted, canDrawCCommnadExecute);
        }
    }
}
