using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockViewApp;

namespace UnitTests.Mocks
{
    public class MockDataManager : IDataManager
    {
        private string Exchange = String.Empty;
        private IFinanceDataBusinessLogic financeDataBL;
        public MockDataManager(string Exchange, IFinanceDataBusinessLogic fdBusinessLogic)
        {
            this.Exchange = Exchange;
            financeDataBL = fdBusinessLogic;
        }
        public List<StockID> StockIDList
        {
            get
            {
                return new List<StockID>() { new StockID("NSE", "TCS"), new StockID("NSE", "ONGC") };
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IFinanceDataBusinessLogic DataBL
        {
            get { throw new NotImplementedException(); }
        }

        public List<StockData> GetStocks()
        {
            return DataBL.GetStockDetails(StockIDList);
        }

        public void AddStockSymbol(string sym)
        {
            throw new NotImplementedException();
        }

        public void DeleteStockSymbol(List<string> selectedItems)
        {
            throw new NotImplementedException();
        }

        public bool IsSymbolPresent(string sym)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCompanySymbols()
        {
            throw new NotImplementedException();
        }
    }
}
