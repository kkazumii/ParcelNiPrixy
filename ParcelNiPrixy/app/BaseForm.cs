using System;
using System.Drawing;
using System.Windows.Forms;

namespace ParcelTrackingSystem
{
    public class BaseForm : Form
    {
        protected Panel pnlContent;
        private Panel pnlSidebar, pnlTopBar;
        public static string AdminName = "Admin";

        public BaseForm()
        {
            InitializeBase();
        }

        private void InitializeBase()
        {
            this.Size = new Size(1150, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 46);
            this.MinimumSize = new Size(900, 600);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            // ── Top bar ──
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(24, 24, 37)
            };

            var btnLogout = new Button
            {
                Text = "Logout",
                Size = new Size(72, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(243, 139, 168),
                ForeColor = Color.FromArgb(30, 30, 46),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Location = new Point(pnlTopBar.Width - btnLogout.Width - 16, 13);
            btnLogout.Click += (s, e) =>
            {
                new LoginForm().Show();
                this.Close();
            };

            var lblAdmin = new Label
            {
                Text = $"👤 {AdminName}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(166, 173, 200),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            lblAdmin.Location = new Point(btnLogout.Left - lblAdmin.PreferredWidth - 14, 18);

            pnlTopBar.Controls.AddRange(new Control[] { lblAdmin, btnLogout });

            // ── Sidebar ──
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 185,
                BackColor = Color.FromArgb(24, 24, 37)
            };

            var lblApp = new Label
            {
                Text = "📦 ParcelTrack",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(137, 180, 250),
                AutoSize = true,
                Location = new Point(18, 18)
            };
            pnlSidebar.Controls.Add(lblApp);

            var navItems = new[] {
                ("🏠", "Dashboard"),
                ("📦", "Parcels"),
                ("🚚", "Shipments"),
                ("📍", "Tracking"),
                ("🚴", "Riders"),
                ("👤", "Senders"),
                ("📬", "Recipients"),
                ("💳", "Payments"),
                ("📋", "Audit Log")
            };

            int y = 70;
            foreach (var (icon, label) in navItems)
            {
                var btn = new Button
                {
                    Text = $"{icon}  {label}",
                    Location = new Point(0, y),
                    Size = new Size(185, 42),
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(166, 173, 200),
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(14, 0, 0, 0),
                    Cursor = Cursors.Hand,
                    Tag = label
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(49, 50, 68);

                // Highlight current page
                if (label == this.GetType().Name.Replace("Form", ""))
                {
                    btn.BackColor = Color.FromArgb(49, 50, 68);
                    btn.ForeColor = Color.FromArgb(137, 180, 250);
                }

                btn.Click += NavBtn_Click;
                pnlSidebar.Controls.Add(btn);
                y += 44;
            }

            // ── Content area ──
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 46),
                Padding = new Padding(25, 20, 25, 20)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlTopBar);
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            string tag = ((Button)sender).Tag?.ToString() ?? "";
            Form next = tag switch
            {
                "Dashboard" => new DashboardForm(),
                "Parcels" => new ParcelsForm(),
                "Shipments" => new ShipmentsForm(),
                "Tracking" => new TrackingForm(),
                "Riders" => new RidersForm(),
                "Senders" => new SendersForm(),
                "Recipients" => new RecipientsForm(),
                "Payments" => new PaymentsForm(),
                "Audit Log" => new AuditLogForm(),
                _ => null
            };

            if (next != null && next.GetType() != this.GetType())
            {
                next.Show();
                this.Close();
            }
        }

        // ── Shared helpers ──────────────────────────────────────

        protected Label PageTitle(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(205, 214, 244),
                AutoSize = true,
                Location = new Point(0, 0)
            };
        }

        protected DataGridView MakeGrid(int top)
        {
            var dgv = new DataGridView
            {
                Location = new Point(0, top),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.FromArgb(30, 30, 46),
                ForeColor = Color.FromArgb(205, 214, 244),
                GridColor = Color.FromArgb(49, 50, 68),
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                RowTemplate = { Height = 30 }
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(49, 50, 68);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(137, 180, 250);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 46);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(205, 214, 244);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(49, 50, 68);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(137, 180, 250);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(36, 36, 54);
            return dgv;
        }

        protected Button MakeBtn(string text, Color bg, int x, int y, int w = 90)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 30),
                Font = new Font("Segoe UI", 9),
                BackColor = bg,
                ForeColor = Color.FromArgb(30, 30, 46),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        protected TextBox MakeTxt(int x, int y, int w = 260)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 27),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(49, 50, 68),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        protected Label MakeLbl(string text, int x, int y, bool bold = false)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = bold ? Color.FromArgb(166, 173, 200) : Color.FromArgb(205, 214, 244)
            };
        }

        protected Panel MakeCard(int x, int y, int w, int h)
        {
            return new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.FromArgb(49, 50, 68)
            };
        }

        protected void HideCols(DataGridView dgv, params string[] cols)
        {
            foreach (var c in cols)
                if (dgv.Columns.Contains(c))
                    dgv.Columns[c].Visible = false;
        }

        protected void AnchorRowRight(Panel container, int y, params Button[] buttonsLeftToRight)
        {
            int totalWidth = 0;
            foreach (var b in buttonsLeftToRight) totalWidth += b.Width + 8;
            int x = container.Width - totalWidth + 8 - 25; // -25 accounts for container.Padding
            foreach (var b in buttonsLeftToRight)
            {
                b.Location = new Point(x, y);
                b.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                x += b.Width + 8;
            }
        }

        protected void AnchorRowRight(Panel container, int y, TextBox searchBox, params Button[] buttonsLeftToRight)
        {
            int totalBtnWidth = 0;
            foreach (var b in buttonsLeftToRight) totalBtnWidth += b.Width + 8;

            int rightEdge = container.Width - 25;
            int x = rightEdge - totalBtnWidth + 8;

            foreach (var b in buttonsLeftToRight)
            {
                b.Location = new Point(x, y);
                b.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                x += b.Width + 8;
            }

            if (searchBox != null)
            {
                // keep its existing width, just reposition it directly left of the buttons
                searchBox.Location = new Point(x - totalBtnWidth - searchBox.Width - 16, y + 4);
                searchBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }
        }

    }
}
