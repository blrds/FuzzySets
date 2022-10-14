using Fuzzy.Infrastructure.Commands;
using Fuzzy.Models;
using Fuzzy.ViewModels.Base;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Fuzzy.ViewModels
{
    class MainViewModel : ViewModel
    {

        private string oM;
        public string OutputMessage { get => oM; set { oM = value; OnPropertyChanged("OutputMessage"); } }
        public PlotModel model { get; private set; } = new PlotModel();
        public SolidColorBrush Greater { get; set; }    
        public SolidColorBrush GreaterEqual { get; set; }    
        public SolidColorBrush Less { get; set; }    
        public SolidColorBrush LessEqual { get; set; }    
        public SolidColorBrush Equal { get; set; }    
        public SolidColorBrush NonEqual { get; set; }
        #region A
        public ObservableCollection<Alpha> A { get; set; } = new ObservableCollection<Alpha>();
        public ICommand DrawACommand { get; }

        private bool canABe() {
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
        private bool canDrawACommnadExecute(object p)=>canABe();
        private void DrawACommandExecuted(object p)
        {
            LineSeries line = new LineSeries();
            line.SeriesGroupName = "a";
            foreach (var a in A.OrderBy(x=>x.Slice))
                line.Points.Add(new DataPoint(a.Less, a.Slice));
            foreach (var a in A.OrderByDescending(x=>x.Slice))
                line.Points.Add(new DataPoint(a.Greater, a.Slice));

            if (model.Series.Where(x => x.SeriesGroupName == "a").Any())
                model.Series.Remove(model.Series.Where(x => x.SeriesGroupName == "a").First());
            model.Series.Add(line);
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
            OutputMessage = "";
        }
        public void OnAChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (INotifyPropertyChanged oldItem in e.OldItems)
                    oldItem.PropertyChanged -= OnAItemChanged;

            if (e.NewItems != null)
                foreach (INotifyPropertyChanged newItem in e.NewItems)
                    newItem.PropertyChanged += OnAItemChanged;
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
                        OutputMessage += "В A не выполняется условие вложенности\n";
                    }
                }
            }
            if (A.Count() < 2)
            {
                OutputMessage += "В A недостаточно срезов\n";
            }
        }
        #endregion

        #region B
        public ObservableCollection<Alpha> B { get; set; } = new ObservableCollection<Alpha>();
        public ICommand DrawBCommand { get; }

        private bool canBBe()
        {
            if (B.GroupBy(s => new { s.Slice }).Where(g => g.Count() > 1).Select(g => g.Key).Any()) return false;//duplictes
            if (B.Where(x => x.Less >= x.Greater).Any()) return false;//first point bigger than second
            List<Alpha> a = B.OrderBy(x => x.Slice).ToList();
            for (int i = 0; i < B.Count - 1; i++)//Convex
            {
                if (a[i].Less > a[i + 1].Less || a[i].Greater < a[i + 1].Greater)
                    return false;
            }

            if (B.Count() < 2) return false;
            return true;
        }
        private bool canDrawBCommnadExecute(object p) => canBBe();
        
        private void DrawBCommandExecuted(object p)
        {
            LineSeries line = new LineSeries();
            line.SeriesGroupName = "b";
            foreach (var a in B.OrderBy(x => x.Slice))
                line.Points.Add(new DataPoint(a.Less, a.Slice));
            foreach (var a in B.OrderByDescending(x=>x.Slice))
                line.Points.Add(new DataPoint(a.Greater, a.Slice));

            if (model.Series.Where(x => x.SeriesGroupName == "b").Any())
                model.Series.Remove(model.Series.Where(x => x.SeriesGroupName == "b").First());
            model.Series.Add(line);
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
            OutputMessage = "";
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
                        OutputMessage += "В B не выполняется условие вложенности\n";
                    }
                }
            }

            if (B.Count() < 2)
            {
                OutputMessage += "В B недостаточно срезов\n";
            }
        }
        #endregion

        #region C
        public ObservableCollection<Alpha> C { get; set; } = new ObservableCollection<Alpha>();
        public ICommand DrawCCommand { get; }
        private bool canDrawCCommnadExecute(object p) => (C.Count >= 2);
        private void DrawCCommandExecuted(object p)
        {
            LineSeries line = new LineSeries();
            line.SeriesGroupName = "c";
            foreach (var a in C.OrderBy(x=>x.Slice))
                line.Points.Add(new DataPoint(a.Less, a.Slice));
            foreach (var a in C.OrderByDescending(x => x.Slice))
                line.Points.Add(new DataPoint(a.Greater, a.Slice));
            if(model.Series.Where(x => x.SeriesGroupName == "c").Any())
            model.Series.Remove(model.Series.Where(x => x.SeriesGroupName == "c").First());
            model.Series.Add(line);
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
            OutputMessage = "";
        }
        #endregion

        #region Sum
        public ICommand SumCommand { get; }
        private bool canSumCommnadExecute(object p) => canABe() && canBBe();
        private void SumCommandExecuted(object p)
        {
            C=new ObservableCollection<Alpha>( Core.Sum(A.OrderBy(x=>x.Slice).ToList(), B.OrderBy(x=>x.Slice).ToList()));
            OnPropertyChanged("C");
        }
        #endregion

        #region Sub
        public ICommand SubCommand { get; }
        private bool canSubCommnadExecute(object p) => canABe() && canBBe();
        private void SubCommandExecuted(object p)
        {
            C = new ObservableCollection<Alpha>(Core.Sub(A.OrderBy(x => x.Slice).ToList(), B.OrderBy(x => x.Slice).ToList()));
            OnPropertyChanged("C");
        }
        #endregion

        #region Mult
        public ICommand MultCommand { get; }
        private bool canMultCommnadExecute(object p) => canABe() && canBBe();
        private void MultCommandExecuted(object p)
        {
            C = new ObservableCollection<Alpha>(Core.Mult(A.OrderBy(x => x.Slice).ToList(), B.OrderBy(x => x.Slice).ToList()));
            OnPropertyChanged("C");
        }
        #endregion

        #region Div
        public ICommand DivCommand { get; }
        private bool canDivCommnadExecute(object p) => canABe() && canBBe();
        private void DivCommandExecuted(object p)
        {
            try
            {
                C = new ObservableCollection<Alpha>(Core.Div(A.OrderBy(x => x.Slice).ToList(), B.OrderBy(x => x.Slice).ToList()));
                OnPropertyChanged("C");
            }catch(ArgumentException e)
            {
                OutputMessage="Деление на 0";
            }
        }
        #endregion

        #region Comp
        public ICommand CompCommand { get; }
        private bool canCompCommnadExecute(object p) => canABe() && canBBe();
        private void CompCommandExecuted(object p)
        {
            var a = Core.Comp(A.OrderBy(x => x.Slice).ToList(), B.OrderBy(x => x.Slice).ToList());
            Greater = a[0] ? new SolidColorBrush(Color.FromRgb(0, 255, 0)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));
            GreaterEqual  = a[1]?new SolidColorBrush(Color.FromRgb(0, 255, 0)): new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Less = a[2]?new SolidColorBrush(Color.FromRgb(0, 255, 0)): new SolidColorBrush(Color.FromRgb(255, 0, 0));
            LessEqual = a[3]?new SolidColorBrush(Color.FromRgb(0, 255, 0)): new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Equal = a[4]?new SolidColorBrush(Color.FromRgb(0, 255, 0)): new SolidColorBrush(Color.FromRgb(255, 0, 0));
            NonEqual = a[5]?new SolidColorBrush(Color.FromRgb(0, 255, 0)): new SolidColorBrush(Color.FromRgb(255, 0, 0));
            OnPropertyChanged(nameof(Greater));
            OnPropertyChanged(nameof(GreaterEqual));
            OnPropertyChanged(nameof(Less));
            OnPropertyChanged(nameof(LessEqual));
            OnPropertyChanged(nameof(Equal));
            OnPropertyChanged(nameof(NonEqual));
        }
        #endregion

        #region Clean
        public ICommand CleanCommand { get; }
        private bool canCleanCommnadExecute(object p)=>true;
        private void CleanCommandExecuted(object p)
        {
            model.Series.Clear();
            OnPropertyChanged("model");
            model.InvalidatePlot(true);
        }
        #endregion

        #region Exit
        public ICommand ExitCommand { get; }
        private bool canExitCommnadExecute(object p) => true;
        private void ExitCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion
        public MainViewModel()
        {
            model.Axes.Add(new LinearAxis());
            model.Axes.Last().Position = AxisPosition.Bottom;
            model.Axes.Add(new LinearAxis());
            model.Axes.Last().Position = AxisPosition.Left;
            A.CollectionChanged += OnAChanged;
            B.CollectionChanged += OnBChanged;
            A.Add(new Alpha(0, 0, 3));
            A.Add(new Alpha(1, 1, 2));
            B.Add(new Alpha(0, 0, 6));
            B.Add(new Alpha(1, 2, 4));
            B.Add(new Alpha(0.5, 0.5, 5.5));
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
            SumCommand = new LambdaCommand(SumCommandExecuted, canSumCommnadExecute);
            SubCommand = new LambdaCommand(SubCommandExecuted, canSubCommnadExecute);
            MultCommand = new LambdaCommand(MultCommandExecuted, canMultCommnadExecute);
            DivCommand = new LambdaCommand(DivCommandExecuted, canDivCommnadExecute);
            CompCommand = new LambdaCommand(CompCommandExecuted, canCompCommnadExecute);
            ExitCommand = new LambdaCommand(ExitCommandExecuted, canExitCommnadExecute);
            CleanCommand = new LambdaCommand(CleanCommandExecuted, canCleanCommnadExecute);
        }
    }
}
