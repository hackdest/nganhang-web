using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NGANHANG_APP
{
    public partial class FormKhachHang : DevExpress.XtraEditors.XtraForm
    {
       // * vị trí của con trỏ trên grid view*/
        int viTri = 0;
        /********************************************
         * đang thêm mới -> true -> đang dùng btnTHEM
         *              -> false -> có thể là btnGHI( chỉnh sửa) hoặc btnXOA
         *              
         * Mục đích: dùng biến này để phân biệt giữa btnTHEM - thêm mới hoàn toàn
         * và việc chỉnh sửa nhân viên( do mình ko dùng thêm btnXOA )
         * Trạng thái true or false sẽ được sử dụng 
         * trong btnGHI - việc này để phục vụ cho btnHOANTAC
         ********************************************/
        bool dangThemMoi = false;

        String maChiNhanh = "";
        /**********************************************************
         * undoList - phục vụ cho btnHOANTAC -  chứa các thông tin của đối tượng bị tác động 
         * 
         * nó là nơi lưu trữ các đối tượng cần thiết để hoàn tác các thao tác
         * 
         * nếu btnGHI sẽ ứng với INSERT
         * nếu btnXOA sẽ ứng với DELETE
         * nếu btnCHUYENCHINHANH sẽ ứng với CHANGEBRAND
         **********************************************************/
        Stack undoList = new Stack();



        /************************************************************
         * CheckExists:
         * Để tránh việc người dùng ấn vào 1 form đến 2 lần chúng ta 
         * cần sử dụng hàm này để kiểm tra xem cái form hiện tại đã 
         * có trong bộ nhớ chưa
         * Nếu có trả về "f"
         * Nếu không trả về "null"
         ************************************************************/
        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }


        public FormKhachHang()
        {
            InitializeComponent();
        }
        /*
       *Step 1: tat kiem tra khoa ngoai & do du lieu vao form
       *Step 2: lay du lieu dang nhap tu form dang nhap
       *Step 3: bat nut chuc nang theo vai tro khi dang nhap
       */
        private void FormKhachHang_Load(object sender, EventArgs e)
        {
            /*Step 1*/
            /*không kiểm tra khóa ngoại nữa*/
          //DS.EnforceConstraints = false;


            // TODO: This line of code loads data into the 'nGANHANGDataSet2.KhachHang' table. You can move, or remove it, as needed.

           

            /*van con ton tai loi chua sua duoc*/
            //     maChiNhanh = ((DataRowView)bdsNhanVien[0])["MACN"].ToString();
            /*Step 2*/
           
            cmbCHINHANH.DataSource = Program.bindingSource;/*sao chep bingding source tu form dang nhap*/
            cmbCHINHANH.DisplayMember = "TENCN";
            cmbCHINHANH.ValueMember = "TENSERVER";

            cmbCHINHANH.SelectedIndex = Program.brand;
            ////////////////////////////////////
            /*Step 3*/
            /*CONG TY chi xem du lieu*/
              if (Program.role == "CONGTY")
               {
                   cmbCHINHANH.Enabled = true;

                   this.btnThem.Enabled = false;
                   this.btnXoa.Enabled = false;
                   this.btnGhi.Enabled = false;

                   this.btnHoanTac.Enabled = false;
                   this.btnLamMoi.Enabled = true;
                  // this.btnChuyenChiNhanh.Enabled = false;
                   this.btnThoat.Enabled = true;


               }
            /* CHI NHANH & USER co the xem - xoa - sua du lieu nhung khong the 
           chuyen sang chi nhanh khac*/
             if (Program.role == "CHINHANH" || Program.role == "USER")
              {
                  cmbCHINHANH.Enabled = false;

                  this.btnThem.Enabled = true;
                  this.btnXoa.Enabled = true;
                  this.btnGhi.Enabled = true;

                  this.btnHoanTac.Enabled = false;
                  this.btnLamMoi.Enabled = true;
                //  this.btnChuyenChiNhanh.Enabled = true;
                  this.btnThoat.Enabled = true;

               //   this.panelNhapLieu.Enabled = true;
                  this.txtCMND.Enabled = false;
              }



        }

        private void khachHangBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsKhachHang.EndEdit();
           // this.khachHangTableAdapter.Update(this.NGANHANGDATASET);

        }

        /*********************************************************************
         * bdsNhanVien.Position - vitri phuc vu cho btnHOANTAC. Gia su, co 5 nhan vien, con tro chuot
         * dang dung o vi tri nhan vien thu 2 thi chung ta an nut THEM
         * nhung neu chon btnHOANTAC, con tro chuot phai quay lai vi 
         * tri nhan vien thu 2, thay vi o vi tri duoi cung - tuc nhan vien so 5
         * 
         * neu nhap chu cho txtMANV thi se khong chuyen sang cac o khac duoc nua - bat buoc ghi so
         * 
         * Step 1: Kich hoat panel Nhap lieu & lay vi tri cua nhan vien hien tai
         * dat dangThemMoi = true
         * Step 2: gui lenh them moi toi bdsNHANVIEN - tu dong lay maChiNhanh - bo trong dteNGAYSINH
         * Step 3: vo hieu hoa cac nut chuc nang & gridControl - chi btnGHI & btnHOANTAC moi duoc hoat dong
         *********************************************************************/
        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*Step 1*/
            /*lấy vị trí hiện tại của con trỏ*/
            viTri = bdsKhachHang.Position;
            this.panelNhapLieu.Enabled = true;
            dangThemMoi = true;


            /*Step 2*/
            /*AddNew tự động nhảy xuống cuối thêm 1 dòng mới*/
            bdsKhachHang.AddNew();
            txtMaCN.Text = maChiNhanh;



            /*Step 3*/
            this.txtCMND.Enabled = true;
            this.btnThem.Enabled = false;
            this.btnXoa.Enabled = false;
            this.btnGhi.Enabled = true;

            this.btnHoanTac.Enabled = true;
         //  this.btnLamMoi.Enabled = false;
            
            this.btnThoat.Enabled = false;
       

            this.gcKhachHang.Enabled = false;
           // this.panelNhapLieu.Enabled = true;
        }
        /***************************************************************************
      * Step 1: tu biding source kiem tra xem khach hang  nay da co giao dịch chưa 
      *          Neu co thi thong bao la khong the xoa va ket thuc
      *          Neu khong thi bat dau xoa
      * Step 2: Neu chon OK thi tien hanh xoa
      * Step 3: Lay ma nhan vien bi xoa roi luu lai trong manv
      * Step 4: Truong hop xoa nhan vien bi loi thi quay lai dung vi tri manv bi loi
      ***************************************************************************/
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String tenKH = ((DataRowView)bdsKhachHang[bdsKhachHang.Position])["CMND"].ToString();
            /*Step 1*/

         // ktra ddeer k xoa khach hang da co GD CHUYENTIEN,GOITIEN,RUTTIEN ( khach hàng đang Hoạt động )
            
              /*          if (bdsChuyenTien.Count > 0)
                        {
                            MessageBox.Show("Không thể xóa khách hàng vì đã có GD chuyển tiền ", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }

                        if (bdsGoiTien.Count > 0)
                        {
                            MessageBox.Show("Không thể xóa khách hàng vì đã có GD gởi tiền ", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }

                        if (bdsRutTien.Count > 0)
                        {
                            MessageBox.Show("Không thể xóa khách hàng vì đã có GD Rút tiền ", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
          */

            /* Phần này phục vụ tính năng hoàn tác
                    * Đưa câu truy vấn hoàn tác vào undoList 
                    * để nếu chẳng may người dùng ấn hoàn tác thì quất luôn*/
         //   int trangThai = (cbTrangThaiXoa.Checked == true) ? 1 : 0;
            /////////////////////////////////////////////////////////////////////////
            ///


            string cauTruyVanHoanTac =
                string.Format("INSERT INTO DBO.KHACHHANG( CMND,HO,TEN,DIACHI,PHAI,NGAYCAP,SODT,MACN)" +
            "VALUES('{0}','{1}','{2}','{3}','{4}',CAST({5} AS DATETIME),'{6}','{7}')", txtCMND.Text, txtHo.Text, txtTen.Text, txtDiaChi.Text, txtPhai.Text, txtNgayCap.DateTime.ToString("yyyy-MM-dd"), txtSDT.Text, txtMaCN.Text.Trim());


            /////////////////////////
            Console.WriteLine(cauTruyVanHoanTac);
            undoList.Push(cauTruyVanHoanTac);


            /*Step 2*/
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa khach hàng này  không ?", "Thông báo",
                MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    /*Step 3*/
                    viTri = bdsKhachHang.Position;
                    bdsKhachHang.RemoveCurrent();

                   /* this.khachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.khachHangTableAdapter.Update(this.NGANHANGDATASET.KhachHang);*/

                    MessageBox.Show("Xóa thành công ", "Thông báo", MessageBoxButtons.OK);
                    this.btnHoanTac.Enabled = true;
                }
                catch (Exception ex)
                {
                    /*Step 4*/
                    MessageBox.Show("Lỗi xóa khách hàng  . Hãy thử lại\n" + ex.Message, "Thông báo", MessageBoxButtons.OK);
                  /**/  /*this.khachHangTableAdapter.Fill(this.NGANHANGDATASET.KhachHang);*/
                    // tro ve vi tri cua nhan vie dang bi loi
                    bdsKhachHang.Position = viTri;
                    //bdsNhanVien.Position = bdsNhanVien.Find("MANV", manv);
                    return;
                }
            }
            else
            {
                undoList.Pop();
            }
        }
        /**
        * viTriConTro: vi tri con tro chuot dang dung
        * viTriMaNhanVien: vi tri cua ma nhan vien voi btnTHEM or hanh dong sua du lieu
        * sp_TRACUU_KIEMTRACMND  tra ve 0 neu khong ton tai
        *                                    1 neu ton tai
        *                                    
        * Step 0 : Kiem tra du lieu dau vao
        * Step 1 : Dung stored procedure sp_TRACUU_KIEMTRAMANHANVIEN de kiem tra txtMANV
        * Step 2 : Ket hop ket qua tu Step 1 & vi tri cua txtMANV co 2 truong hop xay ra
        * + TH0: ketQua = 1 && viTriConTro != viTriMaNhanVien -> them moi nhung MANV da ton tai
        * + TH1: ketQua = 1 && viTriConTro == viTriMaNhanVien -> sua nhan vien cu
        * + TH2: ketQua = 0 && viTriConTro == viTriMaNhanVien -> co the them moi nhan vien
        * + TH3: ketQua = 0 && viTriConTro != viTriMaNhanVien -> co the them moi nhan vien
        *          
        * Step 3 : Neu khong phai TH0 thi cac TH1 - TH2 - TH3 deu hop le 
        */
        private void btnGhi_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /* Step 0 */
            bool ketQua = kiemTraDuLieuDauVao();
            if (ketQua == false)
                return;



            /*Step 1*/
            /*Lay du lieu truoc khi chon btnGHI - phuc vu btnHOANTAC - sau khi OK thi da la du lieu moi*/
            DataRowView drv = ((DataRowView)bdsKhachHang[bdsKhachHang.Position]);
            String cmnd = txtCMND.Text.Trim();// Trim() de loai bo khoang trang thua 
            String ho = drv["HO"].ToString();
            String ten = drv["TEN"].ToString();
            String diaChi = drv["DIACHI"].ToString();
            String phai = drv["PHAI"].ToString();

            DateTime ngaycap = ((DateTime)drv["NGAYSINH"]);
            Console.WriteLine(ngaycap);

            String sdt = drv["SODT"].ToString();
            String machinhanh = drv["MACN"].ToString();



            /*declare @returnedResult int
              exec @returnedResult = sp_TraCuu_KiemTraMaNhanVien '20'
              select @returnedResult*/
            String cauTruyVan =
                    "DECLARE	@result string " +
                    "EXEC @result = [dbo].[sp_TraCuu_KiemTraCMND] '" +
                   cmnd + "' " +
                    "SELECT 'Value' = @result"; ;
            SqlCommand sqlCommand = new SqlCommand(cauTruyVan, Program.conn);
            try
            {
                Program.myReader = Program.ExecSqlDataReader(cauTruyVan);
                /*khong co ket qua tra ve thi ket thuc luon*/
                if (Program.myReader == null)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thực thi database thất bại!\n\n" + ex.Message, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
                return;
            }
            Program.myReader.Read();
            int  result = int.Parse(Program.myReader.GetValue(0).ToString());      /////
            Program.myReader.Close();



            /*Step 2*/
            int viTriConTro = bdsKhachHang.Position;
            int viTriCMND = bdsKhachHang.Find("CMND", txtCMND.Text);

            if (result == 1 && viTriConTro != viTriCMND)
            {
                MessageBox.Show("CMND này đã được sử dụng !", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            else/*them moi | sua khach hang*/
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc muốn ghi dữ liệu vào cơ sở dữ liệu ?", "Thông báo",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        /*bật các nút về ban đầu*/
                        btnThem.Enabled = true;
                        btnXoa.Enabled = true;
                        btnGhi.Enabled = true;
                        btnHoanTac.Enabled = true;

                        btnLamMoi.Enabled = true;

                        btnThoat.Enabled = true;

                        this.txtCMND.Enabled = false;
                        this.bdsKhachHang.EndEdit();
                       /* this.khachHangTableAdapter.Update(this.NGANHANGDATASET.KhachHang);*/
                        this.gcKhachHang.Enabled = true;

                        /*lưu 1 câu truy vấn để hoàn tác yêu cầu*/
                        String cauTruyVanHoanTac = "";
                        /*trước khi ấn btnGHI là btnTHEM*/
                        if (dangThemMoi == true)
                        {
                            cauTruyVanHoanTac = "" +
                                "DELETE DBO.KHACHHANG " +
                                "WHERE CMND = " + txtCMND.Text.Trim();
                        }
                        /*trước khi ấn btnGHI là sửa thông tin nhân viên*/
                        else
                        {

                            ////////////////////////////////
                            ///
                            cauTruyVanHoanTac =
                                "UPDATE DBO.KhachHang " +
                                "SET " +
                                "HO = '" + ho + "'," +
                                "TEN = '" + ten + "'," +
                                "DIACHI = '" + diaChi + "'," +
                                 "PHAI = '" + phai + "'," +
                                  "NGAYCAP = CAST('" + ngaycap.ToString("yyyy-MM-dd") + "' AS DATETIME)," +
                                 "SODT = '" + sdt + "'," +
                                "MACN = '" + machinhanh + "'," +
                             
                               "WHERE CMND = '" + cmnd + "'";


                            //////////////////////////////
                        }
                        Console.WriteLine(cauTruyVanHoanTac);

                        /*Đưa câu truy vấn hoàn tác vào undoList 
                         * để nếu chẳng may người dùng ấn hoàn tác thì quất luôn*/
                        undoList.Push(cauTruyVanHoanTac);
                        /*cập nhật lại trạng thái thêm mới cho chắc*/
                        dangThemMoi = false;
                        MessageBox.Show("Ghi thành công", "Thông báo", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {

                        bdsKhachHang.RemoveCurrent();
                        MessageBox.Show("Thất bại. Vui lòng kiểm tra lại!\n" + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /**********************************************************************
         * moi lan nhan btnHOANTAC thi nen nhan them btnLAMMOI de 
         * tranh bi loi khi an btnTHEM lan nua
         * 
         * statement: chua cau y nghia chuc nang ngay truoc khi an btnHOANTAC.
         * Vi du: statement = INSERT | DELETE | CHANGEBRAND
         * 
         * bdsNhanVien.CancelEdit() - phuc hoi lai du lieu neu chua an btnGHI
         * Step 0: trường hợp đã ấn btnTHEM nhưng chưa ấn btnGHI
         * Step 1: kiểm tra undoList có trông hay không ?
         * Step 2: Neu undoList khong trống thì lấy ra khôi phục
         *********************************************************************/
        private void btnHoanTac_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /* Step 0 - */
            if (dangThemMoi == true && this.btnThem.Enabled == false)
            {
                dangThemMoi = false;

                this.txtCMND.Enabled = false;
                this.btnThem.Enabled = true;
                this.btnXoa.Enabled = true;
                this.btnGhi.Enabled = true;

                this.btnHoanTac.Enabled = false;
                this.btnLamMoi.Enabled = true;
       
                this.btnThoat.Enabled = true;
              

                this.gcKhachHang.Enabled = true;
                this.panelNhapLieu.Enabled = true;

                bdsKhachHang.CancelEdit();
                /*xoa dong hien tai*/
                bdsKhachHang.RemoveCurrent();
                /* trở về lúc đầu con trỏ đang đứng*/
                bdsKhachHang.Position = viTri;
                return;
            }


            /*Step 1*/
            if (undoList.Count == 0)
            {
                MessageBox.Show("Không còn thao tác nào để khôi phục", "Thông báo", MessageBoxButtons.OK);
                btnHoanTac.Enabled = false;
                return;
            }

            /*Step 2*/
            bdsKhachHang.CancelEdit();
            String cauTruyVanHoanTac = undoList.Pop().ToString();
            //Console.WriteLine(cauTruyVanHoanTac);

            /*Step 2.1*/
            if(1==1)
            {
                if (Program.KetNoi() == 0)
                {
                    return;
                }
                int n = Program.ExecSqlNonQuery(cauTruyVanHoanTac);

            }
           /* this.khachHangTableAdapter.Fill(this.NGANHANGDATASET.KhachHang);*/


        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();






        }
     

        private void btnLamMoi_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // do du lieu moi tu dataSet vao gridControl NHANVIEN
                /*this.khachHangTableAdapter.Fill(this.NGANHANGDATASET.KhachHang);*/
                this.gcKhachHang.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Làm mới" + ex.Message, "Thông báo", MessageBoxButtons.OK);
                return;
            }
        }








        ////////////////////////////





        private void panelControl2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mACNTextEdit_EditValueChanged(object sender, EventArgs e)
        {

        }

      
       
        /**********************************************************************
       * moi lan nhan btnHOANTAC thi nen nhan them btnLAMMOI de 
       * tranh bi loi khi an btnTHEM lan nua
       * 
       * statement: chua cau y nghia chuc nang ngay truoc khi an btnHOANTAC.
       * Vi du: statement = INSERT | DELETE | CHANGEBRAND
       * 
       * bdsNhanVien.CancelEdit() - phuc hoi lai du lieu neu chua an btnGHI
       * Step 0: trường hợp đã ấn btnTHEM nhưng chưa ấn btnGHI
       * Step 1: kiểm tra undoList có trông hay không ?
       * Step 2: Neu undoList khong trống thì lấy ra khôi phục
       *********************************************************************/
      

        /***************************************************************************
         * Step 1: tu biding source kiem tra xem nhan vien nay da lap don  - 
         *chuyen khoan,gdgoi 0gdrut chua
         *          Neu co thi thong bao la khong the xoa va ket thuc
         *          Neu khong thi bat dau xoa
         * Step 2: Neu chon OK thi tien hanh xoa
         * Step 3: Lay ma nhan vien bi xoa roi luu lai trong manv
         * Step 4: Truong hop xoa nhan vien bi loi thi quay lai dung vi tri manv bi loi
         ***************************************************************************/
       
        private void cmbCHINHANH_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
          /*Neu combobox khong co so lieu thi ket thuc luon*/
            if (cmbCHINHANH.SelectedValue.ToString() == "System.Data.DataRowView")
                return;

            Program.serverName = cmbCHINHANH.SelectedValue.ToString();

            /*Neu chon sang chi nhanh khac voi chi nhanh hien tai*/
            if (cmbCHINHANH.SelectedIndex != Program.brand)
            {
                Program.loginName = Program.remoteLogin;
                Program.loginPassword = Program.remotePassword;
            }
            /*Neu chon trung voi chi nhanh dang dang nhap o formDangNhap*/
            else
            {
                Program.loginName = Program.currentLogin;
                Program.loginPassword = Program.currentPassword;
            }

            if (Program.KetNoi() == 0)
            {
                MessageBox.Show("Xảy ra lỗi kết nối với chi nhánh hiện tại", "Thông báo", MessageBoxButtons.OK);
            }
            else
            {
               /* this.chiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
                this.chiNhanhTableAdapter.Fill(this.NGANHANGDATASET.ChiNhanh);
                *//*Do du lieu tu dataSet vao grid Control*//*
                this.khachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.khachHangTableAdapter.Fill(this.NGANHANGDATASET.KhachHang);*/
             
                /*Tu dong lay maChiNhanh hien tai - phuc vu cho phan btnTHEM*/
                /*Cho dong nay chay thi bi loi*/
                //maChiNhanh = ((DataRowView)bdsNhanVien[0])["MACN"].ToString().Trim();
            }
        }

        private bool kiemTraDuLieuDauVao()
        {
            /*kiem tra txtMANV*/
            if (txtCMND.Text == "")
            {
                MessageBox.Show("Không bỏ trống CMND ", "Thông báo", MessageBoxButtons.OK);
                txtCMND.Focus();
                return false;
            }

            if (Regex.IsMatch(txtCMND.Text, @"^[a-zA-Z0-9]+$") == false)
            {
                MessageBox.Show("Mã nhân viên chỉ chấp nhận số", "Thông báo", MessageBoxButtons.OK);
                txtCMND.Focus();
                return false;
            }
            /*kiem tra txtHO*/
            if (txtHo.Text == "")
            {
                MessageBox.Show("Không bỏ trống họ và tên", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            //"^[0-9A-Za-z ]+$"
            if (Regex.IsMatch(txtHo.Text, @"^[A-Za-z ]+$") == false)
            {
                MessageBox.Show("Họ của người chỉ có chữ cái và khoảng trắng", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            if (txtHo.Text.Length > 40)
            {
                MessageBox.Show("Họ không thể lớn hơn 40 kí tự", "Thông báo", MessageBoxButtons.OK);
                txtHo.Focus();
                return false;
            }
            /*kiem tra txtTEN*/
            if (txtTen.Text == "")
            {
                MessageBox.Show("Không bỏ trống họ và tên", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }

            if (Regex.IsMatch(txtTen.Text, @"^[a-zA-Z ]+$") == false)
            {
                MessageBox.Show("Tên người chỉ có chữ cái và khoảng trắng", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }

            if (txtTen.Text.Length > 10)
            {
                MessageBox.Show("Tên không thể lớn hơn 10 kí tự", "Thông báo", MessageBoxButtons.OK);
                txtTen.Focus();
                return false;
            }
            /*kiem tra txtDIACHI*/
            if (txtDiaChi.Text == "")
            {
                MessageBox.Show("Không bỏ trống địa chỉ", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }

            if (Regex.IsMatch(txtDiaChi.Text, @"^[a-zA-Z0-9, ]+$") == false)
            {
                MessageBox.Show("Địa chỉ chỉ chấp nhận chữ cái, số và khoảng trắng", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }

            if (txtDiaChi.Text.Length > 100)
            {
                MessageBox.Show("Không bỏ trống địa chỉ", "Thông báo", MessageBoxButtons.OK);
                txtDiaChi.Focus();
                return false;
            }
           


            return true;
        }

        

     


    }


}