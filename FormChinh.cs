using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NGANHANG_APP
{
    public partial class FormChinh : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public FormChinh()
        {
            InitializeComponent();
        }


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



        /************************************************************
         *enableButtons: kích hoạt các tab chức năng và nút đăng xuất
         ************************************************************/
        public void enableButtons()
        {

            btnDangNhap.Enabled = false;
            btnDangXuat.Enabled = true;

            PageNhapXuat.Visible = true;
            PageBaoCao.Visible = true;
            btnTaoTaiKhoan.Enabled = true;

            if (Program.role == "USER")
            {
                btnTaoTaiKhoan.Enabled = false;
            }

            //PageHeThong.Visible = true;


        }



        /************************************************************
         * Dispose: giải phóng các form khỏi bộ nhớ. Ví dụ form nhân viên,...
         * Close: đóng hoàn toàn chương trình lại
         ************************************************************/
        private void logout()
        {
            foreach (Form f in this.MdiChildren)
                f.Dispose();
        }

     
    

        private void FormChinh_Load(object sender, EventArgs e)
        {

        }

        private void ribbon_Click(object sender, EventArgs e)
        {

        }

        private void btnDangNhap_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormDangNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDangNhap form = new FormDangNhap();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnTaoTaiKhoan_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormTaoTaiKhoan));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormTaoTaiKhoan form = new FormTaoTaiKhoan();
                //form.MdiParent = this;
                form.Show();
            }
        }

        private void btnDangXuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            logout();

            btnDangNhap.Enabled = true;
            btnDangXuat.Enabled = false;

            PageNhapXuat.Visible = false;
            PageBaoCao.Visible = false;
            //pageTaiKhoan.Visible = false;

            Form f = this.CheckExists(typeof(FormDangNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDangNhap form = new FormDangNhap();
                //form.MdiParent = this;
                form.Show();
            }

           Program.formChinh.MaNhanVien.Text = "MÃ NHÂN VIÊN:";
           Program.formChinh.HoTen.Text = "HỌ TÊN:";
           Program.formChinh.Nhom.Text = "VAI TRÒ:";
        }


        private void btnThoat_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormNhanVien form = new FormNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnKhachHang_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormKhachHang));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormKhachHang form = new FormKhachHang();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnChuyenKhoan_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormChuyenTien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormChuyenTien form = new FormChuyenTien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnGoiTien_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnRutTien_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnDanhSachNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormDanhSachNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDanhSachNhanVien form = new FormDanhSachNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnDanhSachKhachHang_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormDanhSachKhachHang));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormDanhSachKhachHang form = new FormDanhSachKhachHang();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnHoatDongNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormHoatDongNhanVien));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormHoatDongNhanVien form = new FormHoatDongNhanVien();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void btnChiTietNhapXuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form f = this.CheckExists(typeof(FormChiTietNhapXuat));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                FormChiTietNhapXuat form = new FormChiTietNhapXuat();
                form.MdiParent = this;
                form.Show();
            }
        }

        private void FormChinh_Load_1(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = Program.userName;
            toolStripStatusLabel2.Text = Program.staff;
            toolStripStatusLabel3.Text = Program.role;
            

        }

        private void MaNhanVien_Click(object sender, EventArgs e)
        {

        }
    }
}