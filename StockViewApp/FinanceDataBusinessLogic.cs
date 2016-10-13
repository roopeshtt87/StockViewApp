using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;

namespace StockViewApp
{
    public interface IFinanceDataBusinessLogic
    {
        List<StockData> GetStockDetails(List<StockID> stockIdList);
        List<string> GetCompaniesList(string exchange);
    }
    public class GoogleFinanceDataBL : IFinanceDataBusinessLogic
    {
        private string url;
        private string ListCompaniesUrl;
        private static string ListExchsUrl = "https://www.google.com/googlefinance/disclaimer/";
        public GoogleFinanceDataBL()
        {
            url = "http://www.google.com/finance/info?infotype=infoquoteall&q=";
            ListCompaniesUrl = "https://www.google.com/finance?q=%5B%28exchange+%3D%3D+%22{0}%22%29%5D&restype=company&noIL=1&num=2000";
        }
        public List<StockData> GetStockDetails(List<StockID> stockIdList)
        {
            List<StockData> stockDetails = new List<StockData>();
            try
            {
                if (stockIdList.Count > 0)
                {
                    string googleApi = url;
                    foreach (StockID s in stockIdList)
                    {
                        googleApi += (s.CompanyNameShort + ':' + s.Exchange + ',');
                    }
                    stockDetails = SendRequest(googleApi);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: GetStockDetails failed+ '\n'" + ex.Message);
            }
            return stockDetails;
        }

        private List<StockData> SendRequest(string getUrl)
        {
            Stream objStream;
            try
            {
                WebRequest wr;
                wr = WebRequest.Create(getUrl);

                objStream = wr.GetResponse().GetResponseStream();
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: SendRequest failed+ '\n'" + ex.Message);
            }

            return ExtractStockInfo(objStream);
        }

        private List<StockData> ExtractStockInfo(Stream objStream)
        {
            List<StockData> allData = new List<StockData>();
            string sLine = "";

            try
            {
                StreamReader objReader = new StreamReader(objStream);
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null && sLine.Contains('{'))
                    {
                        StockData data = new StockData();
                        while (!sLine.Contains('}'))
                        {
                            sLine = objReader.ReadLine();
                            if (sLine.Contains("\"t\" : "))
                            {
                                string[] s = sLine.Split('\"');
                                data.CompanyNameShort = s[3];
                            }
                            else if (sLine.Contains("\"e\" : "))
                            {
                                string[] s = sLine.Split('\"');
                                data.Exchange = s[3];
                            }
                            else if (sLine.Contains("\"l\" : "))
                            {
                                string[] s = sLine.Split('\"');
                                data.value = float.Parse(s[3], CultureInfo.InvariantCulture.NumberFormat);
                            }
                            else if (sLine.Contains("\"cp\" : "))
                            {
                                string[] s = sLine.Split('\"');
                                data.percentChanged = float.Parse(s[3], CultureInfo.InvariantCulture.NumberFormat);
                            }
                            else if (sLine.Contains("\"hi52\" : "))
                            {
                                string[] s = sLine.Split('\"');
                                data.hi52 = float.Parse(s[3], CultureInfo.InvariantCulture.NumberFormat);
                            }
                            else if (sLine.Contains("\"lo52\" : "))
                            {
                                string[] s = sLine.Split('\"');
                                data.lo52 = float.Parse(s[3], CultureInfo.InvariantCulture.NumberFormat);
                            }
                        }
                        allData.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: ExtractExchangesList failed+ '\n'" + ex.Message);
            }
            return allData;
        }

        public List<string> GetCompaniesList(string exchange)
        {
            string url = String.Format(ListCompaniesUrl, exchange);
            List<string> companies = new List<string>();
            try
            {
                WebRequest wr;
                wr = WebRequest.Create(url);

                Stream objStream;
                objStream = wr.GetResponse().GetResponseStream();
                companies = ExtractCompaniesList(objStream, exchange);
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: GetCompaniesList failed+ '\n'" + ex.Message);
            }
            return companies;
        }

        private List<string> ExtractCompaniesList(Stream objStream, string exchange)
        {
            List<string> allCompanies = new List<string>();
            try
            {
                StreamReader objReader = new StreamReader(objStream);
                string page = objReader.ReadToEnd();
                string[] lines = page.Split(new string[] { exchange + ':' }, StringSplitOptions.None);
                bool bFistItem = true;
                foreach (string s in lines)
                {
                    if (!bFistItem)
                    {
                        string sym = s.Substring(0, s.IndexOf("&sq"));
                        if (!String.IsNullOrWhiteSpace(sym))
                        {
                            if (!allCompanies.Contains(sym))
                            {
                                allCompanies.Add(sym);
                            }
                        }
                    }
                    else
                        bFistItem = false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: ExtractCompaniesList failed+ '\n'" + ex.Message);
            }
            return allCompanies;
        }

        public static List<string> GetExchanges()
        {
            List<string> exchanges = new List<string>();
            try
            {
                WebRequest wr;
                wr = WebRequest.Create(ListExchsUrl);

                Stream objStream;
                objStream = wr.GetResponse().GetResponseStream();
                exchanges = ExtractExchangesList(objStream);
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: GetExchanges failed + '\n'" + ex.Message);
            }
            return exchanges;
        }

        private static List<string> ExtractExchangesList(Stream objStream)
        {
            List<string> all = new List<string>();
            try
            {
                StreamReader objReader = new StreamReader(objStream);
                string page = objReader.ReadToEnd();
                page = page.Substring(page.IndexOf("<table>"));
                page = page.Substring(0, page.IndexOf("</table>"));
                string[] lines = page.Split(new string[] { "<tr>" }, StringSplitOptions.None);
                int bFistTwoPass = 0;
                foreach (string s in lines)
                {
                    if (bFistTwoPass == 2)
                    {
                        string[] items = s.Split(new string[] { "<td>", "</td>" }, StringSplitOptions.None);
                        string sym = items[3];
                        sym = sym.Trim(new Char[] { ' ', '\n' });
                        if (!String.IsNullOrWhiteSpace(sym))
                        {
                            if (!all.Contains(sym))
                            {
                                all.Add(sym);
                            }
                        }
                    }
                    else
                        bFistTwoPass++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GoogleFinanceDataBL :: ExtractExchangesList failed" + '\n' + ex.Message);
            }
            return all;
        }
    }
}
