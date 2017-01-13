//Name: Amshar Basheer
//Project Name: chatClient
//File Name: Program.cs
//Date: 11/3/2014
//Description: The chatClient allows the user to connect to the chatServer program (chatServer must be running) and then send messages to the server, which will
//  be received by all chatClient instances that are connected to the same server.  Multiple instances (one, two, or more) of the chatClient can be connected 
//  to the chatServer.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chatClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new chatClientForm());
        }
    }
}
