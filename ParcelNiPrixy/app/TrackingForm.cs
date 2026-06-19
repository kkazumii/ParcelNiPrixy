using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ParcelTrackingSystem
{
    public class TrackingForm : BaseForm
    {
        private ComboBox     cmbParcel;
        private DataGridView dgvTimeline;
        private Panel        pnlInfo;
        private Label        lblSender, lblRecipient, lblRider, lblRoute, lblCurrentStatus;

        public TrackingForm()
        {
            this.Text = "ParcelTrack – Tracking";
            Build();
            LoadParcelList();
        }

        private void Build()
        {
            pnlContent.Controls.Add(PageTitle("📍  Parcel Tracking"));

            // ── Parcel selector ──
            pnlContent.Controls.Add(MakeLbl("Select Parcel:", 0, 46, true));

            cmbParcel = new ComboBox {
                Location = new Point(110, 43), Size = new Size(320, 27),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(49, 50, 68), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList
            };
            pnlContent.Controls.Add(cmbParcel);

            var btnLoad = MakeBtn("🔍 Load", Color.FromArgb(137, 180, 250), 440, 43, 80);
            btnLoad.Click += (s, e) => LoadTracking();
            pnlContent.Controls.Add(btnLoad);

            var btnUpdate = MakeBtn("➕ Add Update", Color.FromArgb(166, 227, 161), 530, 43, 110);
            btnUpdate.Click += (s, e) => AddUpdate();
            pnlContent.Controls.Add(btnUpdate);

            // ── Info panel ──
            pnlInfo = new Panel {
                Location = new Point(0, 84), Size = new Size(1050, 72),
                BackColor = Color.FromArgb(49, 50, 68)
            };
            lblSender        = InfoLbl("Sender: —",         12, 10);
            lblRecipient     = InfoLbl("Recipient: —",      12, 36);
            lblRider         = InfoLbl("Rider: —",          320, 10);
            lblRoute         = InfoLbl("Route: —",          320, 36);
            lblCurrentStatus = InfoLbl("Status: —",         650, 10);
            pnlInfo.Controls.AddRange(new Control[] { lblSender, lblRecipient, lblRider, lblRoute, lblCurrentStatus });
            pnlContent.Controls.Add(pnlInfo);

            // ── Timeline grid ──
            dgvTimeline = MakeGrid(168);
            dgvTimeline.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 198);
            pnlContent.Controls.Add(dgvTimeline);
            pnlContent.Resize += (s, e) => {
                pnlInfo.Width = pnlContent.Width - 50;
                dgvTimeline.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 198);
            };
        }

        private Label InfoLbl(string text, int x, int y) =>
            new Label { Text = text, Location = new Point(x, y), AutoSize = true,
                Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(205, 214, 244) };

        private void LoadParcelList()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_all_parcels");
                // Build display: parcel_id | sender → recipient
                var display = new DataTable();
                display.Columns.Add("parcel_id");
                display.Columns.Add("display");
                foreach (DataRow r in dt.Rows)
                {
                    var row = display.NewRow();
                    row["parcel_id"] = r["parcel_id"];
                    row["display"]   = $"{r["parcel_id"]}  —  {r["sender_name"]} → {r["recipient_name"]}  [{r["current_status"]}]";
                    display.Rows.Add(row);
                }
                cmbParcel.DisplayMember = "display";
                cmbParcel.ValueMember   = "parcel_id";
                cmbParcel.DataSource    = display;
            }
            catch (Exception ex) { MessageBox.Show("Error loading parcels: " + ex.Message); }
        }

        private void LoadTracking()
        {
            if (cmbParcel.SelectedValue == null) return;
            string parcelId = cmbParcel.SelectedValue.ToString();

            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_tracking_by_parcel",
                    new MySqlParameter("p_parcel_id", parcelId));

                if (dt.Rows.Count == 0) { MessageBox.Show("No tracking records found."); return; }

                var first = dt.Rows[0];
                var last  = dt.Rows[dt.Rows.Count - 1];

                lblSender.Text    = "Sender: "    + first["sender_name"];
                lblRecipient.Text = "Recipient: " + first["recipient_name"];
                lblRider.Text     = "Rider: "     + first["rider_name"];
                lblRoute.Text     = "Route: "     + first["origin"] + " → " + first["destination"];

                string status = last["status_type"]?.ToString() ?? "";
                lblCurrentStatus.Text      = "Status: " + status;
                lblCurrentStatus.ForeColor = status switch {
                    "delivered"        => Color.FromArgb(166, 227, 161),
                    "out for delivery" => Color.FromArgb(250, 179, 135),
                    "in transit"       => Color.FromArgb(137, 180, 250),
                    "failed delivery"  => Color.FromArgb(243, 139, 168),
                    _                  => Color.FromArgb(166, 173, 200)
                };

                dgvTimeline.DataSource = dt;
                HideCols(dgvTimeline, "parcel_id","sender_name","recipient_name","rider_name","origin","destination");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void AddUpdate()
        {
            if (cmbParcel.SelectedValue == null) { MessageBox.Show("Load a parcel first."); return; }
            string parcelId = cmbParcel.SelectedValue.ToString();
            using var dlg = new TrackingUpdateDialog(parcelId);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                LoadParcelList();
                LoadTracking();
            }
        }
    }

    // ── Tracking update dialog ─────────────────────────────
    public class TrackingUpdateDialog : Form
    {
        private readonly string _parcelId;
        private ComboBox cmbStatus;
        private TextBox  txtLocation, txtNotes;

        public TrackingUpdateDialog(string parcelId)
        {
            _parcelId = parcelId;
            this.Text = "Add Tracking Update";
            this.Size = new Size(420, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            Lbl("Parcel ID", 20, y);
            this.Controls.Add(new Label { Text=_parcelId, Font=new Font("Segoe UI",9,FontStyle.Bold),
                ForeColor=Color.FromArgb(137,180,250), AutoSize=true, Location=new Point(160,y+3) });
            y += 38;

            Lbl("New Status", 20, y);
            cmbStatus = new ComboBox { Location=new Point(160,y), Size=new Size(220,25),
                BackColor=Color.FromArgb(49,50,68), ForeColor=Color.White,
                FlatStyle=FlatStyle.Flat, DropDownStyle=ComboBoxStyle.DropDownList,
                Font=new Font("Segoe UI",9) };
            cmbStatus.Items.AddRange(new[]{"in transit","out for delivery","delivered","failed delivery"});
            cmbStatus.SelectedIndex = 0;
            this.Controls.Add(cmbStatus); y += 38;

            Lbl("Location", 20, y);
            txtLocation = Tx(160, y); this.Controls.Add(txtLocation); y += 38;

            Lbl("Notes", 20, y);
            txtNotes = Tx(160, y); this.Controls.Add(txtNotes); y += 50;

            var btnSave = new Button { Text="💾 Save", Location=new Point(160,y), Size=new Size(90,30),
                BackColor=Color.FromArgb(166,227,161), ForeColor=Color.FromArgb(30,30,46),
                FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += Save_Click;

            var btnCancel = new Button { Text="Cancel", Location=new Point(260,y), Size=new Size(80,30),
                BackColor=Color.FromArgb(49,50,68), ForeColor=Color.FromArgb(205,214,244),
                FlatStyle=FlatStyle.Flat, Cursor=Cursors.Hand, DialogResult=DialogResult.Cancel };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.AddRange(new Control[]{ btnSave, btnCancel });
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseHelper.ExecuteProcedureNonQuery("sp_add_tracking_update",
                    new MySqlParameter("p_parcel_id", _parcelId),
                    new MySqlParameter("p_status",    cmbStatus.SelectedItem),
                    new MySqlParameter("p_location",  string.IsNullOrWhiteSpace(txtLocation.Text) ? (object)DBNull.Value : txtLocation.Text.Trim()),
                    new MySqlParameter("p_notes",     string.IsNullOrWhiteSpace(txtNotes.Text)    ? (object)DBNull.Value : txtNotes.Text.Trim()));

                MessageBox.Show("Tracking updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Lbl(string t, int x, int y) =>
            this.Controls.Add(new Label { Text=t, Location=new Point(x,y+3), AutoSize=true,
                Font=new Font("Segoe UI",9,FontStyle.Bold), ForeColor=Color.FromArgb(166,173,200) });

        private TextBox Tx(int x, int y) =>
            new TextBox { Location=new Point(x,y), Size=new Size(220,25),
                Font=new Font("Segoe UI",9), BackColor=Color.FromArgb(49,50,68),
                ForeColor=Color.White, BorderStyle=BorderStyle.FixedSingle };
    }
}
