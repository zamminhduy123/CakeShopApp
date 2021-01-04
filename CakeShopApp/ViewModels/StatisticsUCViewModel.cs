using CakeShopApp.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShopApp.ViewModels
{
    class StatisticsUCViewModel : BaseViewModel
    {
        private AsyncObservableCollection<int> _months;
        public AsyncObservableCollection<int> Months { get => _months; set { _months = value; OnPropertyChanged(); } }

        private AsyncObservableCollection<int> _years;
        public AsyncObservableCollection<int> Years { get => _years; set { _years = value; OnPropertyChanged(); } }

        private int _selectedMonth;
        public int SelectedMonth { get => _selectedMonth; set { _selectedMonth = value;
                makePieChart();
                OnPropertyChanged(); } }

        private int _selectedYear;
        public int SelectedYear { get => _selectedYear; set { _selectedYear = value;
                Months = new AsyncObservableCollection<int>();
                if (SelectedYear == FirstDay.Year)
                {
                    for (int i = FirstDay.Month; i < 13; i++)
                    {
                        Months.Add(i);
                    }
                    if (SelectedMonth < FirstDay.Month)
                    {
                        SelectedMonth = FirstDay.Month;
                    }
                }
                else if (SelectedYear == DateTime.Now.Year)
                {
                    for (int i = 1; i < DateTime.Now.Month + 1; i++)
                    {
                        Months.Add(i);
                    }
                    if (SelectedMonth > DateTime.Now.Month)
                    {
                        SelectedMonth = DateTime.Now.Month;
                    }
                }
                else
                {
                    for (int i = 1; i < 13; i++)
                    {
                        Months.Add(i);
                    }
                }
                makePieChart();
                makeCartestianChart();
                OnPropertyChanged(); }
        }

        private DateTime _firstDay;
        public DateTime FirstDay { get => _firstDay; set { _firstDay = value; OnPropertyChanged(); } }

        // Chart

        // Biểu đồ tròn - Loại bánh

        private SeriesCollection _pieChartSeriesCollection;
        public SeriesCollection PieChartSeriesCollection { get => _pieChartSeriesCollection; set { _pieChartSeriesCollection = value; OnPropertyChanged(); } }

        public Func<ChartPoint, string> _pointLabel;
        public Func<ChartPoint, string> PointLabel { get => _pointLabel; set { _pointLabel = value; OnPropertyChanged(); } }


        //Biểu đồ cột - Tháng

        private List<string> _labels = new List<string>();
        public List<string> Labels { get => _labels; set { _labels = value; OnPropertyChanged(); } }

        private Func<double, string> _formatter;
        public Func<double, string> Formatter { get => _formatter; set { _formatter = value; OnPropertyChanged(); } }

        private SeriesCollection _cartesianChartCollection;
        public SeriesCollection CartesianChartCollection { get => _cartesianChartCollection; set { _cartesianChartCollection = value; OnPropertyChanged(); } }

        public StatisticsUCViewModel()
        {
            FirstDay = DataProvider.Ins.DB.Invoices.OrderBy(x => x.CreatedDate).First().CreatedDate;
            Years = new AsyncObservableCollection<int>();
            for (int i = FirstDay.Year; i < DateTime.Now.Year + 1; i++)
            {
                Years.Add(i);
            }
            SelectedYear = DateTime.Now.Year;
            SelectedMonth = DateTime.Now.Month;
            makePieChart();
            makeCartestianChart();
        }


        // take data and tranfer to pie chart
        void makePieChart()
        {
            PieChartSeriesCollection = new SeriesCollection();
            int totalCost = 0;
            List<String> names = new List<string>();
            List<int> values = new List<int>();
            foreach (var category in DataProvider.Ins.DB.Categories)
            {
                names.Add(category.Name);
                values.Add(0);
            }
            foreach (var detailinvoice in DataProvider.Ins.DB.InvoiceDetails.Where(x => x.Invoice.CreatedDate.Month == SelectedMonth && x.Invoice.CreatedDate.Year == SelectedYear))
            {
                var product = DataProvider.Ins.DB.Products.Find(detailinvoice.ProductId);
                int cost = detailinvoice.Amount * (product.SellPrice - product.ImportPrice) * (100 - detailinvoice.Discount) / 100;
                totalCost += cost;
                int index = names.IndexOf(DataProvider.Ins.DB.Categories.First(x => x.Id == product.CategoryId).Name);
                values[index] += cost;
            }
            for (int i = 0; i < names.Count; i++)
            {
                double x = (double)(values[i]) / totalCost * 100;
                double percent = Math.Truncate(x * 100) / 100;
                PointLabel = chartPoint => string.Format("{0:P}", chartPoint.Participation);
                PieChartSeriesCollection.Add(new PieSeries { Title = $"{names[i]}", Values = new ChartValues<int> { values[i] }, DataLabels = true, LabelPoint = PointLabel });
            }
        }

        void makeCartestianChart()
        {
            CartesianChartCollection= new SeriesCollection();
            ChartValues
                <int> values = new ChartValues<int>();
            List<String> months = new List<String>();
            List<dynamic> counts = new List<dynamic>();
            for (int i = 0; i < 12; i++)
            {
                months.Add($"Tháng {i + 1}");
                int cost = 0;
                foreach (var invoice in DataProvider.Ins.DB.Invoices.Where(x => x.CreatedDate.Month == (i + 1) && x.CreatedDate.Year == SelectedYear))
                {
                    cost += invoice.Total;
                }
                values.Add(cost);
            }
            CartesianChartCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Revenue",
                        Values = values
                    }
                };
            // collumn name
            Labels = months;
            Formatter = value => value.ToString("N");
        }

    }
}
