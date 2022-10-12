using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Weather0.BLL.DTO;
using Weather0.BLL.Services;
using Weather0UI.Infrastructure;

// Класс WeatherService получает из интернета данные по погоде на последующие 5 дней, на время дня, соответствующее времени осуществления запроса,
// далее вносятся данные по погоде на текущий момент в БД.

namespace Weather0UI
{
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        private CollectionView _weatherDataView;
        private WeatherDataDTO _selectedWeatherData;
        private string _currentLocation;

        private string _cloudinessBackground;
        private string _labelsForeground;
        private WeatherService WeatherService { get; set; }
        public string CurrentLocation
        {
            get => _currentLocation;
            set
            {
                _currentLocation = value;
                Task.Run(() => AnimationHandling(0));
            }
        }

        public ICommand CmdGetNewWeatherData { get; private set; }
        public ICommand CmdGetAllWeatherData { get; private set; }
        public ICommand CmdRemoveWeatherData { get; private set; }
        public ICommand CmdClearWeatherData { get; private set; }
        // Хранит свойства кнопок.
        public ButtonPropertiesStrctre[] CommandsArr { get; private set; }
        private ObservableCollection<WeatherDataDTO> WeatherData { get; set; }
        public CollectionView WeatherDataView
        {
            get => _weatherDataView;
            private set
            {
                _weatherDataView = value;
                NotifyOfPropertyChange("WeatherDataView");
                Task.Run(() => AnimationHandling(1));
            }
        }

        private WeatherDataDTO[] Forecast5Days { get; set; }
        public WeatherDataDTO SelectedWeatherData
        {
            get => _selectedWeatherData;
            set
            {
                _selectedWeatherData = value;
                SetCloudinessBackground();
                NotifyOfPropertyChange("SelectedWeatherData");
                Task.Run(() => AnimationHandling(2));
            }
        }

        public string CloudinessBackground
        {
            get => _cloudinessBackground;
            private set
            {
                _cloudinessBackground = value;
                NotifyOfPropertyChange("CloudinessBackground");
            }
        }
        public string LabelsForeground
        {
            get => _labelsForeground;
            private set
            {
                _labelsForeground = value;
                NotifyOfPropertyChange("LabelsForeground");
            }
        }

        public double WeatherByCityAvailable => CommandsArr[0]._opacityPropertyDble;
        public double LogClearAvailable => CommandsArr[1]._opacityPropertyDble;
        public double RemoveAvailable => CommandsArr[2]._opacityPropertyDble;
        public int WeatherByCityAvailableHeight => CommandsArr[0]._heightPropertyInt;
        public int LogClearAvailableHeight => CommandsArr[1]._heightPropertyInt;
        public int RemoveAvailableHeight => CommandsArr[2]._heightPropertyInt;

        public ShellViewModel()
        {
            WeatherService = new WeatherService();
            WeatherData = new ObservableCollection<WeatherDataDTO>(WeatherService.GetAllWeatherData());

            CommandsArr = new ButtonPropertiesStrctre[4];
            CommandsArr[0].Command = new RelayCommand(actn => GetNewWeatherData(), prdcte => CurrentLocation?.Length > 0);
            CommandsArr[1].Command = new RelayCommand(actn => GetAllWeatherData(), prdcte => WeatherData.Count > 0);
            CommandsArr[2].Command = new RelayCommand(actn => RemoveWeatherData(), prdcte => WeatherData.Count > 0 && SelectedWeatherData != null);
            CommandsArr[3].Command = new RelayCommand(actn => ClearWeatherData(), prdcte => WeatherData.Count > 0);
            CommandsArr[0].OpacityPropertyChangeToRaise = "WeatherByCityAvailable";
            CommandsArr[1].OpacityPropertyChangeToRaise = "LogClearAvailable";
            CommandsArr[2].OpacityPropertyChangeToRaise = "RemoveAvailable";
            CommandsArr[3].OpacityPropertyChangeToRaise = "LogClearAvailable";
            CommandsArr[0].HeightPropertyChangeToRaise = "WeatherByCityAvailableHeight";
            CommandsArr[1].HeightPropertyChangeToRaise = "LogClearAvailableHeight";
            CommandsArr[2].HeightPropertyChangeToRaise = "RemoveAvailableHeight";
            CommandsArr[3].HeightPropertyChangeToRaise = "LogClearAvailableHeight";
            for (int i = 0; i < CommandsArr.Length; i++)
            {
                CommandsArr[i].EnablingAnimationFlag = true;
            }

            WeatherDataView = (CollectionView)CollectionViewSource.GetDefaultView(WeatherData);

            CloudinessBackground = "../../ViewModels/Images/overcast clouds night.jpg";
            LabelsForeground = "White";
        }

