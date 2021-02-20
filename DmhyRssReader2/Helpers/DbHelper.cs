using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DmhyRssReader2.Models;

namespace DmhyRssReader2.Helpers
{
    class DbHelper
    {
        private const string DatabaseFile = "data.db";
        private static readonly object LockThis = new object();

        public ObservableCollection<ConfigVM> DatabaseInit()
        {
            lock (LockThis)
            {
                if (!CheckDatabase())
                {
                    CreateDatabase();
                    return new ObservableCollection<ConfigVM>();
                }
                else
                {
                    return GetConfigList();
                }
            }
        }

        private bool CheckDatabase()
        {
            if (!File.Exists(DatabaseFile))
            {
                return false;
            }

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=" + DatabaseFile))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;
                    SQLiteHelper sh = new SQLiteHelper(cmd);

                    DataTable dt = sh.GetTableStatus();
                    bool configListExisted = false;
                    bool videoListExisted = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if ((dr["type"] as string) == "table" && (dr["name"] as string) == "ConfigList")
                        {
                            configListExisted = true;
                            continue;
                        }
                        if ((dr["type"] as string) == "table" && (dr["name"] as string) == "VideoList")
                        {
                            videoListExisted = true;
                            continue;
                        }
                    }
                    if (!configListExisted || !videoListExisted)
                    {
                        conn.Close();
                        return false;
                    }

