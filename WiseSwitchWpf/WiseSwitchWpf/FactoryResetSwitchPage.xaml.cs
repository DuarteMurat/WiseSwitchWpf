﻿using System.IO;
using System.IO.Ports;
using System.Management;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WiseSwitchWpf
{
    /// <summary>
    /// Interaction logic for FactoryResetSwitchPage.xaml
    /// </summary>
    public partial class FactoryResetSwitchPage : Page
    {
        SerialPort? serialPort;
        private StreamWriter logStreamWriter;

        public FactoryResetSwitchPage()
        {
            InitializeComponent();
        }

        private async void ResetSwitch(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                string portName = FindComPort();

                if (portName == null)
                {
                    UpdateResetStatus("No compatible COM port found.");
                    // Handle the situation where no suitable COM port is detected
                    return;
                }

                UpdateResetStatus($"Using {portName}.");
                Thread.Sleep(1000);
                InitializeSerialPort(portName);

                ResetSwitch();
            });
        }

        private void UpdateResetStatus(string text)
        {
            Dispatcher.Invoke(()=> ResetStatus.Content = text);
        }
   
        private string FindComPort()
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

        private bool IsSwitchPort(string portName)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM" + portName.Substring(3) + "%'");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (queryObj["Caption"].ToString().Contains("USB"))
                {
                    return true;
                }
            }
            return false;
        }

        void InitializeSerialPort(string portName)
        {
            serialPort = new SerialPort(portName, 9600); // Change baud rate as needed
            serialPort.Open();

            // Initialize the StreamWriter for logging
            string logFileName = $"switch_logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            logStreamWriter = new StreamWriter(logFileName);
        }

        void SendCommand(string command)
        {
            serialPort.WriteLine(command);
            Thread.Sleep(1000); // Adjust this delay based on your switch's response time

            // Read the response from the switch
            Thread.Sleep(500); // Adjust this delay based on your switch's response time
            string response = serialPort.ReadExisting();

            // Save the response to the log file
            LogResponse(command, response);
        }

        void LogResponse(string command, string response)
        {
            // Write the command and response to the log file
            logStreamWriter.WriteLine($"Command: {command}");
            if(!string.IsNullOrEmpty(response))
            {
                logStreamWriter.WriteLine($"Response: {response}");
            }
            else
            {
                logStreamWriter.WriteLine($"Response: <No Response>");
            }
            logStreamWriter.WriteLine(new string('-', 50)); // Separator for better readability
            logStreamWriter.Flush(); // Flush the buffer to ensure data is written immediately
        }

        void ResetSwitch()
        {
            //********************************ENABLE SWITCH*******************************************

            EnableSwitch();

            //********************************DELETE STARTUP CONFIG*******************************************

            DeleteStartupConfig();

            //********************************DELETE CONFIG.TEXT*******************************************

            DeleteConfigText();

            //********************************RESET SWITCH*******************************************

            Reset();

            // Close the log stream writer when done
            logStreamWriter.Close();
        }
        void EnableSwitch()
        {
            UpdateResetStatus("Enabling switch.");
            SendCommand("enable");
            Thread.Sleep(1000); // Wait for 1 second
            string response = serialPort.ReadExisting();
            if (response.Contains("Password"))
            {
                SendCommand("cisco");
                Thread.Sleep(500); // Wait for 1 second
            }
        }

        void DeleteStartupConfig()
        {
            SendCommand("show startup-config"); // Example command to retrieve the switch's version
            Thread.Sleep(100);
            // Read the response from the switch to confirm the status
            string response = serialPort.ReadExisting();

            if (response.Contains("not present"))
            {
                UpdateResetStatus("startup-config is missing.");
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
                    UpdateResetStatus("startup-config erased successfully.");
                }
                else
                {
                    UpdateResetStatus("Failed to delete startup-config");
                }
            }
        }

        void DeleteConfigText()
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
                    UpdateResetStatus($"{filename} is missing.");
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
                        UpdateResetStatus($"{filename} erased successfully.");
                    }
                    else
                    {
                        UpdateResetStatus($"Failed to delete {filename}");
                    }
                }
            }
        }

        void Reset()
        {
            UpdateResetStatus("Resetting Switch, please wait.");
            SendCommand("reload");
            // Wait for the switch to reset (adjust the time according to the switch's response time)
            Thread.Sleep(1000); // Wait for 1 second
            SendCommand("y");
            Thread.Sleep(100000); // Wait for 2 mins
                                  // Send a command to check if the switch is responsive after reset
            SendCommand("show version"); // Example command to retrieve the switch's version

            // Read the response from the switch to confirm the status
            string response = serialPort.ReadExisting();

            if (response.Contains("Cisco IOS Software"))
            {
                UpdateResetStatus("Switch reset successfully.");
                SendCommand("y");
                Thread.Sleep(500); // Wait for 0.5 second
                SendCommand("n");
            }
            else
            {
                UpdateResetStatus("Failed to reset the switch.");
            }
        }
    }
}
