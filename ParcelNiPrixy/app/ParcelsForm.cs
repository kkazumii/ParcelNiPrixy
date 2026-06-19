using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ParcelTrackingSystem
{
    // ══════════════════════════════════════════════════════
    // PARCELS FORM
    // ══════════════════════════════════════════════════════
    public class ParcelsForm : BaseForm
    {
        private DataGridView dgv;
        private TextBox txSearch;

        public ParcelsForm()
        {
            this.Text = "ParcelTrack – Parcels";
            Build();
            LoadData();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("📦  Parcels"));

            txSearch = MakeTxt(0, 46, 280);
            txSearch.Text = "Search parcels...";
            txSearch.GotFocus += (s, e) => { if (txSearch.Text == "Search parcels...") txSearch.Text = ""; };
            txSearch.LostFocus += (s, e) => { if (txSearch.Text == "") txSearch.Text = "Search parcels..."; };
            txSearch.TextChanged += (s, e) => Filter();

            var btnAdd    = MakeBtn("➕ Add",    Color.FromArgb(166, 227, 161), 600, 44);
            var btnEdit   = MakeBtn("✏  Edit",   Color.FromArgb(250, 179, 135), 698, 44);
            var btnDelete = MakeBtn("🗑  Delete", Color.FromArgb(243, 139, 168), 796, 44);
            var btnRef    = MakeBtn("🔄",         Color.FromArgb(49, 50, 68),    894, 44, 36);

            btnAdd.Click    += (s, e) => { using var d = new AddParcelForm(); if (d.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit.Click   += (s, e) => Edit();
            btnDelete.Click += (s, e) => Delete();
            btnRef.Click    += (s, e) => LoadData();

            pnlContent.Controls.AddRange(new Control[] { txSearch, btnAdd, btnEdit, btnDelete, btnRef });

            dgv = MakeGrid(86);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
        }

        private void LoadData()
        {
            try
            {
                dgv.DataSource = DatabaseHelper.ExecuteProcedureTable("sp_get_all_parcels");
                HideCols(dgv, "sender_phone","recipient_phone","recipient_address","payment_id","shipment_id");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Filter()
        {
            if (txSearch.Text == "Search parcels...") return;
            if (dgv.DataSource is DataTable dt)
            {
                string q = txSearch.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                    $"parcel_id LIKE '%{q}%' OR sender_name LIKE '%{q}%' OR recipient_name LIKE '%{q}%' OR current_status LIKE '%{q}%'";
            }
        }

        private void Edit()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a parcel first."); return; }
            string id = dgv.CurrentRow.Cells["parcel_id"].Value?.ToString();
            using var d = new EditParcelForm(id);
            if (d.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void Delete()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a parcel first."); return; }
            string id = dgv.CurrentRow.Cells["parcel_id"].Value?.ToString();
            if (MessageBox.Show($"Delete parcel {id}?\nThis will also remove its shipment, payment and tracking history.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteProcedureNonQuery("sp_delete_parcel",
                        new MySqlParameter("p_parcel_id", id));
                    LoadData();
                    MessageBox.Show("Parcel deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }
    }

    // ══════════════════════════════════════════════════════
    // ADD PARCEL FORM
    // ══════════════════════════════════════════════════════
    public class AddParcelForm : Form
    {
        // Sender
        private TextBox  txtSenderSearch, txtSenderPhone, txtSenderEmail, txtSenderAddress;
        private ListBox  lstSenderSuggest;
        private Label    lblSenderStatus;
        private string   _senderId;
        private bool     _senderIsNew = true;

        // Recipient
        private TextBox  txtRecipSearch, txtRecipPhone, txtRecipEmail, txtRecipAddress;
        private ListBox  lstRecipSuggest;
        private Label    lblRecipStatus;
        private string   _recipientId;
        private bool     _recipientIsNew = true;

        // Route
        private ComboBox cmbOriginProvince, cmbOriginMunicipality;
        private ComboBox cmbDestProvince,   cmbDestMunicipality;

        // Shipment
        private ComboBox       cmbRider;
        private DateTimePicker dtpShip, dtpEst;

        // Parcel
        private TextBox  txtParcelId, txtWeight, txtDesc, txtAmount;
        private ComboBox cmbType, cmbMethod;

        private Panel pnlScroll;

        public AddParcelForm()
        {
            this.Text = "Add New Parcel";
            this.Size = new Size(700, 820);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            pnlScroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 46) };
            this.Controls.Add(pnlScroll);

            int y = 15;

            // ── Parcel Info ──
            Section("📦 Parcel Information", ref y);
            Row("Parcel ID",   ref y); txtParcelId = Txt(200, y-27, 200);
            txtParcelId.Text = "PRC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            pnlScroll.Controls.Add(txtParcelId);

            Row("Type", ref y);
            cmbType = Combo(200, y-27);
            cmbType.Items.AddRange(new[] { "package","document","fragile" });
            cmbType.SelectedIndex = 0; pnlScroll.Controls.Add(cmbType);

            Row("Weight (kg)",  ref y); txtWeight = Txt(200, y-27, 120); pnlScroll.Controls.Add(txtWeight);
            Row("Description",  ref y); txtDesc   = Txt(200, y-27, 420); pnlScroll.Controls.Add(txtDesc);
            y += 8;

            // ── Sender ──
            Section("👤 Sender", ref y);
            Row("Search / Name", ref y);
            txtSenderSearch = Txt(200, y-27, 300);
            txtSenderSearch.TextChanged += SenderSearch_Changed;
            pnlScroll.Controls.Add(txtSenderSearch);

            lstSenderSuggest = new ListBox { Location=new Point(200, y), Size=new Size(300, 0),
                BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White,
                Font=new Font("Segoe UI",9), Visible=false, BorderStyle=BorderStyle.FixedSingle };
            lstSenderSuggest.Click += SenderSuggest_Click;
            pnlScroll.Controls.Add(lstSenderSuggest);
            lstSenderSuggest.BringToFront();

            lblSenderStatus = StatusLbl(200, y); pnlScroll.Controls.Add(lblSenderStatus); y += 20;

            Row("Phone",   ref y); txtSenderPhone   = Txt(200, y-27, 200); pnlScroll.Controls.Add(txtSenderPhone);
            Row("Email",   ref y); txtSenderEmail   = Txt(200, y-27, 280); pnlScroll.Controls.Add(txtSenderEmail);
            Row("Address", ref y); txtSenderAddress = Txt(200, y-27, 420); pnlScroll.Controls.Add(txtSenderAddress);
            y += 8;

            // ── Recipient ──
            Section("📬 Recipient", ref y);
            Row("Search / Name", ref y);
            txtRecipSearch = Txt(200, y-27, 300);
            txtRecipSearch.TextChanged += RecipSearch_Changed;
            pnlScroll.Controls.Add(txtRecipSearch);

            lstRecipSuggest = new ListBox { Location=new Point(200, y), Size=new Size(300, 0),
                BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White,
                Font=new Font("Segoe UI",9), Visible=false, BorderStyle=BorderStyle.FixedSingle };
            lstRecipSuggest.Click += RecipSuggest_Click;
            pnlScroll.Controls.Add(lstRecipSuggest);
            lstRecipSuggest.BringToFront();

            lblRecipStatus = StatusLbl(200, y); pnlScroll.Controls.Add(lblRecipStatus); y += 20;

            Row("Phone",   ref y); txtRecipPhone   = Txt(200, y-27, 200); pnlScroll.Controls.Add(txtRecipPhone);
            Row("Email",   ref y); txtRecipEmail   = Txt(200, y-27, 280); pnlScroll.Controls.Add(txtRecipEmail);
            Row("Address", ref y); txtRecipAddress = Txt(200, y-27, 420); pnlScroll.Controls.Add(txtRecipAddress);
            y += 8;

            // ── Route ──
            Section("🗺  Route", ref y);
            Row("Origin Province", ref y);
            cmbOriginProvince = Combo(200, y-27, 280);
            cmbOriginProvince.SelectedIndexChanged += (s,e) => {
                if (cmbOriginProvince.SelectedValue != null)
                    LoadMunicipalities(cmbOriginMunicipality, cmbOriginProvince.SelectedValue.ToString());
            };
            pnlScroll.Controls.Add(cmbOriginProvince);

            Row("Origin Municipality", ref y);
            cmbOriginMunicipality = Combo(200, y-27, 280); pnlScroll.Controls.Add(cmbOriginMunicipality);

            Row("Dest. Province", ref y);
            cmbDestProvince = Combo(200, y-27, 280);
            cmbDestProvince.SelectedIndexChanged += (s,e) => {
                if (cmbDestProvince.SelectedValue != null)
                    LoadMunicipalities(cmbDestMunicipality, cmbDestProvince.SelectedValue.ToString());
            };
            pnlScroll.Controls.Add(cmbDestProvince);

            Row("Dest. Municipality", ref y);
            cmbDestMunicipality = Combo(200, y-27, 280); pnlScroll.Controls.Add(cmbDestMunicipality);
            y += 8;

            // ── Shipment ──
            Section("🚚 Rider & Schedule", ref y);
            Row("Rider", ref y);
            cmbRider = Combo(200, y-27, 280); pnlScroll.Controls.Add(cmbRider);

            Row("Ship Date", ref y);
            dtpShip = new DateTimePicker { Location=new Point(200,y-27), Size=new Size(200,27),
                Format=DateTimePickerFormat.Short };
            dtpShip.Value = DateTime.Today;
            dtpShip.ValueChanged += (s,e) => dtpEst.Value = dtpShip.Value.AddDays(3);
            pnlScroll.Controls.Add(dtpShip);

            Row("Est. Delivery", ref y);
            dtpEst = new DateTimePicker { Location=new Point(200,y-27), Size=new Size(200,27),
                Format=DateTimePickerFormat.Short };
            dtpEst.Value = DateTime.Today.AddDays(3);
            pnlScroll.Controls.Add(dtpEst);
            y += 8;

            // ── Payment ──
            Section("💳 Payment", ref y);
            Row("Amount (₱)", ref y); txtAmount = Txt(200, y-27, 150); txtAmount.Text="0.00"; pnlScroll.Controls.Add(txtAmount);
            Row("Method", ref y);
            cmbMethod = Combo(200, y-27, 240);
            cmbMethod.Items.AddRange(new[]{"Cash on Delivery","GCash","PayPal","Credit Card","Bank Transfer"});
            cmbMethod.SelectedIndex = 0; pnlScroll.Controls.Add(cmbMethod);
            y += 16;

            // ── Buttons ──
            var btnSave = new Button { Text="💾 Save Parcel", Location=new Point(200,y),
                Size=new Size(150,36), Font=new Font("Segoe UI",10,FontStyle.Bold),
                BackColor=Color.FromArgb(166,227,161), ForeColor=Color.FromArgb(30,30,46),
                FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand };
            btnSave.FlatAppearance.BorderSize=0;
            btnSave.Click += Save_Click;
            pnlScroll.Controls.Add(btnSave);

            var btnCancel = new Button { Text="Cancel", Location=new Point(360,y),
                Size=new Size(90,36), Font=new Font("Segoe UI",10),
                BackColor=Color.FromArgb(49,50,68), ForeColor=Color.FromArgb(205,214,244),
                FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, DialogResult=DialogResult.Cancel };
            btnCancel.FlatAppearance.BorderSize=0;
            pnlScroll.Controls.Add(btnCancel);

            this.Load += (s,e) => { LoadProvinces(); LoadRiders(); };
        }

        // ── Data loaders ──────────────────────────────────────

        private void LoadProvinces()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_provinces");
                cmbOriginProvince.DisplayMember = "province_name";
                cmbOriginProvince.ValueMember   = "province_id";
                cmbOriginProvince.DataSource    = dt.Copy();
                cmbDestProvince.DisplayMember   = "province_name";
                cmbDestProvince.ValueMember     = "province_id";
                cmbDestProvince.DataSource      = dt.Copy();
            }
            catch (Exception ex) { MessageBox.Show("Error loading provinces: " + ex.Message); }
        }

        private void LoadMunicipalities(ComboBox cmb, string provinceId)
        {
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_municipalities",
                    new MySqlParameter("p_province_id", provinceId));
                cmb.DisplayMember = "municipality_name";
                cmb.ValueMember   = "municipality_id";
                cmb.DataSource    = dt;
            }
            catch { }
        }

        private void LoadRiders()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_active_riders");
                cmbRider.DisplayMember = "full_name";
                cmbRider.ValueMember   = "rider_id";
                cmbRider.DataSource    = dt;
            }
            catch { }
        }

        // ── Sender autocomplete ───────────────────────────────

        private void SenderSearch_Changed(object sender, EventArgs e)
        {
            string q = txtSenderSearch.Text.Trim();
            _senderIsNew = true;
            lblSenderStatus.Text = "🆕 New sender will be created";
            lblSenderStatus.ForeColor = Color.FromArgb(250, 179, 135);
            ClearFields(txtSenderPhone, txtSenderEmail, txtSenderAddress);

            if (q.Length < 2) { lstSenderSuggest.Visible=false; lstSenderSuggest.Height=0; return; }
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_search_senders",
                    new MySqlParameter("p_query", q));
                lstSenderSuggest.Items.Clear();
                foreach (DataRow r in dt.Rows) lstSenderSuggest.Items.Add(new PersonItem(r,"sender_id"));
                if (lstSenderSuggest.Items.Count > 0)
                {
                    lstSenderSuggest.Height  = Math.Min(lstSenderSuggest.Items.Count * 22, 110);
                    lstSenderSuggest.Visible = true;
                    lstSenderSuggest.BringToFront();
                }
                else { lstSenderSuggest.Visible=false; lstSenderSuggest.Height=0; }
            }
            catch { }
        }

        private void SenderSuggest_Click(object sender, EventArgs e)
        {
            if (lstSenderSuggest.SelectedItem is PersonItem item)
            {
                _senderId = item.Id; _senderIsNew = false;
                txtSenderSearch.Text  = item.Row["full_name"]?.ToString();
                txtSenderPhone.Text   = item.Row["phone_number"]?.ToString();
                txtSenderEmail.Text   = item.Row["email"]?.ToString();
                txtSenderAddress.Text = item.Row["address"]?.ToString();
                SetReadOnly(true, txtSenderPhone, txtSenderEmail, txtSenderAddress);
                lstSenderSuggest.Visible=false; lstSenderSuggest.Height=0;
                lblSenderStatus.Text = "✅ Existing sender selected";
                lblSenderStatus.ForeColor = Color.FromArgb(166, 227, 161);
            }
        }

        // ── Recipient autocomplete ────────────────────────────

        private void RecipSearch_Changed(object sender, EventArgs e)
        {
            string q = txtRecipSearch.Text.Trim();
            _recipientIsNew = true;
            lblRecipStatus.Text = "🆕 New recipient will be created";
            lblRecipStatus.ForeColor = Color.FromArgb(250, 179, 135);
            ClearFields(txtRecipPhone, txtRecipEmail, txtRecipAddress);

            if (q.Length < 2) { lstRecipSuggest.Visible=false; lstRecipSuggest.Height=0; return; }
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_search_recipients",
                    new MySqlParameter("p_query", q));
                lstRecipSuggest.Items.Clear();
                foreach (DataRow r in dt.Rows) lstRecipSuggest.Items.Add(new PersonItem(r,"recipient_id"));
                if (lstRecipSuggest.Items.Count > 0)
                {
                    lstRecipSuggest.Height  = Math.Min(lstRecipSuggest.Items.Count * 22, 110);
                    lstRecipSuggest.Visible = true;
                    lstRecipSuggest.BringToFront();
                }
                else { lstRecipSuggest.Visible=false; lstRecipSuggest.Height=0; }
            }
            catch { }
        }

        private void RecipSuggest_Click(object sender, EventArgs e)
        {
            if (lstRecipSuggest.SelectedItem is PersonItem item)
            {
                _recipientId = item.Id; _recipientIsNew = false;
                txtRecipSearch.Text  = item.Row["full_name"]?.ToString();
                txtRecipPhone.Text   = item.Row["phone_number"]?.ToString();
                txtRecipEmail.Text   = item.Row["email"]?.ToString();
                txtRecipAddress.Text = item.Row["address"]?.ToString();
                SetReadOnly(true, txtRecipPhone, txtRecipEmail, txtRecipAddress);
                lstRecipSuggest.Visible=false; lstRecipSuggest.Height=0;
                lblRecipStatus.Text = "✅ Existing recipient selected";
                lblRecipStatus.ForeColor = Color.FromArgb(166, 227, 161);
            }
        }

        // ── Save ──────────────────────────────────────────────

        private void Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSenderSearch.Text))
            { MessageBox.Show("Please enter a sender name."); return; }
            if (string.IsNullOrWhiteSpace(txtRecipSearch.Text))
            { MessageBox.Show("Please enter a recipient name."); return; }
            if (cmbOriginProvince.SelectedValue==null || cmbOriginMunicipality.SelectedItem==null)
            { MessageBox.Show("Please select origin province and municipality."); return; }
            if (cmbDestProvince.SelectedValue==null || cmbDestMunicipality.SelectedItem==null)
            { MessageBox.Show("Please select destination province and municipality."); return; }
            if (cmbRider.SelectedValue==null)
            { MessageBox.Show("Please select a rider."); return; }
            if (dtpEst.Value.Date < dtpShip.Value.Date)
            { MessageBox.Show("Estimated delivery cannot be before ship date."); return; }

            if (_senderIsNew)    _senderId    = "S"  + DateTime.Now.ToString("yyyyMMddHHmmss");
            if (_recipientIsNew) _recipientId = "R"  + DateTime.Now.ToString("yyyyMMddHHmmss");

            string originMun  = (cmbOriginMunicipality.SelectedItem as DataRowView)?["municipality_name"]?.ToString() ?? "";
            string destMun    = (cmbDestMunicipality.SelectedItem   as DataRowView)?["municipality_name"]?.ToString() ?? "";
            string originProv = (cmbOriginProvince.SelectedItem     as DataRowView)?["province_name"]?.ToString()     ?? "";
            string destProv   = (cmbDestProvince.SelectedItem       as DataRowView)?["province_name"]?.ToString()     ?? "";

            try
            {
                var pShipId  = new MySqlParameter("p_shipment_id", MySqlDbType.VarChar, 20) { Direction = System.Data.ParameterDirection.Output };
                var pRouteId = new MySqlParameter("p_route_id",    MySqlDbType.VarChar, 20) { Direction = System.Data.ParameterDirection.Output };

                using var conn = DatabaseHelper.GetConnection();
                DatabaseHelper.ExecuteProcedureWithOutput(conn, "sp_create_parcel_full",
                    new MySqlParameter("p_parcel_id",           txtParcelId.Text.Trim()),
                    new MySqlParameter("p_weight_kg",           decimal.TryParse(txtWeight.Text, out var w) ? w : (object)DBNull.Value),
                    new MySqlParameter("p_description",         txtDesc.Text.Trim()),
                    new MySqlParameter("p_parcel_type",         cmbType.SelectedItem),
                    new MySqlParameter("p_sender_id",           _senderId),
                    new MySqlParameter("p_sender_name",         txtSenderSearch.Text.Trim()),
                    new MySqlParameter("p_sender_phone",        txtSenderPhone.Text.Trim()),
                    new MySqlParameter("p_sender_email",        txtSenderEmail.Text.Trim()),
                    new MySqlParameter("p_sender_address",      txtSenderAddress.Text.Trim()),
                    new MySqlParameter("p_sender_is_new",       _senderIsNew ? 1 : 0),
                    new MySqlParameter("p_recipient_id",        _recipientId),
                    new MySqlParameter("p_recipient_name",      txtRecipSearch.Text.Trim()),
                    new MySqlParameter("p_recipient_phone",     txtRecipPhone.Text.Trim()),
                    new MySqlParameter("p_recipient_email",     txtRecipEmail.Text.Trim()),
                    new MySqlParameter("p_recipient_address",   txtRecipAddress.Text.Trim()),
                    new MySqlParameter("p_recipient_is_new",    _recipientIsNew ? 1 : 0),
                    new MySqlParameter("p_origin_province",     originProv),
                    new MySqlParameter("p_origin_municipality", originMun),
                    new MySqlParameter("p_dest_province",       destProv),
                    new MySqlParameter("p_dest_municipality",   destMun),
                    new MySqlParameter("p_rider_id",            cmbRider.SelectedValue),
                    new MySqlParameter("p_ship_date",           dtpShip.Value.Date),
                    new MySqlParameter("p_est_delivery",        dtpEst.Value.Date),
                    new MySqlParameter("p_amount",              decimal.TryParse(txtAmount.Text, out var a) ? a : 0m),
                    new MySqlParameter("p_method",              cmbMethod.SelectedItem),
                    pShipId, pRouteId);

                MessageBox.Show($"Parcel saved!\nShipment ID: {pShipId.Value}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── UI Helpers ────────────────────────────────────────

        private void Section(string title, ref int y)
        {
            pnlScroll.Controls.Add(new Panel { Location=new Point(0,y), Size=new Size(650,1), BackColor=Color.FromArgb(49,50,68) });
            y += 6;
            pnlScroll.Controls.Add(new Label { Text=title, Location=new Point(0,y), AutoSize=true,
                Font=new Font("Segoe UI",10,FontStyle.Bold), ForeColor=Color.FromArgb(137,180,250) });
            y += 28;
        }

        private void Row(string label, ref int y)
        {
            pnlScroll.Controls.Add(new Label { Text=label, Location=new Point(10,y+3), AutoSize=true,
                Font=new Font("Segoe UI",9,FontStyle.Bold), ForeColor=Color.FromArgb(166,173,200) });
            y += 30;
        }

        private TextBox Txt(int x, int y, int w=260) =>
            new TextBox { Location=new Point(x,y), Size=new Size(w,27),
                Font=new Font("Segoe UI",9), BackColor=Color.FromArgb(49,50,68),
                ForeColor=Color.White, BorderStyle=BorderStyle.FixedSingle };

        private ComboBox Combo(int x, int y, int w=200) =>
            new ComboBox { Location=new Point(x,y), Size=new Size(w,27),
                Font=new Font("Segoe UI",9), BackColor=Color.FromArgb(49,50,68),
                ForeColor=Color.White, FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList };

        private Label StatusLbl(int x, int y) =>
            new Label { Text="", Location=new Point(x,y), AutoSize=true,
                Font=new Font("Segoe UI",8,FontStyle.Italic), ForeColor=Color.FromArgb(166,173,200) };

        private void ClearFields(params TextBox[] boxes)
        {
            foreach (var b in boxes) { b.Text=""; b.ReadOnly=false; b.BackColor=Color.FromArgb(49,50,68); }
        }

        private void SetReadOnly(bool ro, params TextBox[] boxes)
        {
            foreach (var b in boxes) { b.ReadOnly=ro; b.BackColor=ro?Color.FromArgb(24,24,37):Color.FromArgb(49,50,68); }
        }
    }

    // ── Edit parcel dialog ────────────────────────────────────
    public class EditParcelForm : Form
    {
        private TextBox  txtWeight, txtDesc;
        private ComboBox cmbType;
        private readonly string _id;

        public EditParcelForm(string id)
        {
            _id = id;
            this.Text="Edit Parcel"; this.Size=new Size(440,260);
            this.StartPosition=FormStartPosition.CenterParent;
            this.BackColor=Color.FromArgb(30,30,46);
            this.FormBorderStyle=FormBorderStyle.FixedDialog; this.MaximizeBox=false;

            int y=20;
            L("Parcel ID",y); this.Controls.Add(new Label{Text=id,Font=new Font("Segoe UI",9,FontStyle.Bold),ForeColor=Color.FromArgb(137,180,250),AutoSize=true,Location=new Point(160,y+3)});y+=36;
            L("Weight (kg)",y); txtWeight=T(160,y);this.Controls.Add(txtWeight);y+=36;
            L("Description",y); txtDesc=T(160,y,240);this.Controls.Add(txtDesc);y+=36;
            L("Type",y);
            cmbType=new ComboBox{Location=new Point(160,y),Size=new Size(180,25),BackColor=Color.FromArgb(49,50,68),ForeColor=Color.White,FlatStyle=FlatStyle.Flat,DropDownStyle=ComboBoxStyle.DropDownList,Font=new Font("Segoe UI",9)};
            cmbType.Items.AddRange(new[]{"package","document","fragile"});
            this.Controls.Add(cmbType); y+=46;

            var s=new Button{Text="💾 Save",Location=new Point(160,y),Size=new Size(90,30),BackColor=Color.FromArgb(166,227,161),ForeColor=Color.FromArgb(30,30,46),FlatStyle=FlatStyle.Flat,Cursor=Cursors.Hand};
            s.FlatAppearance.BorderSize=0; s.Click+=Save;
            var c=new Button{Text="Cancel",Location=new Point(260,y),Size=new Size(80,30),BackColor=Color.FromArgb(49,50,68),ForeColor=Color.FromArgb(205,214,244),FlatStyle=FlatStyle.Flat,Cursor=Cursors.Hand,DialogResult=DialogResult.Cancel};
            c.FlatAppearance.BorderSize=0;
            this.Controls.AddRange(new Control[]{s,c});
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var dt=DatabaseHelper.ExecuteProcedureTable("sp_get_parcel_by_id",new MySqlParameter("p_id",_id));
                if(dt.Rows.Count>0){var r=dt.Rows[0];txtWeight.Text=r["weight_kg"]?.ToString();txtDesc.Text=r["description"]?.ToString();cmbType.Text=r["parcel_type"]?.ToString();}
            }
            catch{}
        }

        private void Save(object s,EventArgs e)
        {
            try
            {
                DatabaseHelper.ExecuteProcedureNonQuery("sp_update_parcel",
                    new MySqlParameter("p_parcel_id",  _id),
                    new MySqlParameter("p_weight_kg",  decimal.TryParse(txtWeight.Text,out var w)?w:(object)DBNull.Value),
                    new MySqlParameter("p_description",txtDesc.Text.Trim()),
                    new MySqlParameter("p_parcel_type",cmbType.SelectedItem));
                MessageBox.Show("Saved!","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
                this.DialogResult=DialogResult.OK; this.Close();
            }
            catch(Exception ex){MessageBox.Show("Error: "+ex.Message);}
        }

        private void L(string t,int y)=>this.Controls.Add(new Label{Text=t,Location=new Point(20,y+3),AutoSize=true,Font=new Font("Segoe UI",9,FontStyle.Bold),ForeColor=Color.FromArgb(166,173,200)});
        private TextBox T(int x,int y,int w=160)=>new TextBox{Location=new Point(x,y),Size=new Size(w,25),Font=new Font("Segoe UI",9),BackColor=Color.FromArgb(49,50,68),ForeColor=Color.White,BorderStyle=BorderStyle.FixedSingle};
    }

    // ── Helper for autocomplete list items ────────────────────
    class PersonItem
    {
        public DataRow Row;
        public string  Id;
        public PersonItem(DataRow r, string idCol) { Row=r; Id=r[idCol]?.ToString(); }
        public override string ToString() => $"{Row["full_name"]}  |  {Row["phone_number"]}";
    }
}