                    DataTable dt2 = sh.GetColumnStatus("ConfigList");
                    bool[] flag1 = new bool[8] { false, false, false, false, false, false, false, false };

                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "Name" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[0] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "Keyword" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[1] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "TeamId" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[2] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "CategoryId" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[3] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "LastUpdate" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[4] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "LastRefresh" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[5] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "Hash" && Convert.ToInt32(dr2["pk"]) == 1)
                        {
                            flag1[6] = true;
                            continue;
                        }
                        if ((dr2["type"] as string) == "text" && (dr2["name"] as string) == "Selected" && Convert.ToInt32(dr2["pk"]) == 0)
                        {
                            flag1[7] = true;
                            continue;
                        }
                    }
                    if (!flag1[0] || !flag1[1] || !flag1[2] || !flag1[3] || !flag1[4] || !flag1[5] || !flag1[6] || !flag1[7])
                    {
                        conn.Close();
                        return false;
                    }

                    DataTable dt3 = sh.GetColumnStatus("VideoList");
                    bool[] flag2 = new bool[10] { false, false, false, false, false, false, false, false, false, false };

                    foreach (DataRow dr3 in dt3.Rows)
                    {
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Title" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[0] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Link" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[1] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "PubDate" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[2] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Description" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[3] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "MagnetLink" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[4] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Author" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[5] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Guid" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[6] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Category" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[7] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "CategoryLink" && Convert.ToInt32(dr3["pk"]) == 0)
                        {
                            flag2[8] = true;
                            continue;
                        }
                        if ((dr3["type"] as string) == "text" && (dr3["name"] as string) == "Hash" && Convert.ToInt32(dr3["pk"]) == 1)
                        {
                            flag2[9] = true;
                            continue;
                        }
                    }
                    if (!flag2[0] || !flag2[1] || !flag2[2] || !flag2[3] || !flag2[4] || !flag2[5] || !flag2[6] || !flag2[7] || !flag2[8] || !flag2[9])
                    {
                        conn.Close();
                        return false;
                    }
                    conn.Close();
                    return true;
                }
            }
        }

        private void CreateDatabase()
        {
            if (File.Exists(DatabaseFile))
            {
                File.Delete(DatabaseFile);
            }
            SQLiteConnection.CreateFile(DatabaseFile);
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    SQLiteHelper helper = new SQLiteHelper(command);

                    SQLiteTable tb = new SQLiteTable("ConfigList");
                    tb.Columns.Add(new SQLiteColumn("Name", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("Keyword", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("TeamId", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("CategoryId", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("LastUpdate", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("LastRefresh", ColType.Text));
                    tb.Columns.Add(new SQLiteColumn("Hash", ColType.Text, true, false, true, null));
                    tb.Columns.Add(new SQLiteColumn("Selected", ColType.Text));
                    helper.CreateTable(tb);

                    SQLiteTable tb2 = new SQLiteTable("VideoList");
                    tb2.Columns.Add(new SQLiteColumn("Title", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Link", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("PubDate", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Description", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("MagnetLink", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Author", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Guid", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Category", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("CategoryLink", ColType.Text));
                    tb2.Columns.Add(new SQLiteColumn("Hash", ColType.Text, true, false, true, null));
                    helper.CreateTable(tb2);

                    connection.Close();
                }
            }
        }

        private ObservableCollection<ConfigVM> GetConfigList()
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    SQLiteHelper helper = new SQLiteHelper(command);

                    string cmd = "select * from ConfigList;";
                    DataTable dt = helper.Select(cmd);
                    ObservableCollection<ConfigVM> configList = new ObservableCollection<ConfigVM>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        ConfigVM config = new ConfigVM();
                        config.Name = dr["Name"] as string;
                        config.Keyword = dr["Keyword"] as string;
                        config.TeamId = dr["TeamId"] as string;
                        config.CategoryId = dr["CategoryId"] as string;
                        config.LastUpdate = dr["LastUpdate"] as string;
                        config.LastRefresh = dr["LastRefresh"] as string;
                        config.Selected = (dr["Selected"] as string) == "True";
                        configList.Add(config);
                    }
                    connection.Close();
                    return configList;
                }
            }
        }

        public void AddConfig(ConfigVM config)
        {
            lock (LockThis)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        Dictionary<string, object> dic = new Dictionary<string, object>
                        {
                            ["Name"] = config.Name,
                            ["Keyword"] = config.Keyword,
                            ["TeamId"] = config.TeamId,
                            ["CategoryId"] = config.CategoryId,
                            ["LastUpdate"] = config.LastUpdate,
                            ["LastRefresh"] = config.LastRefresh,
                            ["Hash"] = config.Hash,
                            ["Selected"] = config.Selected ? "True" : "False"
                        };

                        helper.Insert("ConfigList", dic);

                        connection.Close();
                    }
                }
            }
        }

        public void AddConfigWithRemove(ConfigVM config)
        {
            if (ConfigExisted(config))
            {
                RemoveConfig(config);
            }
            AddConfig(config);
        }

        public void RemoveConfig(ConfigVM config)
        {
            lock (LockThis)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "delete from ConfigList where Hash='" + config.Hash + "';";
                        helper.Execute(cmd);

                        connection.Close();
                    }
                }
            }
        }

        public bool ConfigExisted(ConfigVM v)
        {
            lock (LockThis)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "select * from ConfigList where Hash='" + v.Hash + "';";
                        DataTable dt = helper.Select(cmd);
                        connection.Close();
                        return dt.Rows.Count > 0;
                    }
                }
            }
        }

        public bool VideoExisted(VideoVM v)
        {
            lock (LockThis)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        string cmd = "select * from VideoList where Hash='" + v.Hash + "';";
                        DataTable dt = helper.Select(cmd);
                        connection.Close();
                        return dt.Rows.Count > 0;
                    }
                }
            }
        }

        public void AddVideo(VideoVM v)
        {
            lock (LockThis)
            {
                using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + DatabaseFile))
                {
                    using (SQLiteCommand command = new SQLiteCommand())
                    {
                        command.Connection = connection;
                        connection.Open();
                        SQLiteHelper helper = new SQLiteHelper(command);

                        Dictionary<string, object> dic = new Dictionary<string, object>
                        {
                            ["Title"] = v.Title,
                            ["Link"] = v.Link,
                            ["PubDate"] = v.PubDate,
                            ["Description"] = v.Description,
                            ["MagnetLink"] = v.MagnetLink,
                            ["Author"] = v.Author,
                            ["Guid"] = v.Guid,
                            ["Category"] = v.Category,
                            ["CategoryLink"] = v.CategoryLink,
                            ["Hash"] = v.Hash
                        };

                        helper.Insert("VideoList", dic);

                        connection.Close();
                    }
                }
            }
        }
        public void AddVideoWithCheck(VideoVM v)
        {
            if (!VideoExisted(v))
            {
                AddVideo(v);
            }
        }
    }
}
