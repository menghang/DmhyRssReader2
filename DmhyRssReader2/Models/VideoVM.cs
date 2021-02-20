using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using DmhyRssReader2.Utils;

namespace DmhyRssReader2.Models
{
    class VideoVM : BaseViewModel
    {
        private bool downloaded = false;
        public bool Downloaded
        {
            get => this.downloaded;
            set => SetProperty(ref this.downloaded, value);
        }
        private bool selected = false;
        public bool Selected
        {
            get => this.selected;
            set => SetProperty(ref this.selected, value);
        }

        public string Title { get; private set; }

        public string Link { get; private set; }

        private DateTimeOffset pubDate;
        public string PubDate => this.pubDate.DateTime.ToString("yyyy-MM-dd dddd HH:mm:ss");

        public string Description { get; private set; }

        public string MagnetLink { get; private set; }

        public string Author { get; private set; }

        public string Guid { get; private set; }

        public string Category { get; private set; }

        public string CategoryLink { get; private set; }

        public VideoVM(SyndicationItem item)
        {
            this.Title = item.Title.Text;
            this.pubDate = item.PublishDate;
            this.Description = item.Summary.Text;
            this.Author = item.Authors.Count > 0 ? item.Authors[0].Email : string.Empty;
            this.Guid = item.Id;
            this.Category = item.Categories.Count > 0 ? item.Categories[0].Name : string.Empty;
            this.CategoryLink = item.Categories.Count > 0 ? item.Categories[0].Scheme : string.Empty;
            this.Link = string.Empty;
            this.MagnetLink = string.Empty;
            foreach (SyndicationLink link in item.Links)
            {
                switch (link.RelationshipType)
                {
                    case "alternate":
                        this.Link = link.Uri.AbsoluteUri;
                        break;
                    case "enclosure":
                        this.MagnetLink = link.MediaType == "application/x-bittorrent" ? link.Uri.OriginalString : string.Empty;
                        break;
                    default:
                        break;
                }
            }
        }

        public string Hash => HashUtil.SHA256EncryptString(this.Guid);

        public override int GetHashCode() => this.Guid.GetHashCode();
        public override bool Equals(object obj) => obj.GetType() == typeof(VideoVM) && (obj as VideoVM).Guid == this.Guid;
    }
}
