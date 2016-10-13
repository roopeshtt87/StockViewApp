using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace StockViewApp
{
    public class StockData//IStock
    {
        public bool IsSelected { get; set; }
        public string Exchange { get; set; }
        public string CompanyNameShort { get; set; }
        public float value { get; set; }
        public float percentChanged { get; set; }
        public float hi52 { get; set; }
        public float lo52 { get; set; }
    }
    public class StockID
    {
        public StockID(string exch, string Company)
        {
            Exchange = exch;
            CompanyNameShort = Company;
        }
        public string Exchange;
        public string CompanyNameShort;
    }
    public interface IDataManager
    {
        List<StockID> StockIDList { get; set; }
        IFinanceDataBusinessLogic DataBL { get; }
        List<StockData> GetStocks();
        void AddStockSymbol(string sym);
        void DeleteStockSymbol(List<string> selectedItems);
        bool IsSymbolPresent(string sym);
        List<string> GetCompanySymbols();
    }
    public class DataManager : IDataManager, INotifyPropertyChanged
    {
        private string Exchange = String.Empty;
        private NameValueCollection StocksConfigured = new NameValueCollection();
        List<string> allCompanies = new List<string>();
        private List<StockID> stockIDList = new List<StockID>();
        public List<StockID> StockIDList
        {
            get { return stockIDList; }
            set 
            { 
                stockIDList = value;
                OnPropertyChanged("StockIDList");
            }
        }

        private List<StockData> Stocks = new List<StockData>();
        private IFinanceDataBusinessLogic financeDataBL;
        private readonly BackgroundWorker workerThread = new BackgroundWorker();
        public DataManager(string Exchange, IFinanceDataBusinessLogic fdBusinessLogic)
        {
            this.Exchange = Exchange;
            financeDataBL = fdBusinessLogic;// new GoogleFinanceDataBL();

            this.PropertyChanged += (obj, args) =>
            {
                if (args.PropertyName == "StockIDList")
                {
                    UpdateStocks();
                }
            };

            workerThread.DoWork += new DoWorkEventHandler(workerThread_DoWork);
            workerThread.RunWorkerAsync();
            StockIDList = SettingsManager.ReadFile().Where(w=> w.Exchange == Exchange).ToList() ;

            BackgroundWorker ListCompaniesWorker = new BackgroundWorker();
            ListCompaniesWorker.DoWork += new DoWorkEventHandler(ListCompaniesWorker_DoWork);
            ListCompaniesWorker.RunWorkerAsync();
        }

        void ListCompaniesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                allCompanies = financeDataBL.GetCompaniesList(Exchange); 
            }
            catch (Exception ex)
            {
                string str = "Failed to get the list of stock symbols";
                LogManager.Write(str + "; Details:\n" + ex.Message);
            }
        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    UpdateStocks();
                }
            }
            catch (Exception ex)
            {
                string str = "Failed to update stock information";
                LogManager.Write(str + "; Details:\n" + ex.Message);
            }
        }
        
        public void AddStockSymbol(string sym)
        {
            try
            {
                if (!IsSymbolPresent(sym))
                {
                    StockIDList.Add(new StockID(Exchange, sym));
                    SettingsManager.WriteToFile(StockIDList);
                }
            }
            catch (Exception ex)
            {
                string str = "Failed to add stock symbol";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                throw new Exception(str + ex.Message);
            } 
        }

        public void DeleteStockSymbol(List<string> selectedItems)
        {
            try
            {
                StockIDList.RemoveAll(s => selectedItems.Any(a => a == s.CompanyNameShort));
                SettingsManager.WriteToFile(StockIDList);
            }
            catch (Exception ex)
            {
                string str = "Failed to delete stock symbols";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                throw new Exception(str + ex.Message);
            } 
        }
        

        public bool IsSymbolPresent(string sym)
        {
            return StockIDList.Exists(x => (x.Exchange == Exchange && x.CompanyNameShort == sym));
        }
        private void UpdateStocks()
        {
            try
            {
                Stocks = financeDataBL.GetStockDetails(StockIDList);
            }
            catch (Exception ex)
            {
                string str = "Failed to update stock informations";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                throw new Exception(str + ex.Message);
            } 
        }
        public List<StockData> GetStocks()
        {
            return Stocks;
        }

        public List<string> GetCompanySymbols()
        {
            try
            {
                if (allCompanies.Count == 0)
                {
                    return allCompanies = financeDataBL.GetCompaniesList(Exchange);
                }
            }
            catch (Exception ex)
            {
                string str = "Failed to get company symbols";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                throw new Exception(str + ex.Message);
            }
            return allCompanies;           
        }

        public static List<string> GetExchanges()
        {
            return GoogleFinanceDataBL.GetExchanges();
        }

        //private IFinanceDataBusinessLogic dataBL = new GoogleFinanceDataBL();
        public IFinanceDataBusinessLogic DataBL
        {
            get { return financeDataBL; } 
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
