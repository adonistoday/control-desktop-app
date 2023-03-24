using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Speech.Recognition;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;
using NAudio.Wave;
using DeepSpeechClient.Models;
using DeepSpeechClient.Interfaces;
using DeepSpeechClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

using System.Collections.ObjectModel;
using OpenQA.Selenium.Support.UI;

//using SpacyDotNet;
//using SpacyDotNet.Loaders;

using System.Diagnostics;
using System.Windows.Automation;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Runtime.InteropServices;
using OpenQA.Selenium.Remote;
using Polly;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace InteractionApp
{
    
    public partial class Form1 : Form
    {
        public static string gstr;
        IWebDriver driver = null;
        PictureBox[] pb1 = new PictureBox[20];
        int[] size = new int[20];
        GrammarBuilder grammarBuilder = new GrammarBuilder();
        private BufferedGraphics buffer;
        Boolean flag = true;
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        int cnt = 0, cnt1 = 0;
        //Panel[] panels1=new Panel[100];
        //Panel[] panels2 = new Panel[100];
        SQLiteConnection sqlitecon;
        int sh;
        List<string>[] list = new List<string>[4];
        List<string>[] cmds = new List<string>[3]; 
        int selid;
        bool newC = true;
        // Define the constants used by the ShowWindow function
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        string browser;
        // Import the ShowWindow function from the user32.dll library
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public Form1()
        {
            InitializeComponent();

            this.MinimumSize = new Size(Screen.PrimaryScreen.Bounds.Width * 5 / 20, Screen.PrimaryScreen.Bounds.Height * 5 / 20);
            this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width * 20 / 20, Screen.PrimaryScreen.Bounds.Height * 20 / 20);
            this.StartPosition = FormStartPosition.Manual; // set the start position to manual
            this.Location = new Point(0, 0);
            InitializeBuffer();

            //speechtoText();
            //int distance = ComputeLevenshteinDistance("chrome", "Chrome");
            //  voicereceive();

            string wordPath = "wordlist.txt";
            string filePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, wordPath);
            string[] words = new string[2001];
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = "chrome";
            }
            if (File.Exists(filePath))
            {
                int k = 0;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        words[k++] = line;
                        if (k > 2000) break;
                        // grammarBuilder.Append(line);
                        //Console.WriteLine(line); // process the line as needed
                    }
                }
            }

            grammarBuilder.Append(new Choices(words));

            //// Create "File" menu
            //ToolStripMenuItem fileMenu = new ToolStripMenuItem("Model");

            //// Create "New" submenu item
            //ToolStripMenuItem newSubMenu = new ToolStripMenuItem("small");
            //fileMenu.DropDownItems.Add(newSubMenu);

            //// Create "Open" submenu item
            //ToolStripMenuItem openSubMenu = new ToolStripMenuItem("medium");
            //fileMenu.DropDownItems.Add(openSubMenu);

            //// Add "File" menu to MenuStrip control
            //menuStrip.Items.Add(fileMenu);
          //  GroupBox groupBox1 = new GroupBox();
            //RadioButton radioButton1 = new RadioButton();
            //RadioButton radioButton2 = new RadioButton();

         //   groupBox1.Controls.Add(radioButton1);
         //   groupBox1.Controls.Add(radioButton2);

            //radioButton1.Text = "Option 1";
            //radioButton1.GroupName = "myRadioGroup";

            //radioButton2.Text = "Option 2";
            //radioButton2.GroupName = "myRadioGroup";

            // optionally set radio button 1 as selected
            //radioButton1.Checked = true;

           // this.Controls.Add(groupBox1);
            // Get the primary screen
            Screen primaryScreen = Screen.PrimaryScreen;

            // Get the width and height of the screen
            int screenWidth = primaryScreen.Bounds.Width;
            int screenHeight = primaryScreen.Bounds.Height;
            this.Size = new Size(screenWidth * 7 / 10, screenHeight * 4 / 5);
            panel2.Size = new Size(this.Width / 5, this.Height);
            panel1.Size = new Size(this.Width * 4 / 5 - 10, this.Height * 3 / 4);
            panel1.Location = new Point(this.Width / 5 - 1, 0);
            panel3.Size = new Size(this.Width * 4 / 5, this.Height / 4);
            panel3.Location = new Point(this.Width / 5 - 1, this.Height * 3 / 4);
            richTextBox1.Size = new Size(panel3.Width * 7 / 10, panel3.Height / 5);
            richTextBox1.Location = new Point((panel3.Width - richTextBox1.Width) / 2, (panel3.Height - richTextBox1.Height) / 2);
            int[] columnWidths = { 1 };
            richTextBox1.SelectionTabs = columnWidths;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;
            button1.Size = new Size(richTextBox1.Width / 25, richTextBox1.Height * 3 / 4);
            button1.Location = new Point(richTextBox1.Location.X + richTextBox1.Width - button1.Width * 5 / 4, richTextBox1.Location.Y + richTextBox1.Height / 8);
            button2.Size = new Size(panel2.Width * 9 / 10, panel2.Height / 20);
            button2.Location = new Point((panel2.Width - button2.Width) / 2, button2.Height / 4);
            button3.Size = new Size(richTextBox1.Width / 15, richTextBox1.Height * 4 / 4);
            button3.Location = new Point(richTextBox1.Location.X - button1.Width * 2, richTextBox1.Location.Y);

            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            //  button1.Size = new Size(100, 40);
            button1.Font = new Font("Arial", 10, FontStyle.Regular);
            button1.Paint += (sender, e) =>
            {
                GraphicsPath path1 = new GraphicsPath();
                int cornerRadius1 = 5;
                int width = button1.Width;
                int height = button1.Height;
                path1.AddArc(0, 0, cornerRadius1, cornerRadius1, 180, 90);
                path1.AddArc(width - cornerRadius1, 0, cornerRadius1, cornerRadius1, 270, 90);
                path1.AddArc(width - cornerRadius1, height - cornerRadius1, cornerRadius1, cornerRadius1, 0, 90);
                path1.AddArc(0, height - cornerRadius1, cornerRadius1, cornerRadius1, 90, 90);
                path1.CloseFigure();
                button1.Region = new Region(path1);
            };
            button3.BackgroundImage = Image.FromFile(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "icons8-record-64.png"));
            button1.BackColor = Color.FromArgb(64, 65, 79);
            button3.BackColor = Color.FromArgb(68, 70, 84);
            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;
            button3.Paint += (sender, e) =>
            {
                GraphicsPath path1 = new GraphicsPath();
                int cornerRadius1 = 2;
                int width = button3.Width;
                int height = button3.Height;
                path1.AddArc(0, 0, cornerRadius1, cornerRadius1, 180, 90);
                path1.AddArc(width - cornerRadius1, 0, cornerRadius1, cornerRadius1, 270, 90);
                path1.AddArc(width - cornerRadius1, height - cornerRadius1, cornerRadius1, cornerRadius1, 0, 90);
                path1.AddArc(0, height - cornerRadius1, cornerRadius1, cornerRadius1, 90, 90);
                path1.CloseFigure();
                button3.Region = new Region(path1);
            };
            button2.Text = "   +   New chat";
            button2.TextAlign = ContentAlignment.MiddleLeft;
            button1.FlatAppearance.BorderColor = Color.FromArgb(241, 198, 198);
            button2.Font = new Font("Arial", 12, FontStyle.Regular);
            button2.BackColor = Color.FromArgb(32, 33, 35);
            button2.ForeColor = Color.FromArgb(255, 255, 255);
            button2.FlatAppearance.BorderSize = 1;
            button2.Paint += (sender, e) =>
            {
                GraphicsPath path1 = new GraphicsPath();
                int cornerRadius1 = 5;
                int width = button2.Width;
                int height = button2.Height;
                path1.AddArc(0, 0, cornerRadius1, cornerRadius1, 180, 90);
                path1.AddArc(width - cornerRadius1, 0, cornerRadius1, cornerRadius1, 270, 90);
                path1.AddArc(width - cornerRadius1, height - cornerRadius1, cornerRadius1, cornerRadius1, 0, 90);
                path1.AddArc(0, height - cornerRadius1, cornerRadius1, cornerRadius1, 90, 90);
                path1.CloseFigure();
                button2.Region = new Region(path1);
            };

            richTextBox1.ForeColor = Color.FromArgb(242, 243, 244);
            richTextBox1.BackColor = Color.FromArgb(64, 65, 79);
            richTextBox1.Font = new Font("Arial", 15, FontStyle.Regular);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int cornerRadius = 2;
            int x = richTextBox1.Width;
            int y = richTextBox1.Height;
            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddLine(cornerRadius, 0, x - cornerRadius, 0);
            path.AddArc(x - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddLine(x, cornerRadius * 2, x, y - cornerRadius * 2);
            path.AddArc(x - cornerRadius * 2, y - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddLine(x - cornerRadius, y, cornerRadius, y);
            path.AddArc(0, y - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.AddLine(0, y - cornerRadius * 2, 0, cornerRadius);
            richTextBox1.Region = new System.Drawing.Region(path);


            panel3.BackColor = Color.FromArgb(52, 54, 64);
            panel3.ForeColor = Color.FromArgb(31, 31, 33);
            richTextBox1.SelectionCharOffset = -10;
            
            panel2.BackColor = Color.FromArgb(32, 33, 35);
            panel2.AutoScroll = true;
            panel1.BackColor = Color.FromArgb(68, 70, 84);
            panel1.AutoScroll = true;

            string imagePath = "aaa.png";
            //   panel4.Size = new Size(richTextBox1.Width*3/4,richTextBox1.Height*2);
            panel4.Location = new Point(richTextBox1.Location.X + richTextBox1.Width / 7, richTextBox1.Location.Y - richTextBox1.Height * 5 / 4);
            for (int i = 0; i < 20; i++)
            {
                size[i] = 0;
                pb1[i] = new PictureBox();
                pb1[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pb1[i].Image = Image.FromFile(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, imagePath));
                pb1[i].Size = new Size(panel4.Width / 100, panel4.Height);
                pb1[i].Location = new Point(panel4.Width / 20 * i, 0);
                databaseConnect();
                databaseLoad();
                foreach (Panel p in panel2.Controls.OfType<Panel>())
                {
                    p.BackColor = Color.FromArgb(32, 33, 35);
                }
            }

        }

        private void requestView(string str)
        {

            if (str == "") { str = ""; }
            else
            {
                int sh = 0;
                Label label1 = new Label();
                label1.ForeColor = Color.FromArgb(255, 255, 255);
                label1.Font = new Font("Arial", 14);
                int width = TextRenderer.MeasureText(str, label1.Font).Width;
                int height = TextRenderer.MeasureText(str, label1.Font).Height;
                Panel subpanel1 = new Panel();
                for (int i = 0; i < cnt; i++)
                {
                    sh += (3*height);
                }
                subpanel1.Location = new Point(0, sh);

                //  label1.AutoSize = true; // Set this to true to make the label adjust its size to fit its contents



                subpanel1.Size = new Size(panel1.Size.Width, height*3 );
                subpanel1.BackColor = Color.FromArgb(52, 53, 65);
                label1.Size = new Size(width, height);
                label1.Location = new Point((subpanel1.Width) / 4 + 40, (subpanel1.Height - label1.Height) / 2);
                PictureBox pB = new PictureBox();
                pB.Location = new Point((subpanel1.Width) / 4, (subpanel1.Height - label1.Height) / 2);
                pB.Size = new Size(Screen.PrimaryScreen.Bounds.Width / 80, Screen.PrimaryScreen.Bounds.Height / 50);
                pB.SizeMode = PictureBoxSizeMode.StretchImage;
                string imagePath = "communication.png";
                string filePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, imagePath);

                pB.Image = Image.FromFile(filePath);
                label1.Text = str;

                subpanel1.Controls.Add(label1);
                subpanel1.Controls.Add(pB);
                panel1.Controls.Add(subpanel1);
                cnt++;


            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //recognizer.RecognizeAsyncCancel();
            if (richTextBox1.Text == "") MessageBox.Show("Type your command!", "Message Box Title", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                string tstr = richTextBox1.Text;
                // Enter key was pressed
                //e.Handled = true;
                if (newC == true)
                {
                    if (tstr.Length > 20)
                        newInsert(tstr.Substring(0, 20));
                    else newInsert(tstr);
                    newC = false;
                }

                insertReq(tstr);
                requestView(tstr);
                this.Cursor = Cursors.WaitCursor;
                gstr= tstr;
                Task task1 = Task.Factory.StartNew(() => doStuff());
                Task.WaitAll(task1);
                this.Cursor = Cursors.Default;
                int[] columnWidths = { 1 };
                richTextBox1.SelectionTabs = columnWidths;

                richTextBox1.Text = "";
            }

        }
        public void doStuff()
        {
            //do stuff here
            controlApp(gstr);
        }
        private void databaseConnect()
        {
           sqlitecon = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
           sqlitecon.Open();
            // Assuming you have a connection to your SQLite database:
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM cmdlog LIMIT 1", sqlitecon);

            // Execute the query and get the first row of the result set
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Access the data in the first row
                    //string column1Value = reader.GetString(0);
                    int column2Value = reader.GetInt32(0);
                    selid = column2Value;
                    // etc.
                }
            }

            //selid = k;
            //   MessageBox.Show("connected successfully");

        }
        private void databaseLoad()
        {
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();
            SQLiteCommand command = sqlitecon.CreateCommand();
            command.CommandText = "SELECT * FROM cmdlog";

            // Execute the command and retrieve the results
            SQLiteDataReader reader = command.ExecuteReader();
            int cn = 0;
            while (reader.Read())
            {
                cn++;
                list[0].Add(reader["id"] + "");
                list[1].Add(reader["title"] + "");
                list[2].Add(reader["created_at"]+"");
                list[3].Add(reader["updated_at"]+"");
                // Process the data
            }
            if (cn > 0) selid = int.Parse(list[0][0]);
            else selid = 0;
            cnt1 = 0;
            for (int i = 0; i < Count(); i++)
            {
                drawchatlist(int.Parse(list[0][i]),list[1][i]);
                cnt1++;
            }
            
        }
        public int Count()
        {
            string query = "SELECT Count(*) FROM cmdlog";
            int Count = -1;

            //Open Connection
            // databaseConnect();
            //Create Mysql Command
            SQLiteCommand cmd = new SQLiteCommand(query, sqlitecon);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                return Count;
            
        }
        public void insert(string title)
        {
            //DateTime dateToInsert = DateTime.Now;
            string dateToInsert = DateTime.Now.ToString();
             SQLiteCommand command = new SQLiteCommand("INSERT INTO cmdlog (title,created_at) VALUES (@title,@DateValue)", sqlitecon);
             command.Parameters.AddWithValue("@DateValue", dateToInsert);
            command.Parameters.AddWithValue("@title", title);
            int lastInsertedId = Convert.ToInt32(command.ExecuteScalar());
        }
        public void delete(int id)
        {
            
            using (var command = new SQLiteCommand("DELETE FROM cmdlog WHERE id = @id", sqlitecon))
            {
                // Set the value of the parameter
                command.Parameters.AddWithValue("@id", id);

                // Execute the command
                int rowsDeleted = command.ExecuteNonQuery();
               // Console.WriteLine("Rows deleted: " + rowsDeleted);
            }
            
            using (var command = new SQLiteCommand("DELETE FROM commands WHERE log_id = @id", sqlitecon))
            {
                // Set the value of the parameter
                command.Parameters.AddWithValue("@id", id);

                // Execute the command
                int rowsDeleted = command.ExecuteNonQuery();
                // Console.WriteLine("Rows deleted: " + rowsDeleted);
            }
            if (selid == id)
            {
                selid = 0;
                panel1.Controls.Clear();
            }
            panel2.SuspendLayout();
            panel2.Controls.Clear();

            panel2.Controls.Add(button2);
            /* for (int i=0;i<100;i++)
             {
                 panels1[i].Controls.Clear();
                 panels2[i].Controls.Clear();
             }*/

            //int tempid = selid;
            databaseLoad();
            panel2.ResumeLayout();

            // Redraw the off-screen buffer and transfer it to the screen to be displayed
            RedrawBuffer();
            //selid = tempid;

        }
        public void insertReq(string req)
        {
           // string dateToInsert = DateTime.Now.ToString();
            SQLiteCommand command = new SQLiteCommand("INSERT INTO commands (content,log_id) VALUES (@req,@logid)", sqlitecon);
          //  command.Parameters.AddWithValue("@DateValue", dateToInsert);
            command.Parameters.AddWithValue("@req", req);
            command.Parameters.AddWithValue("@logid", selid);
            command.ExecuteNonQuery();
        }
        public void getcmds(int logid)
        {
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            int num = 0;
            SQLiteCommand command = new SQLiteCommand("select * from commands where log_id=@logid", sqlitecon);
            command.Parameters.AddWithValue("@logid", logid);
            // Execute the query
            using (var reader = command.ExecuteReader())
            {
                // Process the results
                while (reader.Read())
                {
                    num++;
                    list[0].Add(reader["cmd_id"] + "");
                    list[1].Add(reader["content"] + "");
                    list[2].Add(reader["log_id"] + "");
                    // Access the data using the column names or indexes
                    //int id = reader.GetInt32(0);
                    //string cmds = reader.GetString(1);
                    //Console.WriteLine("ID: " + id + ", Name: " + name);
                }
            }
            for(int i=0;i<num;i++)
            {
                requestView(list[1][i]);
            }
        }
        private void button2_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void button2_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            newC = true;
            cnt = 0;
            
            panel1.Controls.Clear();
            foreach (Panel p in panel2.Controls.OfType<Panel>())
            {
                p.BackColor = Color.FromArgb(32, 33, 35);
            }
            //  databaseConnect();
            /*for (int i = 0; i < 100; i++)
            {
                panels1[i].Controls.Clear();

            }*/
            
        }
        void drawchatlist(int id, string title)
        {
            Label temp = new Label();
            temp.Text = id.ToString();
            temp.Hide();
            sh = button1.Height * 7 / 4;
            for (int i = 0; i < cnt1; i++)
            {
                sh += button1.Height * 7 / 4;
            }
            Panel subpanel2 = new Panel();
            subpanel2.Controls.Add(temp);
            subpanel2.Size = new Size(panel2.Width * 9 / 10, panel2.Height / 20);
            subpanel2.Location = new Point((panel2.Width - button2.Width) / 2, sh + button2.Height / 4);
            if (selid == id)
            {
                subpanel2.BackColor = Color.FromArgb(52, 53, 65);
            }
            else
            {
                subpanel2.BackColor = Color.FromArgb(32, 33, 35);
            }
            
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int cornerRadius = 10;
            int x = subpanel2.Width;
            int y = subpanel2.Height;
            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddLine(cornerRadius, 0, x - cornerRadius, 0);
            path.AddArc(x - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddLine(x, cornerRadius * 2, x, y - cornerRadius * 2);
            path.AddArc(x - cornerRadius * 2, y - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddLine(x - cornerRadius, y, cornerRadius, y);
            path.AddArc(0, y - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.AddLine(0, y - cornerRadius * 2, 0, cornerRadius);
            subpanel2.Region = new System.Drawing.Region(path);
            // Create some controls to add to the GroupBox
            Label label1 = new Label();
            label1.Text = title;
            label1.ForeColor = Color.FromArgb(215, 215, 215);
            label1.Font = new Font("Arial", 12);
            int width = TextRenderer.MeasureText(label1.Text, label1.Font).Width;
            int height = TextRenderer.MeasureText(label1.Text, label1.Font).Height;
            label1.Size = new Size(width, height);
            label1.Location = new Point((subpanel2.Width) / 50, (subpanel2.Height - label1.Height) / 2);
            subpanel2.Controls.Add(label1);

            PictureBox pBdel = new PictureBox();
            pBdel.Size = new Size(Screen.PrimaryScreen.Bounds.Width/70 , Screen.PrimaryScreen.Bounds.Height/50);
            pBdel.Location = new Point(subpanel2.Width*17/20, (subpanel2.Height - label1.Height) / 2);

            pBdel.SizeMode = PictureBoxSizeMode.StretchImage;
            string imagePath = "icons8-waste-24.png";
            string filePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, imagePath);

            pBdel.Image = Image.FromFile(filePath);
            pBdel.MouseEnter += (sender, e) =>
            {
                this.Cursor = Cursors.Hand;
                // Code to handle the Enter event
                //  pBdel.BackColor = Color.FromArgb(0, 0, 0);
                if (selid != id)
                {
                    label1.ForeColor = Color.FromArgb(255, 255, 255);
                    subpanel2.BackColor = Color.FromArgb(52, 53, 60);
                }

            };
            pBdel.MouseLeave += (sender, e) =>
            {
                this.Cursor = Cursors.Default;
                if (selid != id)
                {
                    label1.ForeColor = Color.FromArgb(255, 255, 255);
                    subpanel2.BackColor = Color.FromArgb(52, 53, 60);

                }
                // Code to handle the Leave event
                //  pBdel.BackColor = Color.FromArgb(52, 53, 65);

            };
            pBdel.Click += (sender, e) =>
            {
                // Code to handle the Click event
                string str = label1.Text;
                //MessageBox.Show(id.ToString());
                delete(id);
            };
            label1.MouseEnter += (sender, e) =>
            {
                // Code to handle the Enter event
                if (selid != id)
                {
                    label1.ForeColor = Color.FromArgb(255, 255, 255);
                    subpanel2.BackColor = Color.FromArgb(52, 53, 60);

                }

            };
            label1.MouseLeave += (sender, e) =>
            {
                // Code to handle the Leave event
                if (selid != id)
                {
                    // Code to handle the Leave event
                    label1.ForeColor = Color.FromArgb(215, 215, 215);
                    subpanel2.BackColor = Color.FromArgb(32, 33, 35);
                }
            };
            label1.Click += (sender, e) =>
            {
                // Code to handle the Click event
                //MessageBox.Show(id.ToString() + "selected");
                selid = id;
                foreach (Panel p in panel2.Controls.OfType<Panel>())
                {
                    p.BackColor = Color.FromArgb(32, 33, 35);
                }
                subpanel2.BackColor = Color.FromArgb(52, 53, 65);
                panel1.Controls.Clear();
                cnt = 0;
                getcmds(selid);

            };
            subpanel2.MouseEnter += (sender, e) =>
            {
                if (selid != id)
                {
                    label1.ForeColor = Color.FromArgb(255, 255, 255);
                    subpanel2.BackColor = Color.FromArgb(52, 53, 60);

                }
                // Code to handle the Enter event


            };
            subpanel2.MouseLeave += (sender, e) =>
            {
                if (selid != id)
                {
                    // Code to handle the Leave event
                    label1.ForeColor = Color.FromArgb(215, 215, 215);
                    subpanel2.BackColor = Color.FromArgb(32, 33, 35);
                }
            };
            subpanel2.Click += (sender, e) =>
            {
                // Code to handle the Click event
                //MessageBox.Show(id.ToString()+"selected");
                selid = id;
                foreach (Panel p in panel2.Controls.OfType<Panel>())
                {
                    p.BackColor = Color.FromArgb(32, 33, 35);
                }
                subpanel2.BackColor = Color.FromArgb(52, 53, 65);
                panel1.Controls.Clear();
                cnt = 0;
                getcmds(selid);
                newC = false;
            };
            subpanel2.Controls.Add(pBdel);


            panel2.Controls.Add(subpanel2);
            //cnt1++;
            
            richTextBox1.Text = "";
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true; // Cancel the form closing event 
            }
            if(driver!=null) driver.Dispose();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            // code to handle mouse enter event
            button1.BackColor = Color.FromArgb(32, 33, 35);
            this.Cursor = Cursors.Hand;
        }

        private void richTextBox1_MouseEnter(object sender, EventArgs e)
        {
            richTextBox1.SelectionCharOffset = -10;
            int[] columnWidths = { 1 };
            richTextBox1.SelectionTabs = columnWidths;
        }
        public void newInsert(string title)
        {
            //insert("Log History" + cnt1.ToString());
            insert(title);
            int id=0;
            SQLiteCommand command = sqlitecon.CreateCommand();
            command.CommandText = "SELECT * FROM cmdlog WHERE title=@title";
            command.Parameters.AddWithValue("@title", title);
            selid =int.Parse( command.ExecuteScalar()+"");
            id = selid;
            //drawchatlist(id,"Log History"+cnt1.ToString());
            drawchatlist(id,title);
            cnt1++;
            //selid = id;
            //panel2.Controls.Clear();
            //databaseLoad();
        }
        public int ComputeLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                // Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                if (richTextBox1.Text == "") MessageBox.Show("Type your command!", "Message Box Title", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    string tstr = richTextBox1.Text;
                    // Enter key was pressed
                    e.Handled = true;
                    if (newC == true)
                    {
                        if (tstr.Length > 20)
                            newInsert(tstr.Substring(0, 20));
                        else newInsert(tstr);
                        newC = false;
                    }
                    
                    insertReq(tstr);
                    requestView(tstr);
                    this.Cursor = Cursors.WaitCursor;
                    gstr = tstr;
                    Task task1 = Task.Factory.StartNew(() => doStuff());
                    Task.WaitAll(task1);
                    this.Cursor = Cursors.Default;
                    int[] columnWidths = { 1 };
                    richTextBox1.SelectionTabs = columnWidths;

                    richTextBox1.Text = "";
                }
                
            }
        }
        public void ner()
        {

            // Load the pre-trained NER model
          //  var loader = new SpacyLoader();
        //    var model = loader.LoadLanguage("en_core_web_sm");

            // Process the input text
       //     var doc = model.Process("Apple is looking at buying U.K. startup for $1 billion");

            // Extract the named entities
      //      foreach (var ent in doc.Entities)
      //      {
      //          Console.WriteLine(ent.Text + " - " + ent.Label);
      //      }

        }
        public void controlApp(string str)
        {
            string wordPath = "sens.txt";
            string filePath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, wordPath);
            string filePath1 = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "senfire.txt");

            string[] sens = new string[100];
            string[] sensfire = new string[100];
            for (int i = 0; i < sens.Length; i++)
            {
                sens[i] = "hey google";
                sensfire[i] = "hey firefox";
            }
            if (File.Exists(filePath))
            {
                int k = 0;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        sens[k++] = line;
                        if (k > 100) break;
                        // grammarBuilder.Append(line);
                        //Console.WriteLine(line); // process the line as needed
                    }
                }
            }
            if (File.Exists(filePath1))
            {
                int k = 0;
                using (StreamReader reader = new StreamReader(filePath1))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        sensfire[k++] = line;
                        if (k > 100) break;
                        // grammarBuilder.Append(line);
                        //Console.WriteLine(line); // process the line as needed
                    }
                }
            }

            
            int chromeC =-1 ;
            int firefox=-1;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' }; //define the delimiter characters

            string[] words = str.Split(delimiterChars); //split the sentence into an array of words
            string temp = "";
            for (int i = 0; i < words.Length; i++)
            {
                temp = temp + words[i] + " ";
                for (int j = 0; j < sens.Length; j++)
                {
                    if (ComputeLevenshteinDistance(temp, sens[j]) <= 3)
                    {
                        chromeC = i;
                        for (int pp = 0; pp <= i; pp++)
                            str = str.Replace(words[pp], "");
                        break;
                    }
                    if (ComputeLevenshteinDistance(temp, sensfire[j]) <= 3)
                    {
                        firefox = j;
                        for (int pp = 0; pp <= i; pp++)
                            str = str.Replace(words[pp], "");
                        break;
                    }
                }
                if (chromeC != -1||firefox!=-1) break;
            }
            temp = "";
            if (chromeC == -1&&firefox==-1)
            {
                for (int i = words.Length - 1; i >= 0; i--)
                {
                    temp = words[i] + " " + temp;
                    for (int j = 0; j < sens.Length; j++)
                    {
                        if (ComputeLevenshteinDistance(temp, sens[j]) <= 3)
                        {
                            chromeC = i;
                            for (int pp = i; pp < words.Length; pp++)
                                str = str.Replace(words[pp], "");
                            break;
                        }
                        if (ComputeLevenshteinDistance(temp, sensfire[j]) <= 3)
                        {
                            firefox = j;
                            for (int pp = 0; pp <= i; pp++)
                                str = str.Replace(words[pp], "");
                            break;
                        }
                    }
                    if (chromeC != -1 || firefox != -1) break;
                }
            }
            if (chromeC==-1&&firefox==-1)
            {

                // Remove all non-letter characters from the input string
                //  string lettersOnlyString = new string(inputString.Where(char.IsLetter).ToArray());

                // Split the string into words
                string[] words1 = str.Split(' ');

                // Get all possible word groups
                List<string> wordGroups = new List<string>();
                for (int i = 0; i < words1.Length; i++)
                {
                    for (int j = i + 1; j <= words1.Length; j++)
                    {
                        string temp1 = "";
                        for (int kk = i; kk < j; kk++)
                            temp1 += words1[kk] + " ";
                        string wordGroup = string.Join(" ", temp1);
                        wordGroups.Add(wordGroup);
                    }
                }
                for(int i=0;i<wordGroups.Count;i++)
                {
                    for(int j=0;j<sens.Length;j++)
                    {
                        if (ComputeLevenshteinDistance(wordGroups[i], sens[j]) <= 3)
                        {
                            chromeC = j;
                            str = str.Replace(wordGroups[i], "");
                            break;
                        }
                        if (ComputeLevenshteinDistance(wordGroups[i], sensfire[j]) <= 3)
                        {
                            firefox = j;
                            for (int pp = 0; pp <= i; pp++)
                                str = str.Replace(words[pp], "");
                            break;
                        }
                    }
                    if (chromeC != -1 || firefox != -1) break;
                }
            }
           
            if (firefox==-1&&browser== "google" || chromeC!=-1)
            {
                string chromedriverPath = @"C:\Users\KGH\.cache\selenium\chromedriver\win32\110.0.5481.77\chromedriver.exe";

                // Check if the chromedriver executable file exists
                //if (File.Exists(chromedriverPath))
                //{
                //Console.WriteLine("Chromedriver exists at " + chromedriverPath);


                // This line of code will hide the console window
                // options.AddArgument("headless");

                // Create the ChromeDriver with the options
                try
                {
                    // Try to use the driver
                    if (driver == null||driver!=null&&browser=="firefox")
                    {
                        ChromeOptions options = new ChromeOptions();
                        options.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                        string browserpath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                        if (!File.Exists(browserpath))
                        {
                            MessageBox.Show("You should install chrome!");
                            return;
                        }
                        options.AddArgument("--disable-logging");
                        options.AddArgument("--log-level=3");

                        var chromeDriverService = ChromeDriverService.CreateDefaultService();
                        chromeDriverService.HideCommandPromptWindow = true;
                        driver = new ChromeDriver(chromeDriverService, options);
                    }

                    string searchstr = str;
                    driver.Navigate().GoToUrl("https://www.google.com/");
                    IWebElement searchBox = driver.FindElement(By.Name("q"));
                    searchBox.SendKeys(searchstr);

                    // Submit the search query
                    searchBox.Submit();

                    browser = "google";
                }
                //OpenQA.Selenium.NoSuchWindowException
                catch (Exception)
                {
                    // The driver has been disposed
                    // Add code to handle this case
                    ChromeOptions options = new ChromeOptions();
                    options.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                    string browserpath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                    if (!File.Exists(browserpath))
                    {
                        MessageBox.Show("You should install chrome!");
                        return;
                    }
                    options.AddArgument("--disable-logging");
                    options.AddArgument("--log-level=3");

                    var chromeDriverService = ChromeDriverService.CreateDefaultService();
                    chromeDriverService.HideCommandPromptWindow = true;
                    driver = new ChromeDriver(chromeDriverService, options);
                    
                    string searchstr = str;
                    driver.Navigate().GoToUrl("https://www.google.com/");
                    IWebElement searchBox = driver.FindElement(By.Name("q"));
                    searchBox.SendKeys(searchstr);

                    // Submit the search query
                    searchBox.Submit();
                    browser = "google";
                }
              
            }
            else if (chromeC==-1&&browser == "firefox" || firefox != -1)
            {
               //string chromedriverPath = @"C:\Users\KGH\.cache\selenium\chromedriver\win32\110.0.5481.77\chromedriver.exe";
                try
                {
                    // Try to use the driver
                    if (driver == null || driver != null && browser == "google")
                    {
                        string browserpath = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
                        if (!File.Exists(browserpath))
                        {
                            MessageBox.Show("You should install firefox!");
                            return;
                        }
                        var firefoxdriverservice = FirefoxDriverService.CreateDefaultService(browserpath);
                        firefoxdriverservice.HideCommandPromptWindow = true;
                        driver = new FirefoxDriver(firefoxdriverservice);
                    }

                    string searchstr = str;
                    driver.Navigate().GoToUrl("https://www.google.com/");
                    IWebElement searchBox = driver.FindElement(By.Name("q"));
                    searchBox.SendKeys(searchstr);

                    // Submit the search query
                    searchBox.Submit();

                    browser = "firefox";
                }
                //OpenQA.Selenium.NoSuchWindowException
                catch (Exception)
                {
                    string browserpath = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
                    if (!File.Exists(browserpath))
                    {
                        MessageBox.Show("You should install firefox!");
                        return;
                    }
                    var firefoxdriverservice = FirefoxDriverService.CreateDefaultService(browserpath);
                    firefoxdriverservice.HideCommandPromptWindow = true;
                    driver = new FirefoxDriver(firefoxdriverservice);
                    
                    string searchstr = str;
                    driver.Navigate().GoToUrl("https://www.google.com/");
                    IWebElement searchBox = driver.FindElement(By.Name("q"));
                    searchBox.SendKeys(searchstr);

                    // Submit the search query
                    searchBox.Submit();
                    browser = "firefox";
                }

            }

        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            richTextBox1.SelectionCharOffset = -10;
            int[] columnWidths = { 1 };
            richTextBox1.SelectionTabs = columnWidths;
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackColor =Color.FromArgb(32, 33, 35);
            this.Cursor = Cursors.Hand;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(68, 70, 84);
            this.Cursor = Cursors.Default;
        }
        public void WaveIn_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            // copy buffer into an array of integers
            Int16[] values = new Int16[e.Buffer.Length / 2];
            Buffer.BlockCopy(e.Buffer, 0, values, 0, e.Buffer.Length);

            // determine the highest value as a fraction of the maximum possible value
            float fraction = (float)values.Max() / 32768;

            // print a level meter using the console
            string bar = "";
            for (int i = 0; i < (int)(fraction * 70); i++)
            {
                bar += '#';
            }
            /// string bar = new('#', (int)(fraction * 70));

            string meter = "[" + bar.PadRight(60, '-') + "]:"+ (fraction * 100).ToString()+"%";

            for (int i = 1; i < 20; i++)
            {
                size[i] = size[i - 1];
            }
            size[0] = (int)(fraction * 100);
            panel4.Controls.Clear();
            int k = (int)(fraction * 70);
            for(int i=0;i<k%20;i++)
            {
                panel4.Controls.Add(pb1[i]);
            }
         //   richTextBox1.Text = meter;
            //rep.Text += meter;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string fileName1 = @"C:\Users\KGH\PycharmProjects\speechRec\main.py";
            string fileName2 = @"C:\Users\KGH\PycharmProjects\speechRec\main1.py";
            Process p = new Process();
            if (radioButton1.Checked)
            {
                p.StartInfo = new ProcessStartInfo(@"C:\Users\KGH\AppData\Local\Programs\Python\Python37\python.exe", fileName1)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

            }
            else
            {
                p.StartInfo = new ProcessStartInfo(@"C:\Users\KGH\AppData\Local\Programs\Python\Python37\python.exe", fileName2)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            

            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            string newString = output.Replace("\n", "");
            newString = newString.Replace("\r", "");
            richTextBox1.Text = newString;
            p.WaitForExit();

        }
        private void Recognizer_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            // Add code to handle recognized result, such as displaying the text
            //  Console.WriteLine("Recognized: " + e.Result.Text);
        }
        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            richTextBox1.Text +=e.Result.Text;
            if (e.Result.Text == "chrome")
            {
                string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                // string url = "https://chat.openai.com";

                //  Process.Start(chromePath);

            }
            else if (e.Result.Text == "firefox")
            {
                Process.Start("firefox.exe");
            }
            else if (e.Result.Text == "opera")
            {
                //Process.Start("opera.exe");
            }
            else if(e.Result.Text=="reacttutorial")
            {
                string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
                string url = "https://reacttutorial";

                Process.Start(chromePath,url);
            }
        }


        private void button1_MouseLeave(object sender, EventArgs e)
        {
            // code to handle mouse enter event
            button1.BackColor = Color.FromArgb(64, 65, 79);
            this.Cursor = Cursors.Default;
        }
        static string MetadataToString(CandidateTranscript transcript)
        {
            var nl = Environment.NewLine;
            string retval =
             Environment.NewLine + $"Recognized text: {string.Join("", transcript?.Tokens?.Select(x => x.Text))} {nl}"
             + $"Confidence: {transcript?.Confidence} {nl}"
             + $"Item count: {transcript?.Tokens?.Length} {nl}"
             + string.Join(nl, transcript?.Tokens?.Select(x => $"Timestep : {x.Timestep} TimeOffset: {x.StartTime} Char: {x.Text}"));
            return retval;
        }

        public string speechtoText()
        {

            string speechResult,pstr="";
            using (IDeepSpeech sttClient = new DeepSpeech("deepspeech-0.9.3-models.pbmm"))
            {
                string audioFile = "2830-3980-0043.wav";
                var waveBuffer = new WaveBuffer(File.ReadAllBytes(audioFile));
                using(var waveInfo=new WaveFileReader(audioFile))
                {
                   // DeepSpeechClient.Models.Metadata metaResult = sttClient.SpeechToTextWithMetadata(waveBuffer.ShortBuffer,
                   //     Convert.ToUInt32(waveBuffer.MaxSize / 2), 1);
                   //  speechResult = MetadataToString(metaResult.Transcripts[0]);
                    speechResult = sttClient.SpeechToText(waveBuffer.ShortBuffer, Convert.ToUInt32(waveBuffer.MaxSize / 2));

                }
                waveBuffer.Clear();

            };
           // richTextBox1.Text = speechResult;
            return speechResult;
        }
        private void InitializeBuffer()
        {
            // Create a graphics object for the off-screen buffer
            buffer = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), this.ClientRectangle);

            // Redraw the off-screen buffer
            RedrawBuffer();
        }

        private void RedrawBuffer()
        {
            // Clear the off-screen buffer
            buffer.Graphics.Clear(Color.White);

            // Draw your shapes, images, text, etc. onto the off-screen buffer
            buffer.Graphics.DrawLine(Pens.Black, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);

            // Transfer the off-screen buffer to the screen to be displayed
            buffer.Render(this.CreateGraphics());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Form1_SizeChanged(object se, EventArgs ee)
        {
            //MessageBox.Show(this.Bounds.Width.ToString());
            
            panel2.Size = new Size(this.Width / 5, this.Height);
            panel1.Size = new Size(this.Width * 4 / 5 - 10, this.Height * 3 / 4);
            panel1.Location = new Point(this.Width / 5 - 1, 0);
            panel3.Size = new Size(this.Width * 4 / 5, this.Height / 4);
            panel3.Location = new Point(this.Width / 5 - 1, this.Height * 3 / 4);
            richTextBox1.Size = new Size(panel3.Width * 7 / 10, panel3.Height / 5);
            richTextBox1.Location = new Point((panel3.Width - richTextBox1.Width) / 2, (panel3.Height - richTextBox1.Height) / 2);
            int[] columnWidths = { 1 };
            richTextBox1.SelectionTabs = columnWidths;
            button1.Size = new Size(richTextBox1.Width / 25, richTextBox1.Height * 3 / 4);
            button1.Location = new Point(richTextBox1.Location.X + richTextBox1.Width - button1.Width * 5 / 4, richTextBox1.Location.Y + richTextBox1.Height / 8);
            button2.Size = new Size(panel2.Width * 9 / 10, panel2.Height / 20);
            button2.Location = new Point((panel2.Width - button2.Width) / 2, button2.Height / 4);
            button3.Size = new Size(richTextBox1.Width / 15, richTextBox1.Height * 4 / 4);
            button3.Location = new Point(richTextBox1.Location.X - button1.Width * 2, richTextBox1.Location.Y);

            button1.ForeColor = Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            //  button1.Size = new Size(100, 40);
            button1.Font = new Font("Arial", 10, FontStyle.Regular);
            button1.Paint += (sender, e) =>
            {
                GraphicsPath path1 = new GraphicsPath();
                int cornerRadius1 = 5;
                int width = button1.Width;
                int height = button1.Height;
                path1.AddArc(0, 0, cornerRadius1, cornerRadius1, 180, 90);
                path1.AddArc(width - cornerRadius1, 0, cornerRadius1, cornerRadius1, 270, 90);
                path1.AddArc(width - cornerRadius1, height - cornerRadius1, cornerRadius1, cornerRadius1, 0, 90);
                path1.AddArc(0, height - cornerRadius1, cornerRadius1, cornerRadius1, 90, 90);
                path1.CloseFigure();
                button1.Region = new Region(path1);
            };
            button3.BackgroundImage = Image.FromFile(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "icons8-record-64.png"));
            button1.BackColor = Color.FromArgb(64, 65, 79);
            button3.BackColor = Color.FromArgb(68, 70, 84);
            button3.FlatStyle = FlatStyle.Flat;
            button3.FlatAppearance.BorderSize = 0;
            button3.Paint += (sender, e) =>
            {
                GraphicsPath path1 = new GraphicsPath();
                int cornerRadius1 = 2;
                int width = button3.Width;
                int height = button3.Height;
                path1.AddArc(0, 0, cornerRadius1, cornerRadius1, 180, 90);
                path1.AddArc(width - cornerRadius1, 0, cornerRadius1, cornerRadius1, 270, 90);
                path1.AddArc(width - cornerRadius1, height - cornerRadius1, cornerRadius1, cornerRadius1, 0, 90);
                path1.AddArc(0, height - cornerRadius1, cornerRadius1, cornerRadius1, 90, 90);
                path1.CloseFigure();
                button3.Region = new Region(path1);
            };
            button2.Text = "   +   New chat";
            button2.TextAlign = ContentAlignment.MiddleLeft;
            button1.FlatAppearance.BorderColor = Color.FromArgb(241, 198, 198);
            button2.Font = new Font("Arial", 12, FontStyle.Regular);
            button2.BackColor = Color.FromArgb(32, 33, 35);
            button2.ForeColor = Color.FromArgb(255, 255, 255);
            button2.FlatAppearance.BorderSize = 1;
            button2.Paint += (sender, e) =>
            {
                GraphicsPath path1 = new GraphicsPath();
                int cornerRadius1 = 5;
                int width = button2.Width;
                int height = button2.Height;
                path1.AddArc(0, 0, cornerRadius1, cornerRadius1, 180, 90);
                path1.AddArc(width - cornerRadius1, 0, cornerRadius1, cornerRadius1, 270, 90);
                path1.AddArc(width - cornerRadius1, height - cornerRadius1, cornerRadius1, cornerRadius1, 0, 90);
                path1.AddArc(0, height - cornerRadius1, cornerRadius1, cornerRadius1, 90, 90);
                path1.CloseFigure();
                button2.Region = new Region(path1);
            };

            richTextBox1.ForeColor = Color.FromArgb(242, 243, 244);
            richTextBox1.BackColor = Color.FromArgb(64, 65, 79);
            richTextBox1.Font = new Font("Arial", 15, FontStyle.Regular);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int cornerRadius = 2;
            int x = richTextBox1.Width;
            int y = richTextBox1.Height;
            path.AddArc(0, 0, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddLine(cornerRadius, 0, x - cornerRadius, 0);
            path.AddArc(x - cornerRadius * 2, 0, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddLine(x, cornerRadius * 2, x, y - cornerRadius * 2);
            path.AddArc(x - cornerRadius * 2, y - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            path.AddLine(x - cornerRadius, y, cornerRadius, y);
            path.AddArc(0, y - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.AddLine(0, y - cornerRadius * 2, 0, cornerRadius);
            richTextBox1.Region = new System.Drawing.Region(path);


            panel3.BackColor = Color.FromArgb(52, 54, 64);
            panel3.ForeColor = Color.FromArgb(31, 31, 33);
            richTextBox1.SelectionCharOffset = -10;

            panel2.BackColor = Color.FromArgb(32, 33, 35);
            panel2.AutoScroll = true;
            panel1.BackColor = Color.FromArgb(68, 70, 84);
            panel1.AutoScroll = true;

            string imagePath = "aaa.png";
            //   panel4.Size = new Size(richTextBox1.Width*3/4,richTextBox1.Height*2);
            panel4.Location = new Point(richTextBox1.Location.X + richTextBox1.Width / 7, richTextBox1.Location.Y - richTextBox1.Height * 5 / 4);
            for (int i = 0; i < 20; i++)
            {
                size[i] = 0;

                pb1[i] = new PictureBox();
                pb1[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pb1[i].Image = Image.FromFile(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, imagePath));
                pb1[i].Size = new Size(panel4.Width / 100, panel4.Height);
                pb1[i].Location = new Point(panel4.Width / 20 * i, 0);
                // panel4.Controls.Add(pb1[i]);
            }
            
            foreach (Panel p in panel2.Controls.OfType<Panel>())
            {
                p.Hide();
            }
            foreach (Panel p in panel1.Controls.OfType<Panel>())
            {
                p.Width = panel1.Width;
            }

            databaseConnect();
            databaseLoad();


        }

        private void ClearAndAddElements()
        {
            // Suspend layout logic to optimize repainting of the panel


            // Clear the panel
            panel1.Controls.Clear();

            // Add new elements to the panel
            panel1.Controls.Add(new Button() { Text = "Button 1", Location = new Point(10, 10) });
            panel1.Controls.Add(new Button() { Text = "Button 2", Location = new Point(10, 50) });
            panel1.Controls.Add(new Button() { Text = "Button 3", Location = new Point(10, 90) });

            // Resume layout logic to apply the changes and optimize repainting of the panel
            panel1.ResumeLayout();

            // Redraw the off-screen buffer and transfer it to the screen to be displayed
            RedrawBuffer();
        }

    }

}
