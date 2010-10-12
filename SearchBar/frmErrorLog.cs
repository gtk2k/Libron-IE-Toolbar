using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LibronToolbar
{
    public partial class frmErrorLog : Form
    {
        private DataView dvErrorLog;

        public frmErrorLog(DataTable tblLog)
        {
            InitializeComponent();
            dvErrorLog = new DataView(tblLog);
            dvErrorLog.Sort = "No DESC";
            dgvErrorLog.DataSource = dvErrorLog; 
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataRowView rv in dvErrorLog)
            {
                sb.AppendLine(rv["No"].ToString().PadLeft(4, '0') + ":" + rv["ErrorMessage"].ToString());
            }
            Clipboard.SetText(sb.ToString());
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
