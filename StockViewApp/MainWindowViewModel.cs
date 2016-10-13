using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;

namespace StockViewApp
{
    public interface IMainWindowViewModel
    {
        ///// <summary>
        ///// Id
        ///// </summary>
        //String Id { get; }
        //TODO
    }
    public class MainWindowViewModel : IMainWindowViewModel, INotifyPropertyChanged
    {
        public IDataManager dataManager;
        public ObservableCollection<StockData> list1 { get; set; }
        public ObservableCollection<string> listOfCompanies { get; set; }
        public string Exchange { get; set; }
        private readonly BackgroundWorker workerThread = new BackgroundWorker();
        public Dispatcher currentDispatcher;
        private string _message;
        public string ErrorMessage
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged("ErrorMessage");
            }
        }
        private string newSymbol;
        public string NewSymbol
        {
            get
            {
                return newSymbol;
            }
            set
            {
                newSymbol = value;
                OnPropertyChanged("NewSymbol");
            }
        }
        public ICommand AddCommand
        {
            get;
            set;
        }
        public ICommand DeleteCommand
        {
            get;
            set;
        }


        public MainWindowViewModel(string vmName)
        {
            Exchange = vmName;
            list1 = new ObservableCollection<StockData>();
            listOfCompanies = new ObservableCollection<string>();
            currentDispatcher = Dispatcher.CurrentDispatcher;

            AddCommand = new RelayCommand(ExecuteAddCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            try
            {

                dataManager = new DataManager(vmName, new GoogleFinanceDataBL());

                list1.Clear();
                List<StockData> stockList = dataManager.GetStocks();
                foreach (var v in stockList)
                {
                    list1.Add(v);
                }
                workerThread.DoWork += new DoWorkEventHandler(workerThread_DoWork);
                workerThread.RunWorkerAsync();


                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                string str = "Failed to initialize the view";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            try
            {
                List<string> list = new List<string>();
                list = dataManager.GetCompanySymbols();
                while ((list.Count == 0))
                {
                    Thread.Sleep(1000);
                    list = dataManager.GetCompanySymbols();
                }

                foreach (var v in list)
                {
                    listOfCompanies.Add(v);
                }
            }
            catch (Exception ex)
            {
                string str = "Failed to get the list of stock(company) symbols";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
        }

        public void ExecuteAddCommand(object obj)
        {
            try
            {
                dataManager.AddStockSymbol(NewSymbol);
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                string str = "Failed to add stock(company) symbol";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
        }
        public void ExecuteDeleteCommand(object obj)
        {
            try
            {
                List<string> selectedItems = new List<string>();
                selectedItems = list1.Where(s => s.IsSelected == true).Select(s => s.CompanyNameShort).ToList();
                dataManager.DeleteStockSymbol(selectedItems);
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                string str = "Failed to delete stock(company) symbol";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
            
        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    List<StockData> stockList = dataManager.GetStocks();
                    this.currentDispatcher.BeginInvoke((Action)delegate()
                    {
                        List<string> selectedItems = new List<string>();
                        selectedItems = list1.Where(s => s.IsSelected == true).Select(s => s.CompanyNameShort).ToList();
                        list1.Clear();

                        foreach (StockData s in stockList)
                        {
                            if (selectedItems.Contains(s.CompanyNameShort))
                                s.IsSelected = true;
                            list1.Add(s);
                        }
                    });
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                string str = "Failed to get stock information";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
