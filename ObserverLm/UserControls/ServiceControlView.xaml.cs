using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ObserverLm
{
    public partial class ServiceControlView : UserControl
    {
        public ServiceControlView()
        {
            InitializeComponent();
            DataContext = new ServiceControlViewModel();
        }
    }
    public class ServiceControlViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ServiceItem> Services { get; } = new ObservableCollection<ServiceItem>();

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand RefreshCommand { get; }

        public ServiceControlViewModel()
        {
            // Добавляем две нужные службы
            Services.Add(new ServiceItem("regime"));
            Services.Add(new ServiceItem("yenisei"));

            StartCommand = new RelayCommand<ServiceItem>(async item => await item.StartAsync(), item => item?.CanStart == true);
            StopCommand = new RelayCommand<ServiceItem>(async item => await item.StopAsync(), item => item?.CanStop == true);
            RefreshCommand = new RelayCommand(RefreshAll);

            // Подписка на ошибки для отображения пользователю
            foreach (var service in Services)
            {
                service.ErrorOccurred += (s, msg) =>
                    MessageBox.Show(msg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshAll()
        {
            foreach (var service in Services)
            {
                service.Refresh();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public class ServiceItem : INotifyPropertyChanged
    {
        private readonly ServiceController _controller;
        private ServiceControllerStatus _status;
        private bool _isBusy;
        private bool _isValid = true;

        public string ServiceName { get; }
        public string DisplayName { get; private set; }

        public bool IsValid
        {
            get => _isValid;
            private set { _isValid = value; OnPropertyChanged(); }
        }

        public ServiceControllerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
            }
        }

        public bool CanStart => !IsBusy && IsValid && (Status == ServiceControllerStatus.Stopped || Status == ServiceControllerStatus.Paused);
        public bool CanStop => !IsBusy && IsValid && (Status == ServiceControllerStatus.Running);

        public event EventHandler<string> ErrorOccurred;

        public ServiceItem(string serviceName)
        {
            ServiceName = serviceName;
            try
            {
                _controller = new ServiceController(serviceName);
                _controller.Refresh();
                DisplayName = _controller.DisplayName;
                Status = _controller.Status;
            }
            catch (InvalidOperationException)
            {
                // Служба не найдена или нет доступа
                DisplayName = $"{serviceName} (not found)";
                Status = ServiceControllerStatus.Stopped;
                IsValid = false;
            }
        }

        public async Task StartAsync()
        {
            if (!CanStart) return;
            IsBusy = true;
            try
            {
                await Task.Run(() => _controller.Start());
                await Task.Run(() => _controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)));
                Refresh();
            }
            catch (Exception ex)
            {
                OnError($"Ошибка запуска службы {DisplayName}: {ex.Message}");
                Refresh(); // обновим статус после ошибки
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task StopAsync()
        {
            if (!CanStop) return;
            IsBusy = true;
            try
            {
                await Task.Run(() => _controller.Stop());
                await Task.Run(() => _controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)));
                Refresh();
            }
            catch (Exception ex)
            {
                OnError($"Ошибка остановки службы {DisplayName}: {ex.Message}");
                Refresh();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Refresh()
        {
            if (!IsValid) return;
            try
            {
                _controller.Refresh();
                Status = _controller.Status;
            }
            catch
            {
                // игнорируем, статус остаётся прежним
            }
        }

        protected virtual void OnError(string message) => ErrorOccurred?.Invoke(this, message);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
    public class ServiceStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ServiceControllerStatus status)
            {
                return status switch
                {
                    ServiceControllerStatus.Running => new SolidColorBrush(Colors.Green),
                    ServiceControllerStatus.Stopped => new SolidColorBrush(Colors.Red),
                    ServiceControllerStatus.Paused => new SolidColorBrush(Colors.Orange),
                    ServiceControllerStatus.StartPending or
                        ServiceControllerStatus.StopPending or
                        ServiceControllerStatus.ContinuePending or
                        ServiceControllerStatus.PausePending => new SolidColorBrush(Colors.Yellow),
                    _ => new SolidColorBrush(Colors.Gray),
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
