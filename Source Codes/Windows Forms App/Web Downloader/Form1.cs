using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace Web_Downloader
{

    public partial class Form1 : Form
    {

        bool isBusy = false;
        private string filePath = null;
       
        public Form1()
        {
            InitializeComponent();
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            if (this.fdbFile.ShowDialog() == DialogResult.OK)
            {
                this.btnDown.Enabled = true;
                filePath = this.fdbFile.SelectedPath;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (isBusy)
            {
                webClient.CancelAsync();
                isBusy = false;
            }
            else
            {
                try
                {
                    var strFileName = this.txtUrl.Text.Split(new char[] { '/' });
                    System.Array.Reverse(strFileName);
                    Uri uri = new Uri(this.txtUrl.Text);
                    var str = webClient.DownloadString(uri);
                    webClient.DownloadFileAsync(uri, filePath + @"\" + strFileName[0]);
                    isBusy = true;
                }
                catch
                {
                    this.btnDown.Enabled = false;
                    MessageBox.Show("다운로드가 실패 하였습니다.", "에러",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.pgbDownload.Value = e.ProgressPercentage;
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            isBusy = false;
            this.btnDown.Enabled = false;
            if (e.Error == null)
            {
                if (this.cbOpen.Checked == false)
                    MessageBox.Show("다운로드가 완료되었습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    Process myProcess = new Process();
                    myProcess.StartInfo.FileName = filePath;
                    myProcess.Start();
                }
            }
            else
                MessageBox.Show("다운로드가 실패하였습니다.:" + e.Error.Message.ToString());
        }



    }
 }
