
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SMSapplication
{

    public partial class SMSapplication : Form
    {

        #region Constructor
        public SMSapplication()
        {
            InitializeComponent();
            //rbReadAll.Checked = true;
            lvwMessages.Columns[1].Width = 130;
            lvwMessages.Columns[3].Width = 400;
            
        }
        #endregion

        #region Private Variables
        public SerialPort port = new SerialPort();
        public SmsHelper smsHelper = new SmsHelper();
        public ShortMessageCollection objShortMessageCollection = new ShortMessageCollection();
        public ShortContactCollection objShortContactCollection = new ShortContactCollection();
        #endregion

        #region Private Methods

        #region Write StatusBar
        //private static System.Threading.Timer timer;
        private static void TimerTask(object timerState)
        {
            //Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: starting a new callback.");
            var state = timerState as TimerState;
            Interlocked.Increment(ref state.Counter);
        }
        public int iter = 0;
        private async Task timer11()
        {
            await Task.Run(() => Tick());
        }
        public async void Tick2()
        {
            int i = 0;
            //objShortContactCollection = smsHelper.ReadContact(port, "AT+CPBR=");

            //iter++;
            


            //backgroundWorker1.RunWorkerAsync();
            try
            {
                button4.Enabled = false;
                string strCommand = "AT+CPBR=";
                ShortContactCollection objShortContactCollection = smsHelper.ReadContact(this.port, strCommand);
                
               foreach (ShortContact contact in objShortContactCollection)
                {
                    //do paska ladowania
                    if(iter<250)
                        iter++;
                    await DoSomeWork();
                    // progressBar1.Value = contact.Index;
                    //backgroundWorker1.RunWorkerAsync();
                    if (contact.Name != null)
                    {
                        contact.Name = contact.Name.Remove(contact.Name.Length - 3, 3);
                        contact.Name = contact.Name.Substring(1, contact.Name.Length - 1);
                        contact.Name = contact.Name.Substring(0, contact.Name.Length - 1);
                        contact.Name = contact.Name.Substring(0, contact.Name.Length - 3);

                        contact.Number = contact.Number.Substring(1, contact.Number.Length - 1);
                        contact.Number = contact.Number.Substring(0, contact.Number.Length - 1);
                        ListViewItem item = new ListViewItem(new string[] { contact.Name, contact.Number, contact.Name, contact.National });
                        //textBox2.Text = contact.Id;
                        // textBox2.Text = contact.Number;
                        // textBox2.Text = contact.Name;
                        //textBox2.Text = contact.National;

                        //textBox2.Text = contact.Name;
                        //progressBar1.Value = ++i;
                        item.Tag = contact;
                        listView2.Invoke(new Action(delegate ()
                        {
                            listView2.Items.Add(item);
                        }));
                        listView2.Invoke(new Action(delegate ()
                        {
                            listView2.Refresh();
                        }));
                    }
                    // backgroundWorker1.ReportProgress(i);

                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }
        public async void Tick()
        {
            //await Task.Delay(2000);
            //app = new SMSapplication();
            int last = CountSMS;
            int last2 = CountSMS2;
            //int last2 = 0;
            CountSMS = 0;
            //CountSMS2 = 0;
            //if (button1.Enabled == false)
           // {
                try
                {
                    string strCommand = "AT+CMGL=\"ALL\"";
                    this.objShortMessageCollection = smsHelper.ReadSMS(port, strCommand);
                    if (textBox3.Text == "")
                    {

                        int uCountSMS = smsHelper.CountSmsMessages(port);
                        //CountSMS = uCountSMS;
                        if (uCountSMS > 0)
                        {


                            //RemoveMessagesFromListView();

                            for (int i = lvwMessages.Items.Count - 1; i >= 0; i--)
                            {
                            lvwMessages.Invoke(new Action(delegate ()
                            {
                                lvwMessages.Items[i].Remove();
                            }));
                            }

                        lvwMessages.Invoke(new Action(delegate ()
                        {
                            lvwMessages.Refresh();
                        }));
                         

                    

                        foreach (ShortMessage msg in objShortMessageCollection)
                            {
                                CountSMS++;

                                ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                                item.Tag = msg;
                                //lvwMessages.Items.Add(item);
                                lvwMessages.Invoke(new Action(delegate ()
                                {
                                    lvwMessages.Items.Add(item);
                                }));

                            }


                        }
                        if (CountSMS > last)
                        {
                            new ToastContentBuilder().AddText("Nowy SMS").Show();
                            flaga = true;
                        }



                    }
                    else
                    {

                        lvwMessages.Items.Clear();
                        CountSMS2 = 0;
                        foreach (ShortMessage msg in objShortMessageCollection)
                        {
                            CountSMS2++;
                            bool check2 =AreStringsSame(msg.Sender.Remove(0, 3), textBox3.Text);
                            if (check2 == true)
                            {


                                ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });

                                item.Tag = msg;

                                lvwMessages.Invoke(new Action(delegate ()
                                {
                                    lvwMessages.Items.Add(item);
                                }));
                                // backgroundWorker1.ReportProgress(i);
                            }
                        }

                        if ((CountSMS2 - 1 == last2) && (last2 != 0))
                        {
                            new ToastContentBuilder().AddText("Nowy SMS").Show();
                            flaga = true;
                        }


                    }


                }

                catch (Exception ex)
                {
                    ErrorLog(ex.Message);
                }
            
            //return true;
            
        }

        class TimerState
        {
            public int Counter;
        }
       

        public  int CountSMS=0;
        public  int CountSMS2 = 0;
        public bool flaga = false;

        public void RemoveMessagesFromListView()
        {
            //for (int i = 0; i < CountSMS; i++)
            for (int i = lvwMessages.Items.Count - 1; i >= 0; i--)
            {
                lvwMessages.Items[i].Remove();               
            }
            lvwMessages.Refresh();
        }

        

        private void WriteStatusBar(string status)
        {
            try
            {
                //statusBar1.Text = "Message: " + status;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #endregion

        #region Private Events

        private void SMSapplication_Load(object sender, EventArgs e)
        {
            try
            {
                #region Display all available COM Ports
                string[] ports = SerialPort.GetPortNames();

                // Add all port names to the combo box:
                foreach (string port in ports)
                {
                    //this.cboPortName.Items.Add(port);
                }
                #endregion

                //Remove tab pages
                //this.tabSMSapplication.TabPages.Remove(tbSendSMS);
                //this.tabSMSapplication.TabPages.Remove(tbReadSMS);
                // this.tabSMSapplication.TabPages.Remove(tbDeleteSMS);
                // Display the ProgressBar control.
                pBar1.Visible = true;
                // Set Minimum to 1 to represent the first file being copied.
                pBar1.Minimum = 0;
                // Set Maximum to the total number of files to copy.
                pBar1.Maximum = 250;
                // Set the initial value of the ProgressBar.
                pBar1.Value = 0;
                // Set the Step property to a value of 1 to represent each file being copied.
                pBar1.Step = 1;
                rbReadAll.Enabled = false;
                this.btnDisconnect.Enabled = false;
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //Open communication port 
                this.port = smsHelper.OpenPort("COM5", 9600, 8, 100, 100);

                //if (this.port != null)
               // {
                    //this.gboPortSettings.Enabled = false;

                    //MessageBox.Show("Modem is connected at PORT " + this.cboPortName.Text);
                   // this.statusBar1.Text = "Modem is connected at PORT " + this.cboPortName.Text;

                    //Add tab pages
                    this.tabSMSapplication.TabPages.Add(tbSendSMS);
                    //this.tabSMSapplication.TabPages.Add(tbReadSMS);
                    //this.tabSMSapplication.TabPages.Add(tbDeleteSMS);

                   // this.lblConnectionStatus.Text = "Connected at " + this.cboPortName.Text;
                    this.btnDisconnect.Enabled = true;
                //}
               // else
               // {
                    //MessageBox.Show("Invalid port settings");
               //     this.statusBar1.Text = "Invalid port settings";
              //  }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
               // this.gboPortSettings.Enabled = true;
                smsHelper.ClosePort(this.port);

                //Remove tab pages
                // this.tabSMSapplication.TabPages.Remove(tbSendSMS);
                //this.tabSMSapplication.TabPages.Remove(tbReadSMS);
                //  this.tabSMSapplication.TabPages.Remove(tbDeleteSMS);

                this.lblConnectionStatus.ForeColor = System.Drawing.Color.Red;
                this.lblConnectionStatus.Text = "Niepo³¹czony";
                this.btnDisconnect.Enabled = false;
                this.button1.Enabled = true;

            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }

        private void btnSendSMS_Click(object sender, EventArgs e)
        {
           // this.statusBar1.Text = "";
            //.............................................. Send SMS ....................................................
            try
            {

                if (smsHelper.SendMessage(this.port, this.txtSIM.Text, this.txtMessage.Text))
                {
                    //MessageBox.Show("Message has sent successfully");
                  //  this.statusBar1.Text = "Message has sent successfully";
                }
                else
                {
                    //MessageBox.Show("Failed to send message");
                   // this.statusBar1.Text = "Failed to send message";
                }

            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }
        private void btnReadSMS_Click(object sender, EventArgs e)
        {
           // this.statusBar1.Text = "";
            //RemoveMessagesFromListView();
            try
            {
                //count SMS 
                int uCountSMS = smsHelper.CountSmsMessages(this.port);
                if (uCountSMS > 0)
                {

                    #region Command
                    string strCommand = "AT+CMGL=\"ALL\"";

                    if (this.rbReadAll.Checked)
                    {
                        strCommand = "AT+CMGL=\"ALL\"";
                    }
                    //else if (this.rbReadUnRead.Checked)
                    //{
                     //   strCommand = "AT+CMGL=\"REC UNREAD\"";
                    //}
                    //else if (this.rbReadStoreSent.Checked)
                    //{
                    //    strCommand = "AT+CMGL=\"STO SENT\"";
                    //}
                    //else if (this.rbReadStoreUnSent.Checked)
                    //{
                    //    strCommand = "AT+CMGL=\"STO UNSENT\"";
                    //}
                    #endregion

                    // If SMS exist then read SMS
                    #region Read SMS
                    //.............................................. Read all SMS ....................................................
                    objShortMessageCollection = smsHelper.ReadSMS(this.port, strCommand);

                    //RemoveMessagesFromListView();

                    foreach (ShortMessage msg in objShortMessageCollection)
                    {
                        ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                        item.Tag = msg;
                        lvwMessages.Items.Add(item);
                    }
                    #endregion
                }
                else
                {
                    //RemoveMessagesFromListView();
                    //this.statusBar1.Text = "There is no message in SIM";
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }

        

        /*private void btnDeleteSMS_Click(object sender, EventArgs e)
        {
           // this.statusBar1.Text = "";
            try
            {
                //Count SMS 
                int uCountSMS = smsHelper.CountSmsMessages(this.port);
                if (uCountSMS > 0)
                {
                    DialogResult dr = MessageBox.Show("Are u sure u want to delete the SMS?", "Delete confirmation", MessageBoxButtons.YesNo);

                    if (dr.ToString() == "Yes")
                    {
                        #region Delete SMS

                        if (this.rbDeleteAllSMS.Checked)
                        {
                            //...............................................Delete all SMS ....................................................

                            #region Delete all SMS
                            string strCommand = "AT+CMGD=1,4";
                            if (smsHelper.DeleteMessage(this.port, strCommand))
                            {
                                //MessageBox.Show("Messages has deleted successfuly ");
                                this.statusBar1.Text = "Messages has deleted successfuly";
                            }
                            else
                            {
                                //MessageBox.Show("Failed to delete messages ");
                                this.statusBar1.Text = "Failed to delete messages";
                            }
                            #endregion

                        }
                        else if (this.rbDeleteReadSMS.Checked)
                        {
                            //...............................................Delete Read SMS ....................................................
                            #region Delete Read SMS
                            string strCommand = "AT+CMGD=1,3";
                            if (smsHelper.DeleteMessage(this.port, strCommand))
                            {
                                //MessageBox.Show("Messages has deleted successfuly");
                                this.statusBar1.Text = "Messages has deleted successfuly";
                            }
                            else
                            {
                                //MessageBox.Show("Failed to delete messages ");
                                this.statusBar1.Text = "Failed to delete messages";
                            }
                            #endregion

                        }
                        else if (this.rbDeleteByIndex.Checked)
                        {
                            //...............................................Delete SMS By Index ....................................................
                            #region Delete Read SMS
                            if (txtDeleteIndex.Text.Equals(""))
                            {
                                MessageBox.Show("Please Specify Delete Message Index");
                                return;
                            }
                            string strCommand = "AT+CMGD="+ txtDeleteIndex.Text.Trim();
                            if (smsHelper.DeleteMessage(this.port, strCommand))
                            {
                                this.statusBar1.Text = "Messages has deleted successfuly";
                            }
                            else
                            {
                                this.statusBar1.Text = "Failed to delete messages";
                            }
                            #endregion
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }

        }*/
        private void btnCountSMS_Click(object sender, EventArgs e)
        {
           // this.statusBar1.Text = "";
            try
            {
                //Count SMS
                int uCountSMS = smsHelper.CountSmsMessages(this.port);
              //  this.txtCountSMS.Text = uCountSMS.ToString();
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
        }

        #endregion

        #region Error Log
        public void ErrorLog(string Message)
        {
            StreamWriter sw = null;

            try
            {
                WriteStatusBar(Message);

                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
                //string sPathName = @"E:\";
                string sPathName = @"SMSapplicationErrorLog_";

                string sYear = DateTime.Now.Year.ToString();
                string sMonth = DateTime.Now.Month.ToString();
                string sDay = DateTime.Now.Day.ToString();

                string sErrorTime = sDay + "-" + sMonth + "-" + sYear;

                sw = new StreamWriter(sPathName + sErrorTime + ".txt", true);

                sw.WriteLine(sLogFormat + Message);
                sw.Flush();

            }
            catch (Exception ex)
            {
                //ErrorLog(ex.ToString());
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                    sw.Close();
                }
            }

        }
        #endregion

        private void lvwMessages_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lvwMessages.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;
            ShortMessage shortMessage = (ShortMessage)item.Tag;

            if (item != null)
            {
                MessageBox.Show(shortMessage.Message);
            }
            else
            {
                this.lvwMessages.SelectedItems.Clear();
                MessageBox.Show("No Item is selected");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //await timer11();
            //await Task.Run(() => Tick());
            try
            {
                //Open communication port 
                this.port = smsHelper.OpenPort(this.textBox1.Text, 9600, 8, 100, 100);

                this.lblConnectionStatus.Text = "Po³¹czony";
                this.lblConnectionStatus.ForeColor = System.Drawing.Color.Green;
                this.button1.Enabled = false;
                //if (this.port != null)
                // {
                //this.gboPortSettings.Enabled = false;

                //MessageBox.Show("Modem is connected at PORT " + this.cboPortName.Text);
                // this.statusBar1.Text = "Modem is connected at PORT " + this.cboPortName.Text;

                //Add tab pages
                //this.tabSMSapplication.TabPages.Add(tbSendSMS);
                //this.tabSMSapplication.TabPages.Add(tbReadSMS);
                //this.tabSMSapplication.TabPages.Add(tbDeleteSMS);

                // this.lblConnectionStatus.Text = "Connected at " + this.cboPortName.Text;
                this.btnDisconnect.Enabled = true;
                //}
                // else
                // {
                //MessageBox.Show("Invalid port settings");
                //     this.statusBar1.Text = "Invalid port settings";
                //  }
                
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
            
                    
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //RemoveMessagesFromListView();
            // Not  5, your TFM must be net5.0-windows10.0.17763.0 or greater

                                                                  // timer3.Tick += new System.EventHandler(OnTimerEvent);
            try
            {
                //count SMS 
                int uCountSMS = smsHelper.CountSmsMessages(this.port);
                //CountSMS = uCountSMS;
                if (uCountSMS > 0)
                {
                    rbReadAll.Enabled = true;
                    //CountSMS = uCountSMS;
                   // timer3.Enabled = true;
                    
                    #region Command
                    string strCommand = "AT+CMGL=\"ALL\"";

                    if (this.rbReadAll.Checked)
                    {
                        strCommand = "AT+CMGL=\"ALL\"";
                    }
                    //else if (this.rbReadUnRead.Checked)
                    //{
                    //   strCommand = "AT+CMGL=\"REC UNREAD\"";
                    //}
                    //else if (this.rbReadStoreSent.Checked)
                    //{
                    //    strCommand = "AT+CMGL=\"STO SENT\"";
                    //}
                    //else if (this.rbReadStoreUnSent.Checked)
                    //{
                    //    strCommand = "AT+CMGL=\"STO UNSENT\"";
                    //}
                    #endregion

                    // If SMS exist then read SMS
                    #region Read SMS
                    //.............................................. Read all SMS ....................................................
                    objShortMessageCollection = smsHelper.ReadSMS(this.port, strCommand);

                    //RemoveMessagesFromListView();
                    

                    foreach (ShortMessage msg in objShortMessageCollection)
                    {
                        ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                        item.Tag = msg;
                        lvwMessages.Items.Add(item);
                        CountSMS++;
                    }

                    //this.button3.Enabled = false;
                    lvwMessages.Items[lvwMessages.Items.Count - 1].EnsureVisible();
                    #endregion
                }
                else
                {
                    //RemoveMessagesFromListView();
                    //this.statusBar1.Text = "There is no message in SIM";
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }
            
        }

        private void gboSendSMS_Enter(object sender, EventArgs e)
        {

        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            /*int last = CountSMS;
            int last2 = CountSMS2;
            //int last2 = 0;
            CountSMS = 0;
            //CountSMS2 = 0;
            
            try
            {
                string strCommand = "AT+CMGL=\"ALL\"";
                objShortMessageCollection = smsHelper.ReadSMS(this.port, strCommand);
                if (textBox3.Text == "")
                {

                    int uCountSMS = smsHelper.CountSmsMessages(this.port);
                    //CountSMS = uCountSMS;
                    if (uCountSMS > 0)
                    {

                        
                        this.RemoveMessagesFromListView();
                        foreach (ShortMessage msg in objShortMessageCollection)
                        {
                            CountSMS++;

                            ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                            item.Tag = msg;
                            lvwMessages.Items.Add(item);

                        }

                      
                    }
                    if (CountSMS > last)
                    {
                        new ToastContentBuilder().AddText("Nowy SMS").Show();
                        flaga = true;
                    }
                   


                }
                else
                {
                    
                    lvwMessages.Items.Clear();
                    CountSMS2 = 0;
                    foreach (ShortMessage msg in objShortMessageCollection)
                    {
                        CountSMS2++;
                        bool check2 = AreStringsSame(msg.Sender.Remove(0, 3), textBox3.Text);
                        if (check2 == true)
                        {

                            
                            ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                            
                            item.Tag = msg;

                            lvwMessages.Items.Add(item);
                            // backgroundWorker1.ReportProgress(i);
                        }
                    }

                    if ((CountSMS2-1 == last2)&&(last2!=0))
                    {
                        new ToastContentBuilder().AddText("Nowy SMS").Show();
                        flaga = true;
                    }
                    
                    
                }
              

            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }*/
        }




        public async Task DoSomeWork()
        {

            pBar1.Invoke(new Action(delegate ()
            {
                pBar1.Value = iter;
            }));
            
            //iter++;
        }
    




    private  void button4_Click(object sender, EventArgs e)
        {
            int i = 0;

            //progressBar1.Maximum = 100;
            //progressBar1.Step = 1;
            //progressBar1.Value = 0;

            objShortContactCollection = smsHelper.ReadContact(this.port, "AT+CPBR=");
            backgroundWorker1.RunWorkerAsync();
            try
            {
                string strCommand = "AT+CPBR=";
                //ShortContactCollection objShortContactCollection = smsHelper.ReadContact(this.port, strCommand);

                foreach (ShortContact contact in objShortContactCollection)
                {
                    // progressBar1.Value = contact.Index;
                    //backgroundWorker1.RunWorkerAsync();
                    contact.Name = contact.Name.Remove(contact.Name.Length - 3, 3);
                    contact.Name = contact.Name.Substring(1, contact.Name.Length - 1);
                    contact.Name = contact.Name.Substring(0, contact.Name.Length - 1);
                    contact.Name = contact.Name.Substring(0, contact.Name.Length - 3);

                    contact.Number = contact.Number.Substring(1, contact.Number.Length - 1);
                    contact.Number = contact.Number.Substring(0, contact.Number.Length - 1);
                    ListViewItem item = new ListViewItem(new string[] { contact.Name, contact.Number, contact.Name, contact.National });
                    //textBox2.Text = contact.Id;
                    // textBox2.Text = contact.Number;
                    // textBox2.Text = contact.Name;
                    //textBox2.Text = contact.National;

                    //textBox2.Text = contact.Name;
                    //progressBar1.Value = ++i;
                    item.Tag = contact;

                    listView2.Items.Add(item);
                    // backgroundWorker1.ReportProgress(i);

                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex.Message);
            }



        }

        

      


        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {

            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem item = listView2.SelectedItems[0];
                txtSIM.Text = item.SubItems[1].Text.ToString();
                textBox3.Text = item.SubItems[1].Text.ToString();
                textBox3.Text = textBox3.Text.Remove(0, 3);
            }
            lvwMessages.Items.Clear();
            foreach (ShortMessage msg in objShortMessageCollection)
            {
                
                bool check2 = AreStringsSame(msg.Sender.Remove(0, 3), textBox3.Text);
                if (check2 == true)
                {
                    

                    ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                   

                    
                    item.Tag = msg;

                    lvwMessages.Items.Add(item);
                    
                }
            }
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            int sortColumn = 0; 
            if (e.Column == sortColumn)
            {
                if (listView2.Sorting == SortOrder.Ascending)
                {
                    listView2.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView2.Sorting = SortOrder.Ascending;
                }
            }
            listView2.Sort();

        }
        

        private void textBox2_Enter(object sender, KeyEventArgs e)
        {

        }

        public bool AreStringsSame(string s1, string s2)
        {
            string capTestStr = s1.ToUpper();
            if (capTestStr.Contains(s2.ToUpper()))
                return true;

            return false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            foreach (ShortContact contact in objShortContactCollection)
            {
                //bool check = contact.Name.Contains(textBox2.Text);

                bool check = AreStringsSame(contact.Name, textBox2.Text);
                
                if (check == true)
                {
                    

                    ListViewItem item = new ListViewItem(new string[] { contact.Name, contact.Number, contact.Name, contact.National });
                    

                   
                    item.Tag = contact;

                    listView2.Items.Add(item);
                   
                }

                

            }
        
        listView2.Refresh();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            lvwMessages.Items.Clear();
            foreach (ShortMessage msg in objShortMessageCollection)
            {
                //bool check = contact.Name.Contains(textBox2.Text);
                //msg.Sender =
                bool check = AreStringsSame(msg.Sender.Remove(0, 3), textBox3.Text);
                if (check == true)
                {
                   

                    ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                    
                    item.Tag = msg;

                    lvwMessages.Items.Add(item);
                    
                }
            }
        }

        private void rbReadAll_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        
    }
}
