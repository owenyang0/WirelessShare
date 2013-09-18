using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
namespace Command
{
    public partial class Form1 : Form
    {
        cCommand cmd = new cCommand();
        string s = "";
        string filepath = Application.StartupPath + "\\NetConfig.ini";
        ErrorProvider er = new ErrorProvider();
        #region 配置文件
        [DllImport("kernel32 ")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32 ")]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #endregion
        public Form1()
        {
            InitializeComponent();
        }
        private void command(string comd)
        {
            tbxResult.Text = "正在开始设置，请稍候...";
            System.Threading.ThreadPool.QueueUserWorkItem(o =>
                {
                   
                    s = cmd.RunCmd(comd);
                    //专心更新界面  
                    //BeginInvoke是相对于调用它的线程来说的（即此处线程池中的线程），它的执行依然在UI线程中完成，所以不要将其它非UI更新的操作放在里面  
                    tbxResult.BeginInvoke(new Action(() =>
                    {
                        tbxResult.Text = s;
                    }));
                });
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //command("ping 192.168.1.1");//netsh wlan start hostednetwork
            command("netsh wlan start hostednetwork");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            command("netsh wlan stop hostednetwork");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string sr = "netsh wlan set hostednetwork mode=allow ssid=" + tbxName.Text + " key=" + tbxPSW.Text;
          
            command(sr);
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void tbxControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                command(tbxControl.Text);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            WritePrivateProfileString("窗口", "宽度", this.Width.ToString (), filepath);
            WritePrivateProfileString("窗口", "高度", this.Height.ToString(), filepath);
            WritePrivateProfileString("相关设置", "热点名", tbxName.Text, filepath);
            WritePrivateProfileString("相关设置", "密码", tbxPSW.Text, filepath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StringBuilder vet = new StringBuilder(255);
            GetPrivateProfileString("窗口", "宽度", "291", vet, 0xff, filepath);
            this.Width = Convert.ToInt32(vet.ToString ()); 
            GetPrivateProfileString("窗口", "高度", "273", vet, 0xff, filepath);
            this.Height = Convert.ToInt32(vet.ToString ());
            GetPrivateProfileString("相关设置", "热点名","MyNet", vet, 0xff, filepath);
            tbxName.Text = vet.ToString();
            GetPrivateProfileString("相关设置", "密码","12345678", vet, 0xff, filepath);
            tbxPSW.Text = vet.ToString();
        }

        private void tbxPSW_KeyUp(object sender, KeyEventArgs e)
        {
           
            if (tbxPSW.Text.Length >7)//验证密码长度是否正确
            {
                er.SetError(tbxPSW, string .Empty);
            }
            else
            {
                er.SetError(tbxPSW, "密码长度不正确,至少8位");
            }

        }
    }
}
