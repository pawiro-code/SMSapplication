using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SMSapplication
{
    class Program
    {

        public static SMSapplication app;
               
        

       
        [STAThread]
        static async Task Main()
        {
            app = new SMSapplication();

            
            Application.Run(app);
            
        }
    }
}