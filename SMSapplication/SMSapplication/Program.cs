using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SMSapplication
{
    class Program
    {

        public static SMSapplication app;

        public class Data
        {
            //public SMSapplication app { get; set; }
            //public int Result { get; set; }
        }
        public static async Task MyMethodAsync()
        {
            app = new SMSapplication();
            Task<SMSapplication> longRunningTask = Tick();
            // independent work which doesn't need the result of LongRunningOperationAsync can be done here
            Console.WriteLine("Independent Works of now executes in MyMethodAsync()");
            //and now we call await on the task 
            SMSapplication result = await longRunningTask;
            //use the result 
            Console.WriteLine("Result of LongRunningOperation() is " + result);
        }

       // public static async Task<int> LongRunningOperation() // assume we return an int from this long running operation 
        //{
        //    Console.WriteLine("LongRunningOperation() Started");
        //    await Task.Delay(2000); // 2 second delay
         //   Console.WriteLine("LongRunningOperation() Finished after 2 Seconds");
        //    return 1;
        //}
        public static async Task<SMSapplication> Tick()
        {
            await Task.Delay(2000);
            //app = new SMSapplication();
            int last = app.CountSMS;
            int last2 = app.CountSMS2;
            //int last2 = 0;
            app.CountSMS = 0;
            //CountSMS2 = 0;
            if (app.button1.Enabled == false)
            {
                try
                {
                    string strCommand = "AT+CMGL=\"ALL\"";
                    app.objShortMessageCollection = app.smsHelper.ReadSMS(app.port, strCommand);
                    if (app.textBox3.Text == "")
                    {

                        int uCountSMS = app.smsHelper.CountSmsMessages(app.port);
                        //CountSMS = uCountSMS;
                        if (uCountSMS > 0)
                        {


                            app.RemoveMessagesFromListView();
                            foreach (ShortMessage msg in app.objShortMessageCollection)
                            {
                                app.CountSMS++;

                                ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });
                                item.Tag = msg;
                                app.lvwMessages.Items.Add(item);

                            }


                        }
                        if (app.CountSMS > last)
                        {
                            // new ToastContentBuilder().AddText("Nowy SMS").Show();
                            app.flaga = true;
                        }



                    }
                    else
                    {

                        app.lvwMessages.Items.Clear();
                        app.CountSMS2 = 0;
                        foreach (ShortMessage msg in app.objShortMessageCollection)
                        {
                            app.CountSMS2++;
                            bool check2 = app.AreStringsSame(msg.Sender.Remove(0, 3), app.textBox3.Text);
                            if (check2 == true)
                            {


                                ListViewItem item = new ListViewItem(new string[] { msg.Index, msg.Sent, msg.Sender, msg.Message });

                                item.Tag = msg;

                                app.lvwMessages.Items.Add(item);
                                // backgroundWorker1.ReportProgress(i);
                            }
                        }

                        if ((app.CountSMS2 - 1 == last2) && (last2 != 0))
                        {
                            // new ToastContentBuilder().AddText("Nowy SMS").Show();
                            app.flaga = true;
                        }


                    }


                }

                catch (Exception ex)
                {
                    app.ErrorLog(ex.Message);
                }
            }
           // Application.Run(new SMSapplication());
            return app;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            app = new SMSapplication();

            //Application.Run(new SMSapplication());

            //var updateTask = Tick();
            // Task.Delay(1000);
            Application.Run(app);
            //SMSapplication form = new SMSapplication();
            //
           // while (true)
          //  {

            //    var a = MyMethodAsync(); //Task started for Execution and immediately goes to Line 19 of the code. Cursor will come back as soon as await operator is met       
                //Console.WriteLine("Cursor Moved to Next Line Without Waiting for MyMethodAsync() completion");
                //Console.WriteLine("Now Waiting for Task to be Finished");
            //    Task.WaitAll(a); //Now Waiting      
                //Console.WriteLine("Exiting CommandLine");
                //wait Task.Run(() => Tick());
                //var weather = await Tick();
                
          //  }
            //Console.WriteLine("Eggs are ready");
            // SMSapplication app = await SMSapplication();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

           /// Application.Run(new SMSapplication());
        }
    }
}