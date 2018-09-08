using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;

namespace DemoServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //成员属性
        Socket Server; //声明服务器Socket
        Dictionary<string, Socket> Clients = new Dictionary<string, Socket>(); //客户端连接集合

        public MainWindow()
        {
            InitializeComponent();
            //加载IP
            txt_IP.Text = GetLocalIP();
            //绑定快捷键
            txt_Send.KeyDown += new KeyEventHandler(OnEnterDown);
        }

        /// <summary>
        /// 快捷键处理程序
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
        /// 开启服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StartServer_Click(object sender, RoutedEventArgs e)
        {
            if (Server != null)
            {
                MessageBox.Show("服务已开启，请勿重复开启！");
                return;
            }
            //实例化服务
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(txt_IP.Text), int.Parse(txt_Port.Text)); //实例化一个网络端点
            Server.Bind(iep); //将网络端点与服务端Socket绑定
            Server.Listen(10); //指定Server可接受的客户端数量
            //开启监听
            Thread thr = new Thread(Listen);
            thr.IsBackground = true;
            thr.Start();
            //更新界面信息
            MsgEntity me = new MsgEntity();
            me.Adress = "系统消息";
            me.Time = "【" + DateTime.Now.ToString() + "】";
            me.Msg = "服务器已经开始侦听，可以开始连接！";
            lb_msg.Items.Add(me);
            lb_msg.ScrollIntoView(me);
        }

        /// <summary>
        /// 侦听服务器连接
        /// </summary>
        private void Listen()
        {
            //循环监听，为阻塞式线程
            while (Clients.Count < 10)
            {
                Socket client = Server.Accept(); //接受上线的Socket客户端
                Clients.Add(client.RemoteEndPoint.ToString(), client); //添加到客户端集合
                //开启客户端消息读取线程
                Thread thr = new Thread(ReciveMsg);
                thr.IsBackground = true;
                thr.Start(client);
                //更新界面信息
                Dispatcher.Invoke(new Action(() =>
                {
                    cb_Clients.Items.Add(client.RemoteEndPoint.ToString()); //更新下拉框
                    //更新消息框
                    MsgEntity me = new MsgEntity();
                    me.Adress = "系统消息";
                    me.Time = "【" + DateTime.Now.ToString() + "】";
                    me.Msg = "与客户端[ " + client.RemoteEndPoint.ToString() + " ]连接成功！";
                    lb_msg.Items.Add(me);
                    lb_msg.ScrollIntoView(me);
                }));
            }
        }



        /// <summary>
        /// 接受消息线程
        /// </summary>
        private void ReciveMsg(object c)
        {
            Socket client = c as Socket; //获取客户端
            while (true)
            {
                //读取信息
                try
                {
                    byte[] buffer = new byte[1500]; //信息流缓存区域
                    int size = client.Receive(buffer); //读取信息并返回大小
                    if (size == 0) break; //无消息则跳出
                    string msg = Encoding.UTF8.GetString(buffer, 0, size); //转码消息
                    lb_msg.Dispatcher.Invoke(new Action(() =>
                    {
                        MsgEntity me = new MsgEntity();
                        me.Adress = "来自：" + client.RemoteEndPoint.ToString();
                        me.Time = "【" + DateTime.Now.ToString() + "】";
                        me.Msg = msg;
                        lb_msg.Items.Add(me);
                        lb_msg.ScrollIntoView(me);
                    }));
                }
                catch
                {
                    cb_Clients.Dispatcher.Invoke(new Action(() =>
                    {
                        cb_Clients.Items.Remove(client.RemoteEndPoint.ToString());
                    }));
                    Clients.Remove(client.RemoteEndPoint.ToString());
                    break;
                }
            }
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        private String GetLocalIP()
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
        /// 发送信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SendMsg_Click(object sender, RoutedEventArgs e)
        {
            var select = cb_Clients.SelectedItem;
            if (select == null)
            {
                MessageBox.Show("请选择消息接受者！");
                return;
            }
            if (string.IsNullOrEmpty(txt_Send.Text))
            {
                MessageBox.Show("请输入您要发送的信息！");
                return;
            }
            if (txt_Send.Text.Length > 500)
            {
                MessageBox.Show("您发送消息的长度过长！");
                return;
            }
            Socket current;
            if (Clients.TryGetValue(select.ToString(), out current))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(txt_Send.Text);
                MsgEntity me = new MsgEntity();
                try
                {
                    current.Send(buffer);
                    me.Msg = txt_Send.Text;
                }
                catch
                {
                    me.Msg = "通信中断，消息发送失败！";
                }
                finally
                {
                    me.Adress = "发送至：" + current.RemoteEndPoint.ToString();
                    me.Time = "【" + DateTime.Now.ToString() + "】";
                    lb_msg.Items.Add(me);
                    lb_msg.ScrollIntoView(me);
                    txt_Send.Clear();
                }
            }
        }

        /// <summary>
        /// 程序退出提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("确认关闭服务器程序？", "系统提示", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                //处理退出
                if (Clients != null)
                {
                    foreach (Socket s in Clients.Values)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes("服务器端已下线，连接断开！");
                        try
                        {
                            s.Send(buffer);
                        }
                        catch
                        {
                            MessageBox.Show("通信发生异常,程序不能正常退出！");
                            System.Environment.Exit(0);
                        }
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
        /// 程序完全退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Server != null)
                Server.Close();
            System.Environment.Exit(0);
        }


    }
}
