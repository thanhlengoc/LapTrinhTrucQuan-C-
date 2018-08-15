using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Demo_Sort
{
    public partial class frm_main : Form
    {
        public frm_main()
        {
            InitializeComponent();
            this.Scale(new SizeF((float)0.8,(float)0.8));
            //Tải code của thuật toán Insertion Sort vào Cửa sổ xem code
            lst_Code.Items.Add("void InsertionSort(int a[], int n)");
            lst_Code.Items.Add("{");
            lst_Code.Items.Add("   int i, pos, x;");
            lst_Code.Items.Add("   for (i = 1; i < n; i++)");
            lst_Code.Items.Add("   {");
            lst_Code.Items.Add("       x = a[i];");
            lst_Code.Items.Add("       pos = i - 1;");
            lst_Code.Items.Add("       while ((pos >= 0) && (a[pos] > x))");
            lst_Code.Items.Add("       {");
            lst_Code.Items.Add("           a[pos + 1] = a[pos];");
            lst_Code.Items.Add("           pos--;");
            lst_Code.Items.Add("       }");
            lst_Code.Items.Add("       a[pos + 1] = x;");
            lst_Code.Items.Add("    }");
            lst_Code.Items.Add("}");
        }
        //Vô hiệu hóa nút X trên form chính
        const int MF_BYPOSITION = 0x400;
        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("User32")]
        private static extern int GetMenuItemCount(IntPtr hWnd);
        private void frm_main_Load(object sender, EventArgs e)
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
        }
        //Khai báo biến toàn cục
        #region Khai báo các biến toàn cục
        TextBox[] Node;
        TextBox[] Node_B, Node_C;
        int[] a, b, c, h;
        int Spt = 0;
        Label[] Chi_so;
        int Toc_do;
        int i, Kthuoc_Code = 12;
        Boolean Da_tao_mang, KT_tam_dung = false; //Biến kiểm tra đã tạo mảng và kiểm tra tạm dừng
        Boolean Sap_xep_tung_buoc; //Biến kiểm tra sắp xếp 1 lần hay từng bước     
        #endregion
        //Khai báo các biến định dạng node
        #region Khai báo các biến định dạng node
        int Khoang_cach;//khoảng cách 2 node liên tiếp
        int Canh_le;//canh lề node
        int Kich_thuoc; // kích thước node
        int Co_chu; //kích thước chữ
        #endregion Khai báo các biến định dạng node
        //Các hàm di chuyển node
        #region CÁC HÀM DI CHUYỂN NODE
        // Hàm swap 2 node có thể hiện
        public void Hoan_Vi_Node(Control t1, Control t2)
        {
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                Point p1 = t1.Location; //lưu vị trí ban đầu của t1
                Point p2 = t2.Location; //lưu vị trí ban đầu của t2
                if (p1 != p2)
                {
                    // t1 lên, t2 xuống
                    while ((t1.Location.Y > p1.Y - (Kich_thuoc + 5)) || (t2.Location.Y < p2.Y + (Kich_thuoc + 5)))
                    {
                        Application.DoEvents();
                        t1.Top -= 1;
                        t2.Top += 1;
                        Tre(Toc_do);
                    }
                    // t1 dịch phải, t2 dịch trái
                    if (t1.Location.X < t2.Location.X)
                    {

                        while ((t1.Location.X < p2.X) || (t2.Location.X > p1.X))
                        {
                            Application.DoEvents();
                            t1.Left += 1;
                            t2.Left -= 1;
                            Tre(Toc_do);
                        }
                    }
                    // t1 dịch trái, t2 dịch phải
                    else
                    {
                        while ((t1.Location.X > p2.X) || (t2.Location.X < p1.X))
                        {
                            Application.DoEvents();
                            t1.Left -= 1;
                            t2.Left += 1;
                            Tre(Toc_do);
                        }
                    }
                    // t1 xuống, t2 lên
                    while ((t1.Location.Y < p2.Y) || (t2.Location.Y > p1.Y))
                    {
                        Application.DoEvents();
                        t1.Top += 1;
                        t2.Top -= 1;
                        Tre(Toc_do);
                    }
                    t1.Refresh();
                    t2.Refresh();
                }
            });
        }
        // t dịch chuyển sang Phải Step Node
        public void Node_qua_phai(Control t, int Step)
        {
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                int Loop_Count = ((Kich_thuoc + Khoang_cach)) * Step; //Số lần dịch chuyển
                {
                    while (Loop_Count > 0)
                    {
                        Application.DoEvents();
                        t.Left += 1;
                        Tre(Toc_do);
                        Loop_Count--;
                    }
                }
                t.Refresh();
            });
        }
        // t dịch chuyển sang trai Step Node
        public void Node_qua_trai(Control t, int Step)
        {
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                int Loop_Count = ((Kich_thuoc + Khoang_cach)) * Step; //Số lần dịch chuyển
                while (Loop_Count > 0)
                {
                    Application.DoEvents();
                    t.Left -= 1;
                    Tre(Toc_do);
                    Loop_Count--;
                }
                t.Refresh();
            });
        }
        // t dịch chuyển lên với quãng đường S
        public void Node_di_len(Control t, int S)
        {
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                int loop_Count = S;
                //t xuống
                while (loop_Count > 0)
                {
                    Application.DoEvents();
                    t.Top -= 1;
                    Tre(Toc_do);
                    loop_Count--;
                }
                t.Refresh();
            });
        }
        // t dịch chuyển xuống với quãng đường S
        public void Node_di_xuong(Control t, int S)
        {
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                int loop_Count = S;
                // t lên
                while (loop_Count > 0)
                {
                    Application.DoEvents();
                    t.Top += 1;
                    Tre(Toc_do);
                    loop_Count--;
                }
                t.Refresh();
            });
        }
        // Dịch chuyển 1 Node về vị trí Node[i]
        public void Den_vtri_node(Control t, int i)
        {
            Point p1 = t.Location; // lưu lại vị trí của t
            Point p2 = new Point(Canh_le + (Kich_thuoc + Khoang_cach) * i, 250);//vị trí của Node i
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                // Di chuyển nút lên hoặc xuống nữa đường
                if (p1.Y < p2.Y)
                {
                    while (t.Location.Y < p2.Y - ((p2.Y - p1.Y) / 2))
                    {
                        Application.DoEvents();
                        t.Top += 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
                else
                {
                    while (t.Location.Y > p2.Y + ((p1.Y - p2.Y) / 2))
                    {
                        Application.DoEvents();
                        t.Top -= 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
                // Di chuyển nút qua phải hoặc trái
                if (p1.X < p2.X)
                {
                    while (t.Location.X < p2.X)
                    {
                        Application.DoEvents();
                        t.Left += 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
                else
                {
                    while (t.Location.X > p2.X)
                    {
                        Application.DoEvents();
                        t.Left -= 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
                // Tiếp tục di chuyển nút lên hoặc xuống nữa đường còn lại
                if (p1.Y < p2.Y)
                {
                    while (t.Location.Y < p2.Y)
                    {
                        Application.DoEvents();
                        t.Top += 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
                else
                {
                    while (t.Location.Y > p2.Y)
                    {
                        Application.DoEvents();
                        t.Top -= 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
            });
        }
        // Dịch chuyển 1 Node về vị trí bằng với X của Node[i]
        public void Den_tdo_x_node(Control t, int i)
        {
            Point p1 = t.Location; // lưu lại vị trí của t
            Point p2 = new Point(Canh_le + (Kich_thuoc + Khoang_cach) * i, 250);//vị trí của Node i
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                // Di chuyển nút qua phải hoặc trái
                if (p1.X < p2.X)
                {
                    while (t.Location.X < p2.X)
                    {
                        Application.DoEvents();
                        t.Left += 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
                else
                {
                    while (t.Location.X > p2.X)
                    {
                        Application.DoEvents();
                        t.Left -= 1;
                        t.Refresh();
                        Tre(Toc_do);
                    }
                }
            });
        }
        #endregion
        //Các hàm khác
        #region CÁC HÀM KHÁC
        //Hàm ngủ
        public void SleepP(int milisecond)
        {
            for (int i = 0; i < milisecond; i++)
            {         
                Application.DoEvents();
                Thread.Sleep(1);
            }
            Application.DoEvents();       
        }
        //Hàm Trễ
        public void Tre(int milisecond)
        {  
           //Nếu tốc độ max thì ko sleep
              if(Trb_Toc_do.Value == Trb_Toc_do.Maximum)
              {
                  Application.DoEvents();
                  return;
              }
              SleepP(milisecond);
        }
        // Hàm hoán vị 2 node mà không thể hiện
        public void Hoan_Tri_Node(int t1, int t2)
        {
            TextBox Temp = Node[t1];
            Node[t1] = Node[t2];
            Node[t2] = Temp;
        }
        // Hàm hoán vị 2 giá trị
        public void Hoan_vi(ref int i, ref int j)
        {
            int Temp = i;
            i = j;
            j = Temp;
        }
        //Sắp xếp hoàn thành
        public void Hoan_thanh()
        {                    
            for (i = 0; i < Spt; i++)
            {
                Dat_mau_node(Node[i], Color.LawnGreen, Color.Black);
            }
            //Ẩn các index
            Mui_ten_xanh_xuong_1.Visible = false;
            Mui_ten_xanh_xuong_2.Visible = false;
            Mui_ten_xanh_len_1.Visible = false;
            Mui_ten_xanh_len_2.Visible = false;
            Mui_ten_do_len.Visible = false;
            //
            lbl_status_02.Visible = true;
            lbl_status_02.Text = "Đã sắp xếp xong!";
            btn_ngaunhien.Enabled = false;
            btn_nhap.Enabled = false;
            btn_dung.Enabled = false;
            btn_chayhet.Enabled = false;
            btn_ketiep.Enabled = true;
            //Thiết lập các nút nhập liệu
            rad_bubblesort.Enabled = true;
            rad_heapsort.Enabled = true;
            rad_insertionsort.Enabled = true;
            rad_mergesort.Enabled = true;
            rad_quicksort.Enabled = true;
            rad_selectionsort.Enabled = true;
            rad_shackersort.Enabled = true;
            rad_shellsort.Enabled = true;
            //     
            btn_docfile.Enabled = true;
            btn_taomang.Enabled = true;
            btn_ngaunhien.Enabled = true;
            btn_nhap.Enabled = true;
            txt_chiso.Enabled = true;
            txt_giatri.Enabled = true;
        }
        //Hàm tạo mảng
        public void Tao_mang()
        {
            if ((Spt < 2) || (Spt > 30))
            {
                lbl_A.Visible = false;
                MessageBox.Show("2 <= Số Phần Tử <= 30");
                this.txt_sophantu.Clear();
                Da_tao_mang = false;
                return;
            }
            #region Thiết lập thuộc tính node ứng với số phần tử
            switch (Spt)
            {
                case 30:
                case 29:
                case 28:
                case 27:
                case 26:
                    Kich_thuoc = 27;
                    Co_chu = 10;
                    Khoang_cach = 6;
                    Canh_le = (1044 - Kich_thuoc * Spt - Khoang_cach * (Spt - 1)) / 2;
                    break;
                case 25:
                case 24:
                case 23:
                case 22:
                case 21:
                    Kich_thuoc = 30;
                    Co_chu = 15;
                    Khoang_cach = 8;
                    Canh_le = (1044 - Kich_thuoc * Spt - Khoang_cach * (Spt - 1)) / 2;
                    break;
                case 20:
                case 19:
                    Kich_thuoc = 35;
                    Co_chu = 15;
                    Khoang_cach = 8;
                    Canh_le = (1044 - Kich_thuoc * Spt - Khoang_cach * (Spt - 1)) / 2;
                    break;
                case 18:
                case 17:
                case 16:
                    Kich_thuoc = 38;
                    Co_chu = 15;
                    Khoang_cach = 11;
                    Canh_le = (1044 - Kich_thuoc * Spt - Khoang_cach * (Spt - 1)) / 2;
                    break;
                case 15:
                case 14:
                case 13:
                case 12:
                case 11:
                    Kich_thuoc = 40;
                    Co_chu = 18;
                    Khoang_cach = 16;
                    Canh_le = (1044 - Kich_thuoc * Spt - Khoang_cach * (Spt - 1)) / 2;
                    break;
                case 10:
                case 9:
                case 8:
                case 7:
                case 6:
                case 5:
                case 4:
                case 3:
                case 2:
                    Kich_thuoc = 45;
                    Co_chu = 22;
                    Khoang_cach = 20;
                    Canh_le = (1044 - Kich_thuoc * Spt - Khoang_cach * (Spt - 1)) / 2;
                    break;
            }
            #endregion
            #region Tạo các mảng dữ liệu
            Chi_so = new Label[Spt];
            a = new int[Spt];
            Node = new TextBox[Spt];
            #endregion
            //Dán nhãn mảng a
            lbl_A.Width = Kich_thuoc;
            lbl_A.Height = Kich_thuoc;
            lbl_A.Location = new Point(Canh_le - (Kich_thuoc), 250);
            lbl_A.Font = new System.Drawing.Font("Arial", Co_chu, FontStyle.Bold);
            lbl_A.Visible = true;
            #region Tạo node và chỉ số
            for (i = 0; i < Spt; i++)
            {
                //node
                a[i] = i;
                Node[i] = new TextBox();
                Node[i].Multiline = true;
                Node[i].Text = a[i].ToString();
                Node[i].TextAlign = HorizontalAlignment.Center;
                Node[i].Width = Kich_thuoc;
                Node[i].Height = Kich_thuoc;
                Node[i].Location = new Point(Canh_le + (Kich_thuoc + Khoang_cach) * i, 250);
                Node[i].BackColor = Color.OrangeRed;
                Node[i].ForeColor = Color.White;
                Node[i].Font = new Font(this.Font, FontStyle.Bold);
                Node[i].Font = new System.Drawing.Font("Arial", Co_chu, FontStyle.Bold);
                Node[i].ReadOnly = true;
                this.Controls.Add(Node[i]);
                //chỉ số
                Chi_so[i] = new Label();
                Chi_so[i].TextAlign = ContentAlignment.MiddleCenter;
                Chi_so[i].Text = i.ToString();
                Chi_so[i].Width = Kich_thuoc;
                Chi_so[i].Height = Kich_thuoc;
                Chi_so[i].ForeColor = Color.Azure;
                Chi_so[i].Location = new Point(Canh_le + (Kich_thuoc + Khoang_cach) * i, 320 + 3 * Kich_thuoc);
                if (Spt <= 10)
                {
                    Chi_so[i].Font = new System.Drawing.Font("Arial", Co_chu - 7, FontStyle.Regular);
                }
                else
                {
                    Chi_so[i].Font = new System.Drawing.Font("Arial", Co_chu - 3, FontStyle.Regular);
                }
                this.Controls.Add(Chi_so[i]);
                Da_tao_mang = true; //Xác nhận đã tạo mảng
                //Cho phép các nút điều khiển có tác dụng khi đã tạo mảng
                btn_sapxep.Enabled = true;
                btn_ngaunhien.Enabled = true;
                btn_nhap.Enabled = true;
            }
            #endregion
        }
        //Hàm xóa mảng
        public void Xoa_mang()
        {
            btn_nhap.Enabled = false;
            btn_ngaunhien.Enabled = false;
            btn_sapxep.Enabled = false;
            if (Da_tao_mang == true)
            {
                for (i = 0; i < Spt; i++)
                {
                    this.Controls.Remove(Node[i]);
                    this.Controls.Remove(Chi_so[i]);
                }
            }
        }
        //Hàm dừng toàn bộ chương trình
        public void Play_or_Stop()
        {
            while (KT_tam_dung == true)
            {
                Application.DoEvents();
            }
        }
        //Hàm Tạm dừng
        public void Tam_dung()
        {
            if (Sap_xep_tung_buoc == true)
            {
                KT_tam_dung = true;
                btn_dung.Enabled = false;
                Play_or_Stop();
            }
        }
        //Hàm đặt màu cho Node
        public void Dat_mau_node(Control t, System.Drawing.Color Mau_nen, System.Drawing.Color Mau_chu)
        {
            t.BackColor = Mau_nen;
            t.ForeColor = Mau_chu;
            t.Refresh();
        }
        #endregion 
        //Các phương thức nhập liệu
        #region Các phương thức nhập dữ liệu
        //Nhập ngẫu nhiên
        private void btn_ngaunhien_Click(object sender, EventArgs e)
        {
            btn_sapxep.Enabled = true;
            Random r = new Random();
            for (i = 0; i < Spt; i++)
            {
                Node[i].BackColor = Color.OrangeRed; // đặt lại màu cho mảng ngẫu nhiên
                Node[i].ForeColor = Color.White;
                a[i] = r.Next(100);
                Node[i].Text = a[i].ToString();
            }
        }
        //Nhập từ file
        private void btn_docfile_Click(object sender, EventArgs e)
        {
            //Gọi hàm xóa mảng
            Xoa_mang();
            //Đọc file
            StreamReader Re = File.OpenText(@"C:\Users\Le Ngoc Thanh\Documents\Visual Studio 2013\Projects\Demo_Sort\Demo_Sort\bin\Debug\TEST.txt");
            string input = Re.ReadLine();
            int i = 0, kt = 0;
            while ((kt < 1) && (input != null))
            {
                Spt = Convert.ToInt32(input);
                kt++;
            }
            //Gọi hàm tạo mảng
            Tao_mang();
            while ((input != null) && (i < Spt))
            {
                Node[i].BackColor = Color.OrangeRed; // đặt lại màu cho mảng ngẫu nhiên
                Node[i].ForeColor = Color.White;
                a[i] = Convert.ToInt32(input);
                Node[i].Text = a[i].ToString();
                i++;
            }
            Re.Close();
        }
        //Nhập bằng tay
        private void btn_nhap_Click(object sender, EventArgs e)
        {
            Boolean KTra = true;
            int giatri;
            try
            {
                i = Convert.ToInt32(txt_chiso.Text);
            }
            catch
            {
                MessageBox.Show("Chỉ số nhập vào không hợp lệ!");
                return;
            }
            if (i > Spt - 1)
            {
                MessageBox.Show("Không có phần tử thứ  " + i);
                return;
            }
            try
            {
                giatri = Convert.ToInt32(txt_giatri.Text);
            }
            catch
            {
                MessageBox.Show("Giá trị nhập vào không hợp lệ!");
                return;
            }
            if ((giatri < 0) || (giatri > 100))
            {
                MessageBox.Show("0 <= Giá trị nhập vào < 100");
                this.txt_giatri.Clear();
                KTra = false;
            }
            if (KTra == true)
            {
                for (int j = 0; j < Spt; j++)
                {
                    Node[j].BackColor = Color.OrangeRed; // đặt lại màu cho mảng không thứ tự
                    Node[j].ForeColor = Color.White;
                }
                a[i] = giatri;
                Node[i].Text = a[i].ToString();
                Chi_so[i].Text = i.ToString();
                //Đổi màu node khi nhận giá trị nhập vào
                Node[i].BackColor = Color.AliceBlue;
                Node[i].ForeColor = Color.Black;
                Node[i].Refresh();
                Thread.Sleep(1000);
                Node[i].BackColor = Color.OrangeRed;
                Node[i].ForeColor = Color.White;
                Node[i].Refresh();
                this.txt_giatri.Text = (giatri + 1).ToString();
                this.txt_chiso.Focus();
                this.txt_chiso.Text = (i + 1).ToString();
                this.txt_chiso.SelectAll();
            }
        }
        #endregion
        //Các hàm sắp xếp
        #region CÁC HÀM SẮP XẾP
        #region INSERTION SORT
        void InsertionSort()
        {
            int i, pos, x;
            TextBox Node_tam;
            int Chi_so_tam;
            int Dem_node;
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                for (i = 1; i < Spt; i++)
                {
                    lst_Code.SelectedIndex = 3;
                    Tre(10 * Toc_do);
                    Application.DoEvents();
                    //Thiết lập Node đầu tiên, là Node đã có thứ tự 
                    Dat_mau_node(Node[0], Color.LawnGreen, Color.Black);
                    //đềm số bước dịch chuyển 1 Node
                    Dem_node = 0;
                    lst_Code.SelectedIndex = 5;
                    x = a[i];
                    Node_tam = Node[i];
                    Chi_so_tam = i;
                    pos = i - 1;
                    //thiết lập mũi tên đánh dấu nút cần chèn
                    Mui_ten_xanh_xuong_1.Visible = true;
                    Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                    Mui_ten_xanh_xuong_1.Text = "i=" + i;
                    Mui_ten_xanh_xuong_1.Refresh();

                    //Di chuyển Node cần chèn lên
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_di_len(Node_tam, (Kich_thuoc + 5));
                    });
                    //Tạm dừng sau 1 bước dịch chuyển Node
                    Tam_dung();
                    //Tìm vị trí chèn Node đã di chuyển lên ở trên
                    lbl_status_02.Visible = true;
                    lbl_status_02.Text = "So_Sanh (a[pos], a[i])";
                    //Thiết lập bàn tay chỉ vi trí có phải vị trí cần chèn không                        
                    Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * pos) + (Kich_thuoc / 2) - 25, Node[pos].Location.Y + Kich_thuoc + 5);
                    Mui_ten_xanh_len_1.Text = "POS=" + pos;
                    Mui_ten_xanh_len_1.Visible = true;
                    Mui_ten_xanh_len_1.Refresh();
                    Tre(10 * Toc_do);
                    //lbl_status_02.Visible = false;
                    while ((pos >= 0) && (a[pos] > x))
                    {
                        Application.DoEvents();
                        lst_Code.SelectedIndex = 7;
                        Tre(10 * Toc_do);
                        //Thiết lập bàn tay chỉ vi trí có phải vị trí cần chèn không                        
                        Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * pos) + (Kich_thuoc / 2) - 25, Node[pos].Location.Y + Kich_thuoc + 5);
                        Mui_ten_xanh_len_1.Text = "POS=" + pos;
                        Mui_ten_xanh_len_1.Visible = true;
                        Mui_ten_xanh_len_1.Refresh();

                        lbl_status_02.Visible = true;
                        lbl_status_02.Text = "So_Sanh (a[pos], a[i])";
                        Tre(10 * Toc_do);
                        lbl_status_02.Visible = false;
                        lst_Code.SelectedIndex = 9;
                        //Dịch chuyển Node qua phải
                        a[pos + 1] = a[pos];
                        Dem_node++;

                        Application.DoEvents();
                        this.Invoke((MethodInvoker)delegate
                        {
                            Node_qua_phai(Node[pos], 1);
                        });
                        Hoan_Tri_Node(pos + 1, pos);
                        lst_Code.SelectedIndex = 10;
                        pos--;
                        lst_Code.SelectedIndex = 12;
                        a[pos + 1] = x;
                        //Tạm dừng sau 1 bước dịch chuyển Node
                        Tam_dung();
                    }
                    //status hoán vị
                    if (Dem_node > 0)
                    {
                        lbl_status_02.Visible = true;
                        lbl_status_02.Text = "Hoan_vi(a[pos],a[i])";
                    }
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_qua_trai(Node_tam, Dem_node);
                    });
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_di_xuong(Node_tam, (Kich_thuoc + 5));
                    });
                    //Ẩn status
                    lbl_status_02.Visible = false;
                    //Ẩn mũi tên POS sau khi đã tìm ra POS
                    Mui_ten_xanh_len_1.Visible = false; ;
                    //Thiết lập node nằm trong nhóm đã có thứ tự
                    Node[pos + 1] = Node_tam;
                    Dat_mau_node(Node[pos + 1], Color.LawnGreen, Color.Black);
                    //Tạm dừng sau 1 bước dịch chuyển Node
                    Tam_dung();
                }
            });
            lst_Code.SelectedIndex = 0;
            Hoan_thanh();
        }
        #endregion
        #region SELECTION SORT
        void SelectionSort()
        {
            int min, i, j;
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                for (i = 0; i < Spt - 1; i++)
                {
                    Application.DoEvents();
                    lst_Code.SelectedIndex = 3;
                    Tre(20 * Toc_do);
                    lst_Code.SelectedIndex = 5;
                    Tre(20 * Toc_do);
                    min = i;
                    //Thiết lập
                    Mui_ten_xanh_xuong_1.Visible = true;
                    Mui_ten_xanh_xuong_1.Text = "i=" + i;
                    Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                    Mui_ten_xanh_xuong_1.Refresh();
                    //thiết lập mũi tên chỉ vị trí MIN đầu tiên
                    Mui_ten_xanh_len_1.Visible = true;
                    Mui_ten_xanh_len_1.Text = "min" + min;
                    Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * min) + (Kich_thuoc / 2) - 25, Node[min].Location.Y + 2 * Kich_thuoc + 5);
                    Mui_ten_xanh_len_1.Refresh();
                    Tre(20 * Toc_do);
                    //
                    for (j = i + 1; j < Spt; j++)
                    {
                        lst_Code.SelectedIndex = 6;
                        Tre(20 * Toc_do);
                        Application.DoEvents();
                        lst_Code.SelectedIndex = 8;
                        Tre(20 * Toc_do);
                        lbl_status_02.Visible = true;
                        lbl_status_02.Text = "So_Sanh( a[min] , a[" + j + "] )";
                        lbl_status_02.Refresh();
                        //đánh dấu phần tử lúc so sánh với min                       
                        Dat_mau_node(Node[j], Color.Navy, Color.White);
                        Tre(20 * Toc_do);
                        lbl_status_02.Visible = false;
                        // bỏ đánh dấu sau khi đã có kết quả so sánh
                        Dat_mau_node(Node[j], Color.OrangeRed, Color.White);

                        if (a[j] < a[min])
                        {
                            lst_Code.SelectedIndex = 9;
                            min = j;
                            Tam_dung();
                            //thiết lập mũi tên chỉ vị trí MIN
                            Mui_ten_xanh_len_1.Text = "Min=" + min;
                            Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * min) + (Kich_thuoc / 2) - 25, Node[j].Location.Y + 2 * Kich_thuoc + 5);
                            Mui_ten_xanh_len_1.Refresh();
                            Tre(20 * Toc_do);
                        }                                                                        
                    }
                    //status
                    if (min != i)
                    {
                        lbl_status_02.Visible = true;
                        lbl_status_02.Text = "Hoan_vi( a[i] , a[min] )";
                        Tre(20 * Toc_do);
                        lbl_status_02.Visible = false;
                    }
                    //
                    lst_Code.SelectedIndex = 12;
                    Tre(20 * Toc_do);
                    Hoan_vi(ref a[min], ref a[i]);
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Hoan_Vi_Node(Node[min], Node[i]);
                    });
                    Tam_dung();
                    Hoan_Tri_Node(min, i);
                    //Thiết lập nút đã có thứ tự
                    Dat_mau_node(Node[i], Color.LawnGreen, Color.Black);
                }
                //Thiết lập nút cuối cùng đúng thứ tự
                Dat_mau_node(Node[Spt - 1], Color.LawnGreen, Color.Black);

            });
            lst_Code.SelectedIndex = 0;
            Hoan_thanh();
        }

        #endregion
        #region BUBBLE SORT
        void BubbleSort()
        {
            int i, j;
            Application.DoEvents();
            //Hieu ung xem code
            lst_Code.SelectedIndex = 0;
            Tre(10 * Toc_do);
            this.Invoke((MethodInvoker)delegate
            {
                for (i = 0; i < Spt - 1; i++)
                {
                    //Hieu ung xem code
                    lst_Code.SelectedIndex = 3;
                    Tre(10 * Toc_do);
                    //Thiết lập mũi tên chỉ biến i
                    Mui_ten_xanh_xuong_1.Text = "i=" + i;
                    Mui_ten_xanh_xuong_1.Visible = true;
                    Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                    Mui_ten_xanh_xuong_1.Refresh();
                    Tre(10 * Toc_do);
                    Application.DoEvents();
                    for (j = Spt - 1; j > i; j--)
                    {
                        Application.DoEvents();
                        //Hieu ung xem code
                        lst_Code.SelectedIndex = 4;
                        Tre(10 * Toc_do);
                        //Thiết lập mũi tên chỉ biến j
                        Mui_ten_xanh_len_1.Text = "j=" + j;
                        Mui_ten_xanh_len_1.Visible = true;
                        Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * j) + (Kich_thuoc / 2) - 25, Node[j].Location.Y + 2 * Kich_thuoc + 5);
                        Mui_ten_xanh_len_1.Refresh();
                        lbl_status_02.Visible = true;
                        lbl_status_02.Text = "So_Sanh(a[" + j + "],a[" + (j - 1) + "])";
                        lbl_status_02.Refresh();
                        Tre(20 * Toc_do);
                        lbl_status_02.Visible = false;
                        //Hieu ung xem code
                        lst_Code.SelectedIndex = 5;
                        Tre(10 * Toc_do);
                        if (a[j] < a[j - 1])
                        {
                            //Hieu ung xem code
                            lst_Code.SelectedIndex = 6;
                            lbl_status_02.Visible = true;
                            lbl_status_02.Text = "Hoan_vi(a[" + j + "],a[" + (j - 1) + "])";
                            lbl_status_02.Refresh();

                            Hoan_vi(ref a[j], ref a[j - 1]);
                            Application.DoEvents();
                            this.Invoke((MethodInvoker)delegate
                            {
                                Hoan_Vi_Node(Node[j], Node[j - 1]);
                                Tam_dung();
                            });
                            lbl_status_02.Visible = false;
                            Hoan_Tri_Node(j, j - 1);
                        }
                    }
                    // Đặt lại màu cho phần tử đã được sắp xếp
                    Dat_mau_node(Node[i], Color.LawnGreen, Color.Black);
                    //Cập nhật Status
                    lbl_status_02.Visible = true;
                    lbl_status_02.Text = "a[" + i + "] Đã đúng vị trí";
                    lbl_status_02.Refresh();
                    Tre(20 * Toc_do);
                    lbl_status_02.Visible = false;
                }
            });
            lst_Code.SelectedIndex = 0;
            Hoan_thanh();
        }

        #endregion
        #region SHACKE SORT
        void ShackeSort()
        {
            int i;
            int left, right, k;
            left = 0; right = Spt - 1; k = Spt - 1;
            //Thiết lập mũi tên chỉ left
            Mui_ten_xanh_len_1.Visible = true;
            Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * left) + (Kich_thuoc / 2) - 25, Node[left].Location.Y + 2 * Kich_thuoc + 5);
            Mui_ten_xanh_len_1.Text = "L = " + left;
            Mui_ten_xanh_len_1.Refresh();
            //Thiết lập mũi tên chỉ right
            Mui_ten_xanh_len_2.Visible = true;
            Mui_ten_xanh_len_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * right) + (Kich_thuoc / 2) - 25, Node[right].Location.Y + 2 * Kich_thuoc + 5);
            Mui_ten_xanh_len_2.Text = "R = " + right;
            Mui_ten_xanh_len_2.Refresh();
            //Thiết lập vị trí của k
            Mui_ten_do_len.Visible = true;
            Mui_ten_do_len.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * k) + (Kich_thuoc / 2) - 25, Node[k].Location.Y + 2 * Kich_thuoc + 65);
            Mui_ten_do_len.Text = "K=" + k;
            Mui_ten_do_len.Refresh();
            //
            Application.DoEvents();
            this.Invoke((MethodInvoker)delegate
            {
                while (left < right)
                {
                    lst_Code.SelectedIndex = 5;
                    Tre(20 * Toc_do);
                    Application.DoEvents();
                    for (i = right; i > left; i--)
                    {
                        lst_Code.SelectedIndex = 7;
                        Tre(20 * Toc_do);
                        //Thiết lập mũi tên chỉ i
                        Mui_ten_xanh_xuong_1.Visible = true;
                        Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                        Mui_ten_xanh_xuong_1.Text = "i=" + i;
                        Mui_ten_xanh_xuong_1.Refresh();
                        Application.DoEvents();
                        lst_Code.SelectedIndex = 8;
                        Tre(20 * Toc_do);
                        if (a[i] < a[i - 1])
                        {
                            lst_Code.SelectedIndex = 10;
                            Tre(20 * Toc_do);
                            Hoan_vi(ref a[i], ref a[i - 1]);
                            this.Invoke((MethodInvoker)delegate
                            {
                                Hoan_Vi_Node(Node[i], Node[i - 1]);
                            });
                            Tam_dung();
                            Hoan_Tri_Node(i, i - 1);
                            lst_Code.SelectedIndex = 11;

                            k = i;
                            //Thiết lập vị trí của k
                            Mui_ten_do_len.Visible = true;
                            Mui_ten_do_len.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * k) + (Kich_thuoc / 2) - 25, Node[k].Location.Y + 2 * Kich_thuoc + 65);
                            Mui_ten_do_len.Text = "K=" + k;
                            Mui_ten_do_len.Refresh();
                            Tre(20 * Toc_do);
                        }
                    }
                    //Thiết lập mũi tên chỉ left

                    Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * k) + (Kich_thuoc / 2) - 25, Node[k].Location.Y + 2 * Kich_thuoc + 5);
                    Mui_ten_xanh_len_1.Text = "L = " + left;
                    Mui_ten_xanh_len_1.Refresh();
                    //
                    //Thiết lập Node đã có thứ tự

                    for (i = 0; i < k; i++)
                    {
                        Dat_mau_node(Node[i], Color.LawnGreen, Color.Black);
                        Tre(20 * Toc_do);
                    }
                    left = k;
                    lst_Code.SelectedIndex = 14;
                    Tre(20 * Toc_do);
                    for (i = left; i < right; i++)
                    {
                        //Thiết lập mũi tên chỉ i
                        Mui_ten_xanh_xuong_1.Visible = true;
                        Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                        Mui_ten_xanh_xuong_1.Text = "i=" + i;
                        Mui_ten_xanh_xuong_1.Refresh();
                        Application.DoEvents();
                        lst_Code.SelectedIndex = 15;
                        Tre(20 * Toc_do);
                        if (a[i] > a[i + 1])
                        {
                            lst_Code.SelectedIndex = 17;
                            Tre(20 * Toc_do);
                            Hoan_vi(ref a[i], ref a[i + 1]);
                            this.Invoke((MethodInvoker)delegate
                            {
                                Hoan_Vi_Node(Node[i], Node[i + 1]);
                            });
                            Tam_dung();
                            Hoan_Tri_Node(i, i + 1);
                            lst_Code.SelectedIndex = 18;

                            k = i;
                            //Thiết lập vị trí của k
                            Mui_ten_do_len.Visible = true;
                            Mui_ten_do_len.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * k) + (Kich_thuoc / 2) - 25, Node[k].Location.Y + 2 * Kich_thuoc + 65);
                            Mui_ten_do_len.Text = "K=" + k;
                            Mui_ten_do_len.Refresh();
                            Tre(20 * Toc_do);
                        }
                    }
                    //Thiết lập mũi tên chỉ right

                    Mui_ten_xanh_len_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * k) + (Kich_thuoc / 2) - 25, Node[k].Location.Y + 2 * Kich_thuoc + 5);
                    Mui_ten_xanh_len_2.Text = "R = " + right;
                    Mui_ten_xanh_len_2.Refresh();
                    //
                    //Thiết lập Node đã có thứ tự
                    for (i = Spt - 1; i > k; i--)
                    {
                        Dat_mau_node(Node[i], Color.LawnGreen, Color.Black);
                        Tre(20 * Toc_do);
                    }
                    //                         
                    right = k;
                }

            });
            //sắp xếp hoàn thành
            Hoan_thanh();
        }
        #endregion
        #region HEAP SORT
        void Shift(int l, int r)
        {
            lbl_status_01.Visible = true;
            lbl_status_01.Text = "Hiệu chỉnh heap!";
            lst_Code.SelectedIndex = 2;
            int i, j, index_temp, x;
            TextBox temp;
            i = l; 
            j = 2 * i + 1;
            //Thiết lập mũi tên chỉ i
            Mui_ten_xanh_xuong_1.Visible = true;
            Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
            Mui_ten_xanh_xuong_1.Text = "i=" + i;
            Mui_ten_xanh_xuong_1.Refresh();
            //thiết lập mũi tên chỉ j
            Mui_ten_xanh_xuong_2.Visible = true;
            Mui_ten_xanh_xuong_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * j) + (Kich_thuoc / 2) - 25, Node[j].Location.Y - Kich_thuoc - 60);
            Mui_ten_xanh_xuong_2.Text = "j=" + j;
            Mui_ten_xanh_xuong_2.Refresh();
            x = a[i];
            temp = Node[i];
            index_temp = i;
            while (j <= r)
            {
                lst_Code.SelectedIndex = 6;
                Application.DoEvents();
                lst_Code.SelectedIndex = 9;
                if (j < r)
                    if (a[j] < a[j + 1])
                        j++;
                //thiết lập mũi tên chỉ j
                Mui_ten_xanh_xuong_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * j) + (Kich_thuoc / 2) - 25, Node[j].Location.Y - Kich_thuoc - 60);
                Mui_ten_xanh_xuong_2.Text = "j=" + j;
                Mui_ten_xanh_xuong_2.Refresh();
                if (a[j] <= x)
                {
                    lst_Code.SelectedIndex = 12;
                    return;
                }
                else
                {
                    lst_Code.SelectedIndex = 16;
                    a[i] = a[j];
                    lst_Code.SelectedIndex = 17;
                    a[j] = x;
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Hoan_Vi_Node(Node[j], temp);
                    });
                    Hoan_Tri_Node(j, index_temp);
                    Tam_dung();
                    lst_Code.SelectedIndex = 18;
                    i = j;
                    j = 2 * i + 1;
                    if (j <= r)
                    {
                        //Thiết lập mũi tên chỉ i
                        
                        Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                        Mui_ten_xanh_xuong_1.Text = "i=" + i;
                        Mui_ten_xanh_xuong_1.Refresh();
                        //thiết lập mũi tên chỉ j

                        Mui_ten_xanh_xuong_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * j) + (Kich_thuoc / 2) - 25, Node[j].Location.Y - Kich_thuoc - 60);
                        Mui_ten_xanh_xuong_2.Text = "j=" + j;
                        Mui_ten_xanh_xuong_2.Refresh();
                    }
                    x = a[i];
                    temp = Node[i];
                    index_temp = i;
                }
            }
            lbl_status_01.Visible = false;
        }
        void CreateHeap(int n)
        {
            lbl_status_01.Visible = true;
            lbl_status_01.Text = "Hiệu chỉnh heap!";
            int l = n / 2 - 1;
            
            while (l >= 0)
            {
                Shift(l, n - 1);
                lst_Code.SelectedIndex = 19;
                l--;
                lst_Code.SelectedIndex = 18;
            }
            lbl_status_01.Visible = false;
        }
        void HeapSort(int n)
        {
            int r;
            CreateHeap(n);
            r = n - 1;
            //Thiết lập mũi tên chỉ r
            Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * r) + (Kich_thuoc / 2) - 25, Node[r].Location.Y + 2 * Kich_thuoc + 5);
            Mui_ten_xanh_len_1.Text = "r=" + r;
            Mui_ten_xanh_len_1.Visible = true;
            Mui_ten_xanh_len_1.Refresh();
            while (r > 0)
            {
                lst_Code.SelectedIndex = 37;
                Application.DoEvents();
                Hoan_vi(ref a[0], ref a[r]);
                Mui_ten_xanh_xuong_1.Visible = false;
                Mui_ten_xanh_xuong_2.Visible = false;
                //
                lbl_status_02.Visible = true;
                lbl_status_02.Text = "Hoan_Vi( a[0] , a[" + r + "] )";

                //
                lst_Code.SelectedIndex = 39;
                this.Invoke((MethodInvoker)delegate
                {
                    Hoan_Vi_Node(Node[0], Node[r]);
                });
                lbl_status_02.Visible = false;
                Hoan_Tri_Node(0, r);
                // Đặt lại màu cho phần tử đã được sắp xếp
                Dat_mau_node(Node[r], Color.LawnGreen, Color.Black);
                lbl_status_02.Visible = true;
                lbl_status_02.Text = "a[" + r + "] đã đúng thứ tự!";
                Tam_dung();
                Tre(20 * Toc_do);
                lbl_status_02.Visible = false;
                r--;
                //Thiết lập mũi tên chỉ r
                Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * r) + (Kich_thuoc / 2) - 25, Node[r].Location.Y + 2 * Kich_thuoc + 5);
                Mui_ten_xanh_len_1.Text = "r=" + r;
                Mui_ten_xanh_len_1.Visible = true;
                Mui_ten_xanh_len_1.Refresh();
                lst_Code.SelectedIndex = 41;
                if (r > 0)
                    Shift(0, r);
            }
            // Đặt lại màu cho phần tử đã được sắp xếp cuối cùng
            Dat_mau_node(Node[0], Color.LawnGreen, Color.Black);
            //Sắp xếp hoàn thành
            Hoan_thanh();
        }
        #endregion
        #region SHELL SORT
        void ShellSort()
        {
            int step, i, pos, x, len, Index_temp;
            TextBox temp;
            //Tạo mảng h
            int k = Convert.ToInt32(Math.Log10(Spt) / Math.Log10(2));
            h = new int[k];
            h[k - 1] = 1;
            for (i = k - 2; i >= 0; i--)
            {
                lst_Code.SelectedIndex = 4;
                Application.DoEvents();
                lst_Code.SelectedIndex = 6;
                h[i] = (2 * h[i + 1] + 1);
            }
            //
            for (step = 0; step < k; step++)
            {
                lst_Code.SelectedIndex = 7;
                Application.DoEvents();
                len = h[step];
                lbl_status_02.Visible = true;
                lbl_status_02.Text = "Len = h[" + step + "] = " + len;
                for (i = len; i < Spt; i++)
                {
                    Application.DoEvents();
                    //thiết lập mũi tên đánh dấu nút cần chèn
                    Mui_ten_xanh_xuong_1.Visible = true;
                    Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                    Mui_ten_xanh_xuong_1.Text = "i=" + i;
                    Mui_ten_xanh_xuong_1.Refresh();
                    lst_Code.SelectedIndex = 9;
                    x = a[i];
                    temp = Node[i];
                    Index_temp = i;
                    lst_Code.SelectedIndex = 8;
                    //Đặt màu các Node trong mảng con
                    Dat_mau_node(Node[i], Color.Yellow, Color.Black);
                    pos = i - len;
                    while (pos >= 0)
                    {
                        Dat_mau_node(Node[pos], Color.Yellow, Color.Black);
                        pos = pos - len;
                    }
                    lbl_status_01.Visible = true;
                    lbl_status_01.Text = "Sắp xếp trong mảng con!";
                    //
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_di_len(Node[i], Kich_thuoc + 5);
                    });
                    pos = i - len;
                    lst_Code.SelectedIndex = 9;

                    while ((pos >= 0) && (x < a[pos]))
                    {

                        Application.DoEvents();
                        lst_Code.SelectedIndex = 11;
                        a[pos + len] = a[pos];
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (len == 1)
                            {
                                Node_qua_phai(Node[pos], len);
                            }
                            else
                            {
                                Node_di_xuong(Node[pos], Kich_thuoc + 5);
                                Node_qua_phai(Node[pos], len);
                                Node_di_len(Node[pos], Kich_thuoc + 5);
                            }
                        });
                        lst_Code.SelectedIndex = 12;
                        Node[pos + len] = Node[pos];
                        lst_Code.SelectedIndex = 13;
                        pos = pos - len;

                    }
                    lst_Code.SelectedIndex = 15;
                    a[pos + len] = x;
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_qua_trai(temp, Index_temp - (pos + len));
                        Node_di_xuong(temp, Kich_thuoc + 5);
                    });
                    Tam_dung();

                    Node[pos + len] = temp;
                    lbl_status_01.Visible = false;
                    Dat_mau_node(Node[i], Color.OrangeRed, Color.White);
                    pos = i - len;
                    while (pos >= 0)
                    {
                        Dat_mau_node(Node[pos], Color.OrangeRed, Color.White);
                        pos = pos - len;
                    }
                    //
                }
            }
            //Sắp xếp hoàn thành
            for (int y = 0; y < Spt; y++)
            {
                Dat_mau_node(Node[y], Color.LawnGreen, Color.Black);
            }
            Hoan_thanh();
        }

        #endregion
        #region MERGE SORT


        //Hàm tìm Min trong 2 số a và b
        int min(int a, int b)
        {
            if (a > b)
                return b;
            else
                return a;
        }
        //Hàm phân phối a cho b và c
        void Distribute(ref int nb, ref int nc, int k)
        {
            lbl_status_01.Visible = true;
            lbl_status_01.Text = "Tách mảng a thành b và c.";
            int i, pa, pb, pc;
            pa = pb = pc = 0;
            while (pa < Spt)
            {
                lst_Code.SelectedIndex = 5;
                Application.DoEvents();
                for (i = 0; (pa < Spt) && (i < k); i++, pa++, pb++)
                {
                    lst_Code.SelectedIndex = 6;
                    b[pb] = a[pa];
                    lst_Code.SelectedIndex = 7;
                    Node_B[pb] = Node[pa];
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_di_len(Node_B[pb], 2 * (Kich_thuoc + 5));
                        Den_tdo_x_node(Node_B[pb], pb);
                    });

                    Dat_mau_node(Node_B[pb], Color.Navy, Color.White);
                    Tam_dung();
                }
                for (i = 0; (pa < Spt) && (i < k); i++, pa++, pc++)
                {
                    lst_Code.SelectedIndex = 8;
                    c[pc] = a[pa];
                    lst_Code.SelectedIndex = 10;
                    Node_C[pc] = Node[pa];
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Node_di_xuong(Node_C[pc], 2 * (Kich_thuoc + 5));
                        Den_tdo_x_node(Node_C[pc], pc);
                    });
                    Dat_mau_node(Node_C[pc], Color.White, Color.Navy);
                    Tam_dung();
                }
            }
            lst_Code.SelectedIndex = 11;
            nb = pb;
            lst_Code.SelectedIndex = 12;
            nc = pc;

            //
            lbl_status_01.Visible = false;
        }
        //Hàm kết hợp b và c vào a
        void Merge(int nb, int nc, int k)
        {
            lbl_status_01.Visible = true;
            lbl_status_01.Text = "Gộp mảng b và c vào mảng a";
            int pa, pb, pc, ib, ic, kb, kc, lennb, lennc;
            //lưu những giá trị để đếm Node dư   
            lennb = nb;
            lennc = nc;
            pa = pb = pc = 0; ib = ic = 0;
            while ((nb > 0) && (nc > 0))
            {
                lst_Code.SelectedIndex = 26;
                Application.DoEvents();
                lst_Code.SelectedIndex = 28;
                kb = min(k, nb);
                lst_Code.SelectedIndex = 29;
                kc = min(k, nc);
                lst_Code.SelectedIndex = 30;
                if (b[pb + ib] <= c[pc + ic])
                {
                    lst_Code.SelectedIndex = 32;
                    a[pa] = b[pb + ib];
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Den_vtri_node(Node_B[pb + ib], pa);
                    });
                    Dat_mau_node(Node_B[pb + ib], Color.OrangeRed, Color.White);
                    Tam_dung();
                    Node[pa] = Node_B[pb + ib];
                    lst_Code.SelectedIndex = 33;
                    pa++;
                    ib++;
                    lst_Code.SelectedIndex = 34;
                    if (ib == kb)
                    {
                        lst_Code.SelectedIndex = 36;
                        for (; ic < kc; ic++)
                        {
                            lst_Code.SelectedIndex = 38;
                            a[pa] = c[pc + ic];
                            lst_Code.SelectedIndex = 39;
                            Application.DoEvents();
                            this.Invoke((MethodInvoker)delegate
                            {
                                Den_vtri_node(Node_C[pc + ic], pa);
                            });
                            Dat_mau_node(Node_C[pc + ic], Color.OrangeRed, Color.White);
                            Tam_dung();
                            Node[pa] = Node_C[pc + ic];
                            pa++;
                        }
                        pb += kb;
                        pc += kc;
                        ib = ic = 0;
                        nb -= kb;
                        nc -= kc;
                    }
                }
                else
                {
                    a[pa] = c[pc + ic];
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Den_vtri_node(Node_C[pc + ic], pa);
                        Dat_mau_node(Node_C[pc + ic], Color.OrangeRed, Color.White);
                        Tam_dung();
                    });
                    Node[pa] = Node_C[pc + ic];
                    pa++;
                    ic++;
                    if (ic == kc)
                    {
                        for (; ib < kb; ib++)
                        {
                            a[pa] = b[pb + ib];
                            Application.DoEvents();
                            this.Invoke((MethodInvoker)delegate
                            {
                                Den_vtri_node(Node_B[pb + ib], pa);
                            });
                            Dat_mau_node(Node_B[pb + ib], Color.OrangeRed, Color.White);
                            Tam_dung();
                            Node[pa] = Node_B[pb + ib];
                            pa++;
                        }
                        pb += kb;
                        pc += kc;
                        ib = ic = 0;
                        nb -= kb;
                        nc -= kc;
                    }
                }
            }
            //Di chuyển các Node dư thừa vào vị trí
            for (; nb > 0; nb--)
            {
                Application.DoEvents();
                this.Invoke((MethodInvoker)delegate
                {
                    Den_vtri_node(Node_B[lennb - nb], pa);
                });
                Tam_dung();
                pa++;
            }
            for (; nc > 0; nc--)
            {
                Application.DoEvents();
                this.Invoke((MethodInvoker)delegate
                {
                    Den_vtri_node(Node_C[lennc - nc], pa);
                });
                Tam_dung();
                pa++;
            }
            lbl_status_02.Visible = false;
        }
        //Hàm sắp xếp Merge
        void MergeSort(int n)
        {
            //Dán nhãn mảng b
            lbl_B.Width = Kich_thuoc;
            lbl_B.Height = Kich_thuoc;
            lbl_B.Location = new Point(Canh_le - (Kich_thuoc), 250 - 2 * (Kich_thuoc + 5));
            lbl_B.Font = new System.Drawing.Font("Arial", Co_chu, FontStyle.Bold);
            lbl_B.Visible = true;
            //Dán nhãn mảng c
            lbl_C.Width = Kich_thuoc;
            lbl_C.Height = Kich_thuoc;
            lbl_C.Location = new Point(Canh_le - (Kich_thuoc), 250 + 2 * (Kich_thuoc + 5));
            lbl_C.Font = new System.Drawing.Font("Arial", Co_chu, FontStyle.Bold);
            lbl_C.Visible = true;
            int k, nc = 0, nb = 0;
            for (k = 1; k < n; k *= 2)
            {
                lbl_status_02.Visible = true;
                lbl_status_02.Text = "k = " + k;
                Distribute(ref nb, ref nc, k);
                Merge(nb, nc, k);
            }
            //Sắp xếp hoàn thành
            Hoan_thanh();
        }
        #endregion
        #region QUICK SORT
        void QuickSort(int left, int right)
        {
            int i, j, x, cs_x;


            //Status 01
            lbl_status_01.Text = "Phân hoạch đoạn [" + left + "," + right + "]";
            //đặt mũi tên chỉ left
            Mui_ten_xanh_len_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * left) + (Kich_thuoc / 2) - 25, Node[left].Location.Y + 2 * Kich_thuoc + 5);
            Mui_ten_xanh_len_1.Text = "L = " + left;
            Mui_ten_xanh_len_1.Visible = true;
            Mui_ten_xanh_len_1.Refresh();
            //đặt mũi tên chỉ Right
            Mui_ten_xanh_len_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * right) + (Kich_thuoc / 2) - 25, Node[right].Location.Y + 2 * Kich_thuoc + 5);
            Mui_ten_xanh_len_2.Text = "R = " + right;
            Mui_ten_xanh_len_2.Visible = true;
            Mui_ten_xanh_len_2.Refresh();
            //
            lst_Code.SelectedIndex = 4;
            x = a[(left + right) / 2];
            cs_x = (left + right) / 2;
            //Thiết lập vị trí của x
            Mui_ten_do_len.Visible = true;
            Mui_ten_do_len.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * cs_x) + (Kich_thuoc / 2) - 25, Node[cs_x].Location.Y + 2 * Kich_thuoc + 65);
            Mui_ten_do_len.Text = "X";
            Mui_ten_do_len.Refresh();
            //
            //Đặt màu nút x                    
            //
            lst_Code.SelectedIndex = 5;
            i = left; j = right;
            //Thiết lập mũi tên chỉ i
            Mui_ten_xanh_xuong_1.Visible = true;
            Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
            Mui_ten_xanh_xuong_1.Text = "i=" + i;
            Mui_ten_xanh_xuong_1.Refresh();
            //Thiết lập mũi tên chỉ j
            Mui_ten_xanh_xuong_2.Visible = true;
            Mui_ten_xanh_xuong_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * j) + (Kich_thuoc / 2) - 25, Node[j].Location.Y - Kich_thuoc - 60);
            Mui_ten_xanh_xuong_2.Text = "j=" + j;
            Mui_ten_xanh_xuong_2.Refresh();
            //
            do
            {
                //Hiệu ứng so sánh
                lbl_status_02.Text = "So_Sanh(a[" + i + "], a[x])";
                Tre(20 * Toc_do);
                lst_Code.SelectedIndex = 8;
                while (a[i] < x)
                {
                    i++;
                    //Thiết lập mũi tên chỉ i
                    Mui_ten_xanh_xuong_1.Visible = true;
                    Mui_ten_xanh_xuong_1.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * i) + (Kich_thuoc / 2) - 25, Node[i].Location.Y - Kich_thuoc - 60);
                    Mui_ten_xanh_xuong_1.Text = "i=" + i;
                    Mui_ten_xanh_xuong_1.Refresh();
                    //
                    //Hiệu ứng so sánh
                    lbl_status_02.Text = "So_Sanh(a[" + i + "], a[x])";
                    Tre(20 * Toc_do);
                }
                //Hiệu ứng so sánh
                lbl_status_02.Text = "So_Sanh(a[" + j + "], a[x])";
                Tre(20 * Toc_do);
                lst_Code.SelectedIndex = 9;
                while (a[j] > x)
                {
                    j--;
                    //Thiết lập mũi tên chỉ j
                    Mui_ten_xanh_xuong_2.Visible = true;
                    Mui_ten_xanh_xuong_2.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * j) + (Kich_thuoc / 2) - 25, Node[j].Location.Y - Kich_thuoc - 60);
                    Mui_ten_xanh_xuong_2.Text = "j=" + j;
                    Mui_ten_xanh_xuong_2.Refresh();
                    //
                    //Hiệu ứng so sánh
                    lbl_status_02.Text = "So_Sanh(a[" + j + "], a[x])";
                    Tre(20 * Toc_do);
                }
                lst_Code.SelectedIndex = 10;
                if (i <= j)
                {
                    //status hoán vị
                    lbl_status_02.Text = "Hoan_Vi(a[" + i + "], a[" + j + "])";
                    Tre(20 * Toc_do);
                    lst_Code.SelectedIndex = 12;
                    Hoan_vi(ref a[i], ref a[j]);
                    //Tìm vị trí mới của x
                    if (i == cs_x)
                    {
                        cs_x = j;
                    }
                    else if (j == cs_x)
                    {
                        cs_x = i;
                    }
                    Application.DoEvents();
                    this.Invoke((MethodInvoker)delegate
                    {
                        Hoan_Vi_Node(Node[i], Node[j]);
                    });
                    Tam_dung();
                    Hoan_Tri_Node(i, j);
                    //Thiết lập vị trí của x
                    Mui_ten_do_len.Visible = true;
                    Mui_ten_do_len.Location = new Point((Canh_le + (Kich_thuoc + Khoang_cach) * cs_x) + (Kich_thuoc / 2) - 25, Node[cs_x].Location.Y + 2 * Kich_thuoc + 55);
                    Mui_ten_do_len.Text = "X = " + ((left + right) / 2);
                    Mui_ten_do_len.Refresh();
                    i++; j--;
                }
            } while (i <= j);
            lst_Code.SelectedIndex = 15;
            //Đặt màu nút x
            if (j == 0)
            {
                Dat_mau_node(Node[j], Color.LawnGreen, Color.Black);
            }
            if (i == Spt - 1)
            {
                Dat_mau_node(Node[j], Color.LawnGreen, Color.Black);
            }
            lst_Code.SelectedIndex = 16;
            if (left < j)
                QuickSort(left, j);
            lst_Code.SelectedIndex = 17;
            if (i < right)
                QuickSort(i, right);
        }
        #endregion
        #endregion
        //Nút tạo mảng
        private void btn_taomang_Click(object sender, EventArgs e)
        {
            //Gọi hàm xóa mảng
            Xoa_mang();
            this.txt_sophantu.Focus();
            this.txt_sophantu.SelectAll();
            try
            {
                Spt = Convert.ToInt32(txt_sophantu.Text);
            }
            catch
            {
                MessageBox.Show("Số phần tử vừa nhập vào không hợp lệ!");
                this.txt_sophantu.Clear();
                return;
            }
            //gọi hàm tạo mảng
            Tao_mang();
        }
        //Nút bắt đầu xắp xếp
        private void btn_sapxep_Click(object sender, EventArgs e)
        {
            btn_sapxep.Enabled = false;
            rad_bubblesort.Enabled = false;
            rad_heapsort.Enabled = false;
            rad_insertionsort.Enabled = false;
            rad_mergesort.Enabled = false;
            rad_quicksort.Enabled = false;
            rad_selectionsort.Enabled = false;
            rad_shackersort.Enabled = false;
            rad_shellsort.Enabled = false;

            //Cho phép các nút điều khiển
            btn_dung.Enabled = true;
            btn_chayhet.Enabled = true;
            if (chk_tungbuoc.Checked == true)
            {
                btn_ketiep.Enabled = true;
                Sap_xep_tung_buoc = true;
            }

            //Thiết lập các nút nhập liệu
            btn_docfile.Enabled = false;
            btn_taomang.Enabled = false;
            btn_ngaunhien.Enabled = false;
            btn_nhap.Enabled = false;
            txt_chiso.Enabled = false;
            txt_giatri.Enabled = false;
            #region INSERTION SORT
            if (rad_insertionsort.Checked == true)
            {
                InsertionSort();
            }
            #endregion
            #region SELECTION SORT
            if (rad_selectionsort.Checked == true)
            {
                SelectionSort();
            }
            #endregion
            #region BUBBLE SORT
            if (rad_bubblesort.Checked == true)
            {
                BubbleSort();
            }
            #endregion
            #region SHACKE SORT
            if (rad_shackersort.Checked == true)
            {
                ShackeSort();
            }
            #endregion
            #region HEAP SORT
            if (rad_heapsort.Checked == true)
            {
                Application.DoEvents();
                this.Invoke((MethodInvoker)delegate
                {
                    HeapSort(Spt);
                });
            }
            #endregion
            #region SHELL SORT
            if (rad_shellsort.Checked == true)
            {
                ShellSort();
                for (i = 0; i < Spt; i++)
                {
                    lbl_status_01.Text += a[i].ToString() + " ";
                }
            }
            #endregion
            #region MERGE SORT
            if (rad_mergesort.Checked == true)
            {
                b = new int[Spt];
                c = new int[Spt];
                Node_B = new TextBox[Spt];
                Node_C = new TextBox[Spt];
                Application.DoEvents();
                this.Invoke((MethodInvoker)delegate
                {
                    MergeSort(Spt);
                });
            }
            #endregion
            #region QUICK SORT
            if (rad_quicksort.Checked == true)
            {
                Application.DoEvents();
                this.Invoke((MethodInvoker)delegate
                {
                    QuickSort(0, Spt - 1);
                });
                Hoan_thanh();
            }

            #endregion
        }
        // timer thay đổi tốc độ
        private void Tmr_Thay_doi_toc_do_Tick(object sender, EventArgs e)
        {
            Toc_do = (Trb_Toc_do.Maximum - Trb_Toc_do.Value);
            lbl_Toc_do.Text = Trb_Toc_do.Value.ToString();
            if (Trb_Toc_do.Value == Trb_Toc_do.Maximum)
            {
                lbl_Toc_do.Text = "Max=10";
            }
            else if (Trb_Toc_do.Value == Trb_Toc_do.Minimum)
            {
                lbl_Toc_do.Text = "Min=0";
            }
        }
        // NÚT DỪNG
        private void btn_dung_Click(object sender, EventArgs e)
        {
            btn_chayhet.Focus();

            if (btn_dung.Text == ";")
            {
                btn_dung.Text = "4";
                KT_tam_dung = true;
                Play_or_Stop();
            }
            else
            {
                btn_dung.Text = ";";
                KT_tam_dung = false;
            }
        }
        //NÚT NEXT
        private void btn_ketiep_Click(object sender, EventArgs e)
        {
            btn_dung.Text = ";";
            Sap_xep_tung_buoc = true;
            chk_tungbuoc.Checked = true;
            KT_tam_dung = false;
            btn_dung.Enabled = true;
        }
        // NÚT CHẠY HÊT
        private void btn_chayhet_Click(object sender, EventArgs e)
        {
            btn_dung.Text = ";";
            chk_tungbuoc.Checked = false;
            Sap_xep_tung_buoc = false;
            KT_tam_dung = false;
            btn_dung.Enabled = true;
        }
        //thay đổi tùy chọn sort step
        private void chk_tungbuoc_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_tungbuoc.Checked == true)
            {
                btn_ketiep.Enabled = true;
                Sap_xep_tung_buoc = true;
            }
            else
            {
                btn_ketiep.Enabled = false;
                Sap_xep_tung_buoc = false;
            }
        }
        //THÔNG TIN PHẦN MỀM
        private void Menu_Thông_tin_Click(object sender, EventArgs e)
        {
            Form Frm_Profile = new frm_Thong_tin();
            Frm_Profile.ShowDialog();
        }
        //THOÁT
        private void menu_thoat_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        //Tải code của thuật toán tương ứng với nút chon thuật toán vào cửa sổ xem code
        private void rad_insertionsort_CheckedChanged(object sender, EventArgs e)
        {
             if (rad_insertionsort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Insertion Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void InsertionSort(int a[], int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int i, pos, x;");
                lst_Code.Items.Add("   for (i = 1; i < n; i++)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      x = a[i];");
                lst_Code.Items.Add("      pos = i - 1;");
                lst_Code.Items.Add("      while ((pos >= 0) && (a[pos] > x))");
                lst_Code.Items.Add("      {");
                lst_Code.Items.Add("         a[pos + 1] = a[pos];");
                lst_Code.Items.Add("         pos--;");
                lst_Code.Items.Add("      }");
                lst_Code.Items.Add("      a[pos + 1] = x;");
                lst_Code.Items.Add("    }");
                lst_Code.Items.Add("}");              
            }
        }
        private void rad_selectionsort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_selectionsort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Selection Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void SelectionSort(int a[], int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int min, i, j;");
                lst_Code.Items.Add("   for (i = 0; i < n - 1; i++)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      min = i;");
                lst_Code.Items.Add("      for (j = i + 1; j < n; j++)");
                lst_Code.Items.Add("      {");
                lst_Code.Items.Add("        if (a[j] < a[min])");
                lst_Code.Items.Add("            min = j;");
                lst_Code.Items.Add("      }");
                lst_Code.Items.Add("      if (min != i)");
                lst_Code.Items.Add("         Swap(a[min], a[i]);");
                lst_Code.Items.Add("    }");
                lst_Code.Items.Add("}");
            } 
        }
        private void rad_bubblesort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_bubblesort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Bubble Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void BubbleSort(int a[],int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("  int i, j;");
                lst_Code.Items.Add("  for (i = 0 ; i<n-1; i++)");
                lst_Code.Items.Add("     for (j = n-1; j > i ; j --)");
                lst_Code.Items.Add("        if (a[j] < a[j-1]) ");
                lst_Code.Items.Add("          Swap(a[j], a[j-1]);");
                lst_Code.Items.Add("}");
            }
        }
        private void rad_shackersort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_shackersort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Shacker Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void ShakeSort(int a[], int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int i;");
                lst_Code.Items.Add("   int left, right, k;");
                lst_Code.Items.Add("   left = 0; right = n-1; k = n-1;");
                lst_Code.Items.Add("   while (left < right)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      for (i = right; i > left; i--)");
                lst_Code.Items.Add("         if (a[i]< a[i-1]) ");
                lst_Code.Items.Add("         {");
                lst_Code.Items.Add("	        Swap(a[i], a[i-1]);");
                lst_Code.Items.Add("	        k = i;");
                lst_Code.Items.Add("         }");
                lst_Code.Items.Add("      left = k;");
                lst_Code.Items.Add("      for (i = left; i < right; i++)");
                lst_Code.Items.Add("         if (a[i] > a[i + 1]) ");
                lst_Code.Items.Add("         {");
                lst_Code.Items.Add("            Swap(a[i], a[i + 1]);");
                lst_Code.Items.Add("            k = i; ");
                lst_Code.Items.Add("         }");
                lst_Code.Items.Add("      right = k;");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("}");
            }
        }
        private void rad_heapsort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_heapsort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Heap Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void Shift(int a[], int l, int r)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int i, j, x;");
                lst_Code.Items.Add("   i = l;");
                lst_Code.Items.Add("   j = 2 * i + 1;");
                lst_Code.Items.Add("   x = a[i];");
                lst_Code.Items.Add("   while (j <= r)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      if (j < r)");
                lst_Code.Items.Add("      if (a[j] < a[j+1])");
                lst_Code.Items.Add("        j++;");
                lst_Code.Items.Add("      if (a[j] <= x)");
                lst_Code.Items.Add("        return;");
                lst_Code.Items.Add("      else");
                lst_Code.Items.Add("      {");
                lst_Code.Items.Add("        a[i] = a[j];");
                lst_Code.Items.Add("        a[j] = x;");
                lst_Code.Items.Add("        i = j;");
                lst_Code.Items.Add("        j = 2 * i + 1;");
                lst_Code.Items.Add("        x = a[i];");
                lst_Code.Items.Add("      }");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("}");
                lst_Code.Items.Add("void CreateHeap(int a[], int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int l = n / 2 - 1;");
                lst_Code.Items.Add("   while (l >= 0)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      Shift(a, l, n-1);");
                lst_Code.Items.Add("      l--;");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("}");
                lst_Code.Items.Add("void HeapSort(int a[], int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int r;");
                lst_Code.Items.Add("   CreateHeap(a, n);");
                lst_Code.Items.Add("   r = n - 1;");
                lst_Code.Items.Add("   while (r > 0)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      Swap(a[0], a[r]);");
                lst_Code.Items.Add("        r--;");
                lst_Code.Items.Add("      if (r > 0)");
                lst_Code.Items.Add("         Shift(a, 0, r);");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("}");
            }
        }
        private void rad_shellsort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_shellsort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Shell Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void ShellSort(int a[], int n, int h[], int k)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int step, i, pos, x, len;");
                lst_Code.Items.Add("   for (step = 0; step < k; step++)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      len = h[step];");
                lst_Code.Items.Add("      for (i = len; i < n; i++)");
                lst_Code.Items.Add("      {	");
                lst_Code.Items.Add("          x = a[i]; ");
                lst_Code.Items.Add("          pos = i - len;");
                lst_Code.Items.Add("          while((pos >= 0)&&(x < a[pos]))");
                lst_Code.Items.Add("          { ");
                lst_Code.Items.Add("	          a[pos + len]  = a[pos];");
                lst_Code.Items.Add("	          pos = pos - len;");
                lst_Code.Items.Add("          }");
                lst_Code.Items.Add("          a[pos + len] = x;");
                lst_Code.Items.Add("       }");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("}");
            }
        }
        private void rad_mergesort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_mergesort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Merge Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void Distribute(int a[], int n, int &nb, int &nc, int k)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int i, pa, pb, pc;");
                lst_Code.Items.Add("   pa = pb = pc = 0;");
                lst_Code.Items.Add("   while (pa < n)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      for (i = 0; (pa < n) && (i < k); i++, pa++, pb++)");
                lst_Code.Items.Add("        b[pb] = a[pa];");
                lst_Code.Items.Add("      for (i = 0; (pa < n) && (i < k); i++, pa++, pc++)");
                lst_Code.Items.Add("      {");
                lst_Code.Items.Add("        c[pc] = a[pa];");
                lst_Code.Items.Add("        nb = pb;	");
                lst_Code.Items.Add("        nc = pc;");
                lst_Code.Items.Add("      }");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("   int min(int a, int b)");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      if(a > b)");
                lst_Code.Items.Add("         return b;");
                lst_Code.Items.Add("      else");
                lst_Code.Items.Add("         return a;");
                lst_Code.Items.Add("    }");
                lst_Code.Items.Add("void Merge(int a[], int nb, int nc, int k)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int p, pb, pc, ib, ic, kb, kc;");
                lst_Code.Items.Add("   p = pb = pc = 0; ib = ic = 0;");
                lst_Code.Items.Add("   while((nb > 0) && (nc > 0))");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("       kb = min(k, nb);");
                lst_Code.Items.Add("       kc = min(k, nc);");
                lst_Code.Items.Add("       if(b[pb + ib] <= c[pc + ic])");
                lst_Code.Items.Add("       {");
                lst_Code.Items.Add("           a[p++]=b[pb+ib];");
                lst_Code.Items.Add("           ib++;");
                lst_Code.Items.Add("           if(ib == kb)");
                lst_Code.Items.Add("           { ");
                lst_Code.Items.Add("               for(;ic<kc;ic++)");
                lst_Code.Items.Add("               {");
                lst_Code.Items.Add("	               a[p++] = c[pc+ic];");
                lst_Code.Items.Add("                   pb += kb; ");
                lst_Code.Items.Add("                   pc += kc; ");
                lst_Code.Items.Add("                   ib = ic = 0;");
                lst_Code.Items.Add("                   nb -= kb; ");
                lst_Code.Items.Add("                   nc -= kc;");
                lst_Code.Items.Add("                }");
                lst_Code.Items.Add("            }");
                lst_Code.Items.Add("            else");
                lst_Code.Items.Add("            {");
                lst_Code.Items.Add("               a[p++] = c[pc+ic];");
                lst_Code.Items.Add("               ic++;");
                lst_Code.Items.Add("               if(ic == kc)");
                lst_Code.Items.Add("               {");
                lst_Code.Items.Add("               for(; ib < kb; ib++)");
                lst_Code.Items.Add("	               a[p++] = b[pb+ib];");
                lst_Code.Items.Add("                   pb += kb; ");
                lst_Code.Items.Add("                   pc += kc; ");
                lst_Code.Items.Add("                   ib = ic = 0;");
                lst_Code.Items.Add("                   nb -= kb; ");
                lst_Code.Items.Add("                   nc -= kc;");
                lst_Code.Items.Add("                }");
                lst_Code.Items.Add("        }");
                lst_Code.Items.Add("}");
                lst_Code.Items.Add("           }");
                lst_Code.Items.Add("void MergeSort(int a[], int n)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("  int	k, nc=0, nb=0;");
                lst_Code.Items.Add("  for (k = 1; k < n; k *= 2) ");
                lst_Code.Items.Add("  {");
                lst_Code.Items.Add("     Distribute(a, n, nb, nc, k);");
                lst_Code.Items.Add("     Merge(a, nb, nc, k);");
                lst_Code.Items.Add("  }");
                lst_Code.Items.Add("}");
            }
        }
        private void rad_quicksort_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_quicksort.Checked == true)
            {
                lst_Code.Items.Clear();
                //Tải code của thuật toán Quick Sort vào Cửa sổ xem code
                lst_Code.Items.Add("void QuickSort(int a[], int left, int right)");
                lst_Code.Items.Add("{");
                lst_Code.Items.Add("   int i, j, x;");
                lst_Code.Items.Add("   x = a[(left + right) / 2]; ");
                lst_Code.Items.Add("   i = left; j = right;");
                lst_Code.Items.Add("   do");
                lst_Code.Items.Add("   {");
                lst_Code.Items.Add("      while(a[i] < x) i++;");
                lst_Code.Items.Add("      while(a[j] > x) j--;");
                lst_Code.Items.Add("      if(i <= j)");
                lst_Code.Items.Add("      { ");
                lst_Code.Items.Add("           Swap(a[i], a[j]);");
                lst_Code.Items.Add("           i++ ; j--;");
                lst_Code.Items.Add("      }");
                lst_Code.Items.Add("   }");
                lst_Code.Items.Add("   while(i <= j);");
                lst_Code.Items.Add("   if(left < j)");
                lst_Code.Items.Add("        QuickSort(a, left, j);");
                lst_Code.Items.Add("   if(i < right)");
                lst_Code.Items.Add("        QuickSort(a, i, right);");
                lst_Code.Items.Add("}");
            }
        }
        // PHÓNG TO
        private void btn_phongto_Click(object sender, EventArgs e)
        {
            Kthuoc_Code++;
            lst_Code.Font = new System.Drawing.Font("Times new roman", Kthuoc_Code, FontStyle.Bold);
        }
        // THU NHỎ
        private void btn_thunho_Click(object sender, EventArgs e)
        {
            Kthuoc_Code--;
            lst_Code.Font = new System.Drawing.Font("Times new roman", Kthuoc_Code, FontStyle.Bold);
        }
    }
}
