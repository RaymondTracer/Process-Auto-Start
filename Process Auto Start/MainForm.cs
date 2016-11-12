﻿using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Deployment.Application;
using System.Reflection;
using System.Drawing;
using Process_Auto_Start.Properties;

namespace Process_Auto_Start
{
    public partial class MainForm : Form
    {
        OpenFileDialog openDialog = new OpenFileDialog()
        {
            Title = "Select an executable",
            Filter = "Executable files|*.exe",
            InitialDirectory = "."
        };

        string targetFile; // The file name of the process to look for.
        string targetPath; // The file path of the process to look for.
        string launchFile; // The file name of the process to launch.
        string launchPath; // The file path of the process to launch.

        bool targetFileSelected; // Boolean to check if we've selected a file to look for.
        bool launchFileSelected; // Boolean to check if we've selected a file to launch.
        bool processOpened; // Boolean to check if the file we want to launch has opened.
        bool allowLogging = true; // Boolean to allow logging.

        string logFile = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "log.txt"; // This allows for easy referencing of the log file.

        string CurrentVersion // Credit to Matteo Mosca & Co;
        {                     // https://stackoverflow.com/a/6499302
            get
            {
                return ApplicationDeployment.IsNetworkDeployed
                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                       : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public MainForm()
        {
            if (Settings.Default.UpgradeRequired) // Credit to Markus Olsson;
            {                                     // http://stackoverflow.com/a/534335
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            InitializeComponent();

            Text += " " + CurrentVersion; // Add the current version to the form title.

            if (File.Exists(logFile)) Log("");
            Log(Text);
            Log("procTarget: " + Settings.Default.procTarget);
            Log("procLaunch: " + Settings.Default.procLaunch);
            Log("autoStart: " + Settings.Default.autoStart);
            Log("autoClose: " + Settings.Default.autoClose);

            //#if DEBUG
            //Settings.Default.procLook = String.Empty;
            //Settings.Default.procLaunch = String.Empty;
            //#else
            if (File.Exists(Settings.Default.procTarget)) // Check the settings for a file to look for.
            {
                Log("Found target file in settings.");

                // Assign file path and file name.
                targetPath = Settings.Default.procTarget;
                String[] procLookArray = Settings.Default.procTarget.Split('\\');
                targetFile = procLookArray[procLookArray.Length - 1];

                // Change the label text, update checkbox positions and tell the program we have the file.
                lblTarget.Text = targetFile;

                updateCheckboxPositions();

                targetFileSelected = true;
            }

            if (File.Exists(Settings.Default.procLaunch)) // Same as above.
            {
                Log("Found launch file in settings.");

                launchPath = Settings.Default.procLaunch;
                String[] procLaunchArray = Settings.Default.procLaunch.Split('\\');
                launchFile = procLaunchArray[procLaunchArray.Length - 1];

                lblLaunch.Text = launchFile;

                updateCheckboxPositions();

                launchFileSelected = true;
            }
            //#endif

            cbxAutoStart.Checked = Settings.Default.autoStart; // Set the checkboxs from the settings.
            cbxAutoClose.Checked = Settings.Default.autoClose;

            if (targetFileSelected && launchFileSelected) btnStart.Enabled = true; // If we have both files already, enable the Start button.
            if (btnStart.Enabled && cbxAutoStart.Checked) // If the Start button is enabled and the Auto Start checkbox is checked; invoke mainMethod().
            {
                btnStart.Text = "Stop";

                asyncMainMethod asyncMM = new asyncMainMethod(mainMethod);
                IAsyncResult result = asyncMM.BeginInvoke(null, null);
            }
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            Log("Open target file.");

            // Ask for a file.
            DialogResult result = openDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Log("File selected: " + openDialog.FileName);

                // Check to see if we actually has an execuatable, set file path and file name variables.
                if (!openDialog.SafeFileName.EndsWith(".exe")) { MessageBox.Show("Please select an execuatable.", "Invalid file selected."); Log("Invalid file."); return; }

                targetPath = openDialog.FileName;
                targetFile = openDialog.SafeFileName;

                // Check to see if the file is the same as the process to launch.
                if (targetPath == launchPath)
                {
                    targetPath = String.Empty;
                    targetFile = String.Empty;

                    Log("Same file selected.");

                    MessageBox.Show("Please select a different file.", "Same file selected.");
                }
                else // If it isnt, set the process to look for's label, boolean, setting then update the checkbox positions.
                {
                    lblTarget.Text = targetFile;
                    targetFileSelected = true;

                    Settings.Default.procTarget = targetPath;
                    Settings.Default.Save();

                    updateCheckboxPositions();

                    if (targetFileSelected && launchFileSelected) btnStart.Enabled = true; // If we have both booleans set to true, enable the Start button.
                }
            }
            else if (result == DialogResult.Cancel)
            {
                Log("No file selected.");
                Console.WriteLine("No file selected.");
            }
        }

        private void btnLaunch_Click(object sender, EventArgs e) // Same as above.
        {
            Log("Open launch file.");

            DialogResult result = openDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Log("File selected: " + openDialog.FileName);

                if (!openDialog.SafeFileName.EndsWith(".exe")) { MessageBox.Show("Please select an execuatable.", "Invalid file selected."); Log("Invalid file."); return; }

                launchPath = openDialog.FileName;
                launchFile = openDialog.SafeFileName;

                if (launchPath == targetPath)
                {
                    launchPath = String.Empty;
                    launchFile = String.Empty;

                    Log("Same file selected.");

                    MessageBox.Show("Please select a different file.", "Same file selected.");
                }
                else
                {
                    lblLaunch.Text = launchFile;
                    launchFileSelected = true;

                    Settings.Default.procLaunch = launchPath;
                    Settings.Default.Save();

                    updateCheckboxPositions();

                    if (targetFileSelected && launchFileSelected) btnStart.Enabled = true;
                }
            }
            else if (result == DialogResult.Cancel)
            {
                Log("No file selected.");
                Console.WriteLine("No file selected.");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            asyncMainMethod asyncMM = new asyncMainMethod(mainMethod);

            if (btnStart.Text == "Start") // The user has pressed Start.
            {
                btnStart.Text = "Stop";

                IAsyncResult result = asyncMM.BeginInvoke(null, null); // Invoke mainMethod()
            }
            else if (btnStart.Text == "Stop") // The user has pressed Stop.
            {
                btnStart.Enabled = false; // Disable the button becauase we don't want the user to click it until it gets reenabled just before mainMethod stops.
                btnStart.Text = "Start";
            }
        }
         
        private void btnReset_Click(object sender, EventArgs e) // This method resets everything.
        {
            Log("Everything reset.");

            btnStart.Enabled = false;

            targetPath = String.Empty; // Target process related variables.
            targetFile = String.Empty;
            lblTarget.Text = String.Empty;
            targetFileSelected = false;

            launchPath = String.Empty; // Launch process related variables.
            launchFile = String.Empty;
            lblLaunch.Text = String.Empty;
            launchFileSelected = false;

            Settings.Default.procTarget = String.Empty; // Settings.
            Settings.Default.procLaunch = String.Empty;
            Settings.Default.Save();

            lblStatus.Text = "Waiting to start."; // Status label.

            cbxAutoStart.Location = new Point(309, 13); // Checkboxes and form size.
            cbxAutoClose.Location = new Point(309, 43);
            Size = new Size(400, 150);
        }

        private void cbxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.autoStart = cbxAutoStart.Checked;
            Settings.Default.Save();
        }

        private void cbxAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.autoClose = cbxAutoClose.Checked;
            Settings.Default.Save();
        }

