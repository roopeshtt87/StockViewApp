using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.ComponentModel;

namespace StockViewApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Exchange> exchangeList = new ObservableCollection<Exchange>();
        public ObservableCollection<Exchange> ExchangeList
        {
            get
            {
                return exchangeList;
            }
            set
            {
                exchangeList = value;
                OnPropertyChanged("ExchangeList");
            }
        }

        public Exchange selectedExchange = new Exchange();
        public Exchange SelectedExchange
        {
            get
            {
                return selectedExchange;
            }
            set
            {
                if (value != null && (value.Name != selectedExchange.Name))
                {
                    selectedExchange = value;
                    OnPropertyChanged("SelectedExchange");
                }
            }
        }

        private string newExchSymbol;
        public string NewExchSymbol
        {
             get
            {
                return newExchSymbol;
            }
            set
            {
                newExchSymbol = value;
                OnPropertyChanged("NewExchSymbol");
            }
        }

        public ICommand AddExchCommand
        {
            get;
            set;
        }
        public ICommand DeleteExchCommand
        {
            get;
            set;
        }
       
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

        private Dictionary<String, int> ExchangeVMIndexes = new Dictionary<string, int>();
        private ObservableCollection<IMainWindowViewModel> allExchangeVMs = new ObservableCollection<IMainWindowViewModel>();
        public ObservableCollection<string> listOfExchanges { get; set; }
        public MainWindow()
        {       
            InitializeComponent();
            try
            {
                trayNotifyIcon = new TrayNotifyIcon();
                trayNotifyIcon.MouseClick += () =>
                                            {
                                                if (this.WindowState != WindowState.Minimized)//this.ShowInTaskbar)
                                                {
                                                    this.Hide();
                                                    this.WindowState = WindowState.Minimized;
                                                    this.ShowInTaskbar = false;
                                                }
                                                else
                                                {
                                                    this.Show();
                                                    this.WindowState = WindowState.Normal;
                                                    this.ShowInTaskbar = true;
                                                }
                                            };

                SetNotifyIcon();
                List<StockID> stockIDList = SettingsManager.ReadFile();
                int i = 0;
                foreach (var s in stockIDList.Select(s => s.Exchange).Distinct().ToList())
                {
                    ExchangeList.Add(new Exchange(s));
                    ExchangeVMIndexes[s] = i;
                    allExchangeVMs.Add(new MainWindowViewModel(s));
                    ++i;
                }
                this.PropertyChanged += (obj, args) =>
                                        {
                                            if (args.PropertyName == "SelectedExchange")
                                            {
                                                if (ExchangeVMIndexes.ContainsKey(SelectedExchange.Name))
                                                {
                                                    this.DataContext = allExchangeVMs[ExchangeVMIndexes[SelectedExchange.Name]];
                                                }
                                                else
                                                    this.DataContext = null;
                                            }
                                        };

                AddExchCommand = new RelayCommand(ExecuteAddExchCommand);
                DeleteExchCommand = new RelayCommand(ExecuteDeleteExchCommand);
                listOfExchanges = new ObservableCollection<string>(DataManager.GetExchanges());
            }
            catch(Exception ex)
            {
                string str = "Failed initialize.";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
        }

        public void ExecuteAddExchCommand(object obj)
        {
            try
            {
                if (!ExchangeVMIndexes.ContainsKey(NewExchSymbol))
                {
                    ExchangeVMIndexes[NewExchSymbol] = ExchangeList.Count;
                    ExchangeList.Add(new Exchange(NewExchSymbol));
                    allExchangeVMs.Add(new MainWindowViewModel(NewExchSymbol));
                }
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                string str = "Failed to add new exchange";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
        }
        public void ExecuteDeleteExchCommand(object obj)
        {
            try
            {
                if (ExchangeVMIndexes.ContainsKey(SelectedExchange.Name))
                {
                    ExchangeVMIndexes.Remove(SelectedExchange.Name);
                    foreach (var v in ExchangeList)
                    {
                        if (v.Name == SelectedExchange.Name)
                        {
                            ExchangeList.Remove(v);
                            break;
                        }
                    }
                    int i = 0;
                    foreach (MainWindowViewModel v in allExchangeVMs)
                    {
                        if (v!=null && v.Exchange == SelectedExchange.Name)
                        {
                            allExchangeVMs[i] = null;
                            break;
                        }
                        ++i;
                    }
                    //allExchangeVMs.Add(new MainWindowViewModel(NewExchSymbol));
                }
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                string str = "Failed to delete exchange";
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

        private TrayNotifyIcon trayNotifyIcon;

        private void SetNotifyIcon()
        {
            try
            {
                System.IO.Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,/Common/Icon1.ico")).Stream;
                trayNotifyIcon.notifyIcon.Icon = new System.Drawing.Icon(iconStream);
                trayNotifyIcon.notifyIcon.Visible = true;
            }
            catch (Exception ex)
            {
                string str = "Failed to Set Notify Icon";
                LogManager.Write(str + "; Details:\n" + ex.Message);
                ErrorMessage = str;
            }
            
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState.Minimized == WindowState)
            {
                //this.Hide();
                //this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }
    }

    public class Exchange
    {
        public string Name { get; set; }
        public Exchange()
        {

        }
        public Exchange(string name)
        {
            Name = name;
        }
    }
}
