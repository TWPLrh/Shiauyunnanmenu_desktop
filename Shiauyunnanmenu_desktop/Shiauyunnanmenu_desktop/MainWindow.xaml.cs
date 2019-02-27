using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Xml;
using System.IO;

namespace Shiauyunnanmenu_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        const int MaxsizeContainer = 6;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        public MainWindow()
        {
            InitializeComponent();

            //MessageBox.Show(DateTime.Now.ToString("MMddHHmmss"));

            if (!Directory.Exists("圖片"))
                Directory.CreateDirectory("圖片");

            if (!File.Exists("coder"))
            {
                using (StreamWriter streamWriter = new StreamWriter("coder", false))
                {
                    streamWriter.WriteLine(1);
                    streamWriter.Close();
                }
            }
            else
            {
               if(!DateTime.Now.ToString("MMdd").Equals(File.GetLastWriteTime("coder").ToString("MMdd")))
                {
                    using (StreamWriter streamWriter = new StreamWriter("coder", false))
                    {
                        streamWriter.WriteLine(1);
                        streamWriter.Close();
                    }
                }
            }

            GlobalInfo.SetTcpListener(9955);

            DispatcherTimer dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };

            dispatcherTimer.Start();
            dispatcherTimer.Tick += (s, e) => {
            };

            var listener = GlobalInfo.GetTcpListener();
            listener.Start();

            new Task(() =>
            {
                while (true)
                {
                    var m_socket = listener.AcceptSocket();

                    Console.WriteLine(IPAddress.Parse(((IPEndPoint)m_socket.LocalEndPoint).Address.ToString()));

                    new Task(() =>
                    {

                        byte[] bytes = new byte[4096];

                        Dispatcher.Invoke(() =>
                        {
                            m_socket.Receive(bytes);

                            string recv = Encoding.UTF8.GetString(bytes);
                            
                            OrderMenuList orderMenuList = new OrderMenuList();

                            recv = recv.Substring(0, recv.IndexOf('\0'));

                            Console.WriteLine(recv);

                            string[] SplitSemi = recv.Split(';');

                            string TableNumber = SplitSemi[SplitSemi.Length - 1];

                            SplitSemi = SplitSemi.Where(w => w != SplitSemi[SplitSemi.Length-1]).ToArray();

                            foreach (string x in SplitSemi)
                            {
                                string[] SplitComma = x.Split(',');

                                orderMenuList.AddItem(new myItem(SplitComma[0], int.Parse(SplitComma[1]), int.Parse(SplitComma[2])));
                            }

                            AddMenuForm(orderMenuList, TableNumber);

                            GlobalInfo.resort();
                        });

                        m_socket.Close();

                    }).Start();
                }
            }).Start();

            var TotalOrderListBox = GlobalInfo.TotalOrderListBox;

            TotalOrderListBox.Parent.SetValue(ContentPresenter.ContentProperty, null);

            MainGrid.Children.Add(TotalOrderListBox);

            Grid.SetColumn(TotalOrderListBox, 1);
        }

        private void AddMenuForm(OrderMenuList orderMenuList, string tbNumber)
        {
            OrderMenuForm orderMenuForm = new OrderMenuForm(orderMenuList, tbNumber);

            var OrderMenu = orderMenuForm.OrderMenu;

            OrderMenu.Parent.SetValue(ContentPresenter.ContentProperty, null);

            Orderment.QueueOrderment.Add(OrderMenu);

            List1.Items.Clear();
            List2.Items.Clear();

            for (int i = 0; i < Orderment.QueueOrderment.Count; i++)
            {
                if (i == 10) break;

                if (i < 5)
                    List1.Items.Add(Orderment.QueueOrderment[i]);
                else
                    List2.Items.Add(Orderment.QueueOrderment[i]);
            }
        }
    }
}