        void updateCheckboxPositions()
        {
            Log("Updating checkbox positions.");

            if (lblTarget.Text.Length > lblLaunch.Text.Length) // if the Process Look label is larger then the Process Launch label
            {                                                  // we set the checkboxes positions and form size according to the Process Look label.
                Log("lblTarget.Text.Length > lblLaunch.Text.Length");

                cbxAutoStart.Location = new Point(lblTarget.Right + 2, cbxAutoStart.Location.Y);
                cbxAutoClose.Location = new Point(lblTarget.Right + 2, cbxAutoClose.Location.Y);

                Size = new Size(cbxAutoStart.Right + 3, Size.Height);
            }
            else // Otherwise we set the checkboxes postions and form size according to the Process Launch label.
            {
                Log("lblTarget.Text.Length < lblLaunch.Text.Length");

                cbxAutoStart.Location = new Point(lblLaunch.Right + 2, cbxAutoStart.Location.Y);
                cbxAutoClose.Location = new Point(lblLaunch.Right + 2, cbxAutoClose.Location.Y);

                Size = new Size(cbxAutoStart.Right + 3, Size.Height);
            }
        }

        void mainMethod()
        {
            asyncLog aLog = new asyncLog(Log);
            Invoke(aLog, "Monitoring started. Auto Start: " + cbxAutoStart.Checked + ". Auto Close: " + cbxAutoClose.Checked + ".");

            // Disable the open buttons and the reset button.
            asyncButtonEnabledMethod btnEM = new asyncButtonEnabledMethod(buttonEnabledMethod);
            btnLaunch.Invoke(btnEM, btnLaunch, false);
            btnTarget.Invoke(btnEM, btnTarget, false);
            btnReset.Invoke(btnEM, btnReset, false);

            // Initialize variables for the various async methods we'll use.
            asyncButtonTextMethod btnTM = new asyncButtonTextMethod(buttonTextMethod);
            asyncLabelChangeTextMethod lblCTM = new asyncLabelChangeTextMethod(labelChangeTextMethod);
            asyncFormSizeMethod formSM = new asyncFormSizeMethod(formSizeMethod);

            bool thereIsntAProblem = true; // This boolean is used for when launching an executable doesn't work.

            while (btnStart.Text == "Stop") // Keep looping while the start/stop button says stop. (Yes, that's our condition.)
            {
                Thread.Sleep(1000);

                // Look for our processes.
                Process[] procArrayLook = Process.GetProcessesByName(targetFile.Substring(0, targetFile.Length - 4));
                Process[] procArrayLaunch = Process.GetProcessesByName(launchFile.Substring(0, launchFile.Length - 4));

                // If our process we need to launch isn't there, we tell the user we're looking for the process.
                if (procArrayLook.Length == 0) lblStatus.Invoke(lblCTM, lblStatus, "Waiting for " + targetFile);

                if (procArrayLook.Length == 1 && !processOpened) // If we've found the process, we'll tell the user we've found the process,
                {                                                // and we're launching the specific executable.
                    Invoke(aLog, "Found the target process.");

                    lblStatus.Invoke(lblCTM, lblStatus, targetFile + " opened. Launching " + launchFile);

                    if (procArrayLaunch.Length == 0) // This is to make sure the process we want to launch isn't open already.
                    {
                        Invoke(aLog, "Launch process isnt opened, opening.");

                        try
                        {
                            ProcessStartInfo process = new ProcessStartInfo(launchPath);
                            process.WindowStyle = ProcessWindowStyle.Minimized;

                            Process.Start(process); // Launch the process
                        }
                        catch (Exception ex) // If there's a problem launching the process, log the problem to a file (error.txt)
                        {                    // and tell the user we've found a problem. We'll also stop this method.
                            Invoke(aLog, "Process couldn't be opened. Error:" + Environment.NewLine + ex);

                            thereIsntAProblem = false;
                            Invoke(btnTM, btnStart, "Start");
                            lblStatus.Invoke(lblCTM, lblStatus, targetFile + " opened. " + "Problem launching " + launchFile + " - Check log file.");
                        }
                    }

                    // If we don't have a problem, tell the user we've launched the program.
                    if (thereIsntAProblem) { lblStatus.Invoke(lblCTM, lblStatus, targetFile + " opened. " + launchFile + " launched."); Invoke(aLog, "Process launched."); };

                    processOpened = true; // Set this boolean to true so we know the process is there.
                }

                if (processOpened && procArrayLook.Length == 0 && cbxAutoClose.Checked) // If the process we're looking for is closed.
                {
                    Invoke(aLog, "Target process closed, closing launch process."); // Tell the user we're closing the program we want to look for.
                    lblStatus.Invoke(lblCTM, lblStatus, targetFile + " closed. Closing " + launchFile);

                    Console.WriteLine("Closing: " + launchFile.Split('.')[0]); // Close the program
                    Process.GetProcessesByName(launchFile.Split('.')[0])[0].CloseMainWindow();

                    lblStatus.Invoke(lblCTM, lblStatus, targetFile + " closed. " + launchFile + " closed."); // Tell the user we've closed the program.
                    Invoke(formSM, this, 400, 150); // Set the form size back to normal.
                    processOpened = false;
                }
            }

            Invoke(aLog, "Monitoring stopped.");

            // Enable our open buttons and the Start and Reset buttons.
            btnLaunch.Invoke(btnEM, btnLaunch, true);
            btnTarget.Invoke(btnEM, btnTarget, true);
            btnReset.Invoke(btnEM, btnReset, true);
            btnStart.Invoke(btnEM, btnStart, true);

            if (btnStart.Text == "Stop") // Change the button name to "Start".
            {
                btnStart.Invoke(btnTM, btnStart, "Start");
            }

            if (thereIsntAProblem) // If there wasn't a problem opening the process we want to launch, tell the user that we're waiting to start again.
            {
                lblStatus.Invoke(lblCTM, lblStatus, "Waiting to start.");
            }

            Invoke(formSM, this, 400, 150); // Reset the form size.
        }

        void Log(string text) // Method for logging.
        {
            if (allowLogging)
            {
                StreamWriter logger = new StreamWriter(logFile, true);
                logger.WriteLine(DateTime.Now.ToString("dd/MM/yyyy - h:mm:ss tt") + ": " + text);
                logger.Close();
            }
        }

        void buttonEnabledMethod(Button button, bool enabled) // async method to enable and disable a button.
        {
            button.Enabled = enabled;
        }

        void buttonTextMethod(Button button, string text) // async method to change a button's text.
        {
            button.Text = text;
        }

        void labelChangeTextMethod(Label label, string text) // async method to change a label's text.
        {
            label.Text = text;
        }

        void formSizeMethod(Form form, int width, int height) // async method to change the form's size.
        {
            form.Size = new Size(width, height);
        }

        // async delegates for invoking while we're running mainMethod().
        delegate void asyncMainMethod();
        delegate void asyncLog(string text);
        delegate void asyncButtonEnabledMethod(Button button, bool enabled);
        delegate void asyncButtonTextMethod(Button button, string text);
        delegate void asyncLabelChangeTextMethod(Label label, string text);
        delegate void asyncFormSizeMethod(Form form, int width, int height);
    }
}