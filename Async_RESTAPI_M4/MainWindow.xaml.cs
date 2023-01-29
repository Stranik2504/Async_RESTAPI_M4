using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Async_RESTAPI_M4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            var stopwatch = Stopwatch.StartNew();
            LoadDataSync();
            stopwatch.Stop();
            TextBlockInfo.Text += $"\n------------------\nTime elapsed: {stopwatch.ElapsedMilliseconds} ms\n";
        }

        private async void ButtonAsync_Click(object sender, RoutedEventArgs e)
        {
            var stopwatch = Stopwatch.StartNew();
            // await LoadDataAsync();
            await LoadDataAsyncParallel();
            stopwatch.Stop();
            TextBlockInfo.Text += $"\n------------------\nTime elapsed: {stopwatch.ElapsedMilliseconds} ms\n";
        }
        
        private async Task LoadDataAsync()
        {
            var sites = PrepareLoadSizes();
            
            foreach (var site in sites)
            {
                var data = await Task.Run(() => LoadSite(site));
                PrintData(data);
            }
        }
        
        private async Task LoadDataAsyncParallel()
        {
            var sites = PrepareLoadSizes();
            var tasks = sites.Select(site => Task.Run(() => LoadSite(site))).ToList();

            var dataModels = await Task.WhenAll(tasks);
            
            foreach (var data in dataModels)
            {
                PrintData(data);
            }
        }
        
        private void LoadDataSync()
        {
            var sites = PrepareLoadSizes();
            
            foreach (var data in sites.Select(LoadSite))
            {
                PrintData(data);
            }
        }
        
        private void PrintData(DataModel data)
        {
            TextBlockInfo.Text += $"\nUrl: {data.Url}(Length: {data.Data.Length})";   
        }
        
        private static IEnumerable<string> PrepareLoadSizes()
        {
            var sizes = new List<string>()
            {
                "https://google.com",
                "https://my.progtime.net",
                // ...
            };
            
            return sizes;
        }

        private DataModel LoadSite(string site)
        {
            var data = new DataModel
            {
                Url = site
            };

            var client = new WebClient();
            data.Data = client.DownloadString(site);
            
            /*Dispatcher.BeginInvoke(new System.Action(() =>
            {
                TextBlockInfo.Text = "Info";
            }));*/

            return data;
        }
    }
}