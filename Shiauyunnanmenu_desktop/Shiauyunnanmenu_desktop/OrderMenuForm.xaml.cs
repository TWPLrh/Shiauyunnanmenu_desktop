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
using System.Windows.Shapes;
using System.IO;

namespace Shiauyunnanmenu_desktop
{
    /// <summary>
    /// Interaction logic for OrderMenuForm.xaml
    /// </summary>
    /// 

    public partial class OrderMenuForm : Window
    {
        int Tablenum;
        int OrderCode;

        //ListBox InitialStatusListBox;
        List<myItem> StatusSteps;

        Image imageBlock = new Image();
        TextBlock DishNameBlock = new TextBlock();
        TextBlock DishCountBlock = new TextBlock();

        OrderMenuList ordermenu;

        private async void SolveCoder()
        {
            if (File.Exists("coder"))
            {
                using (StreamReader streamReader = new StreamReader("coder"))
                { 
                    OrderCode = int.Parse(streamReader.ReadLine());
                    streamReader.Close();
                }

                using (StreamWriter streamWriter = new StreamWriter("coder", false))
                {
                    streamWriter.WriteLine((OrderCode + 1).ToString());
                    streamWriter.Close();
                }
            }
        }

        public OrderMenuForm(OrderMenuList orderMenuList, string TableNumber)
        {
            InitializeComponent();

            ordermenu = orderMenuList;

            var ItemCOL = OrderMenu.Items;

            SolveCoder();

            int CountComboItem = 0;
            foreach(TextBlock tblkcombo in comboBox.Items)
            {
                if(tblkcombo.Text.Contains(TableNumber))
                {
                    break;
                }

                CountComboItem++;
            }

            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            comboBox.SelectedIndex = CountComboItem;

            ListBox PrintBox = new Printform().Content as ListBox;
            string Uid = OrderMenu.Uid = DateTime.Now.ToString("HHmmssMMdd");

            TextBlock tbnbk = new TextBlock();
            tbnbk.FontSize = 16;
            tbnbk.Text = "桌號 : " + (comboBox.SelectedItem as TextBlock).Text + "  " + Uid + OrderCode.ToString("D2");

            //MessageBox.Show(tbnbk.Text);
            PrintBox.Items.Add(tbnbk);

            int Amount = 0;
            foreach (myItem x in orderMenuList.GetItems())
            {
                x.vblk2text = new Viewbox();
                TextBlock textblk = new TextBlock();

                textblk.FontSize = 18;

                x.ck.Height = 24;
                x.ck.Children.Add(x.vblk2text);
                x.vblk2text.Child = textblk;
                x.vblk2text.PreviewMouseLeftButtonUp += previewMouseLeftButtonUP;

                if (x.Count != 1)
                    textblk.Text = x.IName + "*" + x.Count;
                else
                    textblk.Text = x.IName;

                ItemCOL.Insert(ItemCOL.Count - 1, x);

                Amount += x.Count * x.Price;
                TextBlock tbnlk = new TextBlock();
                tbnlk.Text = x.IName + "(" + x.Price + ")" + "*" + x.Count;
                tbnlk.FontSize = 20;
                PrintBox.Items.Add(tbnlk);

                if(GlobalInfo.isItemSpecial(x.IName)) continue;

                bool isContain = false;
                foreach (var cg in GlobalInfo.TotalOrderListBox.Items)
                {
                    try
                    {
                        if ((cg as CountGrid).linkItem.IName.Equals(x.IName))
                        {
                            isContain = true;
                            (cg as CountGrid).KindCount += x.Count;
                            break;
                        }
                    }
                    catch
                    {

                    }
                }

                if (!isContain)
                {
                    GlobalInfo.TotalOrderListBox.Items.Add(new CountGrid(x));
                }
            }

            TextBlock tbAmount = new TextBlock();
            tbAmount.Text = "總價 : " + Amount.ToString();
            tbAmount.FontSize = 24;
            tbAmount.HorizontalAlignment = HorizontalAlignment.Right;
            PrintBox.Items.Add(tbAmount);

            PrintDialog pd = new PrintDialog();
            pd.PrintVisual(PrintBox, OrderMenu.Uid);

            //InitialStatusListBox = SerializeObj.DeepCopy<ListBox>(OrderMenu);

            StatusSteps = new List<myItem>();
        }

