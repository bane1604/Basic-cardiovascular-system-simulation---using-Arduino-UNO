using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;


namespace projekat_arudino_brm_brm
{
    
    public partial class Form1 : Form
    {       bool isConnected = false;
        String[] ports;
        SerialPort port;

        public Form1()
        {
            InitializeComponent();
            disableControls();
            getAvailableComPorts();

            foreach (string port in ports)
            {
                comboBox1.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    comboBox1.SelectedItem = ports[0];
                }
            }

            chart1.Series.Add("Protok");
            chart1.Series["Protok"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Protok"].Color = Color.Red;
            chart1.Series[0].IsVisibleInLegend = false;
        }

        void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void disableControls()
        {

        }

        private void enableControls()
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                connectToArduino();
            }
            else
            {
                disconnectFromArduino();
            }
        }

        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            port.Write("#STAR\n");
            button2.Text = "Disconnect";
            port.DataReceived += serialPort1_DataReceived;
            enableControls();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string line = port.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }


        private delegate void LineReceivedEvent(string line);
        private void LineReceived(string line)
        {
            try
            {
                sum += double.Parse(line, System.Globalization.CultureInfo.InvariantCulture) / 60;
                udpate_grafik(double.Parse(line, System.Globalization.CultureInfo.InvariantCulture));
            }
            catch
            {

            }
        }

        public void udpate_grafik(double num1)
        {
            var chart = chart1.ChartAreas[0];
            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.IsEndLabelVisible = true;

            chart.AxisX.Minimum = 0;
            chart.AxisY.Minimum = 0;
            chart.AxisX.Maximum = sum + 0.25;
            chart.AxisY.Maximum = 90;
            chart.AxisY.Interval = 15;
            chart.AxisX.Interval = 10;

            chart1.Series["Protok"].Points.AddXY(sum, num1);

            try
            {
                label1.Text = "Max = " + chart1.Series["Protok"].Points.FindMaxByValue().YValues[0].ToString();
                label2.Text = "Min = " + chart1.Series["Protok"].Points.FindMinByValue().YValues[0].ToString();
            }
            catch
            {
                label1.Text = "Max = N/A";
                label2.Text = "Min = N/A";
            }
        }

        private void disconnectFromArduino()
        {
            isConnected = false;
            port.Write("#STOP\n");
            port.Close();
            button2.Text = "Connect";
            disableControls();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = true;
                chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = true;
            }
            else
            {
                chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series["Protok"].Points.Clear();
            sum = 0;
        }
    }
}
