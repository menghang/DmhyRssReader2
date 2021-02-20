using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DmhyRssReader2.Models
{
    class ConfigManageTabVM : BaseViewModel
    {
        private ConfigVM currentConfig;
        public ConfigVM CurrentConfig
        {
            get => this.currentConfig;
            set => SetProperty(ref this.currentConfig, value);
        }

        private ObservableCollection<VideoVM> searchResults;
        public ObservableCollection<VideoVM> SearchResults
        {
            get => this.searchResults;
            set => SetProperty(ref this.searchResults, value);
        }
        public VideoVM SelectedResult { get; set; }

        private ObservableCollection<ConfigVM> configList;
        public ObservableCollection<ConfigVM> ConfigList
        {
            get => this.configList;
            set => SetProperty(ref this.configList, value);
        }
        public ConfigVM SelectedConfig { get; set; }

        private ObservableCollection<VideoVM> updateList;
        public ObservableCollection<VideoVM> UpdateList
        {
            get => this.updateList;
            set => SetProperty(ref this.updateList, value);
        }
        public VideoVM SelectedVideo { get; set; }

        public enum OpStatus { IDLE = 0, BUSY = 1 }
        private OpStatus status = OpStatus.IDLE;

        public void SetStatus(OpStatus s)
        {
            this.status = s;
            OnPropertyChanged(nameof(this.StatusText));
            OnPropertyChanged(nameof(this.StatusColor));
        }

        public string StatusText
        {
            get
            {
                switch (this.status)
                {
                    case OpStatus.BUSY:
                        return "busy";
                    case OpStatus.IDLE:
                    default:
                        return "idle";
                }
            }
        }
        public Brush StatusColor
        {
            get
            {
                switch (this.status)
                {
                    case OpStatus.BUSY:
                        return Brushes.LightPink;
                    case OpStatus.IDLE:
                    default:
                        return Brushes.LightGreen;
                }
            }
        }
        public delegate void AutoUpdateHandler(bool enable);
        public event AutoUpdateHandler AutoUpdateEvent;

        private bool enableAutoUpdate;
        public bool EnableAutoUpdate
        {
            get => this.enableAutoUpdate;
            set
            {
                SetProperty(ref this.enableAutoUpdate, value);
                AutoUpdateEvent(this.enableAutoUpdate);
            }
        }

        public ConfigManageTabVM()
        {
            this.currentConfig = new ConfigVM();
            this.searchResults = new ObservableCollection<VideoVM>();
            this.configList = new ObservableCollection<ConfigVM>();
            //this.searchResults.Add(new VideoVM() { Title = "xx", Author = "xxx", Category = "x", Description = "xxxx", PubDate = "xx.xx.xx" });
        }
    }
}
