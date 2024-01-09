using System.IO.Ports;
using System.Management;
using System.Windows;
using System.Windows.Controls;

namespace WiseSwitchWpf
{
    /// <summary>
    /// Interaction logic for FactoryResetSwitchPage.xaml
    /// </summary>
    public partial class FactoryResetSwitchPage : Page
    {
        static SerialPort? serialPort;
        public FactoryResetSwitchPage()
        {
            InitializeComponent();
        }

        private void ResetSwitch(object sender, RoutedEventArgs e)
        {
            string portName = FindComPort();

            if (portName == null)
            {
                MessageBox.Show("No compatible COM port found.");
                // Handle the situation where no suitable COM port is detected
                return;
            }
            MessageBox.Show($"Using {portName}.");

            InitializeSerialPort(portName);

            ResetSwitch();
        }
   
        private static string FindComPort()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                if (IsSwitchPort(port))
                {
                    return port;
                }
            }
            return null; // No compatible COM port found
        }

        private static bool IsSwitchPort(string portName)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM" + portName.Substring(3) + "%'");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (queryObj["Caption"].ToString().Contains("USB Serial Port") || queryObj["Caption"].ToString().Contains("USB"))
                {
                    return true;
                }
            }
            return false;
        }

        static void InitializeSerialPort(string portName)
        {
            serialPort = new SerialPort(portName, 9600); // Change baud rate as needed
            serialPort.Open();
        }

        static void SendCommand(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000); // Adjust delay based on your switch's response time
        }

        static void ResetSwitch()
        {
            //********************************ENABLE SWITCH*******************************************

            EnableSwitch();

            //********************************DELETE STARTUP CONFIG*******************************************

            DeleteStartupConfig();

            //********************************DELETE CONFIG.TEXT*******************************************

            DeleteConfigText();

            //********************************RESET SWITCH*******************************************

            Reset();
        }
        static void EnableSwitch()
        {
            MessageBox.Show("Enabling switch.");
            SendCommand("enable");
            Thread.Sleep(1000); // Wait for 1 second
            string response = serialPort.ReadExisting();
            if (response.Contains("Password"))
            {
                SendCommand("cisco");
                Thread.Sleep(500); // Wait for 1 second
            }
        }

        static void DeleteStartupConfig()
        {
            SendCommand("show startup-config"); // Example command to retrieve the switch's version
            Thread.Sleep(100);
            // Read the response from the switch to confirm the status
            string response = serialPort.ReadExisting();

            if (response.Contains("not present"))
            {
                MessageBox.Show("startup-config is missing.");
            }

            else
            {
                SendCommand("y"); // Example command to retrieve the switch's version
                Thread.Sleep(300); // Wait for 2 seconds
                                   // Send commands to delete startup file
                SendCommand("erase startup-config");
                Thread.Sleep(500); // Wait for 0.5 second
                SendCommand("y");
                Thread.Sleep(2000); // Wait for 9 secs
                SendCommand("show startup-config"); // Example command to retrieve the switch's version
                Thread.Sleep(500);
                response = serialPort.ReadExisting();
                if (response.Contains("not present"))
                {
                    MessageBox.Show("startup-config erased successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to delete startup-config");
                }
            }
        }

        static void DeleteConfigText()
        {
            var filenames = new string[]
            {
            "private-config.text.renamed", "private-config.text", "config.text.renamed", "config.text"
            };
            foreach (string filename in filenames)
            {
                SendCommand($"more flash:{filename}");
                Thread.Sleep(500);
                // Read the response from the switch to confirm the status
                string response = serialPort.ReadExisting();

                if (response.Contains("No such file or directory"))
                {
                    MessageBox.Show($"{filename} is missing.");
                }

                else
                {
                    // Send commands to delete startup file
                    SendCommand($"delete flash:{filename}");
                    Thread.Sleep(1000);
                    SendCommand($"{filename}");
                    Thread.Sleep(1000);
                    SendCommand("y");
                    Thread.Sleep(1500); // Wait for 1.5 secs
                    SendCommand($"more flash:{filename}"); // Example command to retrieve the switch's version
                    Thread.Sleep(500);
                    response = serialPort.ReadExisting();
                    if (response.Contains("No such file or directory"))
                    {
                        MessageBox.Show($"{filename} erased successfully.");
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete {filename}");
                    }
                }
            }
        }

        static void Reset()
        {
            MessageBox.Show("Resetting Switch, please wait.");
            SendCommand("reload");
            // Wait for the switch to reset (adjust the time according to the switch's response time)
            Thread.Sleep(1000); // Wait for 1 second
            SendCommand("y");
            Thread.Sleep(200000); // Wait for 4 mins
                                  // Send a command to check if the switch is responsive after reset
            SendCommand("show version"); // Example command to retrieve the switch's version

            // Read the response from the switch to confirm the status
            string response = serialPort.ReadExisting();

            if (response.Contains("Cisco IOS Software"))
            {
                MessageBox.Show("Switch reset successfully.");
                SendCommand("y");
                Thread.Sleep(500); // Wait for 0.5 second
                SendCommand("n");
            }
            else
            {
                MessageBox.Show("Failed to reset the switch.");
            }
        }
    }
}
