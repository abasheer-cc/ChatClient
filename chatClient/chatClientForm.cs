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
using System.IO;
using System.Security.Cryptography;

namespace chatClient
{
    public partial class chatClientForm : Form
    {
        private static bool shutdown { get; set; }
        private static Socket senderSocket;
        delegate void StringParameterDelegate(string value);
        private string name;
        private string serverIP;
        private const int kKeyLength = 32;
        private const int kIvLength = 16;
        
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
                        recMsg = string.Format("{0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        //if read string as is and don't find <EOF> that means encrypted
                        if (recMsg.IndexOf("<EOF>") <= -1) //encrypted
                        {
                            PrintRecMsg(recMsg); //print encrypted msg

                            //determine actual filledBytesRec
                            int i = bytes.Length - 1;
                            while (bytes[i] == 0)
                            {
                                --i;
                            }
                            // now bytes[i] is at the last non-zero byte
                            int filledBytesRec = i + 1;

                            //make byte array of size of filled bytes received then copy into it the filled bytes of the 1024
                            byte[] filledBytes = new byte[filledBytesRec];
                            Array.Copy(bytes, 0, filledBytes, 0, filledBytesRec);

                            //make byte arrays for key, iv, and encrypted message
                            byte[] key = new byte[kKeyLength];
                            byte[] iv = new byte[kIvLength];
                            byte[] encryptedMsg = new byte[filledBytes.Length - kKeyLength - kIvLength];

                            //bytes received contains key in first kKeyLength bytes, then iv in next kIvLength, then rest is the encryped message
                            //copy respective portions into their byte arrays
                            Array.Copy(filledBytes, 0, key, 0, kKeyLength);
                            Array.Copy(filledBytes, kKeyLength, iv, 0, kIvLength);
                            Array.Copy(filledBytes, kKeyLength + kIvLength, encryptedMsg, 0, filledBytes.Length - kKeyLength - kIvLength);

                            //Decrypt the encryptedMsg bytes to a string.
                            recMsg = DecryptStringFromBytes_Aes(encryptedMsg, key, iv);                            
                        }

                        //print string without <EOF> (5 chars long) by using substring missing last 5 chars
                        PrintRecMsg(recMsg.Substring(0, recMsg.Length-5));
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
            byte[] encryptedMsgPlusAesData;
            
            //make sure input clear before getting new input
            input = "";
            
            input = sendText.Text;
            sendText.Clear();

            // Create a new instance of the AesManaged
            // class.  This generates a new key and initialization 
            // vector (IV).
            using (AesManaged myAes = new AesManaged())
            {
                byte[] encryptedMsg;
                // Encrypt the string to an array of bytes.
                encryptedMsg = EncryptStringToBytes_Aes(name + ": " + input + "<EOF>", myAes.Key, myAes.IV);

                //make a new byte array that contains the key, iv, and encrypted message
                encryptedMsgPlusAesData = new byte[kKeyLength + kIvLength + encryptedMsg.Length];
                Array.Copy(myAes.Key, 0, encryptedMsgPlusAesData, 0, kKeyLength);
                Array.Copy(myAes.IV, 0, encryptedMsgPlusAesData, kKeyLength, kIvLength);
                Array.Copy(encryptedMsg, 0, encryptedMsgPlusAesData, kKeyLength+kIvLength, encryptedMsg.Length);                
            }

            //Encode the data string into a byte array.
            //byte[] msg = Encoding.ASCII.GetBytes(name + ": " + input + "<EOF>");

            // Send the data through the socket.
            int bytesSent = senderSocket.Send(encryptedMsgPlusAesData);

            if (input == "quit")
            {
                shutdown = true;
                sendBtn.Enabled = false;
                sendText.Enabled = false;
            }
        }

        //this method encrypts a plainText string to an array of bytes using AES encryption Key and IV
        //This method was borrowed as is from: https://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aspx
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }

        //this method decrypts an encrypted array of bytes to a plainText string using AES encryption Key and IV
        //this method was borrowed as is from: https://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aspx
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }
    }
}