        private void previewMouseLeftButtonUP(object sender, object EventArgs)
        {
            myItem CastSender = ((sender as Viewbox).Parent as Grid).Parent as myItem;

            (CastSender.Parent as ListBox).Items.Remove(CastSender);

            StatusSteps.Add(CastSender);

            int t = GlobalInfo.TotalOrderListBox.Items.Count;

            for (int i = 0; i < t; i++)
            {
                var P = GlobalInfo.TotalOrderListBox.Items[i] as CountGrid;

                if(P.linkItem.IName.Equals(CastSender.IName))
                {
                    P.KindCount -= CastSender.Count;

                    if (P.KindCount == 0)
                    {
                        GlobalInfo.TotalOrderListBox.Items.Remove(P);
                    }
                    break;
                }
            }
            /*CountGrid countGrid = new CountGrid(CastSender);

            if (countGrid.ifContain(CastSender))
            { 

            }*/
            /*
            var Castsender = sender as myItem;

            (Castsender.Parent as ListBox).Items.Remove(Castsender);

            TextBlock tb = (Castsender.Children[0] as Viewbox).Child as TextBlock;
                
            StatusSteps.Add(Castsender);

            int Cnt = GlobalInfo.TotalOrderListBox.Items.Count;

            for (int i = 0; i < Cnt; i++)
            {
                Grid gd = GlobalInfo.TotalOrderListBox.Items[i] as Grid;
                TextBlock Nbk = gd.Children[1] as TextBlock;
                TextBlock Cbk = gd.Children[2] as TextBlock;

                string tryc = tb.Text;

                if (tryc.Contains("(招)")) tryc = tryc.Substring(3);

                try
                {
                    if (GlobalInfo.SpecialItem.Contains(tryc.Substring(0, Nbk.Text.Length - 4)) && tryc.Length > 4)
                        continue;
                }
                catch
                {
                    ;
                }

                int Count = int.Parse(Cbk.Text);

                if (Nbk.Text.Equals(tryc))
                {
                    Count--;
                    Cbk.Text = Count.ToString();     
                }

                if (Count <= 0)
                {
                    GlobalInfo.TotalOrderListBox.Items.Remove(Cbk.Parent as Grid);
                    break;
                }
            }*/
        }

