namespace DmhyRssReader2.Models
{
    class MainWindowVM : BaseViewModel
    {
        public ConfigManageTabVM ConfigManageTab
        {
            get;
            private set;
        }

        public MainWindowVM()
        {
            this.ConfigManageTab = new ConfigManageTabVM();
        }
    }
}
