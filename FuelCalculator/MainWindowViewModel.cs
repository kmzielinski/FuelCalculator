using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FuelCalculator
{
    using global::FuelCalculator.FuelCalculator;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Properties

        private DateTime _refuelingDate;
        public DateTime RefuelingDate
        {
            get
            {
                return _refuelingDate;
            }
            set
            {
                _refuelingDate = value;
                OnPropertyChanged("RefuelingDate");
            }
        }


        private int _counter;
        public int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                _counter = value;
                OnPropertyChanged("Counter");
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                OnPropertyChanged("Amount");
            }
        }

        private decimal _price;
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public ObservableCollection<MeasurementModel> Records { get; set; }       
        public event PropertyChangedEventHandler PropertyChanged;

        
        #endregion

        #region Constructors


        public MainWindowViewModel()
        {

            RefuelingDate = DateTime.Now;
            Records = new ObservableCollection<MeasurementModel>();
            ReloadDataGrid();
        }


        #endregion


        #region Methods

        private void OnPropertyChanged(string propertyName)

        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool CanAddRecordExecute()
        {
            return true;
        }

        private void AddRecordExecute()
            
        {
            if (!IsValid())
                return;
            //var msg = $"data:{RefuelingDate}, licznik:{Counter}, ";
            var newRecord = new MeasurementModel()
            {
                RefuelingDate = this.RefuelingDate,
                Amount = this.Amount,
                Counter = this.Counter,
                Price = this.Price,
            };
            //Records.Add(newRecord);

            var _connectionString = "Data Source=MyLocalDatabase.sqlite;Version=3;";
            var insertSQL = @"insert into Measurements (RefuelingDate, Counter, Amount, Price) 
                             values (@refuelingDate, @counter, @amount, @price)";
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(insertSQL, connection);

                var sqlParam = new SQLiteParameter("@refuelingDate", DbType.DateTime);
                sqlParam.Value = RefuelingDate;
                command.Parameters.Add(sqlParam);

                //command.Parameters.Add(new SQLiteParameter("@refuelingDate", DbType.DateTime)
                //{
                //    Value = RefuelingDate
                //});

                command.Parameters.Add(new SQLiteParameter("@counter", DbType.Int32) { Value = Counter });
                command.Parameters.Add(new SQLiteParameter("@amount", DbType.Decimal) { Value = Amount });
                command.Parameters.Add(new SQLiteParameter("@price", DbType.Decimal) { Value = Price });

                connection.Open();
                command.ExecuteNonQuery();
            }

            ReloadDataGrid();
            CleanForm();
        }

        private void ReloadDataGrid()
        {
            var getSQL = @"select * from Measurements";
            var _connectionString = "Data Source=MyLocalDatabase.sqlite;Version=3;";

            var tmpRecords = new List<MeasurementModel>();
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(getSQL, connection);
                connection.Open();

                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tmpRecords.Add(
                        new MeasurementModel()
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            Counter = int.Parse(reader["Counter"].ToString()),
                            RefuelingDate = (DateTime)reader["RefuelingDate"],
                            Amount = (decimal)reader["Amount"],
                            Price = (decimal)reader["Price"],
                        }
                    );
                }
            }

            Records.Clear();

            MeasurementModel recordBefore = null;
            tmpRecords.ForEach (rec =>
            {
                if (recordBefore != null)
                {
                    rec.Kilometers = rec.Counter - recordBefore.Counter;
                    rec.PriceperLiter = Math.Round(rec.Price / rec.Amount, 2);
                    rec.PricePre100Km = Math.Round(rec.Price / rec.Kilometers, 2);
                    rec.FuelConsumption = Math.Round(rec.Amount / rec.Kilometers, 2);



                }
                Records.Insert();
                recordBefore = rec;
            });

        }

        private bool IsValid()
        {
            var isValid = true;
            var errorMsg = string.Empty;

            if (RefuelingDate > DateTime.Now)
            {
                errorMsg = $"{errorMsg} Data tankowania nie może być z przyszłości.{Environment.NewLine}";
                isValid = false;
            }

            if (Price <= 0)
            {
                errorMsg = $"{errorMsg} Cena musi większa od zera.{Environment.NewLine}";
                isValid = false;
            }
            if (Amount <= 0)
            {
                errorMsg = $"{errorMsg} Ilość musi większa od zera.{Environment.NewLine}";
                isValid = false;
            }
            if (Counter < 0)
            {
                errorMsg = $"{errorMsg} Stan licznika nie może być mniejszy od zera.{Environment.NewLine}";
                isValid = false;
            }

            if (!isValid)
                MessageBox.Show(errorMsg, "Złe dane wejściowe");
 

           return isValid;
        }


        private void CleanForm()
        {
            RefuelingDate = DateTime.Now;
            Counter = 0;
            Amount = 0;
            Price = 0;
        }

        #endregion

        #region Commands
        public ICommand AddRecord
        {
            get
            {
                return new RelayCommand(AddRecordExecute, CanAddRecordExecute);
            }
        }
        #endregion
    }
}