        private void GetNewWeatherData()
        {
            Forecast5Days = WeatherService.GetNewWeatherData(CurrentLocation);
            if (Forecast5Days != null)
            {
                WeatherData.Clear();
                for (int i = 0; i < Forecast5Days.Length; i++)
                {
                    WeatherData.Add(Forecast5Days[i]);
                }
                WeatherDataView = (CollectionView)CollectionViewSource.GetDefaultView(WeatherData);
            }
        }
        private void GetAllWeatherData()
        {
            WeatherData = new ObservableCollection<WeatherDataDTO>(WeatherService.GetAllWeatherData());
            WeatherDataView = (CollectionView)CollectionViewSource.GetDefaultView(WeatherData);
        }
        private void RemoveWeatherData()
        {
            WeatherService.RemoveWeatherData(SelectedWeatherData.Id);
            WeatherData = new ObservableCollection<WeatherDataDTO>(WeatherService.GetAllWeatherData());
            WeatherDataView = (CollectionView)CollectionViewSource.GetDefaultView(WeatherData);
        }
        private void ClearWeatherData()
        {
            WeatherService.ClearWeatherData();
            WeatherData.Clear();
            Task.Run(() => AnimationHandling(1));
        }
        // Изображение фона соответствует облачности, светлому/тёмному времени суток.
        private void SetCloudinessBackground()
        {
            LabelsForeground = (_selectedWeatherData?.Date.Hour > 6 && _selectedWeatherData?.Date.Hour < 21 ? "Black" : "White");
            switch (_selectedWeatherData?.Cloudiness)
            {
                case "clear sky":
                    CloudinessBackground = (_selectedWeatherData.Date.Hour > 6 && _selectedWeatherData.Date.Hour < 21 ?
                    "../../ViewModels/Images/clear sky day.jpg"
                    : "../../ViewModels/Images/clear sky night.jpg");
                    break;
                case "few clouds":
                    CloudinessBackground = (_selectedWeatherData.Date.Hour > 6 && _selectedWeatherData.Date.Hour < 21 ?
                        "../../ViewModels/Images/few clouds day.jpg"
                        : "../../ViewModels/Images/few clouds night.jpg");
                    break;
                case "scattered clouds":
                    CloudinessBackground = (_selectedWeatherData.Date.Hour > 6 && _selectedWeatherData.Date.Hour < 21 ?
                    "../../ViewModels/Images/scattered clouds day.jpg"
                    : "../../ViewModels/Images/scattered clouds night.jpg");
                    break;
                case "overcast clouds":
                    CloudinessBackground = (_selectedWeatherData.Date.Hour > 6 && _selectedWeatherData.Date.Hour < 21 ?
                        "../../ViewModels/Images/overcast clouds day.jpg"
                        : "../../ViewModels/Images/overcast clouds night.jpg");
                    break;
                case "broken clouds":
                    CloudinessBackground = (_selectedWeatherData?.Date.Hour > 6 && _selectedWeatherData?.Date.Hour < 21 ?
                        "../../ViewModels/Images/broken clouds day.jpg"
                        : "../../ViewModels/Images/broken clouds night.jpg");
                    break;
                default:
                    LabelsForeground = "Black";
                    CloudinessBackground = "../../ViewModels/Images/broken clouds day.jpg";
                    break;
            }
        }

        private void AnimationHandling(int buttonPropertiesIndex)
        {
            if (CommandsArr[buttonPropertiesIndex].Command.CanExecute(null) && CommandsArr[buttonPropertiesIndex].EnablingAnimationFlag)
            {
                CommandsArr[buttonPropertiesIndex].EnablingAnimationFlag = false;
                HeightUpdate(buttonPropertiesIndex);
                OpacityUpdate(buttonPropertiesIndex);
                CommandsArr[buttonPropertiesIndex].DisablingAnimationFlag = true;
            }
            if (!CommandsArr[buttonPropertiesIndex].Command.CanExecute(null) && CommandsArr[buttonPropertiesIndex].DisablingAnimationFlag)
            {
                CommandsArr[buttonPropertiesIndex].DisablingAnimationFlag = false;
                OpacityUpdate(buttonPropertiesIndex, false);
                HeightUpdate(buttonPropertiesIndex, false);
                CommandsArr[buttonPropertiesIndex].EnablingAnimationFlag = true;
            }
        }
        private void OpacityUpdate(int buttonPropertiesIndex, bool ignition = true)
        {
            int k = 0;
            int m = -1;
            if (!ignition)
            {
                k = 1;
                m = 1;
            }
            for (int i = 0; i <= 10; i++)
            {
                CommandsArr[buttonPropertiesIndex]._opacityPropertyDble = (k - i * 0.1) * m;
                NotifyOfPropertyChange(CommandsArr[buttonPropertiesIndex].OpacityPropertyChangeToRaise);
                Thread.Sleep(30);//
            }
        }
        private void HeightUpdate(int buttonPropertiesIndex, bool unfolding = true)
        {
            int k = 100;
            int m = 1;
            if (unfolding)
            {
                k = 0;
                m = -1;
            }
            for (int i = 0; i <= 100; i++)
            {
                CommandsArr[buttonPropertiesIndex]._heightPropertyInt = (k - i) * m;
                NotifyOfPropertyChange(CommandsArr[buttonPropertiesIndex].HeightPropertyChangeToRaise);
                Thread.Sleep(6);//
            }
        }
    }
}