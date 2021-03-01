using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DmhyRssReader2.Helpers;
using DmhyRssReader2.Models;
using DmhyRssReader2.Utils;
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;

namespace DmhyRssReader2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM view;
        private readonly DbHelper db;
        private Client client;
        private Timer timer;
        private readonly DmhyHelper dmhy;
        public MainWindow()
        {
            InitializeComponent();
            this.view = new MainWindowVM();
            this.db = new DbHelper();
            this.view.ConfigManageTab.ConfigList = this.db.DatabaseInit();
            this.view.ConfigManageTab.AutoUpdateEvent += ConfigManageTab_AutoUpdateEvent;
            this.dmhy = new DmhyHelper();
            this.DataContext = this.view;
        }

        private void ConfigManageTab_AutoUpdateEvent(bool enable)
        {
            if (enable)
            {
                this.timer = new Timer(TimeUp, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                this.timer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
            }
            else
            {
                this.timer?.Dispose();
            }
        }
        private async void TimeUp(object o)
        {
            await UpdateAndDownloadVideo();
            this.timer.Change(TimeSpan.FromMinutes(30), Timeout.InfiniteTimeSpan);
        }

        private async Task UpdateAndDownloadVideo()
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            v.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            LogUtil.Log("-> start.");
            foreach (ConfigVM c in v.ConfigList)
            {
                if (c.Selected)
                {
                    List<VideoVM> list = await GetVideoList(c.Keyword, c.CategoryId, c.TeamId);
                    if (list != null)
                    {
                        c.LastRefresh = DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss");
                        if (list.Count > 0)
                        {
                            c.LastUpdate = list[0].PubDate;
                            foreach (VideoVM item in list)
                            {
                                if (!this.db.VideoExisted(item))
                                {
                                    if (await DownloadVideo(item.MagnetLink))
                                    {
                                        this.db.AddVideoWithCheck(item);
                                    }
                                }
                            }
                        }
                    }
                }
                this.db.AddConfigWithRemove(c);
            }
            LogUtil.Log("-> complete.");
            v.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            v.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            ConfigVM c = v.CurrentConfig;
            List<VideoVM> list = await GetVideoList(c.Keyword, c.CategoryId, c.TeamId);
            if (list != null)
            {
                v.SearchResults = new ObservableCollection<VideoVM>(list);
                foreach (VideoVM item in v.SearchResults)
                {
                    if (this.db.VideoExisted(item))
                    {
                        item.Downloaded = true;
                    }
                }
            }
            v.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async Task<List<VideoVM>> GetVideoList(string keyword, string categoryId, string teamId)
        {
            List<VideoVM> list;
            if (string.IsNullOrEmpty(keyword))
            {
                list = StringUtil.Str2UInt16(categoryId, out int category)
                    ? await this.dmhy.GetCategoryAsync(category)
                    : StringUtil.Str2UInt16(teamId, out int team) ? await this.dmhy.GetTeamAsync(team) : await this.dmhy.SearchAsync(null, 0, 0);
            }
            else
            {
                int c = StringUtil.Str2UInt16(categoryId, out int category) ? category : 0;
                int t = StringUtil.Str2UInt16(teamId, out int team) ? team : 0;
                list = await this.dmhy.SearchAsync(keyword, c, t);
            }
            return list;
        }

        private void MenuItemOpenVideoPage_Click(object sender, RoutedEventArgs e)
        {
            VideoVM v = this.view.ConfigManageTab.SelectedResult;
            OpenLinkWithChrome(v.Link);
        }

        private void OpenLinkWithChrome(string link)
        {
            try
            {
                Process.Start("chrome.exe", link);
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                try
                {
                    Process.Start(link);
                }
                catch (Exception ex2)
                {
                    LogUtil.Log(ex2);
                }
            }
        }

        private void MenuItemCopyMagnetLink_Click(object sender, RoutedEventArgs e)
        {
            VideoVM v = this.view.ConfigManageTab.SelectedResult;
            if (v != null)
            {
                Copy2Clipboard(v.MagnetLink);
            }
        }

        private void MenuItemCopySelectedMagnetLink_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.SearchResults;
            StringBuilder sb = new StringBuilder();
            foreach (VideoVM item in list)
            {
                if (item.Selected)
                {
                    sb.Append(item.MagnetLink).Append(Environment.NewLine);
                }
            }
            Copy2Clipboard(sb.ToString());
        }

        private void MenuItemCopyAllMagnetLink_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.SearchResults;
            StringBuilder sb = new StringBuilder();
            foreach (VideoVM item in list)
            {
                sb.Append(item.MagnetLink).Append(Environment.NewLine);
            }
            Copy2Clipboard(sb.ToString());
        }

        private async void MenuItemDownloadMagnetLink_Click(object sender, RoutedEventArgs e)
        {
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            VideoVM v = this.view.ConfigManageTab.SelectedResult;
            if (v != null)
            {
                if (await DownloadVideo(v.MagnetLink))
                {
                    this.db.AddVideoWithCheck(v);
                    v.Downloaded = true;
                }
            }
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async void MenuItemDownloadSelectedMagnetLink_Click(object sender, RoutedEventArgs e)
        {
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.SearchResults;
            foreach (VideoVM item in list)
            {
                if (item.Selected)
                {
                    if (await DownloadVideo(item.MagnetLink))
                    {
                        this.db.AddVideoWithCheck(item);
                        item.Downloaded = true;
                    }
                }
            }
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async void MenuItemDownloadAllMagnetLink_Click(object sender, RoutedEventArgs e)
        {
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.SearchResults;
            foreach (VideoVM item in list)
            {
                if (await DownloadVideo(item.MagnetLink))
                {
                    this.db.AddVideoWithCheck(item);
                    item.Downloaded = true;
                }
            }
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async void ButtonTestServer_Click(object sender, RoutedEventArgs e)
        {
            DownloadTrVM v = this.view.DownloadTrTab;
            this.client = new Client(v.FullPath);
            SessionInfo sessionInfo = await this.client.GetSessionInformationAsync();
            StringBuilder sb = new StringBuilder();
            sb.Append("程序版本:");
            sb.Append(sessionInfo.Version);
            sb.Append(Environment.NewLine);
            sb.Append("RPC版本:");
            sb.Append(sessionInfo.RpcVersion);
            v.TestResult = sb.ToString();

        }

        private async Task<bool> DownloadWithBitcomet(string link)
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "BitComet.exe";
                    p.StartInfo.Arguments = string.Format("--silent --url=\"{0}\"", link);
                    p.Start();
                    await Task.Run(() => p.WaitForExit());
                    await Task.Delay(100);//Stop for 100ms
                    int exitCode = p.ExitCode;
                    p.Close();
                    //Exit code is always -1 here.
                    return true;//return exitCode == 0;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Log(ex);
                return false;
            }
        }
        private async Task<bool> DownloadVideo(string link) => StringUtil.CheckMagnetlink(link) && await DownloadWithBitcomet(link);

        private void MenuItemEditConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            if (v.SelectedConfig != null)
            {
                v.CurrentConfig = new ConfigVM(v.SelectedConfig);
            }
        }

        private void MenuItemDeleteConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            v.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            if (v.SelectedConfig != null)
            {
                if (this.db.ConfigExisted(v.SelectedConfig))
                {
                    this.db.RemoveConfig(v.SelectedConfig);
                }
                if (v.ConfigList.Contains(v.SelectedConfig))
                {
                    v.ConfigList.Remove(v.SelectedConfig);
                }
            }
            v.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private void ButtonAddConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            v.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            ConfigVM config = new ConfigVM(v.CurrentConfig);
            if (v.ConfigList.Contains(config))
            {
                if (MessageBox.Show("同名配置已存在，是否覆盖？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    v.ConfigList.Remove(config);
                    v.ConfigList.Add(config);
                    this.db.AddConfigWithRemove(config);
                }
            }
            else
            {
                v.ConfigList.Add(config);
                this.db.AddConfigWithRemove(config);
            }
            v.SetStatus(ConfigManageTabVM.OpStatus.IDLE);

        }

        private async void MenuItemRefreshUpdateList_Click(object sender, RoutedEventArgs e)
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            v.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            v.UpdateList = new ObservableCollection<VideoVM>();
            foreach (ConfigVM c in v.ConfigList)
            {
                if (c.Selected)
                {
                    ObservableCollection<VideoVM> list = new ObservableCollection<VideoVM>(await GetVideoList(c.Keyword, c.CategoryId, c.TeamId));
                    if (list != null)
                    {
                        c.LastRefresh = DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss");
                        if (list.Count > 0)
                        {
                            c.LastUpdate = list[0].PubDate;
                            foreach (VideoVM item in list)
                            {
                                if (!this.db.VideoExisted(item))
                                {
                                    v.UpdateList.Add(item);
                                }
                            }
                        }
                    }
                }
                this.db.AddConfigWithRemove(c);
            }
            v.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private void MenuItemOpenVideoPage2_Click(object sender, RoutedEventArgs e)
        {
            VideoVM v = this.view.ConfigManageTab.SelectedVideo;
            OpenLinkWithChrome(v.Link);
        }

        private void MenuItemCopyMagnetLink2_Click(object sender, RoutedEventArgs e)
        {
            VideoVM v = this.view.ConfigManageTab.SelectedVideo;
            if (v != null)
            {
                Copy2Clipboard(v.MagnetLink);
            }
        }

        private void MenuItemCopySelectedMagnetLink2_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.UpdateList;
            StringBuilder sb = new StringBuilder();
            foreach (VideoVM item in list)
            {
                if (item.Selected)
                {
                    sb.Append(item.MagnetLink).Append(Environment.NewLine);
                }
            }
            Copy2Clipboard(sb.ToString());
        }

        private void MenuItemCopyAllMagnetLink2_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.UpdateList;
            StringBuilder sb = new StringBuilder();
            foreach (VideoVM item in list)
            {
                sb.Append(item.MagnetLink).Append(Environment.NewLine);
            }
            Copy2Clipboard(sb.ToString());
        }

        private async void MenuItemDownloadMagnetLink2_Click(object sender, RoutedEventArgs e)
        {
            ConfigManageTabVM v = this.view.ConfigManageTab;
            v.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            if (v.SelectedVideo != null)
            {
                if (await DownloadVideo(v.SelectedVideo.MagnetLink))
                {
                    this.db.AddVideoWithCheck(v.SelectedVideo);
                    v.UpdateList.Remove(v.SelectedVideo);
                }
            }
            v.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async void MenuItemDownloadSelectedMagnetLink2_Click(object sender, RoutedEventArgs e)
        {
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.UpdateList;
            for (int ii = list.Count - 1; ii >= 0; ii--)
            {
                VideoVM item = list[ii];
                if (item.Selected)
                {
                    if (await DownloadVideo(item.MagnetLink))
                    {
                        this.db.AddVideoWithCheck(item);
                        list.Remove(item);
                    }
                }
            }
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private async void MenuItemDownloadAllMagnetLink2_Click(object sender, RoutedEventArgs e)
        {
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.BUSY);
            ObservableCollection<VideoVM> list = this.view.ConfigManageTab.UpdateList;
            for (int ii = list.Count - 1; ii >= 0; ii--)
            {
                VideoVM item = list[ii];
                if (await DownloadVideo(item.MagnetLink))
                {
                    this.db.AddVideoWithCheck(item);
                    list.Remove(item);
                }
            }
            this.view.ConfigManageTab.SetStatus(ConfigManageTabVM.OpStatus.IDLE);
        }

        private void Copy2Clipboard(string text) => this.Dispatcher.Invoke(() =>
                                                  {
                                                      Clipboard.Clear();
                                                      Clipboard.SetDataObject(text);
                                                  });
    }
}
