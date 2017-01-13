//Name: Amshar Basheer
//Project Name: chatClient
//File Name: chatClientForm.cs
//Date: 11/3/2014
//Description: This file contains the code for the form and its methods to handle events.  
//Note: the synchronous client socket example on MSDN was used as a starting point: http://msdn.microsoft.com/en-us/library/w89fhyex%28v=vs.110%29.aspx


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace chatClient
{
    public partial class chatClientForm : Form
    {
        private static bool shutdown { get; set; }
        private static Socket senderSocket;
        delegate void StringParameterDelegate(string value);
        private string name;
        private string serverIP;
        
        public chatClientForm()
        {
            InitializeComponent();
            //start with chat components invisible, which will later become visible once connection made
            recText.Visible = false;
            sendBtn.Visible = false;
            sendTextLabel.Visible = false;
            sendText.Visible = false;
        }


        //Method Name: connectBtn_Click
        //Parameters: object sender, EventArgs e
        //Return: void
        //Description: event handler for connect button being clicked.  First error checks name and server ip, and then attempts to connect to the server.
        //  Then will display connection message, start thread that deals with client messaging, and reveal chat components on form and hide connection ones.
        private void connectBtn_Click(object sender, EventArgs e)
        {
            
            try
            {
                string checkName;
                string checkServerIP;
                
                //get text from name and server text boxes so can error check
                checkName = nameBox.Text;
                checkServerIP = serverIPBox.Text;
                //borrowed regex expression for ip from http://stackoverflow.com/questions/4890789/regex-for-an-ip-address
                Match match = Regex.Match(checkServerIP, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", RegexOptions.IgnoreCase);

                if (checkName.Length > 0)
                {
                    name = checkName;
                }
                else
                {
                    MessageBox.Show("Error: must enter a name");
                    return;    
                }
                if (match.Success)
                {
                    serverIP = checkServerIP;
                }
                else
                {
                    MessageBox.Show("Error: invalid IP format");
                    return;    
                }

                // Connect to a remote device.
                // Establish the remote endpoint for the socket.
                // using port 11000 on the local computer.
                IPAddress ipAddress = IPAddress.Parse(serverIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    string connectMsg;
                    
                    senderSocket.Connect(remoteEP);
                    connectMsg = string.Format("Socket connected to {0}", senderSocket.RemoteEndPoint.ToString());
                    recText.Text = connectMsg;
                    
                    //once connect then make new thread that runs MsgCheck
                    Thread t = new Thread(new ThreadStart(MsgCheck));

                    //start thread
                    t.Start();
                    
                    //now that connected can reveal chat components and hide initial components used to connect
                    connectLabel.Visible = false;
                    nameLabel.Visible = false;
                    nameBox.Visible = false;
                    serverIPLabel.Visible = false;
                    serverIPBox.Visible = false;
                    connectBtn.Visible = false;
                    recText.Visible = true;
                    sendBtn.Visible = true;
                    sendTextLabel.Visible = true;
                    sendText.Visible = true;
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected exception : {0}", ex.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }


        //Method Name: MsgCheck
        //Parameters: none
        //Return: void
        //Description: loops to keep receiving messages and calling method to print the received messages. Once shutdown flag becomes true then closes socket.
        private void MsgCheck()
        {
            string recMsg;
            
            try
            {
                do
                {
                    // Data buffer for incoming data.
                    byte[] bytes = new byte[1024];

                    if (shutdown != true)
                    {
                        // Receive the response from the remote device.
                        int bytesRec = senderSocket.Receive(bytes);
                        recMsg = "";
                        recMsg = string.Format("{0}", Encoding.ASCII.GetString(bytes, 0, bytesRec - 5));
                        PrintRecMsg(recMsg);
                    }
                } while (shutdown != true);
                
                //release the socket
                senderSocket.Shutdown(SocketShutdown.Both);
                senderSocket.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception : {0}", ex.ToString());
            }
            
        }


        //Method Name: PrintRecMsg
        //Parameters: string msg: message to print in recText text box
        //Return: void
        //Description: if InvokeRequired then uses BeginInvoke before can print msg in recText text box
        private void PrintRecMsg(string msg)
        {

            if (InvokeRequired) //can't change elements on form directly from separate thread http://www.yoda.arachsys.com/csharp/threads/winforms.shtml
            {
                // We're not in the main thread, so we need to call BeginInvoke
                BeginInvoke(new StringParameterDelegate(PrintRecMsg), new object[] { msg });
                return;
            }

            recText.Text += "\r\n";
            recText.Text += msg;

        }


        //Method Name: sendBtn_Click
        //Parameters: object sender, EventArgs e
        //Return: void
        //Description: event for send button being clicked calls sendBtnPressed()
        private void sendBtn_Click(object sender, EventArgs e)
        {
            sendBtnPressed();
        }


        //Method Name: sendText_KeyDown
        //Parameters: object sender, KeyEventArgs e
        //Return: void
        //Description: if enter key pressed from sendText text box then calls sendBtnPressed()
        private void sendText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sendBtnPressed();
            }
        }


        //Method Name: sendBtnPressed
        //Parameters: none
        //Return: void
        //Description: gets input from sendText text box, clears the text box, and sends the message to server. If message was quit message then sets shutdown flag
        //  to true, and disables sendBtn and sendText text box
        private void sendBtnPressed()
        {
            string input;

            //make sure input clear before getting new input
            input = "";
            
            input = sendText.Text;
            sendText.Clear();

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(name + ": " + input + "<EOF>");

            // Send the data through the socket.
            int bytesSent = senderSocket.Send(msg);

            if (input == "quit")
            {
                shutdown = true;
                sendBtn.Enabled = false;
                sendText.Enabled = false;
            }
        }
    }
}
