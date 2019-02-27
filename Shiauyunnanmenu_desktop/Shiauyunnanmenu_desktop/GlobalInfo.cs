using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shiauyunnanmenu_desktop
{

    public static class Orderment
    {
        public static List<ListBox> QueueOrderment = new List<ListBox>();

        public static int List1ItemsCount
        {
            get { return (((Grid)Application.Current.MainWindow.Content).Children[0] as ListBox).Items.Count; }
        }
        public static int List2ItemsCount
        {
            get { return (((Grid)Application.Current.MainWindow.Content).Children[1] as ListBox).Items.Count; }
        }
    }

    public static class GlobalInfo
    {
        private static TcpListener tcpListener;

        public static void SetTcpListener(int port)
        {
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    MessageBox.Show(ip.ToString());
                    tcpListener = new TcpListener(ip, port);
                }
            }
        }

        public static TcpListener GetTcpListener()
        {
            return tcpListener;
        }

        private static TotalOrderForm totalOrderForm = new TotalOrderForm();

        public static ListBox TotalOrderListBox
        {
            get { return totalOrderForm.TotalOrderListBox; }
        }

        //public static List<CountGrid> countGrids = new List<CountGrid>();

        public static string[] SpecialItem = new string[] { "白飯", "附飯", "金啤", "羊乳片", "十二菜一湯", "八菜一湯", "七菜一湯", "六菜一湯", "五菜一湯", "四菜一湯", "烤雞套餐", "鱒魚套餐", "鱘龍魚套餐"};

        public static bool isItemSpecial(string itemname)
        {
            foreach(string x in SpecialItem)
            {
                if(x.Equals(itemname))
                {
                    return true;
                }
            }
            return false;
        }

        public static string[] ImageNameList = Directory.GetFiles("圖片", "*.*", SearchOption.AllDirectories);

        public static void resort()
        {
            int kCount = GlobalInfo.TotalOrderListBox.Items.Count;

            var kItem = GlobalInfo.TotalOrderListBox.Items;

            List<object> TempGrid = new List<object>();

            for (int i = 0; i < kCount; i++)
            {
                TempGrid.Add(kItem[i]);
            }

            kItem.Clear();

            for (int i = 0; i < kCount - 1; i++)
            {
                for (int j = 0; j < kCount - i - 1; j++)
                {
                    if ((TempGrid[j] as CountGrid).KindCount < (TempGrid[j + 1] as CountGrid).KindCount)
                    {

                        var tempItem = TempGrid[j];
                        TempGrid[j] = TempGrid[j + 1];
                        TempGrid[j + 1] = tempItem;
                    }
                }
            }

            foreach (var x in TempGrid)
            {
                kItem.Add(x);
            }
        }
    }

    public class myItem : Border
    {
        public string IName;
        public int Price;
        public int Count;
        public CountGrid LinkCountGrid;
        public Viewbox vblk2text;
        public Grid ck = new Grid();
        
        public myItem(string ItemName, int ItemPrice, int ItemCount)
        {
            IName = ItemName;
            Price = ItemPrice;
            Count = ItemCount;

            BorderBrush = Brushes.Gray;
            BorderThickness = new Thickness(0, 0, 0, 1);

            Child = ck;
        }
    }

    public class CountGrid : Border
    {
        public myItem linkItem;

        Image Picbk = new Image();
        TextBlock Namebk = new TextBlock();
        TextBlock Countbk = new TextBlock();

        public int KindCount
        {
            set {

                Countbk.Text = value.ToString();
            }

            get
            {
                return int.Parse(Countbk.Text);
            }
        }

        
        public CountGrid(myItem item)
        {
            string localpack = "pack://siteoforigin:,,,/";

            BorderThickness = new Thickness(0,0,0,1);
            BorderBrush = Brushes.Gray;

            Width = 550;
            Picbk.Height = 58;

            linkItem = item;

            string hAndle = item.IName;
            string PickBase = "";

            if(hAndle.Contains("(招)"))
            {
                hAndle = hAndle.Substring(3);
            }

            if(hAndle.Contains("*"))
            {
                hAndle = hAndle.Substring(0, hAndle.IndexOf("*"));
            }

            Picbk.Stretch = Stretch.Uniform;

            foreach(string k in GlobalInfo.ImageNameList)
            {
                string px = k.Substring(3);
                px = px.Substring(0, px.IndexOf("."));

                if (px.Equals(hAndle))
                {
                    PickBase = k; 
                    break;
                }
            }

            //MessageBox.Show(PickBase);

            try
            {
                BitmapImage img = new BitmapImage();
              
                Uri uri = new Uri(localpack + PickBase);
                img.BeginInit();
                img.UriSource = uri;
                img.EndInit();

                Picbk.Source = img;
            }
            catch
            {
                //MessageBox.Show("error");
                Console.WriteLine("找不到圖片");
            }

            Grid R = new Grid();
            
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(4, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(9, GridUnitType.Star);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(3, GridUnitType.Star);

            R.ColumnDefinitions.Add(c1);
            R.ColumnDefinitions.Add(c2);
            R.ColumnDefinitions.Add(c3);

            R.Children.Add(Picbk);
            R.Children.Add(Namebk);
            R.Children.Add(Countbk);

            Grid.SetColumn(Namebk, 1);
            Grid.SetColumn(Countbk, 2);

            Namebk.Text = linkItem.IName;
            Namebk.FontSize = 44;
            Namebk.HorizontalAlignment = HorizontalAlignment.Center;
            Namebk.VerticalAlignment = VerticalAlignment.Center;

            if (linkItem.IName.Contains("(招)"))
            {
                Namebk.Text = Namebk.Text.Substring(3);
            }

            Countbk.Text = linkItem.Count.ToString();
            Countbk.FontSize = 44;
            Countbk.Foreground = Brushes.RoyalBlue;
            Countbk.HorizontalAlignment = HorizontalAlignment.Center;
            Countbk.VerticalAlignment = VerticalAlignment.Center;

            Child = R;
            /*if (!reverse)
                GlobalInfo.TotalOrderListBox.Items.Insert(GlobalInfo.TotalOrderListBox.Items.Count, this);
            else
                GlobalInfo.TotalOrderListBox.Items.Insert(1, this);*/
        }
        /*
        public bool SameKindTryAdd()
        {
            foreach (Grid gd in GlobalInfo.TotalOrderListBox.Items)
            {
                TextBlock Nbk = gd.Children[1] as TextBlock;
                TextBlock Cbk = gd.Children[2] as TextBlock;
                if (Nbk.Text.Equals(linkItemList[0].IName))
                {
                    Cbk.Text = KindCount.ToString();
                    return true;
                }
            }
            return false;
        }*/
        /*
        public bool SameKindTrySub()
        {
            foreach (Grid gd in GlobalInfo.TotalOrderListBox.Items)
            {
                TextBlock Nbk = gd.Children[1] as TextBlock;
                TextBlock Cbk = gd.Children[2] as TextBlock;
                if (Nbk.Text.Equals(linkItemList[0].IName))
                {
                    Cbk.Text = (KindCount - linkItemList[0].Count).ToString();
                    return true;
                }
            }
            return false;
        }*/
        /*
        public void AddGridToTotalList(bool reverse = false)
        {
            Namebk.Text = linkItemList[0].IName;
            Namebk.FontSize = 48;
            Namebk.HorizontalAlignment = HorizontalAlignment.Center;

            if (linkItemList[0].IName.Contains("(招)"))
            {
                Namebk.Text = Namebk.Text.Substring(3);
            }

            Countbk.Text = linkItemList[0].Count.ToString();
            Countbk.FontSize = 48;
            Countbk.Foreground = Brushes.RoyalBlue;
            Countbk.HorizontalAlignment = HorizontalAlignment.Center;

            if (!reverse)
                GlobalInfo.TotalOrderListBox.Items.Insert(GlobalInfo.TotalOrderListBox.Items.Count, this);
            else
                GlobalInfo.TotalOrderListBox.Items.Insert(1, this);
        }*/
    } 
    

    public class OrderMenuList
    {
        ArrayList Items;

        public OrderMenuList() => Items = new ArrayList();

        public void AddItem(myItem item) => Items.Add(item);

        public void removeItem(myItem item) => Items.Remove(item);

        public void clearItems() => Items.Clear();

        public ArrayList GetItems()
        {
            return Items;
        }
    }

    public static class SerializeObj
    { 
        public static T DeepCopy<T>(T element)
        {
            var xaml = XamlWriter.Save(element);
            var xamlString = new StringReader(xaml);
            var xmlTextReader = new XmlTextReader(xamlString);
            var deepCopyObject = (T)XamlReader.Load(xmlTextReader);
            return deepCopyObject;
        }
    }
}
