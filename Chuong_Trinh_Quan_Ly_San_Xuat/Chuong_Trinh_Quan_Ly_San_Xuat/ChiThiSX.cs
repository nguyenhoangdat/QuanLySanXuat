﻿using Chuong_Trinh_Quan_Ly_San_Xuat.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Chuong_Trinh_Quan_Ly_San_Xuat
{
    public partial class FrmChiThiSX : Form
    {
        int actionSX = 0;
        string phanquyen;
        bool selectByMouse = false;
        string usernhap;
        string casanxuat;
        public FrmChiThiSX()
        {
            InitializeComponent();
            dtgChiThiSX.Dock = DockStyle.Fill;
            dtpTuNgay.CustomFormat = "yyyy-MM-dd";
            dtpDenNgay.CustomFormat = "yyyy-MM-dd";
            dtpNgaySX.CustomFormat = "yyyy-MM-dd";
            DateTime now = DateTime.Now;
            dtpDenNgay.Value = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);
            dtpTuNgay.Value = new DateTime(now.Year, now.Month, 1);
            System.Windows.Forms.Form f = System.Windows.Forms.Application.OpenForms["FrmMain"];
            phanquyen = ((FrmMain)f).quyensudung;
            usernhap = ((FrmMain)f).tbTenDN.Text.ToString();
            casanxuat = ((FrmMain)f).casx;
            cbCaSX.SelectedIndex = 0;
            if (casanxuat != "") cbCaSX.Text = casanxuat;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.SetProperty, null,
            dtgChiThiSX, new object[] { true });
            enablecontrolCTSX();
            GetChiThiSX();
            SetEventForNumerric(this.panelQLSX);
            NewChiThiSX(this.panelFilterCTSX);
            loadmaymoc();
            loadnhanvien();

        }
        void xuatexceldtg(DataGridView dtg)
        {
            Excel._Application excel = new Excel.Application();
            Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Excel._Worksheet worksheet = null;

            try
            {
                worksheet = (Excel._Worksheet)workbook.ActiveSheet;
                object[,] arr = new object[dtg.Rows.Count + 1, dtg.Columns.Count + 1];
                for (int c = 0; c < dtg.Columns.Count; c++)
                {
                    arr[0, c] = dtg.Columns[c].HeaderText;
                }
                int rowindex = 1;
                int colindex = 0;
                for (int r = 0; r < dtg.Rows.Count; r++)
                {
                    for (int c = 0; c < dtg.Columns.Count; c++)
                    {
                        if (dtg.Rows[r].Cells[c].Value != null) arr[rowindex, colindex] = dtg.Rows[r].Cells[c].Value.ToString();
                        colindex++;
                    }
                    colindex = 0;
                    rowindex++;
                }

                Excel.Range c1 = (Excel.Range)worksheet.Cells[1, 1];
                Excel.Range c2 = (Excel.Range)worksheet.Cells[1 + dtg.Rows.Count, dtg.Columns.Count + 1];
                Excel.Range range = worksheet.get_Range(c1, c2);
                range.Value = arr;
                excel.Visible = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                workbook = null;
                excel = null;
                worksheet = null;
            }
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            actionSX = 1;
            enablecontrolCTSX();
            NewChiThiSX(this.panelQLSX);
            dtpNgaySX.Value = DateTime.Now;
        }

        void loadmaymoc()
        {
            DataTable data = Import_Manager.Instance.GetMayMoc();
            cbTenMay.ValueMember = "ID";
            cbTenMay.DisplayMember = "TEN_MAY";
            cbTenMay.DataSource = data;
            CbSomay.DisplayMember = "SO_MAY";
            CbSomay.DataSource = data;
            cbmamay.DisplayMember = "MA_MAY";
            cbmamay.DataSource = data;
        }

        void loadnhanvien()
        {
            DataTable data = Import_Manager.Instance.LoadNhanVien();
            cbGiaCong.DisplayMember = "NV";
            cbGiaCong.ValueMember = "ID";
            cbGiaCong.DataSource = data;
        }

        void NewChiThiSX(Control col)
        {
            foreach (Control c in col.Controls)
            {
                //if (c.GetType().Name == "TextBox") c.Text = "";
                //if (c.GetType().Name == "ComboBox") c.Text = "";
                if (c.GetType().Name == "NumericUpDown") c.Text = "0";
                NewChiThiSX(c);
            }
           
        }

        void CheckChiThiSX(Control col)
        {
            foreach (Control c in col.Controls)
            {

                if (c.GetType().Name == "ComboBox")
                {
                    if(c.Text == "" && c.Name != "cbTenMay" && c.Name != "CbSomay" && c.Name != "cbmamay") MessageBox.Show("Bạn chưa nhâp thông tin cho " + c.Name);
                }
                CheckChiThiSX(c);
            }
        }


        private void frm_Closing(object sender, FormClosingEventArgs e)
        {
            this.Show();
        }
        void SetEventForNumerric(Control col)
        {
            foreach (Control c in col.Controls)
            {
                if (c.GetType().Name == "NumericUpDown")
                {
                    c.Enter += quickBoxs_Enter;
                    c.MouseDown += quickBoxs_MouseDown;
                    c.KeyDown += quickBoxs_KeyDown;
                    c.TextChanged += quickBoxs_TextChanged;
                }
                if (c.GetType().Name == "TextBox")
                {
                    c.KeyDown += tb_KeyDown;
                }
                SetEventForNumerric(c);
            }
        }
        void tinhtongtgSX()
        {
            int tongtgsx = 0;
            foreach (Control c in this.groupBoxThoiGian.Controls)
            {
                if (c.GetType().Name == "NumericUpDown")
                {
                    if(c.Text != "") tongtgsx += Int32.Parse(c.Text);
                }
            }
            labelTGSX.Text = "Tổng thời gian SX: " + tongtgsx.ToString();

        }

        private void quickBoxs_TextChanged(object sender, EventArgs e)
        {
            NumericUpDown curBox = sender as NumericUpDown;
            if (curBox.Parent.Name == "groupBoxThoiGian") tinhtongtgSX();
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox curBox = sender as TextBox;
            if (e.Control && e.KeyCode == Keys.A)
            {
                curBox.SelectAll();
            }
        }
        private void quickBoxs_Enter(object sender, EventArgs e)
        {
            NumericUpDown curBox = sender as NumericUpDown;
            curBox.Select();
            curBox.Select(0, curBox.Text.Length);
            if (MouseButtons == MouseButtons.Left)
            {
                selectByMouse = true;
            }
        }
        private void quickBoxs_MouseDown(object sender, MouseEventArgs e)
        {
            NumericUpDown curBox = sender as NumericUpDown;
            if (selectByMouse)
            {
                curBox.Select(0, curBox.Text.Length);
                selectByMouse = false;
            }
        }

        private void quickBoxs_KeyDown(object sender, KeyEventArgs e)
        {
            NumericUpDown curBox = sender as NumericUpDown;
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    curBox.Parent.SelectNextControl(curBox, true, true, true, true);
                    e.Handled = true;
                    break;
            }
        }

        void getmacdtheomsql()
        {
            DataTable data = Import_Manager.Instance.GetMayMocTheoCD(cbMaCongDoan.Text);
            if (data.Rows.Count > 0)
            {
                cbTenMay.Text = data.Rows[0][1].ToString();
                CbSomay.Text = data.Rows[0][2].ToString();
                cbmamay.Text = data.Rows[0][3].ToString();
            }
            else
            {
                cbTenMay.Text = "";
            }
        }

        void enablecontrolCTSX()
        {
            if (actionSX == 0)
            {
                btnNew.Enabled = true;
                BtnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnSave.Enabled = false;
                btnCancel.Enabled = false;
                panelQLSX.Visible = false;
                //if (phanquyen != "Full")
                //{
                //    BtnEdit.Enabled = false;
                //    btnDelete.Enabled = false;
                //}
            }
            else
            {
                btnNew.Enabled = false;
                BtnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                panelQLSX.Visible = true;
            }

        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            actionSX = 2;
            enablecontrolCTSX();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            actionSX = 3;
            if (MessageBox.Show("Are you sure to delete?", "Information", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
            try
            {
                int results = Import_Manager.Instance.UpdateChiThiSX(actionSX, (int)dtgChiThiSX.CurrentRow.Cells[0].Value, cbMaCongDoan.Text, (int)cbTenMay.SelectedValue, dtpNgaySX.Value, cbCaSX.Text, (int)numSoLuong.Value, tbSoLot.Text, (int)numtgSX.Value, (int)numtgChuanBi.Value
                                                                    , (int)numtgSua.Value, (int)numtgChaoLe.Value, (int)numtgDaoTao.Value, (int)numtgChoKhuon.Value, (int)numSuoc.Value, (int)numMop.Value, (int)numSet.Value, (int)numBienDang.Value, (int)numhongKhac.Value, (int)numBaoLuu.Value, (int)numXiMa.Value, (int)numNhiet.Value, Int32.Parse(cbGiaCong.SelectedValue.ToString()), tbGhiChu.Text, usernhap, (int)numVesinh6S.Value, (int)numMopbaoluu.Value, (int)numNgKiemTra.Value);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            actionSX = 0;
            enablecontrolCTSX();
            GetChiThiSX(); 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            actionSX = 0;
            enablecontrolCTSX();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CheckChiThiSX(this.panelQLSX);
            int idCTSX = 0;
            int actioncur = actionSX;
            if (dtgChiThiSX.Rows.Count > 1 && dtgChiThiSX.CurrentRow.Cells[0].Value.ToString() != "") idCTSX = (int)dtgChiThiSX.CurrentRow.Cells[0].Value;
            try
            {
                DataTable data = Import_Manager.Instance.CheckSoluongcdsxtrc((int)cbMaCongDoan.SelectedValue);
                if (data.Rows.Count ==1)
                {
                    int soluongdatcdtrc = Int32.Parse(data.Rows[0][0].ToString());
                    if ((int)numSoLuong.Value != soluongdatcdtrc)
                    {
                        MessageBox.Show("Số lượng không khớp với công đoạn trước");
                        //return;
                     }
                }
                int results = Import_Manager.Instance.UpdateChiThiSX(actionSX, idCTSX, cbMaCongDoan.Text, (int)cbTenMay.SelectedValue, dtpNgaySX.Value, cbCaSX.Text, (int)numSoLuong.Value, tbSoLot.Text, (int)numtgSX.Value, (int)numtgChuanBi.Value
                                                                    , (int)numtgSua.Value, (int)numtgChaoLe.Value, (int)numtgDaoTao.Value, (int)numtgChoKhuon.Value, (int)numSuoc.Value, (int)numMop.Value, (int)numSet.Value, (int)numBienDang.Value, (int)numhongKhac.Value, (int)numBaoLuu.Value, (int)numXiMa.Value, (int)numNhiet.Value, (int)cbGiaCong.SelectedValue, tbGhiChu.Text, usernhap, (int)numVesinh6S.Value, (int)numMopbaoluu.Value, (int)numNgKiemTra.Value);

                GetChiThiSX();
                if(dtgChiThiSX.Rows.Count >2) dtgChiThiSX.CurrentCell = dtgChiThiSX.Rows[dtgChiThiSX.Rows.Count - 2].Cells[0];
                actionSX = 0;
                enablecontrolCTSX();
                if(actioncur ==1) btnNew_Click(btnNew, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        
        void GetChiThiSX()
        {
            DataTable chithi = Import_Manager.Instance.GetChiThiSanXuat(dtpTuNgay.Value, dtpDenNgay.Value, tbMSQLFilter.Text);
            dtgChiThiSX.DataSource = chithi;
        }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            GetChiThiSX();
        }

        private void dtgChiThiSX_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgChiThiSX.CurrentRow.Cells[0].Value.ToString() != "" && actionSX != 1)
            {
                tbMSQL.Text = dtgChiThiSX.CurrentRow.Cells[1].Value.ToString(); ;
                cbMaCongDoan.Text = dtgChiThiSX.CurrentRow.Cells[4].Value.ToString();
                dtpNgaySX.Value = Convert.ToDateTime(dtgChiThiSX.CurrentRow.Cells[9].Value.ToString());
                cbCaSX.Text = dtgChiThiSX.CurrentRow.Cells[10].Value.ToString();
                numSoLuong.Text = dtgChiThiSX.CurrentRow.Cells[11].Value.ToString();
                tbSoLot.Text = dtgChiThiSX.CurrentRow.Cells[12].Value.ToString();
                numtgSX.Text = dtgChiThiSX.CurrentRow.Cells[13].Value.ToString();
                numtgChuanBi.Text = dtgChiThiSX.CurrentRow.Cells[14].Value.ToString();
                numtgSua.Text = dtgChiThiSX.CurrentRow.Cells[15].Value.ToString();
                numtgChaoLe.Text = dtgChiThiSX.CurrentRow.Cells[16].Value.ToString();
                numtgDaoTao.Text = dtgChiThiSX.CurrentRow.Cells[17].Value.ToString();
                numtgChoKhuon.Text = dtgChiThiSX.CurrentRow.Cells[18].Value.ToString();
                numVesinh6S.Text = dtgChiThiSX.CurrentRow.Cells[19].Value.ToString();
                numSuoc.Text = dtgChiThiSX.CurrentRow.Cells[20].Value.ToString();
                numMop.Text = dtgChiThiSX.CurrentRow.Cells[21].Value.ToString();
                numSet.Text = dtgChiThiSX.CurrentRow.Cells[22].Value.ToString();
                numBienDang.Text = dtgChiThiSX.CurrentRow.Cells[23].Value.ToString();
                numhongKhac.Text = dtgChiThiSX.CurrentRow.Cells[24].Value.ToString();
                numBaoLuu.Text = dtgChiThiSX.CurrentRow.Cells[25].Value.ToString();
                numXiMa.Text = dtgChiThiSX.CurrentRow.Cells[26].Value.ToString();
                numNhiet.Text = dtgChiThiSX.CurrentRow.Cells[27].Value.ToString();
                numNgKiemTra.Text = dtgChiThiSX.CurrentRow.Cells[28].Value.ToString();
                numMopbaoluu.Text = dtgChiThiSX.CurrentRow.Cells[29].Value.ToString();
                cbGiaCong.Text = dtgChiThiSX.CurrentRow.Cells[30].Value.ToString();
                tbGhiChu.Text = dtgChiThiSX.CurrentRow.Cells[31].Value.ToString();
                cbTenMay.SelectedValue = dtgChiThiSX.CurrentRow.Cells[33].Value;
            }
        }

        private void dtpTuNgay_ValueChanged(object sender, EventArgs e)
        {
            GetChiThiSX();
        }

        private void dtgChiThiSX_SelectionChanged(object sender, EventArgs e)
        {

        }


        private void cbMaSP_MouseClick(object sender, MouseEventArgs e)
        {
            cbMaCongDoan.DroppedDown = true;
        }

        private void nhânSựToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNhanSu f = new Chuong_Trinh_Quan_Ly_San_Xuat.FrmNhanSu();
            f.Show();
        }

        private void cbMaCongDoan_SelectedIndexChanged(object sender, EventArgs e)
        {
            getmacdtheomsql();
        }
        void getmacongdoantheomsql()
        {
            DataTable data = Import_Manager.Instance.Loadcongdoantheomsql(tbMSQL.Text);
            cbMaCongDoan.DisplayMember = "MA_CONG_DOAN";
            cbMaCongDoan.ValueMember = "ID";
            cbMaCongDoan.DataSource = data;

            cbTenCongDoan.DisplayMember = "TEN_CONG_DOAN";
            cbTenCongDoan.DataSource = data;
            cbtenhang.DisplayMember = "TEN_SP";
            cbtenhang.DataSource = data;
        }

        private void tbMSQL_TextChanged(object sender, EventArgs e)
        {
            getmacongdoantheomsql();
        }

        private void tbMSQLFilter_TextChanged(object sender, EventArgs e)
        {
            GetChiThiSX();
        }

        private void mnutripXuatExcel_Click(object sender, EventArgs e)
        {
            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {


                //Getting the location and file name of the excel to save from user.
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveDialog.FilterIndex = 2;

                if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.ActiveSheet;
                    //worksheet.Name = "Exported";
                    //lúc nào cũng có dòng trắng cuối cùng nên mảng luu bao gom có tieu de = row.cout + 1(tieu de) - 1 (dong trang cuo)
                    object[,] arr = new object[dtgChiThiSX.Rows.Count, dtgChiThiSX.Columns.Count + 1];

                    for (int c = 0; c < dtgChiThiSX.Columns.Count; c++)
                    {
                        arr[0, c] = dtgChiThiSX.Columns[c].HeaderText;
                    }
                    int rowindex = 1;
                    int colindex = 0;
                    for (int r = 0; r < dtgChiThiSX.Rows.Count-1; r++)
                    {
                        for (int c = 0; c < dtgChiThiSX.Columns.Count; c++)
                        {
                            arr[rowindex, colindex] = dtgChiThiSX.Rows[r].Cells[c].Value.ToString();
                            colindex++;
                        }
                        colindex = 0;
                        rowindex++;
                    }
                    Excel.Range c1 = (Excel.Range)worksheet.Cells[1, 1];
                    Excel.Range c2 = (Excel.Range)worksheet.Cells[1 + dtgChiThiSX.Rows.Count-1, dtgChiThiSX.Columns.Count + 1];
                    Excel.Range range = worksheet.get_Range(c1, c2);
                    range.Value = arr;
                    workbook.SaveAs(saveDialog.FileName);
                    excel.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                //excel.Quit();
                workbook = null;
                excel = null;
                worksheet = null;
            }

        }

        private void cbGiaCong_Enter(object sender, EventArgs e)
        {
            cbGiaCong.DroppedDown = true;
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            xuatexceldtg(dtgChiThiSX);
        }
    }
}
