using System;
using System.Windows.Forms;
using BandObjectLib;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using mshtml;
using SHDocVw;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;

namespace LibronToolbar
{
	[Guid("7FCC8C8D-04F0-4903-B953-90616ECA25AA")]
    [Description("Libron Toolbar")]
    [BandObject("Libron Toolbar", BandObjectStyle.ExplorerToolbar | BandObjectStyle.Horizontal, HelpText = "全国の図書館にある本を検索")]
    public class Toolbar : BandObject
    {
        private IContainer components;

        private ToolStripSplitButton logo;
        private ToolStripComboBox cmbPrefectures;
        private ToolStripComboBox cmbLibraries;
        private ToolStripButton btnSave;
        private ToolStripLabel lblLoading;
        private ToolStripButton btnChangeOrCancel;

        private ToolStrip toolStripLeft;
        private ToolStripLabel lblDescription;
        private ToolStripLabel lblVersion;

        private LibronClass libron;
        private ToolStripLabel lblError;
        private Timer tmrError;

        private EnableVS context;
        private ToolStripMenuItem tsShowErrorLog;
        private string userId;

        class fixedLibraryItemData
        {
            public string group { get; set; }
            public string systemid { get; set; }
            public string systemname { get; set; }
        }

        public class saveData
        {
            public string selectedPrefecture;
            public string selectedSystemId;
            public string selectedSystemName;
        }

        private string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }



        //#endregion

