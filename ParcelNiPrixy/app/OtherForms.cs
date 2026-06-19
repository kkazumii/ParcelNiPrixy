using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ParcelTrackingSystem
{
    // ══════════════════════════════════════════════════════
    // RIDERS FORM
    // ══════════════════════════════════════════════════════
    public class RidersForm : BaseForm
    {
        private DataGridView dgv;
        private TextBox txSearch;
        
        public RidersForm()
        {
            this.Text = "ParcelTrack – Riders";
            Build();
            Load();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("🚴  Riders"));

            txSearch = MakeTxt(0, 46, 260);
            txSearch.Text = "Search riders...";
            txSearch.TextChanged += (s, e) => Filter();

            var btnAdd    = MakeBtn("➕ Add",    Color.FromArgb(166, 227, 161), 600, 44);
            var btnEdit   = MakeBtn("✏  Edit",   Color.FromArgb(250, 179, 135), 698, 44);
            var btnDelete = MakeBtn("🗑  Delete", Color.FromArgb(243, 139, 168), 796, 44);
            var btnRef    = MakeBtn("🔄",         Color.FromArgb(49,  50,  68),  894, 44, 36);

            btnAdd.Click    += (s, e) => { using var d = new RiderDialog(); if (d.ShowDialog() == DialogResult.OK) Load(); };
            btnEdit.Click   += (s, e) => Edit();
            btnDelete.Click += (s, e) => Delete();
            btnRef.Click    += (s, e) => Load();

            pnlContent.Controls.AddRange(new Control[] { txSearch, btnAdd, btnEdit, btnDelete, btnRef });

            dgv = MakeGrid(86);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
        }

        private void Load()
        {
            try { dgv.DataSource = DatabaseHelper.ExecuteProcedureTable("sp_get_all_riders"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Filter()
        {
            if (dgv.DataSource is DataTable dt)
            {
                string q = txSearch.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                    $"full_name LIKE '%{q}%' OR phone_number LIKE '%{q}%' OR license_plate LIKE '%{q}%'";
            }
        }

        private void Edit()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a rider first."); return; }
            string id = dgv.CurrentRow.Cells["rider_id"].Value?.ToString();
            using var d = new RiderDialog(id);
            if (d.ShowDialog() == DialogResult.OK) Load();
        }

        private void Delete()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a rider first."); return; }
            string id = dgv.CurrentRow.Cells["rider_id"].Value?.ToString();
            if (MessageBox.Show($"Delete rider {id}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteProcedureNonQuery("sp_delete_rider", new MySqlParameter("p_id", id));
                    Load();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }
    }

    public class RiderDialog : Form
    {
        private readonly string _id;
        private readonly bool   _isEdit;
        private TextBox  txtId, txtName, txtPhone, txtEmail, txtLicense, txtVehicle;
        private ComboBox cmbStatus;

        public RiderDialog(string id = null)
        {
            _id = id; _isEdit = id != null;
            this.Text = _isEdit ? "Edit Rider" : "Add Rider";
            this.Size = new Size(460, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 18;
            L("Rider ID",      y); txtId      = T(160, y); y += 36;
            L("Full Name",     y); txtName    = T(160, y); y += 36;
            L("Phone",         y); txtPhone   = T(160, y); y += 36;
            L("Email",         y); txtEmail   = T(160, y); y += 36;
            L("License Plate", y); txtLicense = T(160, y); y += 36;
            L("Vehicle Type",  y); txtVehicle = T(160, y); y += 36;
            L("Status",        y);
            cmbStatus = new ComboBox { Location=new Point(160,y), Size=new Size(180,25),
                BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White,
                FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList,
                Font=new Font("Segoe UI",9) };
            cmbStatus.Items.AddRange(new[] { "active", "inactive" });
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus); y += 46;

            if (_isEdit) { txtId.ReadOnly = true; txtId.BackColor = Color.FromArgb(24, 24, 37); }

            Btns(y);
            if (_isEdit) LoadData();
        }

        private void LoadData()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_all_riders");
                foreach (DataRow r in dt.Rows)
                    if (r["rider_id"]?.ToString() == _id)
                    {
                        txtId.Text      = r["rider_id"]?.ToString();
                        txtName.Text    = r["full_name"]?.ToString();
                        txtPhone.Text   = r["phone_number"]?.ToString();
                        txtEmail.Text   = r["email"]?.ToString();
                        txtLicense.Text = r["license_plate"]?.ToString();
                        txtVehicle.Text = r["vehicle_type"]?.ToString();
                        cmbStatus.Text  = r["status"]?.ToString();
                        break;
                    }
            }
            catch { }
        }

        private void Save(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("Rider ID and Full Name are required."); return; }
            try
            {
                string proc = _isEdit ? "sp_update_rider" : "sp_create_rider";
                DatabaseHelper.ExecuteProcedureNonQuery(proc,
                    new MySqlParameter("p_id",      txtId.Text.Trim()),
                    new MySqlParameter("p_name",    txtName.Text.Trim()),
                    new MySqlParameter("p_phone",   txtPhone.Text.Trim()),
                    new MySqlParameter("p_email",   txtEmail.Text.Trim()),
                    new MySqlParameter("p_license", txtLicense.Text.Trim()),
                    new MySqlParameter("p_vehicle", txtVehicle.Text.Trim()),
                    new MySqlParameter("p_status",  cmbStatus.SelectedItem));
                MessageBox.Show("Saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void L(string t, int y) => this.Controls.Add(new Label { Text=t, Location=new Point(20,y+3), AutoSize=true, Font=new Font("Segoe UI",9,FontStyle.Bold), ForeColor=Color.FromArgb(166,173,200) });
        private TextBox T(int x, int y) { var tx = new TextBox { Location=new Point(x,y), Size=new Size(260,25), Font=new Font("Segoe UI",9), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White, BorderStyle=BorderStyle.FixedSingle }; this.Controls.Add(tx); return tx; }
        private void Btns(int y)
        {
            var s = new Button { Text="💾 Save", Location=new Point(160,y), Size=new Size(90,30), BackColor=Color.FromArgb(166,227,161), ForeColor=Color.FromArgb(30,30,46), FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand };
            s.FlatAppearance.BorderSize = 0; s.Click += Save;
            var c = new Button { Text="Cancel", Location=new Point(260,y), Size=new Size(80,30), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.FromArgb(205,214,244), FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, DialogResult=DialogResult.Cancel };
            c.FlatAppearance.BorderSize = 0;
            this.Controls.AddRange(new Control[] { s, c });
            this.Height = y + 80;
        }
    }

    // ══════════════════════════════════════════════════════
    // SENDERS FORM
    // ══════════════════════════════════════════════════════
    public class SendersForm : BaseForm
    {
        private DataGridView dgv;
        private TextBox txSearch;

        public SendersForm()
        {
            this.Text = "ParcelTrack – Senders";
            Build();
            Load();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("👤  Senders"));

            txSearch = MakeTxt(0, 46, 260);
            txSearch.Text = "Search senders...";
            txSearch.TextChanged += (s, e) => Filter();

            var btnAdd    = MakeBtn("➕ Add",    Color.FromArgb(166, 227, 161), 600, 44);
            var btnEdit   = MakeBtn("✏  Edit",   Color.FromArgb(250, 179, 135), 698, 44);
            var btnDelete = MakeBtn("🗑  Delete", Color.FromArgb(243, 139, 168), 796, 44);
            var btnRef    = MakeBtn("🔄",         Color.FromArgb(49,  50,  68),  894, 44, 36);

            btnAdd.Click    += (s, e) => { using var d = new PersonDialog("Sender"); if (d.ShowDialog() == DialogResult.OK) Load(); };
            btnEdit.Click   += (s, e) => Edit();
            btnDelete.Click += (s, e) => Delete();
            btnRef.Click    += (s, e) => Load();

            pnlContent.Controls.AddRange(new Control[] { txSearch, btnAdd, btnEdit, btnDelete, btnRef });

            dgv = MakeGrid(86);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
        }

        private void Load()
        {
            try { dgv.DataSource = DatabaseHelper.ExecuteProcedureTable("sp_get_all_senders"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Filter()
        {
            if (dgv.DataSource is DataTable dt)
            {
                string q = txSearch.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                    $"full_name LIKE '%{q}%' OR phone_number LIKE '%{q}%' OR email LIKE '%{q}%'";
            }
        }

        private void Edit()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a sender first."); return; }
            string id = dgv.CurrentRow.Cells["sender_id"].Value?.ToString();
            using var d = new PersonDialog("Sender", id);
            if (d.ShowDialog() == DialogResult.OK) Load();
        }

        private void Delete()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a sender first."); return; }
            string id = dgv.CurrentRow.Cells["sender_id"].Value?.ToString();
            if (MessageBox.Show($"Delete sender {id}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { DatabaseHelper.ExecuteProcedureNonQuery("sp_delete_sender", new MySqlParameter("p_id", id)); Load(); }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }
    }

    // ══════════════════════════════════════════════════════
    // RECIPIENTS FORM
    // ══════════════════════════════════════════════════════
    public class RecipientsForm : BaseForm
    {
        private DataGridView dgv;
        private TextBox txSearch;

        public RecipientsForm()
        {
            this.Text = "ParcelTrack – Recipients";
            Build();
            Load();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("📬  Recipients"));

            txSearch = MakeTxt(0, 46, 260);
            txSearch.Text = "Search recipients...";
            txSearch.TextChanged += (s, e) => Filter();

            var btnAdd    = MakeBtn("➕ Add",    Color.FromArgb(166, 227, 161), 600, 44);
            var btnEdit   = MakeBtn("✏  Edit",   Color.FromArgb(250, 179, 135), 698, 44);
            var btnDelete = MakeBtn("🗑  Delete", Color.FromArgb(243, 139, 168), 796, 44);
            var btnRef    = MakeBtn("🔄",         Color.FromArgb(49,  50,  68),  894, 44, 36);

            btnAdd.Click    += (s, e) => { using var d = new PersonDialog("Recipient"); if (d.ShowDialog() == DialogResult.OK) Load(); };
            btnEdit.Click   += (s, e) => Edit();
            btnDelete.Click += (s, e) => Delete();
            btnRef.Click    += (s, e) => Load();

            pnlContent.Controls.AddRange(new Control[] { txSearch, btnAdd, btnEdit, btnDelete, btnRef });

            dgv = MakeGrid(86);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 116);
        }

        private void Load()
        {
            try { dgv.DataSource = DatabaseHelper.ExecuteProcedureTable("sp_get_all_recipients"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Filter()
        {
            if (dgv.DataSource is DataTable dt)
            {
                string q = txSearch.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                    $"full_name LIKE '%{q}%' OR phone_number LIKE '%{q}%' OR email LIKE '%{q}%'";
            }
        }

        private void Edit()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a recipient first."); return; }
            string id = dgv.CurrentRow.Cells["recipient_id"].Value?.ToString();
            using var d = new PersonDialog("Recipient", id);
            if (d.ShowDialog() == DialogResult.OK) Load();
        }

        private void Delete()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a recipient first."); return; }
            string id = dgv.CurrentRow.Cells["recipient_id"].Value?.ToString();
            if (MessageBox.Show($"Delete recipient {id}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { DatabaseHelper.ExecuteProcedureNonQuery("sp_delete_recipient", new MySqlParameter("p_id", id)); Load(); }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }
    }

    // ══════════════════════════════════════════════════════
    // SHARED: PersonDialog for Sender + Recipient
    // ══════════════════════════════════════════════════════
    public class PersonDialog : Form
    {
        private readonly string _entity; // "Sender" or "Recipient"
        private readonly string _id;
        private readonly bool   _isEdit;
        private TextBox txtId, txtName, txtPhone, txtEmail, txtAddress;

        public PersonDialog(string entity, string id = null)
        {
            _entity = entity; _id = id; _isEdit = id != null;
            this.Text = (_isEdit ? "Edit " : "Add ") + entity;
            this.Size = new Size(480, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 18;
            L($"{entity} ID", y); txtId      = T(160, y); y += 36;
            L("Full Name",    y); txtName    = T(160, y); y += 36;
            L("Phone",        y); txtPhone   = T(160, y); y += 36;
            L("Email",        y); txtEmail   = T(160, y); y += 36;
            L("Address",      y); txtAddress = T(160, y, 280); y += 46;

            if (_isEdit) { txtId.ReadOnly = true; txtId.BackColor = Color.FromArgb(24, 24, 37); }

            var btnS = new Button { Text="💾 Save", Location=new Point(160,y), Size=new Size(90,30), BackColor=Color.FromArgb(166,227,161), ForeColor=Color.FromArgb(30,30,46), FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += Save;
            var btnC = new Button { Text="Cancel", Location=new Point(260,y), Size=new Size(80,30), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.FromArgb(205,214,244), FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, DialogResult=DialogResult.Cancel };
            btnC.FlatAppearance.BorderSize = 0;
            this.Controls.AddRange(new Control[] { btnS, btnC });
            this.Height = y + 80;

            if (_isEdit) LoadData();
        }

        private void LoadData()
        {
            string proc = _entity == "Sender" ? "sp_get_sender_by_id" : "sp_get_recipient_by_id";
            string idParam = _entity == "Sender" ? "p_id" : "p_id";
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable(proc, new MySqlParameter(idParam, _id));
                if (dt.Rows.Count > 0)
                {
                    var r = dt.Rows[0];
                    string idCol = _entity == "Sender" ? "sender_id" : "recipient_id";
                    txtId.Text      = r[idCol]?.ToString();
                    txtName.Text    = r["full_name"]?.ToString();
                    txtPhone.Text   = r["phone_number"]?.ToString();
                    txtEmail.Text   = r["email"]?.ToString();
                    txtAddress.Text = r["address"]?.ToString();
                }
            }
            catch { }
        }

        private void Save(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            { MessageBox.Show("ID and Full Name are required."); return; }
            try
            {
                string proc = _entity == "Sender"
                    ? (_isEdit ? "sp_update_sender"    : "sp_create_sender")
                    : (_isEdit ? "sp_update_recipient" : "sp_create_recipient");
                DatabaseHelper.ExecuteProcedureNonQuery(proc,
                    new MySqlParameter("p_id",      txtId.Text.Trim()),
                    new MySqlParameter("p_name",    txtName.Text.Trim()),
                    new MySqlParameter("p_phone",   txtPhone.Text.Trim()),
                    new MySqlParameter("p_email",   txtEmail.Text.Trim()),
                    new MySqlParameter("p_address", txtAddress.Text.Trim()));
                MessageBox.Show("Saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void L(string t, int y) => this.Controls.Add(new Label { Text=t, Location=new Point(20,y+3), AutoSize=true, Font=new Font("Segoe UI",9,FontStyle.Bold), ForeColor=Color.FromArgb(166,173,200) });
        private TextBox T(int x, int y, int w = 260) { var tx = new TextBox { Location=new Point(x,y), Size=new Size(w,25), Font=new Font("Segoe UI",9), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White, BorderStyle=BorderStyle.FixedSingle }; this.Controls.Add(tx); return tx; }
    }

    // ══════════════════════════════════════════════════════
    // PAYMENTS FORM
    // ══════════════════════════════════════════════════════
    public class PaymentsForm : BaseForm
    {
        private DataGridView dgv;
        private TextBox txSearch;

        public PaymentsForm()
        {
            this.Text = "ParcelTrack – Payments";
            Build();
            Load();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("💳  Payments"));

            txSearch = MakeTxt(0, 46, 260);
            txSearch.Text = "Search by parcel, sender, recipient...";
            txSearch.TextChanged += (s, e) => Filter();

            var btnEdit = MakeBtn("✏  Edit Status", Color.FromArgb(250, 179, 135), 600, 44, 115);
            var btnRef  = MakeBtn("🔄",              Color.FromArgb(49,  50,  68),  724, 44, 36);

            btnEdit.Click += (s, e) => Edit();
            btnRef.Click  += (s, e) => Load();

            pnlContent.Controls.AddRange(new Control[] { txSearch, btnEdit, btnRef });

            var note = MakeLbl("ℹ️  Payments are created automatically when a parcel is added.", 0, 84);
            note.ForeColor = Color.FromArgb(137, 180, 250);
            pnlContent.Controls.Add(note);

            dgv = MakeGrid(110);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 140);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 140);
        }

        private void Load()
        {
            try { dgv.DataSource = DatabaseHelper.ExecuteProcedureTable("sp_get_all_payments"); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Filter()
        {
            if (dgv.DataSource is DataTable dt)
            {
                string q = txSearch.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                    $"parcel_id LIKE '%{q}%' OR sender_name LIKE '%{q}%' OR recipient_name LIKE '%{q}%' OR payment_status LIKE '%{q}%'";
            }
        }

        private void Edit()
        {
            if (dgv.CurrentRow == null) { MessageBox.Show("Select a payment first."); return; }
            string payId  = dgv.CurrentRow.Cells["payment_id"].Value?.ToString();
            string status = dgv.CurrentRow.Cells["payment_status"].Value?.ToString();
            string method = dgv.CurrentRow.Cells["method"].Value?.ToString();
            string amount = dgv.CurrentRow.Cells["amount"].Value?.ToString();
            using var d = new PaymentDialog(payId, status, method, amount);
            if (d.ShowDialog() == DialogResult.OK) Load();
        }
    }

    public class PaymentDialog : Form
    {
        private readonly string _payId;
        private ComboBox     cmbStatus, cmbMethod;
        private TextBox      txtAmount;
        private DateTimePicker dtp;

        public PaymentDialog(string payId, string currentStatus, string currentMethod, string currentAmount)
        {
            _payId = payId;
            this.Text = "Update Payment";
            this.Size = new Size(400, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 18;
            L("Payment ID", y);
            this.Controls.Add(new Label { Text=payId, Font=new Font("Segoe UI",9,FontStyle.Bold), ForeColor=Color.FromArgb(137,180,250), AutoSize=true, Location=new Point(155,y+3) });
            y += 36;

            L("Amount (₱)", y);
            txtAmount = new TextBox { Location=new Point(155,y), Size=new Size(150,25), Font=new Font("Segoe UI",9), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White, BorderStyle=BorderStyle.FixedSingle, Text=currentAmount };
            this.Controls.Add(txtAmount); y += 36;

            L("Method", y);
            cmbMethod = C(155, y);
            cmbMethod.Items.AddRange(new[]{"Cash on Delivery","GCash","PayPal","Credit Card","Bank Transfer"});
            cmbMethod.Text = currentMethod; y += 36;

            L("Status", y);
            cmbStatus = C(155, y);
            cmbStatus.Items.AddRange(new[] { "pending", "paid", "failed" });
            cmbStatus.Text = currentStatus; y += 36;

            L("Payment Date", y);
            dtp = new DateTimePicker { Location=new Point(155,y), Size=new Size(180,25), Format=DateTimePickerFormat.Short };
            this.Controls.Add(dtp); y += 46;

            var btnS = new Button { Text="💾 Save", Location=new Point(155,y), Size=new Size(90,30), BackColor=Color.FromArgb(166,227,161), ForeColor=Color.FromArgb(30,30,46), FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand };
            btnS.FlatAppearance.BorderSize = 0; btnS.Click += Save;
            var btnC = new Button { Text="Cancel", Location=new Point(255,y), Size=new Size(80,30), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.FromArgb(205,214,244), FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, DialogResult=DialogResult.Cancel };
            btnC.FlatAppearance.BorderSize = 0;
            this.Controls.AddRange(new Control[] { btnS, btnC });
            this.Height = y + 80;
        }

        private void Save(object s, EventArgs e)
        {
            try
            {
                DatabaseHelper.ExecuteProcedureNonQuery("sp_update_payment",
                    new MySqlParameter("p_payment_id", _payId),
                    new MySqlParameter("p_amount",     decimal.TryParse(txtAmount.Text, out var a) ? a : 0m),
                    new MySqlParameter("p_method",     cmbMethod.SelectedItem ?? cmbMethod.Text),
                    new MySqlParameter("p_status",     cmbStatus.SelectedItem ?? cmbStatus.Text),
                    new MySqlParameter("p_date",       dtp.Value.Date));
                MessageBox.Show("Payment updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void L(string t, int y) => this.Controls.Add(new Label { Text=t, Location=new Point(20,y+3), AutoSize=true, Font=new Font("Segoe UI",9,FontStyle.Bold), ForeColor=Color.FromArgb(166,173,200) });
        private ComboBox C(int x, int y) { var c = new ComboBox { Location=new Point(x,y), Size=new Size(200,25), BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White, FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList, Font=new Font("Segoe UI",9) }; this.Controls.Add(c); return c; }
    }

    // ══════════════════════════════════════════════════════
    // AUDIT LOG FORM
    // ══════════════════════════════════════════════════════
    public class AuditLogForm : BaseForm
    {
        private DataGridView dgv;
        private TextBox txSearch;

        public AuditLogForm()
        {
            this.Text = "ParcelTrack – Audit Log";
            Build();
            Load();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("📋  Audit Log"));

            txSearch = MakeTxt(0, 46, 260);
            txSearch.Text = "Filter by table, action, record...";
            txSearch.TextChanged += (s, e) => Filter();

            var btnRef = MakeBtn("🔄 Refresh", Color.FromArgb(49, 50, 68), 600, 44, 95);
            btnRef.Click += (s, e) => Load();

            var note = MakeLbl("ℹ️  All entries here are auto-generated by database triggers.", 0, 84);
            note.ForeColor = Color.FromArgb(137, 180, 250);

            pnlContent.Controls.AddRange(new Control[] { txSearch, btnRef, note });

            dgv = MakeGrid(110);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 140);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 140);
        }

        private void Load()
        {
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                using var cmd  = new MySqlCommand("SELECT log_id, table_name, action, record_id, change_time, details FROM audit_log ORDER BY change_time DESC", conn);
                using var da   = new MySqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void Filter()
        {
            if (dgv.DataSource is DataTable dt)
            {
                string q = txSearch.Text.Trim().Replace("'", "''");
                dt.DefaultView.RowFilter = string.IsNullOrEmpty(q) ? "" :
                    $"table_name LIKE '%{q}%' OR action LIKE '%{q}%' OR record_id LIKE '%{q}%' OR details LIKE '%{q}%'";
            }
        }
    }
}
