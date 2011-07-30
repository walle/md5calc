using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MD5Calc
{
    public partial class MainForm : Form
    {
        #region // Private data

        private StreamReader dataStreamReader = null;
        private StreamWriter dataStreamWriter = null;
        private FileStream dataFileStream = null;
        private StreamReader wordsStreamReader = null;
        private StreamWriter wordsStreamWriter = null;
        private FileStream wordsFileStream = null;

        private bool saveWords = Properties.Settings.Default.SaveWords;
        private bool saveData = Properties.Settings.Default.SaveData;

        #endregion

        #region // Constructor

        public MainForm()
        {
            InitializeComponent();

            // Set the properties
            saveDataToolStripMenuItem.Checked = saveData;
            saveWordsToolStripMenuItem.Checked = saveWords;

            // Initialize the streams
            dataFileStream = new FileStream("data.xyz", FileMode.OpenOrCreate);
            dataStreamReader = new StreamReader(dataFileStream);
            dataStreamWriter = new StreamWriter(dataFileStream);
            wordsFileStream = new FileStream("words.xyz", FileMode.OpenOrCreate);
            wordsStreamReader = new StreamReader(wordsFileStream);
            wordsStreamWriter = new StreamWriter(wordsFileStream);

            // Read the current data and load it to the output textbox
            outputTextBox.Text = dataStreamReader.ReadToEnd();

            // Read the words and load it to the append box
            string[] words = wordsStreamReader.ReadToEnd().Split(wordsStreamWriter.NewLine.ToCharArray());
            inputTextBox.AutoCompleteCustomSource.AddRange(words);

            // Close the files
            dataFileStream.Close();
            wordsFileStream.Close();
        }

        #endregion

        #region // Eventhandlers

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save the append words
            if (saveWords)
            {
                // Truncate the file and fill it        
                if (File.Exists("words.xyz"))
                {
                    wordsFileStream = new FileStream("words.xyz", FileMode.Truncate);
                }
                else
                {
                    wordsFileStream = new FileStream("words.xyz", FileMode.Create);
                }
                wordsStreamReader = new StreamReader(wordsFileStream);
                wordsStreamWriter = new StreamWriter(wordsFileStream);

                for (int i = 0; i < inputTextBox.AutoCompleteCustomSource.Count; i++)
                {
                    if (inputTextBox.AutoCompleteCustomSource[i].Length > 0)
                    {
                        wordsStreamWriter.WriteLine(inputTextBox.AutoCompleteCustomSource[i]);
                    }
                }
                wordsStreamWriter.Flush();
            }

            // Save the data 
            if (saveData)
            {
                // Truncate the file and fill it
                if (File.Exists("data.xyz"))
                {
                    dataFileStream = new FileStream("data.xyz", FileMode.Truncate);
                }
                else
                {
                    dataFileStream = new FileStream("data.xyz", FileMode.Create);
                }
                dataStreamReader = new StreamReader(dataFileStream);
                dataStreamWriter = new StreamWriter(dataFileStream);

                dataStreamWriter.Write(outputTextBox.Text);
                dataStreamWriter.Flush();
            }
        }

        private void calcButton_Click(object sender, EventArgs e)
        {
            // Insert data in the output textbox
            outputTextBox.Text = inputTextBox.Text + " -> " + Md5(inputTextBox.Text) + "\r\n" + outputTextBox.Text; ;

            // Save the word
            if (inputTextBox.Text.Length > 0)
            {
                int index = inputTextBox.AutoCompleteCustomSource.IndexOf(inputTextBox.Text);
                if (index < 0)
                {
                    inputTextBox.AutoCompleteCustomSource.Add(inputTextBox.Text);
                }
            }

            // Clear the input
            inputTextBox.Text = "";
        }

        private void saveWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveWords = !saveWords;
            saveWordsToolStripMenuItem.Checked = saveWords;
            Properties.Settings.Default.SaveWords = saveWords;

            Properties.Settings.Default.Save();
        }

        private void saveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveData = !saveData;
            saveDataToolStripMenuItem.Checked = saveData;
            Properties.Settings.Default.SaveData = saveData;

            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }     

        private void clearDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearOutput();
        }

        private void clearWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearInput();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearInput();
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.ClearOutput();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Simple MD5 calculator that outputs PHP, MySQL and other UNIX compliant MD5 hashes.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region // Private functions

        private string Md5(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        void ClearInput()
        {
            inputTextBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            if (File.Exists("words.xyz"))
            {
                File.Delete("words.xyz");
            }
        }

        void ClearOutput()
        {
            outputTextBox.Text = "";
            if (File.Exists("data.xyz"))
            {
                File.Delete("data.xyz");
            }
        }

        #endregion
    }
}