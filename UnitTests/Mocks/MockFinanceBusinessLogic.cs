using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StockViewApp;

namespace UnitTests.Mocks
{
    public class MockFinanceBusinessLogic : IFinanceDataBusinessLogic
    {
        public List<StockData> GetStockDetails(List<StockID> stockIdList)
        {
            List<StockData> stockDetails = new List<StockData>();
            foreach (var v in stockIdList)
            {
                stockDetails.Add(new StockData() { CompanyNameShort = v.CompanyNameShort, Exchange = v.Exchange, value = 200, hi52 = 300, lo52 = 100, percentChanged = 1 });
            }
            return stockDetails;
        }

        public List<string> GetCompaniesList(string exchange)
        {
            List<string> companies = new List<string>();
            if (exchange == "NSE")
            {
                companies.Add("TCS");
                companies.Add("ONGC");
                companies.Add("KSCL");
            }
            return companies;
        }
    }
}
