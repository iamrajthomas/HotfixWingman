//  -------------------------------------------------------------------------
//  <copyright file="Form1.cs"  author="Rajesh Thomas | iamrajthomas" >
//      Copyright (c) 2022 All Rights Reserved.
//  </copyright>
// 
//  <summary>
//       Form1
//  </summary>
//  -------------------------------------------------------------------------

namespace HotfitBot
{
    using HotfitBot.Class;
    using HotfitBot.Constants;
    using HotfitBot.Interface;
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Form1
    /// </summary>
    public partial class Form1 : Form
    {
        // To make use of these 
        private static string SourcePathInput = null;
        private static string BackupPathInput = null;
        private static string DestinationPathInput = null;
        private static bool CreateBackupFlag = false;
        private readonly string HotFixBackupFolderName = null;

        // Dependencies
        private readonly Logger _logger = null;
        private readonly ReadAppConfigData _readAppConfigData = null;

        public Form1()
        {
            InitializeComponent();

            _logger = new Logger();
            _readAppConfigData = new ReadAppConfigData();
            HotFixBackupFolderName =_readAppConfigData.ReadValueByKey("HotFixBackupFolderName") != null ? _readAppConfigData.ReadValueByKey("HotFixBackupFolderName") : HotfixConstant.DefaultHotFixBackupFolderName;
            InitialDefaultLoadForInputs();
        }

        private void FormHotfix_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void textBoxSource_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.Text != null && tb.Text != "")
            {
                SourcePathInput = tb.Text;
                BackupPathInput = tb.Text + @"\" + HotFixBackupFolderName;
            }
            else
            {
                SourcePathInput = null;
                BackupPathInput = null;
            }
        }

        private void textBoxDestination_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && tb.Text != null && tb.Text != "")
            {
                DestinationPathInput = tb.Text;
            }
            else
            {
                DestinationPathInput = null;
            }
        }

        private void cbCreateBackup_CheckedChanged(object sender, EventArgs e)
        {
            var cb = sender as CheckBox;
            if(cb != null)
            {
                CreateBackupFlag = cb.Checked;
            }
            else
            {
                CreateBackupFlag = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCreatePatch_Click(object sender, EventArgs e)
        {
            Initiate();
        }


        /// <summary>
        /// Initiate - Entry Point of the Hotfix routine
        /// </summary>
        public void Initiate()
        {
            try
            {
                if(SourcePathInput == null || SourcePathInput == "" || DestinationPathInput == null || DestinationPathInput == "")
                {
                    MessageBox.Show("Source/ Destination are Empty. Please provide correct data.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _logger.PrintDebugLogs($"Form => Initiate() :: STARTED");

                DialogResult result = MessageBox.Show("Are you sure, you want to create a patch ?", "Confirmation Window", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    IHotfix hf = new Hotfix();
                    var OK = hf.Initiate(SourcePathInput, DestinationPathInput, BackupPathInput, CreateBackupFlag);

                    if (OK)
                    {
                        InitialDefaultLoadForInputs(IsClean: true);
                        MessageBox.Show("Patch Created Successfully..!!", "Operation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Please wait if user wants to change source/ destination and then create a path
                    // Don't do anything for now here!
                }
                _logger.PrintDebugLogs($"Form => Initiate() :: ENDED");
            }
            catch (Exception e)
            {
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");
                _logger.PrintDebugLogs($"Form => Initiate() :: CATCH BLOCK :: FAILED :: {e.ToString()}");
                _logger.PrintDebugLogs($"***************************** ERROR *****************************");

                MessageBox.Show("ERROR:: Some Error Occured!! Please check the log!!", "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void InitialDefaultLoadForInputs(bool IsClean = false)
        {
            if (IsClean)
            {
                textBoxSource.Text = "";
                textBoxDestination.Text = "";
                cbCreateBackup.Checked = false;

                SourcePathInput = null;
                BackupPathInput = null;
                DestinationPathInput = null;
                CreateBackupFlag = false;
            }
            else
            {
                textBoxSource.Text = @"C:\Source";
                textBoxDestination.Text = @"C:\Destination";
                cbCreateBackup.Checked = true;

                SourcePathInput = textBoxSource.Text;
                BackupPathInput = textBoxSource.Text + @"\" + HotFixBackupFolderName;
                DestinationPathInput = textBoxDestination.Text;
                CreateBackupFlag = true;
            }

        }
    }
}
