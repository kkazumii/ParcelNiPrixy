using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ParcelTrackingSystem
{
    // ══════════════════════════════════════════════════════
    // PROGRAM
    // ══════════════════════════════════════════════════════
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }

    // ══════════════════════════════════════════════════════
    // LOGIN FORM
    // ══════════════════════════════════════════════════════
    public class LoginForm : Form
    {
        private TextBox txtUser, txtPass;
        private Label   lblError;

        public LoginForm()
        {
            this.Text = "ParcelTrack – Login";
            this.Size = new Size(460, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var card = new Panel {
                Size = new Size(360, 390),
                Location = new Point(50, 65),
                BackColor = Color.FromArgb(49, 50, 68)
            };

            card.Controls.Add(Lbl("📦 ParcelTrack", 60, 25, 20, true, Color.FromArgb(137, 180, 250)));
            card.Controls.Add(Lbl("Delivery Management System", 75, 60, 9, false, Color.FromArgb(166, 173, 200)));
            card.Controls.Add(Lbl("Username", 40, 108, 9, true, Color.FromArgb(205, 214, 244)));
            txtUser = Txt(40, 130);
            card.Controls.Add(txtUser);
            card.Controls.Add(Lbl("Password", 40, 172, 9, true, Color.FromArgb(205, 214, 244)));
            txtPass = Txt(40, 194); txtPass.PasswordChar = '●';
            txtPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };
            card.Controls.Add(txtPass);

            lblError = new Label { Text = "", ForeColor = Color.FromArgb(243,139,168),
                Font = new Font("Segoe UI", 9), Location = new Point(40, 235),
                Size = new Size(280, 18), TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.Add(lblError);

            var btnLogin = new Button { Text = "Login", Location = new Point(40, 258),
                Size = new Size(280, 42), Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(137, 180, 250),
                ForeColor = Color.FromArgb(30, 30, 46),
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) => DoLogin();
            card.Controls.Add(btnLogin);

            this.Controls.Add(card);
            this.Controls.Add(Lbl("Group 8 | Information Management", 110, 480, 8, false, Color.FromArgb(88, 91, 112)));
        }

        private void DoLogin()
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            { lblError.Text = "Please enter username and password."; return; }
            try
            {
                using var conn = DatabaseHelper.GetConnection();
                var pSuccess = new MySqlParameter("p_success", MySqlDbType.Byte) { Direction = System.Data.ParameterDirection.Output };
                var pName = new MySqlParameter("p_fullname", MySqlDbType.VarChar, 100) { Direction = System.Data.ParameterDirection.Output };
                DatabaseHelper.ExecuteProcedureWithOutput(conn, "sp_login",
                    new MySqlParameter("p_username", txtUser.Text.Trim()),
                    new MySqlParameter("p_password", txtPass.Text),
                    pSuccess, pName);

                if (Convert.ToInt32(pSuccess.Value) == 1)
                {
                    BaseForm.AdminName = pName.Value?.ToString() ?? "Admin";
                    new DashboardForm().Show();
                    this.Hide();
                }
                else { lblError.Text = "Invalid username or password."; txtPass.Clear(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Label Lbl(string t, int x, int y, int size, bool bold, Color color) =>
            new Label { Text=t, Location=new Point(x,y), AutoSize=true,
                Font=new Font("Segoe UI", size, bold?FontStyle.Bold:FontStyle.Regular), ForeColor=color };

        private TextBox Txt(int x, int y) =>
            new TextBox { Location=new Point(x,y), Size=new Size(280,27),
                Font=new Font("Segoe UI",10), BackColor=Color.FromArgb(30,30,46),
                ForeColor=Color.White, BorderStyle=BorderStyle.FixedSingle };
    }

    // ══════════════════════════════════════════════════════
    // DASHBOARD FORM
    // ══════════════════════════════════════════════════════
    public class DashboardForm : BaseForm
    {
        public DashboardForm()
        {
            this.Text = "ParcelTrack – Dashboard";
            BuildDashboard();
        }

        private void BuildDashboard()
        {
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(PageTitle("🏠  Dashboard Overview"));

            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_dashboard_summary");
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    var stats = new[] {
                        ("📦 Total Parcels",    row["total_parcels"]?.ToString()     ?? "0", Color.FromArgb(137,180,250)),
                        ("🚴 Active Riders",    row["active_riders"]?.ToString()     ?? "0", Color.FromArgb(166,227,161)),
                        ("🔄 In Transit",       row["in_transit"]?.ToString()        ?? "0", Color.FromArgb(250,179,135)),
                        ("✅ Delivered Today",  row["delivered_today"]?.ToString()   ?? "0", Color.FromArgb(203,166,247)),
                        ("⏳ Pending Shipments",row["pending_shipments"]?.ToString() ?? "0", Color.FromArgb(249,226,175)),
                        ("💰 Revenue",          "₱" + (row["total_revenue"]==DBNull.Value ? "0.00" : $"{Convert.ToDecimal(row["total_revenue"]):N2}"), Color.FromArgb(166,227,161))
                    };

                    int x = 0; int y = 42;
                    foreach (var (title, val, color) in stats)
                    {
                        if (x > 850) { x = 0; y += 110; }
                        var card = MakeCard(x, y, 155, 95);
                        card.Controls.Add(new Label { Text=val, Font=new Font("Segoe UI",17,FontStyle.Bold),
                            ForeColor=color, AutoSize=true, Location=new Point(12,14) });
                        card.Controls.Add(new Label { Text=title, Font=new Font("Segoe UI",8),
                            ForeColor=Color.FromArgb(166,173,200), AutoSize=true, Location=new Point(12,60) });
                        pnlContent.Controls.Add(card);
                        x += 162;
                    }
                }
            }
            catch { }

            // Recent parcels
            pnlContent.Controls.Add(new Label { Text="Recent Parcels",
                Font=new Font("Segoe UI",12,FontStyle.Bold),
                ForeColor=Color.FromArgb(205,214,244),
                AutoSize=true, Location=new Point(0, 210) });

            var dgv = MakeGrid(238);
            dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 268);
            pnlContent.Controls.Add(dgv);
            pnlContent.Resize += (s, e) => dgv.Size = new Size(pnlContent.Width - 50, pnlContent.Height - 268);

            try
            {
                var dt = DatabaseHelper.ExecuteProcedureTable("sp_get_all_parcels");
                dgv.DataSource = dt;
                HideCols(dgv,"sender_phone","recipient_phone","recipient_address","payment_id","shipment_id");
            }
            catch { }
        }
    }
}
