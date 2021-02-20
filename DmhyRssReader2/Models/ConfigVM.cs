using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DmhyRssReader2.Utils;

namespace DmhyRssReader2.Models
{
    class ConfigVM : BaseViewModel
    {
        private string name = string.Empty;
        public string Name
        {
            get => this.name;
            set => SetProperty(ref this.name, value);
        }

        private string keyword = string.Empty;
        public string Keyword
        {
            get => this.keyword;
            set => SetProperty(ref this.keyword, value);
        }

        private string teamId = string.Empty;
        public string TeamId
        {
            get => this.teamId;
            set => SetProperty(ref this.teamId, value);
        }

        private string categoryId = string.Empty;
        public string CategoryId
        {
            get => this.categoryId;
            set => SetProperty(ref this.categoryId, value);
        }

        private string lastUpdate = string.Empty;
        public string LastUpdate
        {
            get => this.lastUpdate;
            set => SetProperty(ref this.lastUpdate, value);
        }

        private string lastRefresh = string.Empty;
        public string LastRefresh
        {
            get => this.lastRefresh;
            set => SetProperty(ref this.lastRefresh, value);
        }

        private bool selected = false;
        public bool Selected
        {
            get => this.selected;
            set => SetProperty(ref this.selected, value);
        }

        public ConfigVM()
        {

        }

        public ConfigVM(ConfigVM c)
        {
            this.name = c.name;
            this.keyword = c.keyword;
            this.teamId = c.teamId;
            this.categoryId = c.categoryId;
        }

        public string Hash => HashUtil.SHA256EncryptString(this.Name);

        public override int GetHashCode() => this.Name.GetHashCode();
        public override bool Equals(object obj) => obj.GetType() == typeof(ConfigVM) && (obj as ConfigVM).Name == this.Name;
    }
}
