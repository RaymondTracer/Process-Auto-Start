using System;
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
        readonly OpenFileDialog openDialog = new OpenFileDialog()
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
        readonly bool allowLogging = true; // Boolean to allow logging.

        // This allows for easy referencing of the log file.
        private readonly string logFile = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "log.txt";

        /* Credit to Matteo Mosca & Co;
         * https://stackoverflow.com/a/6499302
         */
        private static string CurrentVersion => ApplicationDeployment.IsNetworkDeployed
                                                    ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                                                    : Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public MainForm()
        {
            /* Credit to Markus Olsson;
             * http://stackoverflow.com/a/534335 
             */
            if (Settings.Default.UpgradeRequired)
            {
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
            //Settings.Default.procLook = string.Empty;
            //Settings.Default.procLaunch = string.Empty;
            //#else
            if (File.Exists(Settings.Default.procTarget)) // Check the settings for a file to look for.
            {
                Log("Found target file in settings.");

                // Assign file path and file name.
                targetPath = Settings.Default.procTarget;
                string[] procLookArray = Settings.Default.procTarget.Split('\\');
                targetFile = procLookArray[procLookArray.Length - 1];

                // Change the label text, update checkbox positions and tell the program we have the file.
                lblTarget.Text = targetFile;

                UpdateCheckboxPositions();

                targetFileSelected = true;
            }

            if (File.Exists(Settings.Default.procLaunch)) // Same as above.
            {
                Log("Found launch file in settings.");

                launchPath = Settings.Default.procLaunch;
                string[] procLaunchArray = Settings.Default.procLaunch.Split('\\');
                launchFile = procLaunchArray[procLaunchArray.Length - 1];

                lblLaunch.Text = launchFile;

                UpdateCheckboxPositions();

                launchFileSelected = true;
            }
            //#endif

            cbxAutoStart.Checked = Settings.Default.autoStart; // Set the checkboxs from the settings.
            cbxAutoClose.Checked = Settings.Default.autoClose;

            if (targetFileSelected && launchFileSelected) btnStart.Enabled = true; // If we have both files already, enable the Start button.

            if (btnStart.Enabled && cbxAutoStart.Checked) // If the Start button is enabled and the Auto Start checkbox is checked; invoke mainMethod().
            {
                btnStart.Text = "Stop";

                AsyncMainMethod asyncMM = MainMethod;
                asyncMM.BeginInvoke(null, null);
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
                if (openDialog.SafeFileName != null && !openDialog.SafeFileName.EndsWith(".exe"))
                {
                    MessageBox.Show("Please select an execuatable.", "Invalid file selected.");
                    Log("Invalid file.");
                    return;
                }

                targetPath = openDialog.FileName;
                targetFile = openDialog.SafeFileName;

                // Check to see if the file is the same as the process to launch.
                if (targetPath == launchPath)
                {
                    targetPath = string.Empty;
                    targetFile = string.Empty;

                    Log("Same file selected.");

                    MessageBox.Show("Please select a different file.", "Same file selected.");
                }
                else // If it isnt, set the process to look for's label, boolean, setting then update the checkbox positions.
                {
                    lblTarget.Text = targetFile;
                    targetFileSelected = true;

                    Settings.Default.procTarget = targetPath;
                    Settings.Default.Save();

                    UpdateCheckboxPositions();

                    if (targetFileSelected && launchFileSelected)
                        btnStart.Enabled = true; // If we have both booleans set to true, enable the Start button.
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

                if (openDialog.SafeFileName != null && !openDialog.SafeFileName.EndsWith(".exe"))
                {
                    MessageBox.Show("Please select an execuatable.", "Invalid file selected.");
                    Log("Invalid file.");
                    return;
                }

                launchPath = openDialog.FileName;
                launchFile = openDialog.SafeFileName;

                if (launchPath == targetPath)
                {
                    launchPath = string.Empty;
                    launchFile = string.Empty;

                    Log("Same file selected.");

                    MessageBox.Show("Please select a different file.", "Same file selected.");
                }
                else
                {
                    lblLaunch.Text = launchFile;
                    launchFileSelected = true;

                    Settings.Default.procLaunch = launchPath;
                    Settings.Default.Save();

                    UpdateCheckboxPositions();

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
            AsyncMainMethod asyncMM = MainMethod;

            if (btnStart.Text == "Start") // The user has pressed Start.
            {
                btnStart.Text = "Stop";

                asyncMM.BeginInvoke(null, null);
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

            targetPath = string.Empty; // Target process related variables.
            targetFile = string.Empty;
            lblTarget.Text = string.Empty;
            targetFileSelected = false;

            launchPath = string.Empty; // Launch process related variables.
            launchFile = string.Empty;
            lblLaunch.Text = string.Empty;
            launchFileSelected = false;

            Settings.Default.procTarget = string.Empty; // Settings.
            Settings.Default.procLaunch = string.Empty;
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

        private void UpdateCheckboxPositions()
        {
            Log("Updating checkbox positions.");

            /* if the Process Look label is larger then the Process Launch label
             * we set the checkboxes positions and form size according to the Process Look label.
             */
            if (lblTarget.Text.Length > lblLaunch.Text.Length)
            {
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

        private void MainMethod()
        {
            AsyncLog aLog = Log;
            Invoke(aLog, "Monitoring started. Auto Start: " + cbxAutoStart.Checked + ". Auto Close: " + cbxAutoClose.Checked + ".");

            // Disable the open buttons and the reset button.
            AsyncButtonEnabledMethod btnEM = ButtonEnabledMethod;
            btnEM.Invoke(btnLaunch, false);
            btnEM.Invoke(btnTarget, false);
            btnEM.Invoke(btnReset, false);

            // Initialize variables for the various async methods we'll use.
            AsyncButtonTextMethod btnTM = ButtonTextMethod;
            AsyncLabelChangeTextMethod lblCTM = LabelChangeTextMethod;
            AsyncFormSizeMethod formSM = FormSizeMethod;

            bool thereIsntAProblem = true; // This boolean is used for when launching an executable doesn't work.

            while (btnStart.Text == "Stop") // Keep looping while the start/stop button says stop. (Yes, that's our condition.)
            {
                Thread.Sleep(1000);

                // Look for our processes.
                Process[] procArrayLook = Process.GetProcessesByName(targetFile.Substring(0, targetFile.Length - 4));
                Process[] procArrayLaunch = Process.GetProcessesByName(launchFile.Substring(0, launchFile.Length - 4));

                // If our process we need to launch isn't there, we tell the user we're looking for the process.
                if (procArrayLook.Length == 0) lblCTM.Invoke(lblStatus, "Waiting for " + targetFile);

                /* If we've found the process, we'll tell the user we've found the process,
                 * and we're launching the specific executable.
                 */
                if (procArrayLook.Length == 1 && !processOpened)
                {
                    Invoke(aLog, "Found the target process.");

                    lblCTM.Invoke(lblStatus, targetFile + " opened. Launching " + launchFile);

                    if (procArrayLaunch.Length == 0) // This is to make sure the process we want to launch isn't open already.
                    {
                        aLog.Invoke("Launch process isnt opened, opening.");

                        try
                        {
                            ProcessStartInfo process = new ProcessStartInfo(launchPath);
                            process.WindowStyle = ProcessWindowStyle.Minimized;

                            Process.Start(process); // Launch the process
                        }
                        /* If there's a problem launching the process, log the problem to a file (error.txt)
                         * and tell the user we've found a problem. We'll also stop this method.
                         */
                        catch (Exception ex)
                        {
                            Invoke(aLog, "Process couldn't be opened. Error:" + Environment.NewLine + ex);

                            thereIsntAProblem = false;
                            btnTM.Invoke(btnStart, "Start");
                            lblCTM.Invoke(lblStatus, targetFile + " opened. " + "Problem launching " + launchFile + " - Check log file.");
                        }
                    }

                    if (thereIsntAProblem) // If we don't have a problem, tell the user we've launched the program.
                    {
                        lblCTM.Invoke(lblStatus, targetFile + " opened. " + launchFile + " launched.");
                        aLog.Invoke("Process launched.");
                    }

                    processOpened = true; // Set this boolean to true so we know the process is there.
                }

                // If the process we're looking for is closed.
                if (!processOpened || procArrayLook.Length != 0 || !cbxAutoClose.Checked) continue;

                aLog.Invoke("Target process closed, closing launch process."); // Tell the user we're closing the program we want to look for.
                lblCTM.Invoke(lblStatus, targetFile + " closed. Closing " + launchFile);

                Console.WriteLine("Closing: " + launchFile.Split('.')[0]); // Close the program
                Process.GetProcessesByName(launchFile.Split('.')[0])[0].CloseMainWindow();

                lblCTM.Invoke(lblStatus, targetFile + " closed. " + launchFile + " closed."); // Tell the user we've closed the program.
                formSM.Invoke(this, 400, 150); // Set the form size back to normal.
                processOpened = false;
            }

            aLog.Invoke("Monitoring stopped.");

            // Enable our open buttons and the Start and Reset buttons.
            btnEM.Invoke(btnLaunch, true);
            btnEM.Invoke(btnTarget, true);
            btnEM.Invoke(btnReset, true);
            btnEM.Invoke(btnStart, true);

            if (btnStart.Text == "Stop") // Change the button name to "Start".
                btnTM.Invoke(btnStart, "Start");

            if (thereIsntAProblem) // If there wasn't a problem opening the process we want to launch, tell the user that we're waiting to start again. 
                lblCTM.Invoke(lblStatus, "Waiting to start.");

            formSM.Invoke(this, 400, 150); // Reset the form size.
        }

        private void Log(string text) // Method for logging.
        {
            if (allowLogging)
            {
                StreamWriter logger = new StreamWriter(logFile, true);
                logger.WriteLine(DateTime.Now.ToString("dd/MM/yyyy - h:mm:ss tt") + ": " + text);
                logger.Close();
            }
        }

        private void ButtonEnabledMethod(Button button, bool enabled) // Async method to enable or disable a button.
        {
            button.Enabled = enabled;
        }

        private void ButtonTextMethod(Button button, string text) // Async method to change a button's text.
        {
            button.Text = text;
        }

        private void LabelChangeTextMethod(Label label, string text) // Async method to change a label's text.
        {
            label.Text = text;
        }

        private void FormSizeMethod(Form form, int width, int height) // Async method to change a form's size.
        {
            form.Size = new Size(width, height);
        }

        // Async delegates for invoking while we're running mainMethod().
        public delegate void AsyncMainMethod();

        private delegate void AsyncLog(string text);

        public delegate void AsyncButtonEnabledMethod(Button button, bool enabled);

        public delegate void AsyncButtonTextMethod(Button button, string text);

        private delegate void AsyncLabelChangeTextMethod(Label label, string text);

        private delegate void AsyncFormSizeMethod(Form form, int width, int height);
    }
}