        private void RevertButton_Click(object sender, RoutedEventArgs e)
        {
            Button l1 = sender as Button;
            Grid l2 = l1.Parent as Grid;
            ListBox l3 = l2.Parent as ListBox;

            if (!StatusSteps.Count.Equals(0))
            {
                myItem BackItem = StatusSteps[StatusSteps.Count - 1];
                OrderMenu.Items.Insert(OrderMenu.Items.Count - 1, BackItem);
                StatusSteps.RemoveAt(StatusSteps.Count - 1);

                if (GlobalInfo.isItemSpecial(BackItem.IName)) return;

                bool isContain = false;
                foreach (var cg in GlobalInfo.TotalOrderListBox.Items)
                {
                    try
                    {
                        if ((cg as CountGrid).linkItem.IName.Equals(BackItem.IName))
                        {
                            (cg as CountGrid).KindCount += BackItem.Count;
                            isContain = true;
                            break;
                        }
                    }
                    catch
                    {

                    }
                }

                if (!isContain)
                {
                    GlobalInfo.TotalOrderListBox.Items.Add(new CountGrid(BackItem));
                }
            };
        }
        /*
        private void InitialButton_Click(object sender, RoutedEventArgs e)
        {
            Button l1 = sender as Button;
            Grid l2 = l1.Parent as Grid;
            ListBox l3 = l2.Parent as ListBox;
            ListBox l4 = l3.Parent as ListBox;

            ListBox copylb = SerializeObj.DeepCopy<ListBox>(InitialStatusListBox);

            ((copylb.Items[0] as Grid).Children[1] as ComboBox).SelectionChanged += ComboBox_SelectionChanged;
            ((copylb.Items[0] as Grid).Children[1] as ComboBox).SelectedIndex = Tablenum - 1;

            var ItemCOL = copylb.Items;

            int itc = copylb.Items.Count;
            
            for (int i = 1; i < itc - 1; i++)
            {
                if (i <= copylb.Items.Count - 2)
                {
                    Grid grid = ItemCOL[i] as Grid;

                    grid.PreviewMouseLeftButtonUp += previewMouseLeftButtonUP;

                    TextBlock tb = (grid.Children[0] as Viewbox).Child as TextBlock;

                    int Cnt = GlobalInfo.TotalOrderListBox.Items.Count;

                    for (int j = 0; j < Cnt; j++)
                    {
                        Grid gd = GlobalInfo.TotalOrderListBox.Items[j] as Grid;
                        TextBlock Nbk = gd.Children[1] as TextBlock;
                        TextBlock Cbk = gd.Children[2] as TextBlock;

                        int Count = int.Parse(Cbk.Text);

                        if (Nbk.Text.Equals(tb.Text))
                        {
                            Count--;
                            Cbk.Text = Count.ToString();
                        }

                        if (Count <= 0)
                        {
                            GlobalInfo.TotalOrderListBox.Items.Remove(Cbk.Parent as Grid);
                            break;
                        }
                    }

                    myItem it = new myItem(tb.Text, 0, 1);

                    if (!IsInTotalOrder(it))
                    {
                        AddGrid2Total(it);
                    };

                }
            }

            ((copylb.Items[copylb.Items.Count - 1] as Grid).Children[0] as Button).Click += RevertButton_Click;
            ((copylb.Items[copylb.Items.Count - 1] as Grid).Children[1] as Button).Click += InitialButton_Click;
            ((copylb.Items[copylb.Items.Count - 1] as Grid).Children[2] as Button).Click += OverButton_Click;

            int ItemIndex = Orderment.QueueOrderment.IndexOf(l3);

            Orderment.QueueOrderment.RemoveAt(ItemIndex);
            Orderment.QueueOrderment.Insert(ItemIndex, copylb);

            RefreshListBoxView();

            StatusSteps.Clear();
        }*/

        private void OverButton_Click(object sender, RoutedEventArgs e)
        {
            Button l1 = sender as Button;
            Grid l2 = l1.Parent as Grid;
            ListBox l3 = l2.Parent as ListBox;
            ListBox l4 = l3.Parent as ListBox;

            //MessageBox.Show(l3.Items.Count.ToString());

            if (l3.Items.Count > 2)
            {
                MessageBox.Show("菜單列內必須清空");
                return;
            }

            l4.Items.Remove(l3);
            Orderment.QueueOrderment.Remove(l3);

            RefreshListBoxView();

            StatusSteps.Clear();
        }

        private void RefreshListBoxView()
        {
            ListBox listbox1 = ((Grid)Application.Current.MainWindow.Content).Children[0] as ListBox;
            ListBox listbox2 = ((Grid)Application.Current.MainWindow.Content).Children[1] as ListBox;

            listbox1.Items.Clear();
            listbox2.Items.Clear();

            for (int i = 0; i < Orderment.QueueOrderment.Count; i++)
            {
                if (i == 10) break;

                if (i < 5)
                    listbox1.Items.Add(Orderment.QueueOrderment[i]);
                else
                    listbox2.Items.Add(Orderment.QueueOrderment[i]);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            try
            {
                Tablenum = int.Parse((cb.SelectedItem as TextBlock).Text);
            }
            catch
            {

            }
        }
    }
}
