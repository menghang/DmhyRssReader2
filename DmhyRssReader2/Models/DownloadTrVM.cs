using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DmhyRssReader2.Models
{
    class DownloadTrVM : BaseViewModel
    {
        private string addr;
        public string Addr
        {
            get => this.addr;
            set => SetProperty(ref this.addr, value);
        }

        private string port;
        public string Port
        {
            get => this.port;
            set => SetProperty(ref this.port, value);
        }

        private string rpcPath;
        public string RpcPath
        {
            get => this.rpcPath;
            set => SetProperty(ref this.rpcPath, value);
        }

        private string testResult;
        public string TestResult
        {
            get => this.testResult;
            set => SetProperty(ref this.testResult, value);
        }

        public string FullPath
        {
            get => this.addr + ":" + this.port + this.rpcPath;
        }

        public DownloadTrVM()
        {
            this.addr = "http://127.0.0.1";
            this.port = "9091";
            this.rpcPath = "/transmission/rpc";
        }

    }
}
