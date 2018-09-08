using System;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;

namespace DemoClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //成员变量
        private Socket Client; //声明客户端对象

        public MainWindow()
        {
            InitializeComponent();

            //加载本地IP
            txt_IP.Text = GetLocalIP();

            //绑定快捷键
            txt_Send.KeyDown += new KeyEventHandler(OnEnterDown);
        }

        /// <summary>
        /// 快捷键处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_SendMsg_Click(sender, e);
            }
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        private string GetLocalIP()
        {
            string LocalIP = string.Empty;
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    LocalIP = ip.ToString();
                }
            }
            return LocalIP;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StartConn_Click(object sender, RoutedEventArgs e)
        {
            if (Client != null)
            {
                MessageBox.Show("客户端已连接至服务器，请勿重复连接！");
                return;
            }
            if (string.IsNullOrEmpty(txt_IP.Text) || string.IsNullOrEmpty(txt_Port.Text))
            {
                MessageBox.Show("请输入服务器信息！");
                return;
            }
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(txt_IP.Text), int.Parse(txt_Port.Text));//实例化端点
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//实例化Socket
            MsgEntity me = new MsgEntity(); //消息实体
            try
            {
                Client.Connect(iep); //连接至远程主机
                me.Msg = "已连接至服务器 [" + Client.RemoteEndPoint.ToString() + "] ！";
                //开启接受消息线程
                Thread thr = new Thread(ReciveMsg);
                thr.IsBackground = true;
                thr.Start();
            }
            catch
            {
                me.Msg = "连接到服务器时发生异常 ！";
                Client = null;
            }
            finally
            {
                me.Adress = "系统消息";
                me.Time = "【" + DateTime.Now.ToString() + "】";
                lb_msg.Items.Add(me);
                lb_msg.ScrollIntoView(me);
            }
        }

        /// <summary>
        /// 循环接受来自服务器的消息
        /// </summary>
        private void ReciveMsg()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1500]; //声明消息缓冲区
                    int size = Client.Receive(buffer); //接受消息存储到缓冲区，并返回消息字节数
                    if (size == 0) break;
                    string msg = Encoding.UTF8.GetString(buffer, 0, size); //信息转码
                    //显示信息
                    lb_msg.Dispatcher.Invoke(new Action(() =>
                    {
                        MsgEntity me = new MsgEntity();
                        me.Adress = "来自：" + Client.RemoteEndPoint.ToString();
                        me.Time = DateTime.Now.ToString();
                        me.Msg = msg;
                        lb_msg.Items.Add(me);
                        lb_msg.ScrollIntoView(me);
                    }));
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 发送消息线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SendMsg_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Send.Text))
            {
                MessageBox.Show("您发送的消息不能为空！");
                return;
            }
            if (txt_Send.Text.Length > 500)
            {
                MessageBox.Show("您输入的消息过长！");
                return;
            }
            MsgEntity me = new MsgEntity(); //消息实体
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(txt_Send.Text);
                Client.Send(buffer);
                me.Msg = txt_Send.Text;
            }
            catch
            {
                me.Msg = "通信发生异常，消息发送失败！";
            }
            finally
            {
                me.Time = "【" + DateTime.Now.ToString() + "】";
                me.Adress = "发送至：" + Client.RemoteEndPoint.ToString();
                lb_msg.Items.Add(me);
                lb_msg.ScrollIntoView(me);
                txt_Send.Clear();
            }
        }

        /// <summary>
        /// 程序退出确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("确认退出客户端？", "系统提示", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                if (Client != null)
                {
                    try
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes("客户端 [" + Client.RemoteEndPoint.ToString() + "] 已下线，连接断开！");
                        Client.Send(buffer);
                    }
                    catch
                    {
                        MessageBox.Show("通信中断，客户端不能正常退出！");
                        System.Environment.Exit(0);
                    }
                }
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 完全退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Client != null)
                Client.Close();
            System.Environment.Exit(0);
        }


    }
}