        #region Ctor(s)
        public Toolbar()
        {
            //log.Info("Inside ToolbarToolbar constructor.");
            InitializeComponent();

            context = new EnableVS();

            userId = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    userId = Convert.ToBase64String(Encoding.Unicode.GetBytes(nic.GetPhysicalAddress().ToString()));
                    break;
                }
            }

            libron = new LibronClass();
            libron.initializelibron();
            System.OperatingSystem os = System.Environment.OSVersion;
            cmbPrefectures.Visible = false;
            cmbPrefectures.Items.AddRange(libron.prefectures);
            cmbPrefectures.SelectedItem = libron.selectedPrefecture;
            cmbLibraries.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLibraries.Enabled = false;
            cmbLibraries.Visible = false;
            btnSave.Visible = false;
            lblLoading.Text = "保存内容取得中...";
            lblLoading.Visible = false;
            lblDescription.Visible = false;
            lblDescription.Text = "[" + libron.selectedPrefecture + "]" + libron.selectedSystemName + "で検索 ";
            btnChangeOrCancel.Visible = true;
            btnChangeOrCancel.ForeColor = Color.Gray;
            btnChangeOrCancel.Enabled = false;
            lblVersion.Text = "ver." + libron.libronversion;
            this.toolStripLeft.Renderer = new NoBorderToolStripRenderer();
            lblError.Visible = false;

            //textSearchbox.GotFocus += new EventHandler(textSearchbox_GotFocus);
            //textSearchbox.Focus();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            context.DisableVS();
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        internal class NoBorderToolStripRenderer : ToolStripSystemRenderer
        {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
        } 

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toolbar));
            this.toolStripLeft = new System.Windows.Forms.ToolStrip();
            this.logo = new System.Windows.Forms.ToolStripSplitButton();
            this.tsShowErrorLog = new System.Windows.Forms.ToolStripMenuItem();
            this.lblVersion = new System.Windows.Forms.ToolStripLabel();
            this.cmbPrefectures = new System.Windows.Forms.ToolStripComboBox();
            this.cmbLibraries = new System.Windows.Forms.ToolStripComboBox();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.lblLoading = new System.Windows.Forms.ToolStripLabel();
            this.lblDescription = new System.Windows.Forms.ToolStripLabel();
            this.btnChangeOrCancel = new System.Windows.Forms.ToolStripButton();
            this.lblError = new System.Windows.Forms.ToolStripLabel();
            this.tmrError = new System.Windows.Forms.Timer(this.components);
            this.toolStripLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripLeft
            // 
            this.toolStripLeft.BackColor = System.Drawing.Color.Transparent;
            this.toolStripLeft.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripLeft.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripLeft.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logo,
            this.lblVersion,
            this.cmbPrefectures,
            this.cmbLibraries,
            this.btnSave,
            this.lblLoading,
            this.lblDescription,
            this.btnChangeOrCancel,
            this.lblError});
            this.toolStripLeft.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripLeft.Location = new System.Drawing.Point(0, 0);
            this.toolStripLeft.Name = "toolStripLeft";
            this.toolStripLeft.Size = new System.Drawing.Size(615, 27);
            this.toolStripLeft.Stretch = true;
            this.toolStripLeft.TabIndex = 2;
            // 
            // logo
            // 
            this.logo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.logo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsShowErrorLog});
            this.logo.Image = ((System.Drawing.Image)(resources.GetObject("logo.Image")));
            this.logo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.logo.Name = "logo";
            this.logo.Padding = new System.Windows.Forms.Padding(2, 0, 3, 0);
            this.logo.Size = new System.Drawing.Size(66, 24);
            this.logo.Text = "toolStripLabel1";
            // 
            // tsShowErrorLog
            // 
            this.tsShowErrorLog.Name = "tsShowErrorLog";
            this.tsShowErrorLog.Size = new System.Drawing.Size(152, 22);
            this.tsShowErrorLog.Text = "エラーログ表示";
            this.tsShowErrorLog.Click += new System.EventHandler(this.tsShowErrorLog_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.ActiveLinkColor = System.Drawing.Color.White;
            this.lblVersion.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(121)))), ((int)(((byte)(17)))));
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(66, 24);
            this.lblVersion.Text = "lblVersion";
            // 
            // cmbPrefectures
            // 
            this.cmbPrefectures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPrefectures.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbPrefectures.Name = "cmbPrefectures";
            this.cmbPrefectures.Size = new System.Drawing.Size(121, 27);
            // 
            // cmbLibraries
            // 
            this.cmbLibraries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLibraries.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbLibraries.Name = "cmbLibraries";
            this.cmbLibraries.Size = new System.Drawing.Size(121, 27);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(33, 24);
            this.btnSave.Text = "保存";
            // 
            // lblLoading
            // 
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(75, 24);
            this.lblLoading.Text = "データ取得中...";
            // 
            // lblDescription
            // 
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(51, 24);
            this.lblDescription.Text = "～で検索";
            // 
            // btnChangeOrCancel
            // 
            this.btnChangeOrCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnChangeOrCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeOrCancel.Image")));
            this.btnChangeOrCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnChangeOrCancel.Name = "btnChangeOrCancel";
            this.btnChangeOrCancel.Size = new System.Drawing.Size(33, 24);
            this.btnChangeOrCancel.Text = "変更";
            // 
            // lblError
            // 
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(42, 24);
            this.lblError.Text = "lblError";
            // 
            // tmrError
            // 
            this.tmrError.Interval = 5000;
            // 
            // Toolbar
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.toolStripLeft);
            this.Name = "Toolbar";
            this.Size = new System.Drawing.Size(1419, 24);
            this.Title = "";
            this.ViewMode = ((BandObjectLib.DBIMF)((BandObjectLib.DBIMF.USECHEVRON | BandObjectLib.DBIMF.TOPALIGN)));
            this.ExplorerAttached += new System.EventHandler(this.ToolbarToolbar_ExplorerAttached);
            this.toolStripLeft.ResumeLayout(false);
            this.toolStripLeft.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Event Handlers
        private void textSearchbox_GotFocus(object sender, EventArgs e)
        {
            this.OnGotFocus(e);
        }

        private void ToolbarToolbar_ExplorerAttached(object sender, EventArgs e)
        {
            //Code here, if you want to initialize something as soon as the explorer get attached.
            cmbPrefectures.SelectedIndexChanged += new EventHandler(cmbPrefectures_SelectedIndexChanged);
            cmbLibraries.SelectedIndexChanged += new EventHandler(cmbLibraries_SelectedIndexChanged);
            cmbLibraries.ComboBox.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
            this.btnChangeOrCancel.Click += new System.EventHandler(this.btnChangeOrCancel_Click);
            this.btnSave.Click += new EventHandler(btnSave_Click);
            if (userId != "")
            {
                libron.SaveDataRecieved += new LibronClass.SavedataRecievedEventhandler(libron_SaveDataRecieved);
                libron.GetSaveData(userId);
            }
            //this.Explorer.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(Explorer_DocumentComplete);
            this.Explorer.DownloadComplete += new DWebBrowserEvents2_DownloadCompleteEventHandler(Explorer_DownloadComplete);
            tmrError.Tick += new EventHandler(tmrError_Tick);
            libron.CheckLibraryError += new EventHandler(libron_CheckLibraryError);
            libron.SetSaveDataError += new EventHandler(libron_SetSaveDataError);
        }

        void libron_SetSaveDataError(object sender, EventArgs e)
        {
            lblError.Text = "保存に失敗しました";
            lblError.Visible = true;
            tmrError.Start();
        }

        void libron_SaveDataRecieved(LibronClass.GetDataState state)
        {
            try
            {
                if (state == LibronClass.GetDataState.Error)
                {
                    //lblError.Text = "保存内容の取得に失敗しました。";
                    //lblError.Visible = true;
                    //tmrError.Start();
                }
                lblDescription.Text = "[" + libron.selectedPrefecture + "]" + libron.selectedSystemName + "で検索 ";
                lblDescription.Visible = true;
                libron.DownloadedLibrayNames += new LibronClass.DownloadedLibraryNamesEventHandler(libron_DownloadedLibrayNames);
                libron.DownloadLibraryNames();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void libron_DownloadedLibrayNames(List<Hashtable> libraryNames)
        {
            if (libraryNames == null)
            {
                lblError.Text = "*図書館リストの取得に失敗しました";
                lblError.Visible = true;
                tmrError.Start();
                return;
            }
            List<fixedLibraryItemData> fixedLibraryNames = new List<fixedLibraryItemData>();
            string groupName = null;
            float maxWidth = 0;
            fixedLibraryItemData selectedfixedLibrariItem = null;
            using (Graphics g = this.CreateGraphics())
            {
                foreach (Hashtable libraryName in libraryNames)
                {
                    maxWidth = Math.Max(maxWidth, g.MeasureString((string)libraryName["systemname"], this.Font).Width);
                    if (groupName == null || groupName != (string)libraryName["group"])
                    {
                        fixedLibraryItemData groupData = new fixedLibraryItemData();
                        groupName = (string)libraryName["group"];
                        groupData.group = groupName;
                        groupData.systemid = "";
                        groupData.systemname = "";
                        fixedLibraryNames.Add(groupData);
                    }
                    fixedLibraryItemData itemData = new fixedLibraryItemData();
                    itemData.group = (string)libraryName["group"];
                    itemData.systemid = (string)libraryName["systemid"];
                    if (itemData.systemid == libron.selectedSystemId)
                    {
                        selectedfixedLibrariItem = itemData;
                    }
                    itemData.systemname = (string)libraryName["systemname"];
                    fixedLibraryNames.Add(itemData);
                }
            }
            libraryNames.Clear();
            cmbPrefectures.Enabled = true;
            cmbLibraries.Width = (int)maxWidth + 20;
            cmbLibraries.Enabled = true;
            cmbLibraries.ComboBox.DataSource = fixedLibraryNames;
            cmbLibraries.ComboBox.DisplayMember = "systemname";
            cmbLibraries.ComboBox.ValueMember = "systemid";
            if (selectedfixedLibrariItem == null)
            {
                cmbLibraries.SelectedIndex = 1;
            }
            else
            {
                cmbLibraries.SelectedIndex = fixedLibraryNames.IndexOf(selectedfixedLibrariItem);
            }
            cmbLibraries.ComboBox.Tag = cmbLibraries.SelectedIndex;
            if (lblLoading.Visible)
            {
                lblLoading.Visible = false;
                cmbLibraries.Visible = true;
                btnSave.Visible = true;
            }
            lblLoading.Visible = false;
            btnSave.Enabled = true;
            btnChangeOrCancel.ForeColor = Color.Black;
            btnChangeOrCancel.Enabled = true;
        }

        void libron_CheckLibraryError(object sender, EventArgs e)
        {
            lblError.Text = "*本の検索に失敗しました";
            lblError.Visible = true;
            tmrError.Start();
        }

        void tmrError_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("Tick");
            lblError.Visible = false;
            tmrError.Stop();
        }

        private void Explorer_DownloadComplete()
        {

            HTMLDocument doc = this.Explorer.Document as HTMLDocument;

            if (doc != null)
            {
                IHTMLWindow2 tmpWindow = doc.parentWindow;
                if (tmpWindow != null)
                {
                    HTMLWindowEvents2_Event events = (tmpWindow as HTMLWindowEvents2_Event);
                    try
                    {
                        events.onload -= new HTMLWindowEvents2_onloadEventHandler(RefreshHandler);
                    }
                    catch { }
                    events.onload += new HTMLWindowEvents2_onloadEventHandler(RefreshHandler);
                }
            }
        }

        public void RefreshHandler(IHTMLEventObj e)
        {
            libron.document = (HTMLDocument)this.Explorer.Document;
            libron.Explorer = this.Explorer;
            libron.libraryLinky();
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            libron.SetSaveData(userId);
            
            cmbPrefectures.Visible = false;
            cmbLibraries.Visible = false;
            btnSave.Visible = false;
            lblDescription.Text = "[" + libron.selectedPrefecture + "]" + libron.selectedSystemName + "で検索 ";
            lblDescription.Visible = true;
            ((HTMLDocument)this.Explorer.Document).parentWindow.location.reload(true);

            btnChangeOrCancel.Text = "変更";
        }

        void cmbPrefectures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPrefectures.SelectedIndex == -1) return;

            if (libron.selectedPrefecture != (string)cmbPrefectures.SelectedItem)
            {
                cmbPrefectures.Enabled = false;
                cmbLibraries.Enabled = false;
                cmbLibraries.Visible = false;
                lblLoading.Visible = true;
                btnSave.Enabled = false;
                btnSave.Visible = false;
                libron.selectedPrefecture = (string)cmbPrefectures.SelectedItem;
                libron.DownloadLibraryNames();
            }
        }

        void cmbLibraries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLibraries.SelectedIndex == -1) return;

            if (((fixedLibraryItemData)cmbLibraries.SelectedItem).systemid == "")
            {
                cmbLibraries.ComboBox.SelectedIndexChanged -= new EventHandler(cmbLibraries_SelectedIndexChanged);
                cmbLibraries.SelectedIndex = (int)cmbLibraries.ComboBox.Tag;
                cmbLibraries.ComboBox.SelectedIndexChanged -= new EventHandler(cmbLibraries_SelectedIndexChanged);
            }
            cmbLibraries.ComboBox.Tag = cmbLibraries.SelectedIndex;
            fixedLibraryItemData libraryName = (fixedLibraryItemData)cmbLibraries.ComboBox.SelectedItem;
            libron.selectedSystemId = libraryName.systemid;
            libron.selectedSystemName = libraryName.systemname;
        }

        void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            fixedLibraryItemData libraryName = (fixedLibraryItemData)cmb.Items[e.Index];
            e.DrawBackground();
            Graphics g = e.Graphics;
            if (libraryName.systemid == "")
            {
                using (Font groupFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold, GraphicsUnit.Point))
                {
                    if (e.State == DrawItemState.Selected)
                    {
                        g.DrawString(libraryName.group, groupFont, Brushes.White, e.Bounds.X, e.Bounds.Y);
                    }
                    else
                    {
                        g.DrawString(libraryName.group, groupFont, Brushes.Blue, e.Bounds.X, e.Bounds.Y);
                    }
                }
            }
            else
            {
                using (SolidBrush b = new SolidBrush(e.ForeColor))
                {
                    g.DrawString(libraryName.systemname, this.Font, b, e.Bounds.X + 10, e.Bounds.Y);
                }
            }
        }

        private void btnChangeOrCancel_Click(object sender, EventArgs e)
        {
            if (btnChangeOrCancel.Text == "変更")
            {
                cmbPrefectures.Visible = true;
                cmbLibraries.Visible = cmbPrefectures.Enabled;
                btnSave.Visible = true;
                lblDescription.Visible = false;
                lblLoading.Visible = !cmbPrefectures.Enabled;
                btnChangeOrCancel.Text = "キャンセル";
            }
            else
            {
                cmbPrefectures.Visible = false;
                cmbLibraries.Visible = false;
                btnSave.Visible = false;
                lblLoading.Visible = false;
                lblDescription.Visible = true;
                btnChangeOrCancel.Text = "変更";
            }
        }

        #endregion

        #region Helper Methods
        //private void NavigateUrl(string url)
        //{
        //    object nullObj = null;
        //    object flags = BrowserNavConstants.navOpenInNewWindow;
        //    this.Explorer.Navigate(url, ref flags, ref nullObj, ref nullObj, ref nullObj);
        //}

        private string PrepareSearchURL(string url, string query)
        {
            try
            {
                query = query.Replace(" ", "+");
                url = url.Replace("$utf8query", query);
            }
            catch 
            {
                //log.Error("Exception occured while replacing the query.", ex);
            }
            return url;
        }
        #endregion

        private void tsShowErrorLog_Click(object sender, EventArgs e)
        {
            frmErrorLog frm = new frmErrorLog(libron.tblErrorLog);
            frm.Show();

        }



    }
}
