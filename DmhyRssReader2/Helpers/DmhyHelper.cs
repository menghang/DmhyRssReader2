using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using DmhyRssReader2.Models;
using DmhyRssReader2.Utils;

namespace DmhyRssReader2.Helpers
{
    class DmhyHelper
    {
        private const string BaseSiteUrl = "http://dmhy.anoneko.com";
        private const string SearchRss = "/topics/rss/rss.xml";
        private const string TeamRss = "/topics/rss/team_id/{0}/rss.xml";
        private const string SortRss = "/topics/rss/sort_id/{0}/rss.xml";
        private const string AuthorRss = "/topics/rss/user_id/{0}/rss.xml";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4418.0 Safari/537.36";

        public async Task<List<VideoVM>> SearchAsync(string keyword, int sort, int team)
        {
            string k = HttpUtility.UrlEncode(keyword, Encoding.UTF8);
            Dictionary<string, string> param = null;
            if (!string.IsNullOrEmpty(k))
            {
                param = new Dictionary<string, string> { { "keyword", k } };
                param.Add("sort_id", Convert.ToString(sort));
                param.Add("team_id", Convert.ToString(team));
            }

            return await GetRssAsync(SearchRss, param);
        }

        public async Task<List<VideoVM>> GetTeamAsync(int team) => await GetRssAsync(string.Format(TeamRss, team));
        public async Task<List<VideoVM>> GetCategoryAsync(int category) => await GetRssAsync(string.Format(SortRss, category));

        public async Task<List<VideoVM>> GetAuthorAsync(int author) => await GetRssAsync(string.Format(AuthorRss, author));

        private async Task<List<VideoVM>> GetRssAsync(string url, Dictionary<string, string> param = null)
        {
            string param2 = string.Empty;
            if (param != null)
            {
                foreach (KeyValuePair<string, string> p in param)
                {

                    param2 = (string.IsNullOrEmpty(param2) ? param2 + "?" : param2 + "&") + p.Key + "=" + p.Value;
                }
            }

            HttpWebRequest request = WebRequest.Create(BaseSiteUrl + url + param2) as HttpWebRequest;

            request.UserAgent = UserAgent;
            request.Method = "GET";
            request.Proxy = WebRequest.GetSystemWebProxy();

            request.Timeout = 1000;
            request.ReadWriteTimeout = 1000;

            try
            {
                using (HttpWebResponse response = (await request.GetResponseAsync()) as HttpWebResponse)
                {
                    using (Timer t = new Timer((o) => response?.Dispose(), null, Timeout.InfiniteTimeSpan, TimeSpan.FromSeconds(30)))
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            string text = await sr.ReadToEndAsync();
                            t.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                            using (XmlReader xmlReader = XmlReader.Create(new StringReader(text)))
                            {
                                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
                                List<VideoVM> videos = new List<VideoVM>();
                                foreach (SyndicationItem item in feed.Items)
                                {
                                    videos.Add(new VideoVM(item));
                                }
                                return videos;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return null;
            }
        }
    }
}
