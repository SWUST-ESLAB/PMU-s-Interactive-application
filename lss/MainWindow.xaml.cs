using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using Newtonsoft.Json;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading;
using System.IO;
namespace lss
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort s = new SerialPort();
        System.Timers.Timer timer = new System.Timers.Timer(10);
        System.Timers.Timer timer_1 = new System.Timers.Timer(20000);
        System.Timers.Timer timer_2 = new System.Timers.Timer(1000);

        public bool closing = false;
        public bool listening = false;
        public int mission_check = 0;
        public int mission_recheck = 0;
        public double estimate_hour = 0;
        public double estimate_minute = 0;
        public double estimate_second = 0;
        public int Port_Find_Signal = 0;
        public int Save_flag = 0;
        public int check_data;
        public int Data_FPS = 0;
        public string Data_length;
        public string Data_FPS_Str;
        public string SavePath = string.Empty;

        public string File_name = Environment.CurrentDirectory + "/logs";
        //public string revinFileName = "/log_revin_" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
        //public string revoutFileName = "/log_revout_" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
        public string rev_FileName { get; set; }
        public SeriesCollection OutputVolState { get; set; }
        public SeriesCollection OutputCurState { get; set; }
        public SeriesCollection OutputConsumption { get; set; }
        public SeriesCollection OutputSpeed { get; set; }
        public List<string> Labels { get; set; }

        private double[] temp = { 0, 0, 0, 0, 0, 0, 0, 0 };

        LineSeries mylineseries = new LineSeries();
        LineSeries mylineseries_1 = new LineSeries();
        LineSeries mylineseries_2 = new LineSeries();
        LineSeries mylineseries_3 = new LineSeries();

        LineSeries mylineseries_4 = new LineSeries();
        LineSeries mylineseries_5 = new LineSeries();
        LineSeries mylineseries_6 = new LineSeries();
        LineSeries mylineseries_7 = new LineSeries();

        LineSeries mylineseries_8 = new LineSeries();
        LineSeries mylineseries_9 = new LineSeries();
        LineSeries mylineseries_10 = new LineSeries();
        LineSeries mylineseries_11 = new LineSeries();

        LineSeries mylineseries_12 = new LineSeries();
        LineSeries mylineseries_13 = new LineSeries();
        LineSeries mylineseries_14 = new LineSeries();
        LineSeries mylineseries_15 = new LineSeries();

        LineSeries mylineseries_16 = new LineSeries();
        LineSeries mylineseries_17 = new LineSeries();
        LineSeries mylineseries_18 = new LineSeries();
        LineSeries mylineseries_19 = new LineSeries();

        LineSeries mylineseries_20 = new LineSeries();
        LineSeries mylineseries_21 = new LineSeries();
        LineSeries mylineseries_22 = new LineSeries();
        LineSeries mylineseries_23 = new LineSeries();

        LineSeries mylineseries_24 = new LineSeries();
        LineSeries mylineseries_25 = new LineSeries();
        LineSeries mylineseries_26 = new LineSeries();
        LineSeries mylineseries_27 = new LineSeries();

        LineSeries mylineseries_28 = new LineSeries();
        LineSeries mylineseries_29 = new LineSeries();
        LineSeries mylineseries_30 = new LineSeries();
        LineSeries mylineseries_31 = new LineSeries();

        LineSeries mylineseries_32 = new LineSeries();
        LineSeries mylineseries_33 = new LineSeries();
        LineSeries mylineseries_34 = new LineSeries();
        LineSeries mylineseries_35 = new LineSeries();

        LineSeries mylineseries_36 = new LineSeries();
        LineSeries mylineseries_37 = new LineSeries();
        LineSeries mylineseries_38 = new LineSeries();
        LineSeries mylineseries_39 = new LineSeries();

        public MainWindow()
        {
            InitializeComponent();
            ChartInit();
            logInit();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Paint_event);
            timer.Start();
            timer_1.Elapsed += new System.Timers.ElapsedEventHandler(Error_report);
            timer_2.Elapsed += new System.Timers.ElapsedEventHandler(Data_FPS_UI);
            timer_2.Start();
            string[] ports = SerialPort.GetPortNames();
            Portselect.ItemsSource = ports;
            Portselect.SelectedItem = Portselect.Items[0];
            BPSselect.SelectedItem = BPSselect.Items[0];
        }
        private void logInit()
        {

            if (!Directory.Exists(File_name))
            {
                Directory.CreateDirectory(File_name);
            }
            rev_FileName = "/log_rev_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";

            if (!File.Exists(File_name + rev_FileName))
            {
                File.Create(File_name + rev_FileName).Close();
            }
            FileStream fs = new FileStream(File_name + rev_FileName, FileMode.Create, FileAccess.Write);
            StreamWriter rev = new StreamWriter(fs, Encoding.UTF8);
            string rev_head = string.Format("时间 , 输入电压/mV , 输入电流/mA , 输入耗电量/mA , 输入电芯1/mV , 输入电芯2/mV , 输入电芯3/mV , 输入电芯4/mV , 输入电芯5/mV , 输入电芯6/mV ," +
                " #1输出电压/mV ,#1输出电流/mA , #1输出功率/mW , #1输出转速/r , #1输出PWM ," +
                "#2输出电压/mV ,#2输出电流/mA , #2输出功率/mW , #2输出转速/r , #2输出PWM ," +
                "#3输出电压/mV ,#3输出电流/mA , #3输出功率/mW , #3输出转速/r , #3输出PWM ," +
                "#4输出电压/mV ,#4输出电流/mA , #4输出功率/mW , #4输出转速/r , #4输出PWM ," +
                "#5输出电压/mV ,#5输出电流/mA , #5输出功率/mW , #5输出转速/r , #5输出PWM ," +
                "#6输出电压/mV ,#6输出电流/mA , #6输出功率/mW , #6输出转速/r , #6输出PWM ," +
                "#7输出电压/mV ,#7输出电流/mA , #7输出功率/mW , #7输出转速/r , #7输出PWM ," +
                "#8输出电压/mV ,#8输出电流/mA , #8输出功率/mW , #8输出转速/r , #8输出PWM ," +
                "#9输出电压/mV ,#9输出电流/mA , #9输出功率/mW , #9输出转速/r , #9输出PWM ," +
                "#10输出电压/mV ,#10输出电流/mA , #10输出功率/mW , #10输出转速/r , #10输出PWM , ");

            rev.WriteLine(rev_head);
            rev.Flush();
            rev.Close();
            rev.Dispose();
        }

        private void ChartInit()
        {
            /*画出每一路的输出电压*/
            mylineseries.Title = "Output of voltage #1";
            mylineseries.LineSmoothness = 1;
            mylineseries.PointGeometry = null;
            mylineseries.Fill = Brushes.Transparent;

            mylineseries_4.Title = "Output of voltage #2";
            mylineseries_4.LineSmoothness = 1;
            mylineseries_4.PointGeometry = null;
            mylineseries_4.Fill = Brushes.Transparent;

            mylineseries_8.Title = "Output of voltage #3";
            mylineseries_8.LineSmoothness = 1;
            mylineseries_8.PointGeometry = null;
            mylineseries_8.Fill = Brushes.Transparent;

            mylineseries_12.Title = "Output of voltage #4";
            mylineseries_12.LineSmoothness = 1;
            mylineseries_12.PointGeometry = null;
            mylineseries_12.Fill = Brushes.Transparent;

            mylineseries_16.Title = "Output of voltage #5";
            mylineseries_16.LineSmoothness = 1;
            mylineseries_16.PointGeometry = null;
            mylineseries_16.Fill = Brushes.Transparent;

            mylineseries_20.Title = "Output of voltage #6";
            mylineseries_20.LineSmoothness = 1;
            mylineseries_20.PointGeometry = null;
            mylineseries_20.Fill = Brushes.Transparent;

            mylineseries_24.Title = "Output of voltage #7";
            mylineseries_24.LineSmoothness = 1;
            mylineseries_24.PointGeometry = null;
            mylineseries_24.Fill = Brushes.Transparent;

            mylineseries_28.Title = "Output of voltage #8";
            mylineseries_28.LineSmoothness = 1;
            mylineseries_28.PointGeometry = null;
            mylineseries_28.Fill = Brushes.Transparent;

            mylineseries_32.Title = "Output of voltage #9";
            mylineseries_32.LineSmoothness = 1;
            mylineseries_32.PointGeometry = null;
            mylineseries_32.Fill = Brushes.Transparent;

            mylineseries_36.Title = "Output of voltage #10";
            mylineseries_36.LineSmoothness = 1;
            mylineseries_36.PointGeometry = null;
            mylineseries_36.Fill = Brushes.Transparent; 

            Labels = new List<string> { };
            //添加折线图的数据
            mylineseries.Values = new ChartValues<double>(temp);
            mylineseries_4.Values = new ChartValues<double>(temp);
            mylineseries_8.Values = new ChartValues<double>(temp);
            mylineseries_12.Values = new ChartValues<double>(temp);
            mylineseries_16.Values = new ChartValues<double>(temp);
            mylineseries_20.Values = new ChartValues<double>(temp);
            mylineseries_24.Values = new ChartValues<double>(temp);
            mylineseries_28.Values = new ChartValues<double>(temp);
            mylineseries_32.Values = new ChartValues<double>(temp);
            mylineseries_36.Values = new ChartValues<double>(temp);

            OutputVolState = new SeriesCollection { };

            OutputVolState.Add(mylineseries);
            OutputVolState.Add(mylineseries_4);
            OutputVolState.Add(mylineseries_8);
            OutputVolState.Add(mylineseries_12);
            OutputVolState.Add(mylineseries_16);
            OutputVolState.Add(mylineseries_20);
            OutputVolState.Add(mylineseries_24);
            OutputVolState.Add(mylineseries_28);
            OutputVolState.Add(mylineseries_32);
            OutputVolState.Add(mylineseries_36);


            /*画出每一路的输出电流*/
            mylineseries_1.Title = "Output of current #1";
            mylineseries_1.LineSmoothness = 1;
            mylineseries_1.PointGeometry = null;
            mylineseries_1.Fill = Brushes.Transparent;

            mylineseries_5.Title = "Output of current #2";
            mylineseries_5.LineSmoothness = 1;
            mylineseries_5.PointGeometry = null;
            mylineseries_5.Fill = Brushes.Transparent;

            mylineseries_9.Title = "Output of current #3";
            mylineseries_9.LineSmoothness = 1;
            mylineseries_9.PointGeometry = null;
            mylineseries_9.Fill = Brushes.Transparent;

            mylineseries_13.Title = "Output of current #4";
            mylineseries_13.LineSmoothness = 1;
            mylineseries_13.PointGeometry = null;
            mylineseries_13.Fill = Brushes.Transparent;

            mylineseries_17.Title = "Output of current #5";
            mylineseries_17.LineSmoothness = 1;
            mylineseries_17.PointGeometry = null;
            mylineseries_17.Fill = Brushes.Transparent;

            mylineseries_21.Title = "Output of current #6";
            mylineseries_21.LineSmoothness = 1;
            mylineseries_21.PointGeometry = null;
            mylineseries_21.Fill = Brushes.Transparent;

            mylineseries_25.Title = "Output of current #7";
            mylineseries_25.LineSmoothness = 1;
            mylineseries_25.PointGeometry = null;
            mylineseries_25.Fill = Brushes.Transparent;

            mylineseries_29.Title = "Output of current #8";
            mylineseries_29.LineSmoothness = 1;
            mylineseries_29.PointGeometry = null;
            mylineseries_29.Fill = Brushes.Transparent;

            mylineseries_33.Title = "Output of current #9";
            mylineseries_33.LineSmoothness = 1;
            mylineseries_33.PointGeometry = null;
            mylineseries_33.Fill = Brushes.Transparent;

            mylineseries_37.Title = "Output of current #10";
            mylineseries_37.LineSmoothness = 1;
            mylineseries_37.PointGeometry = null;
            mylineseries_37.Fill = Brushes.Transparent;


            mylineseries_1.Values = new ChartValues<double>(temp);
            mylineseries_5.Values = new ChartValues<double>(temp);
            mylineseries_9.Values = new ChartValues<double>(temp);
            mylineseries_13.Values = new ChartValues<double>(temp);
            mylineseries_17.Values = new ChartValues<double>(temp);
            mylineseries_21.Values = new ChartValues<double>(temp);
            mylineseries_25.Values = new ChartValues<double>(temp);
            mylineseries_29.Values = new ChartValues<double>(temp);
            mylineseries_33.Values = new ChartValues<double>(temp);
            mylineseries_37.Values = new ChartValues<double>(temp);
            OutputCurState = new SeriesCollection { };
            OutputCurState.Add(mylineseries_1);
            OutputCurState.Add(mylineseries_5);
            OutputCurState.Add(mylineseries_9);
            OutputCurState.Add(mylineseries_13);
            OutputCurState.Add(mylineseries_17);
            OutputCurState.Add(mylineseries_21);
            OutputCurState.Add(mylineseries_25);
            OutputCurState.Add(mylineseries_29);
            OutputCurState.Add(mylineseries_33);
            OutputCurState.Add(mylineseries_37);

            /*画出每一路的耗电量*/
            mylineseries_2.Title = "Output of power consumption #1";
            mylineseries_2.LineSmoothness = 1;
            mylineseries_2.PointGeometry = null;
            mylineseries_2.Fill = Brushes.Transparent;

            mylineseries_6.Title = "Output of power consumption #2";
            mylineseries_6.LineSmoothness = 1;
            mylineseries_6.PointGeometry = null;
            mylineseries_6.Fill = Brushes.Transparent;

            mylineseries_10.Title = "Output of power consumption #3";
            mylineseries_10.LineSmoothness = 1;
            mylineseries_10.PointGeometry = null;
            mylineseries_10.Fill = Brushes.Transparent;

            mylineseries_14.Title = "Output of power consumption #4";
            mylineseries_14.LineSmoothness = 1;
            mylineseries_14.PointGeometry = null;
            mylineseries_14.Fill = Brushes.Transparent;

            mylineseries_18.Title = "Output of power consumption #5";
            mylineseries_18.LineSmoothness = 1;
            mylineseries_18.PointGeometry = null;
            mylineseries_18.Fill = Brushes.Transparent;

            mylineseries_22.Title = "Output of power consumption #6";
            mylineseries_22.LineSmoothness = 1;
            mylineseries_22.PointGeometry = null;
            mylineseries_22.Fill = Brushes.Transparent;

            mylineseries_26.Title = "Output of power consumption #7";
            mylineseries_26.LineSmoothness = 1;
            mylineseries_26.PointGeometry = null;
            mylineseries_26.Fill = Brushes.Transparent;

            mylineseries_30.Title = "Output of power consumption #8";
            mylineseries_30.LineSmoothness = 1;
            mylineseries_30.PointGeometry = null;
            mylineseries_30.Fill = Brushes.Transparent;

            mylineseries_34.Title = "Output of power consumption #9";
            mylineseries_34.LineSmoothness = 1;
            mylineseries_34.PointGeometry = null;
            mylineseries_34.Fill = Brushes.Transparent;

            mylineseries_38.Title = "Output of power consumption #10";
            mylineseries_38.LineSmoothness = 1;
            mylineseries_38.PointGeometry = null;
            mylineseries_38.Fill = Brushes.Transparent; 



            mylineseries_2.Values = new ChartValues<double>(temp);
            mylineseries_6.Values = new ChartValues<double>(temp);
            mylineseries_10.Values = new ChartValues<double>(temp);
            mylineseries_14.Values = new ChartValues<double>(temp);
            mylineseries_18.Values = new ChartValues<double>(temp);
            mylineseries_22.Values = new ChartValues<double>(temp);
            mylineseries_26.Values = new ChartValues<double>(temp);
            mylineseries_30.Values = new ChartValues<double>(temp);
            mylineseries_34.Values = new ChartValues<double>(temp);
            mylineseries_38.Values = new ChartValues<double>(temp);

            OutputConsumption = new SeriesCollection { };
            OutputConsumption.Add(mylineseries_2);
            OutputConsumption.Add(mylineseries_6);
            OutputConsumption.Add(mylineseries_10);
            OutputConsumption.Add(mylineseries_14);
            OutputConsumption.Add(mylineseries_18);
            OutputConsumption.Add(mylineseries_22);
            OutputConsumption.Add(mylineseries_26);
            OutputConsumption.Add(mylineseries_30);
            OutputConsumption.Add(mylineseries_34);
            OutputConsumption.Add(mylineseries_38);

            /*画出每一路的转速*/
            mylineseries_3.Title = "Output of speed #1";
            mylineseries_3.LineSmoothness = 1;
            mylineseries_3.PointGeometry = null;
            mylineseries_3.Fill = Brushes.Transparent;

            mylineseries_7.Title = "Output of speed #2";
            mylineseries_7.LineSmoothness = 1;
            mylineseries_7.PointGeometry = null;
            mylineseries_7.Fill = Brushes.Transparent;

            mylineseries_11.Title = "Output of speed #3";
            mylineseries_11.LineSmoothness = 1;
            mylineseries_11.PointGeometry = null;
            mylineseries_11.Fill = Brushes.Transparent;

            mylineseries_15.Title = "Output of speed #4";
            mylineseries_15.LineSmoothness = 1;
            mylineseries_15.PointGeometry = null;
            mylineseries_15.Fill = Brushes.Transparent;

            mylineseries_19.Title = "Output of speed #5";
            mylineseries_19.LineSmoothness = 1;
            mylineseries_19.PointGeometry = null;
            mylineseries_19.Fill = Brushes.Transparent;

            mylineseries_23.Title = "Output of speed #6";
            mylineseries_23.LineSmoothness = 1;
            mylineseries_23.PointGeometry = null;
            mylineseries_23.Fill = Brushes.Transparent;

            mylineseries_27.Title = "Output of speed #7";
            mylineseries_27.LineSmoothness = 1;
            mylineseries_27.PointGeometry = null;
            mylineseries_27.Fill = Brushes.Transparent;

            mylineseries_31.Title = "Output of speed #8";
            mylineseries_31.LineSmoothness = 1;
            mylineseries_31.PointGeometry = null;
            mylineseries_31.Fill = Brushes.Transparent;

            mylineseries_35.Title = "Output of speed #9";
            mylineseries_35.LineSmoothness = 1;
            mylineseries_35.PointGeometry = null;
            mylineseries_35.Fill = Brushes.Transparent;

            mylineseries_39.Title = "Output of speed #10";
            mylineseries_39.LineSmoothness = 1;
            mylineseries_39.PointGeometry = null;
            mylineseries_39.Fill = Brushes.Transparent; 


            mylineseries_3.Values = new ChartValues<double>(temp);
            mylineseries_7.Values = new ChartValues<double>(temp);
            mylineseries_11.Values = new ChartValues<double>(temp);
            mylineseries_15.Values = new ChartValues<double>(temp);
            mylineseries_19.Values = new ChartValues<double>(temp);
            mylineseries_23.Values = new ChartValues<double>(temp);
            mylineseries_27.Values = new ChartValues<double>(temp);
            mylineseries_31.Values = new ChartValues<double>(temp);
            mylineseries_35.Values = new ChartValues<double>(temp);
            mylineseries_39.Values = new ChartValues<double>(temp);

            OutputSpeed = new SeriesCollection { };
            OutputSpeed.Add(mylineseries_3);
            OutputSpeed.Add(mylineseries_7);
            OutputSpeed.Add(mylineseries_11);
            OutputSpeed.Add(mylineseries_15);
            OutputSpeed.Add(mylineseries_19);
            OutputSpeed.Add(mylineseries_23);
            OutputSpeed.Add(mylineseries_27);
            OutputSpeed.Add(mylineseries_31);
            OutputSpeed.Add(mylineseries_35);
            OutputSpeed.Add(mylineseries_39);

            DataContext = this;
        }
        public void Paint_event(object source, System.Timers.ElapsedEventArgs e)
        {
            if (cell.inside_all_vol_mV != null)
            {
                linestart_1();
            }
        }

        public void Data_FPS_UI(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(
                   new Action(
                       delegate
                       {
                           Data_FPS_Str = Data_FPS.ToString();
                           label.Content = Data_FPS_Str + "FPS";
                           label1.Content = Data_length + "Bytes";
                           Data_FPS = 0;
                       }
                      )
                   );
            }
            catch 
            {
                Dispatcher.Invoke(
                  new Action(
                      delegate
                      {
                          label.Content ="0FPS";
                          label1.Content = "0Bytes";
                      }
                     )
                  );
            }
        }
        public void linestart_1()
        {
            if (Port_Find_Signal == 1)
            {
                try
                {
                    Task.Run(() =>
                    {
                        while (true)
                        {
                            Double output_vol_first = Double.Parse((Convert.ToDouble(cell_one.output_vol)/1000).ToString("0.0"));
                            Double output_vol_second = Double.Parse((Convert.ToDouble(cell_two.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_third = Double.Parse((Convert.ToDouble(cell_three.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_fourth = Double.Parse((Convert.ToDouble(cell_four.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_fifth = Double.Parse((Convert.ToDouble(cell_five.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_sixth = Double.Parse((Convert.ToDouble(cell_six.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_seventh = Double.Parse((Convert.ToDouble(cell_seven.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_eighth = Double.Parse((Convert.ToDouble(cell_eight.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_ninth = Double.Parse((Convert.ToDouble(cell_nine.output_vol) / 1000).ToString("0.0"));
                            Double output_vol_tenth = Double.Parse(cell_ten.output_vol) / 1000;

                            Double output_cur_first = Double.Parse((Convert.ToDouble(cell_one.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_second = Double.Parse((Convert.ToDouble(cell_two.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_third = Double.Parse((Convert.ToDouble(cell_three.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_fourth = Double.Parse((Convert.ToDouble(cell_four.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_fifth = Double.Parse((Convert.ToDouble(cell_five.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_sixth = Double.Parse((Convert.ToDouble(cell_six.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_seventh = Double.Parse((Convert.ToDouble(cell_seven.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_eighth = Double.Parse((Convert.ToDouble(cell_eight.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_ninth = Double.Parse((Convert.ToDouble(cell_nine.output_cur) / 1000).ToString("0.0"));
                            Double output_cur_tenth = Double.Parse(cell_ten.output_cur) / 1000;

                            Double output_con_first = Double.Parse(Convert.ToDouble(cell_one.output_pow).ToString("0.0")); 
                            Double output_con_second = Double.Parse(Convert.ToDouble(cell_two.output_pow).ToString("0.0"));;
                            Double output_con_third = Double.Parse(Convert.ToDouble(cell_three.output_pow).ToString("0.0"));
                            Double output_con_fourth = Double.Parse(Convert.ToDouble(cell_four.output_pow).ToString("0.0"));
                            Double output_con_fifth = Double.Parse(Convert.ToDouble(cell_five.output_pow).ToString("0.0"));
                            Double output_con_sixth = Double.Parse(Convert.ToDouble(cell_six.output_pow).ToString("0.0"));
                            Double output_con_seventh = Double.Parse(Convert.ToDouble(cell_seven.output_pow).ToString("0.0"));
                            Double output_con_eighth = Double.Parse(Convert.ToDouble(cell_eight.output_pow).ToString("0.0"));
                            Double output_con_ninth = Double.Parse(Convert.ToDouble(cell_nine.output_pow).ToString("0.0"));
                            Double output_con_tenth = Double.Parse(Convert.ToDouble(cell_ten.output_pow).ToString("0.0"));

                            Double output_spd_first = Double.Parse(cell_one.output_spd);
                            Double output_spd_second = Double.Parse(cell_two.output_spd);
                            Double output_spd_third = Double.Parse(cell_three.output_spd);
                            Double output_spd_fourth = Double.Parse(cell_four.output_spd);
                            Double output_spd_fifth = Double.Parse(cell_five.output_spd);
                            Double output_spd_sixth = Double.Parse(cell_six.output_spd);
                            Double output_spd_seventh = Double.Parse(cell_seven.output_spd);
                            Double output_spd_eighth = Double.Parse(cell_eight.output_spd);
                            Double output_spd_ninth = Double.Parse(cell_nine.output_spd);
                            Double output_spd_tenth = Double.Parse(cell_ten.output_spd);
                            Thread.Sleep(50);
                            //通过Dispatcher在工作线程中更新窗体的UI元素
                            Application.Current.Dispatcher.Invoke(() =>
                                {
                                //更新横坐标时间
                                Labels.Add(DateTime.Now.ToString());
                                    if (OutputVolState[0].Values.Count > 150)
                                    {
                                        Labels.RemoveAt(0);
                                    }
                                //更新纵坐标数据
                                OutputVolState[0].Values.Add(output_vol_first);
                                    if (OutputVolState[0].Values.Count > 200)
                                    {
                                        OutputVolState[0].Values.RemoveAt(0);
                                    }
                                    OutputVolState[1].Values.Add(output_vol_second);
                                    if (OutputVolState[1].Values.Count > 200)
                                    {
                                        OutputVolState[1].Values.RemoveAt(0);
                                    }

                                    OutputVolState[2].Values.Add(output_vol_third);
                                    if (OutputVolState[2].Values.Count > 200)
                                    {
                                        OutputVolState[2].Values.RemoveAt(0);
                                    }
                                    OutputVolState[3].Values.Add(output_vol_fourth);
                                    if (OutputVolState[3].Values.Count > 200)
                                    {
                                        OutputVolState[3].Values.RemoveAt(0);
                                    }
                                    OutputVolState[4].Values.Add(output_vol_fifth);
                                    if (OutputVolState[4].Values.Count > 200)
                                    {
                                        OutputVolState[4].Values.RemoveAt(0);
                                    }
                                    OutputVolState[5].Values.Add(output_vol_sixth);
                                    if (OutputVolState[5].Values.Count > 200)
                                    {
                                        OutputVolState[5].Values.RemoveAt(0);
                                    }
                                    OutputVolState[6].Values.Add(output_vol_seventh);
                                    if (OutputVolState[6].Values.Count > 200)
                                    {
                                        OutputVolState[6].Values.RemoveAt(0);
                                    }
                                    OutputVolState[7].Values.Add(output_vol_eighth);
                                    if (OutputVolState[7].Values.Count > 200)
                                    {
                                        OutputVolState[7].Values.RemoveAt(0);
                                    }
                                    OutputVolState[8].Values.Add(output_vol_ninth);
                                    if (OutputVolState[8].Values.Count > 200)
                                    {
                                        OutputVolState[8].Values.RemoveAt(0);
                                    }
                                    OutputVolState[9].Values.Add(output_vol_tenth);
                                    if (OutputVolState[9].Values.Count > 200)
                                    {
                                        OutputVolState[9].Values.RemoveAt(0);
                                    }


                                    OutputCurState[0].Values.Add(output_cur_first);
                                    if (OutputCurState[0].Values.Count > 200)
                                    {
                                        OutputCurState[0].Values.RemoveAt(0);
                                    }
                                    OutputCurState[1].Values.Add(output_cur_second);
                                    if (OutputCurState[1].Values.Count > 200)
                                    {
                                        OutputCurState[1].Values.RemoveAt(0);
                                    }
                                    OutputCurState[2].Values.Add(output_cur_third);
                                    if (OutputCurState[2].Values.Count > 200)
                                    {
                                        OutputCurState[2].Values.RemoveAt(0);
                                    }
                                    OutputCurState[3].Values.Add(output_cur_fourth);
                                    if (OutputCurState[3].Values.Count > 200)
                                    {
                                        OutputCurState[3].Values.RemoveAt(0);
                                    }
                                    OutputCurState[4].Values.Add(output_cur_fifth);
                                    if (OutputCurState[4].Values.Count > 200)
                                    {
                                        OutputCurState[4].Values.RemoveAt(0);
                                    }
                                    OutputCurState[5].Values.Add(output_cur_sixth);
                                    if (OutputCurState[5].Values.Count > 200)
                                    {
                                        OutputCurState[5].Values.RemoveAt(0);
                                    }
                                    OutputCurState[6].Values.Add(output_cur_seventh);
                                    if (OutputCurState[6].Values.Count > 200)
                                    {
                                        OutputCurState[6].Values.RemoveAt(0);
                                    }
                                    OutputCurState[7].Values.Add(output_cur_eighth);
                                    if (OutputCurState[7].Values.Count > 200)
                                    {
                                        OutputCurState[7].Values.RemoveAt(0);
                                    }
                                    OutputCurState[8].Values.Add(output_cur_ninth);
                                    if (OutputCurState[8].Values.Count > 200)
                                    {
                                        OutputCurState[8].Values.RemoveAt(0);
                                    }
                                    OutputCurState[9].Values.Add(output_cur_tenth);
                                    if (OutputCurState[9].Values.Count > 200)
                                    {
                                        OutputCurState[9].Values.RemoveAt(0);
                                    }


                                    OutputConsumption[0].Values.Add(output_con_first);
                                    if (OutputConsumption[0].Values.Count > 200)
                                    {
                                        OutputConsumption[0].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[1].Values.Add(output_con_second);
                                    if (OutputConsumption[1].Values.Count > 200)
                                    {
                                        OutputConsumption[1].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[2].Values.Add(output_con_third);
                                    if (OutputConsumption[2].Values.Count > 200)
                                    {
                                        OutputConsumption[2].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[3].Values.Add(output_con_fourth);
                                    if (OutputConsumption[3].Values.Count > 200)
                                    {
                                        OutputConsumption[3].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[4].Values.Add(output_con_fifth);
                                    if (OutputConsumption[4].Values.Count > 200)
                                    {
                                        OutputConsumption[4].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[5].Values.Add(output_con_sixth);
                                    if (OutputConsumption[5].Values.Count > 200)
                                    {
                                        OutputConsumption[5].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[6].Values.Add(output_con_seventh);
                                    if (OutputConsumption[6].Values.Count > 200)
                                    {
                                        OutputConsumption[6].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[7].Values.Add(output_con_eighth);
                                    if (OutputConsumption[7].Values.Count > 200)
                                    {
                                        OutputConsumption[7].Values.RemoveAt(0);
                                    }
                                    OutputConsumption[8].Values.Add(output_con_ninth);
                                    if (OutputConsumption[8].Values.Count > 200)
                                    {
                                        OutputConsumption[8].Values.RemoveAt(0);
                                    }
                                   /* OutputConsumption[9].Values.Add(output_con_tenth);
                                    if (OutputConsumption[9].Values.Count > 200)
                                    {
                                        OutputConsumption[9].Values.RemoveAt(0);
                                    }*/


                                    OutputSpeed[0].Values.Add(output_spd_first);
                                    if (OutputSpeed[0].Values.Count > 200)
                                    {
                                        OutputSpeed[0].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[1].Values.Add(output_spd_second);
                                    if (OutputSpeed[1].Values.Count > 200)
                                    {
                                        OutputSpeed[1].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[2].Values.Add(output_spd_third);
                                    if (OutputSpeed[2].Values.Count > 200)
                                    {
                                        OutputSpeed[2].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[3].Values.Add(output_spd_fourth);
                                    if (OutputSpeed[3].Values.Count > 200)
                                    {
                                        OutputSpeed[3].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[4].Values.Add(output_spd_fifth);
                                    if (OutputSpeed[4].Values.Count > 200)
                                    {
                                        OutputSpeed[4].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[5].Values.Add(output_spd_sixth);
                                    if (OutputSpeed[5].Values.Count > 200)
                                    {
                                        OutputSpeed[5].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[6].Values.Add(output_spd_seventh);
                                    if (OutputSpeed[6].Values.Count > 200)
                                    {
                                        OutputSpeed[6].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[7].Values.Add(output_spd_eighth);
                                    if (OutputSpeed[7].Values.Count > 200)
                                    {
                                        OutputSpeed[7].Values.RemoveAt(0);
                                    }
                                    OutputSpeed[8].Values.Add(output_spd_ninth);
                                    if (OutputSpeed[8].Values.Count > 200)
                                    {
                                        OutputSpeed[8].Values.RemoveAt(0);
                                    }
                                  /*  OutputSpeed[9].Values.Add(output_spd_tenth);
                                    if (OutputSpeed[9].Values.Count > 200)
                                    {
                                        OutputSpeed[9].Values.RemoveAt(0);
                                    }*/

                                });
                            break;
                        }

                    });
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void CartesianChart_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CartesianChart_Loaded_2(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (!s.IsOpen)
                {
                    closing = false;
                    s.PortName = Portselect.Text.ToString();
                    s.BaudRate = Convert.ToInt32(BPSselect.Text.ToString());
                    s.DataBits = 8;
                    s.StopBits = StopBits.One;
                    s.Parity = Parity.None;
                    s.Open();
                    Port_Find_Signal = 1;
                    s.DataReceived += s_DataReceived;
                    button2.Content = "关闭端口";

                    Portselect.IsEnabled = false;
                    BPSselect.IsEnabled = false;
                }
                else
                {
                    while(listening)
                    {
                        System.Windows.Forms.Application.DoEvents();
                    }
                    s.Close();
                    closing = true;
                    Port_Find_Signal = 0;
                    button2.Content = "打开端口";
                    s.DataReceived -= s_DataReceived;
                    Portselect.IsEnabled = true;
                    BPSselect.IsEnabled = true;
                    // Thread.Sleep(20);
                }
            }
            catch (Exception ee)
            {
                if (button2.Content.ToString() == "关闭端口")
                {
                    button2.Content = "打开端口";
                }
                else
                {
                    MessageBox.Show(ee.ToString());
                }
            }
        }
        /********************************解析JSON数据格式中定义的类******************************************/
        public class power_state
        {
            public string timeStamp { get; set; }
            public string[] cellState { get; set; }
            public string[][] outputState1 { get; set; }
        }

        public class power_state_last
        {
            public string timeStamp { get; set; }
            public string[] cellState = new string[9];
            public string[,] outputState1 = new string[10,7];   
        }

        public class parameter_calculation
        {
            public string type { get; set; }
            public string[][] script { get; set; }
        }

        public class parameter_passing
        {
            public int type { get; set; }
            public int cmd { get; set; }
            public int value { get; set; }
            public int response { get; set; }
        }
        public class original_configuration
        {
            public int type { get; set; }
            public int cmd { get; set; }
            public int value { get; set; }
            public int response { get; set; }

        }

        public class imme_deal_configuration
        {
            public int type { get; set; }
            public int cmd { get; set; }

            public List<int> ch_value = new List<int>();
            public int response { get; set; }
        }
        struct cell_output
        {
            public string output_vol;
            public string output_cur;
            public string output_pow;
            public string output_spd;
            public string output_consumption;
            public string output_exp_PWM;
            public string output_exp_CAN;
        }
        cell_output cell_one, cell_two, cell_three, cell_four;
        cell_output cell_five, cell_six, cell_seven, cell_eight;
        cell_output cell_nine, cell_ten;
        power_state state = new power_state();
        power_state_last state_last = new power_state_last();
        original_configuration configuration = new original_configuration();
        parameter_calculation calculation = new parameter_calculation();
        parameter_passing passingword = new parameter_passing();
        imme_deal_configuration Imme_Deal_Configuration = new imme_deal_configuration();
        imme_deal_configuration cur_cal_cmd = new imme_deal_configuration();
        /**********************************************************************************************************/


        private void checkBox_Copy_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy.IsChecked == true)
            {
                mylineseries.Visibility = Visibility.Visible;
                mylineseries_1.Visibility = Visibility.Visible;
                mylineseries_2.Visibility = Visibility.Visible;
                mylineseries_3.Visibility = Visibility.Visible;

            }

            if (checkBox_Copy.IsChecked == false)
            {
                mylineseries.Visibility = Visibility.Hidden;
                mylineseries_1.Visibility = Visibility.Hidden;
                mylineseries_2.Visibility = Visibility.Hidden;
                mylineseries_3.Visibility = Visibility.Hidden;
            }
        }

        private void checkBox_Copy1_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy1.IsChecked == true)
            {
                mylineseries_4.Visibility = Visibility.Visible;
                mylineseries_5.Visibility = Visibility.Visible;
                mylineseries_6.Visibility = Visibility.Visible;
                mylineseries_7.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy1.IsChecked == false)
            {
                mylineseries_4.Visibility = Visibility.Hidden;
                mylineseries_5.Visibility = Visibility.Hidden;
                mylineseries_6.Visibility = Visibility.Hidden;
                mylineseries_7.Visibility = Visibility.Hidden;

            }
        }

        private void checkBox_Copy2_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy2.IsChecked == true)
            {
                mylineseries_8.Visibility = Visibility.Visible;
                mylineseries_9.Visibility = Visibility.Visible;
                mylineseries_10.Visibility = Visibility.Visible;
                mylineseries_11.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy2.IsChecked == false)
            {
                mylineseries_8.Visibility = Visibility.Hidden;
                mylineseries_9.Visibility = Visibility.Hidden;
                mylineseries_10.Visibility = Visibility.Hidden;
                mylineseries_11.Visibility = Visibility.Hidden;

            }
        }

        private void checkBox_Copy3_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy3.IsChecked == true)
            {
                mylineseries_12.Visibility = Visibility.Visible;
                mylineseries_13.Visibility = Visibility.Visible;
                mylineseries_14.Visibility = Visibility.Visible;
                mylineseries_15.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy3.IsChecked == false)
            {
                mylineseries_12.Visibility = Visibility.Hidden;
                mylineseries_13.Visibility = Visibility.Hidden;
                mylineseries_14.Visibility = Visibility.Hidden;
                mylineseries_15.Visibility = Visibility.Hidden;

            }
        }

        private void checkBox_Copy4_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy4.IsChecked == true)
            {
                mylineseries_16.Visibility = Visibility.Visible;
                mylineseries_17.Visibility = Visibility.Visible;
                mylineseries_18.Visibility = Visibility.Visible;
                mylineseries_19.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy4.IsChecked == false)
            {
                mylineseries_16.Visibility = Visibility.Hidden;
                mylineseries_17.Visibility = Visibility.Hidden;
                mylineseries_18.Visibility = Visibility.Hidden;
                mylineseries_19.Visibility = Visibility.Hidden;

            }
        }

        private void checkBox_Copy5_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy5.IsChecked == true)
            {
                mylineseries_20.Visibility = Visibility.Visible;
                mylineseries_21.Visibility = Visibility.Visible;
                mylineseries_22.Visibility = Visibility.Visible;
                mylineseries_23.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy5.IsChecked == false)
            {
                mylineseries_20.Visibility = Visibility.Hidden;
                mylineseries_21.Visibility = Visibility.Hidden;
                mylineseries_22.Visibility = Visibility.Hidden;
                mylineseries_23.Visibility = Visibility.Hidden;

            }
        }
        private void checkBox_Copy6_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy6.IsChecked == true)
            {
                mylineseries_24.Visibility = Visibility.Visible;
                mylineseries_25.Visibility = Visibility.Visible;
                mylineseries_26.Visibility = Visibility.Visible;
                mylineseries_27.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy6.IsChecked == false)
            {
                mylineseries_24.Visibility = Visibility.Hidden;
                mylineseries_25.Visibility = Visibility.Hidden;
                mylineseries_26.Visibility = Visibility.Hidden;
                mylineseries_27.Visibility = Visibility.Hidden;

            }
        }
        private void checkBox_Copy7_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy7.IsChecked == true)
            {
                mylineseries_28.Visibility = Visibility.Visible;
                mylineseries_29.Visibility = Visibility.Visible;
                mylineseries_30.Visibility = Visibility.Visible;
                mylineseries_31.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy7.IsChecked == false)
            {
                mylineseries_28.Visibility = Visibility.Hidden;
                mylineseries_29.Visibility = Visibility.Hidden;
                mylineseries_30.Visibility = Visibility.Hidden;
                mylineseries_31.Visibility = Visibility.Hidden;

            }
        }
        private void checkBox_Copy8_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy8.IsChecked == true)
            {
                mylineseries_32.Visibility = Visibility.Visible;
                mylineseries_33.Visibility = Visibility.Visible;
                mylineseries_34.Visibility = Visibility.Visible;
                mylineseries_35.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy8.IsChecked == false)
            {
                mylineseries_32.Visibility = Visibility.Hidden;
                mylineseries_33.Visibility = Visibility.Hidden;
                mylineseries_34.Visibility = Visibility.Hidden;
                mylineseries_35.Visibility = Visibility.Hidden;

            }
        }
        private void checkBox_Copy9_Click(object sender, RoutedEventArgs e)
        {

            if (checkBox_Copy9.IsChecked == true)
            {
                mylineseries_36.Visibility = Visibility.Visible;
                mylineseries_37.Visibility = Visibility.Visible;
                mylineseries_38.Visibility = Visibility.Visible;
                mylineseries_39.Visibility = Visibility.Visible;

            }
            if (checkBox_Copy9.IsChecked == false)
            {
                mylineseries_36.Visibility = Visibility.Hidden;
                mylineseries_37.Visibility = Visibility.Hidden;
                mylineseries_38.Visibility = Visibility.Hidden;
                mylineseries_39.Visibility = Visibility.Hidden;

            }
        }
        private void checkBox_Copy10_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox_Copy10.IsChecked == false)
            {
                mylineseries.Visibility = Visibility.Hidden;
                mylineseries_1.Visibility = Visibility.Hidden;
                mylineseries_2.Visibility = Visibility.Hidden;
                mylineseries_3.Visibility = Visibility.Hidden;
                mylineseries_4.Visibility = Visibility.Hidden;
                mylineseries_5.Visibility = Visibility.Hidden;
                mylineseries_6.Visibility = Visibility.Hidden;
                mylineseries_7.Visibility = Visibility.Hidden;
                mylineseries_8.Visibility = Visibility.Hidden;
                mylineseries_9.Visibility = Visibility.Hidden;
                mylineseries_10.Visibility = Visibility.Hidden;
                mylineseries_11.Visibility = Visibility.Hidden;
                mylineseries_12.Visibility = Visibility.Hidden;
                mylineseries_13.Visibility = Visibility.Hidden;
                mylineseries_14.Visibility = Visibility.Hidden;
                mylineseries_15.Visibility = Visibility.Hidden;
                mylineseries_16.Visibility = Visibility.Hidden;
                mylineseries_17.Visibility = Visibility.Hidden;
                mylineseries_18.Visibility = Visibility.Hidden;
                mylineseries_19.Visibility = Visibility.Hidden;
                mylineseries_20.Visibility = Visibility.Hidden;
                mylineseries_21.Visibility = Visibility.Hidden;
                mylineseries_22.Visibility = Visibility.Hidden;
                mylineseries_23.Visibility = Visibility.Hidden;
                mylineseries_24.Visibility = Visibility.Hidden;
                mylineseries_25.Visibility = Visibility.Hidden;
                mylineseries_26.Visibility = Visibility.Hidden;
                mylineseries_27.Visibility = Visibility.Hidden;
                mylineseries_28.Visibility = Visibility.Hidden;
                mylineseries_29.Visibility = Visibility.Hidden;
                mylineseries_30.Visibility = Visibility.Hidden;
                mylineseries_31.Visibility = Visibility.Hidden;
                mylineseries_32.Visibility = Visibility.Hidden;
                mylineseries_33.Visibility = Visibility.Hidden;
                mylineseries_34.Visibility = Visibility.Hidden;
                mylineseries_35.Visibility = Visibility.Hidden;
                mylineseries_36.Visibility = Visibility.Hidden;
                mylineseries_37.Visibility = Visibility.Hidden;
                mylineseries_38.Visibility = Visibility.Hidden;
                mylineseries_39.Visibility = Visibility.Hidden;
            }
            if (checkBox_Copy10.IsChecked == true)
            {
                mylineseries.Visibility = Visibility.Visible;
                mylineseries_1.Visibility = Visibility.Visible;
                mylineseries_2.Visibility = Visibility.Visible;
                mylineseries_3.Visibility = Visibility.Visible;
                mylineseries_4.Visibility = Visibility.Visible;
                mylineseries_5.Visibility = Visibility.Visible;
                mylineseries_6.Visibility = Visibility.Visible;
                mylineseries_7.Visibility = Visibility.Visible;
                mylineseries_8.Visibility = Visibility.Visible;
                mylineseries_9.Visibility = Visibility.Visible;
                mylineseries_10.Visibility = Visibility.Visible;
                mylineseries_11.Visibility = Visibility.Visible;
                mylineseries_12.Visibility = Visibility.Visible;
                mylineseries_13.Visibility = Visibility.Visible;
                mylineseries_14.Visibility = Visibility.Visible;
                mylineseries_15.Visibility = Visibility.Visible;
                mylineseries_16.Visibility = Visibility.Visible;
                mylineseries_17.Visibility = Visibility.Visible;
                mylineseries_18.Visibility = Visibility.Visible;
                mylineseries_19.Visibility = Visibility.Visible;
                mylineseries_20.Visibility = Visibility.Visible;
                mylineseries_21.Visibility = Visibility.Visible;
                mylineseries_22.Visibility = Visibility.Visible;
                mylineseries_23.Visibility = Visibility.Visible;
                mylineseries_24.Visibility = Visibility.Visible;
                mylineseries_25.Visibility = Visibility.Visible;
                mylineseries_26.Visibility = Visibility.Visible;
                mylineseries_27.Visibility = Visibility.Visible;
                mylineseries_28.Visibility = Visibility.Visible;
                mylineseries_29.Visibility = Visibility.Visible;
                mylineseries_30.Visibility = Visibility.Visible;
                mylineseries_31.Visibility = Visibility.Visible;
                mylineseries_32.Visibility = Visibility.Visible;
                mylineseries_33.Visibility = Visibility.Visible;
                mylineseries_34.Visibility = Visibility.Visible;
                mylineseries_35.Visibility = Visibility.Visible;
                mylineseries_36.Visibility = Visibility.Visible;
                mylineseries_37.Visibility = Visibility.Visible;
                mylineseries_38.Visibility = Visibility.Visible;
                mylineseries_39.Visibility = Visibility.Visible;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "选择数据源文件";
            openFileDialog.Filter = "txt文件|*.txt";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }

            string txtFile = openFileDialog.FileName;
            LoadTextFile(richTextBox1, txtFile);
            TextBox.SelectedText = txtFile;
        }

        /***********************************************打开文件夹文本内容******************************************************/
        private void LoadTextFile(RichTextBox richTextBox, string filename)
        {
            richTextBox.Document.Blocks.Clear();
            using (StreamReader streamReader = File.OpenText(filename))
            {
                richTextBox.Document.Blocks.Add(new Paragraph(new Run(streamReader.ReadToEnd())));
            }
            Send_doc.IsEnabled = true;
            Start_btn.IsEnabled = false;

        }
        /***********************************************读取并解析文件夹文本内容******************************************************/

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (!s.IsOpen)
            {
                s.Open();
                button2.Content = "关闭端口";
            }
            string script = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
            int firstIndex = script.IndexOf("{");
            int lastIndex = script.LastIndexOf("}");
            if (firstIndex >= 0 && lastIndex > 0)
            {
                script = script.Substring(firstIndex, lastIndex - firstIndex + 1);
                script = script.Replace("\\", "");
                script = script.Replace("\"{", "{");
                script = script.Replace("}\"", "}");
                try
                {
                    calculation = JsonConvert.DeserializeObject<parameter_calculation>(script);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.ToString());
                }
                //     char Calculation_Script = Convert.ToChar(calculation.script);
                int CountOfParameter = calculation.script.Length * 100;
                passingword.type = 2;
                passingword.cmd = 1;
                passingword.value = CountOfParameter;
                passingword.response = 0;
                string passingword_Json = JsonConvert.SerializeObject(passingword);
                s.Write(passingword_Json);
                mission_recheck = 1;
                passingword.type = 2;
                passingword.cmd = 2;
                passingword.value = CountOfParameter;
                passingword.response = 0;
                count = 0;
            }
        }
        /*********************************************************************************************************************/

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (mission_check == 1)
            {
                string passingword_filedeal_Json = JsonConvert.SerializeObject(passingword);
                s.Write(passingword_filedeal_Json);
            }

            else if(mission_check == 0)
            {
                Imme_Deal_Configuration.type = 3;
                Imme_Deal_Configuration.cmd = 1;
                /****************************************字符串转整形让JSON格式没有双引号**********************************/
                int PWM1_CONTENT_INT = int.Parse(PWM1_CONTENT.Text);
                int PWM2_CONTENT_INT = int.Parse(PWM2_CONTENT.Text);
                int PWM3_CONTENT_INT = int.Parse(PWM3_CONTENT.Text);
                int PWM4_CONTENT_INT = int.Parse(PWM4_CONTENT.Text);
                int PWM5_CONTENT_INT = int.Parse(PWM5_CONTENT.Text);
                int PWM6_CONTENT_INT = int.Parse(PWM6_CONTENT.Text);
                int PWM7_CONTENT_INT = int.Parse(PWM7_CONTENT.Text);
                int PWM8_CONTENT_INT = int.Parse(PWM8_CONTENT.Text);
                if (PWM1_CONTENT_INT > 1000 || PWM2_CONTENT_INT > 1000 || PWM3_CONTENT_INT > 1000 || PWM4_CONTENT_INT > 1000 || PWM5_CONTENT_INT > 1000 || PWM6_CONTENT_INT > 1000 || PWM7_CONTENT_INT > 1000 || PWM8_CONTENT_INT > 1000
                    ||PWM1_CONTENT_INT < 0 || PWM2_CONTENT_INT < 0 || PWM3_CONTENT_INT < 0 || PWM4_CONTENT_INT < 0 || PWM5_CONTENT_INT < 0 || PWM6_CONTENT_INT < 0 || PWM7_CONTENT_INT < 0 || PWM8_CONTENT_INT < 0)
                {
                    MessageBox.Show("非法操作\r\n提示：PWM设置范围为0-1000");
                }
                else
                {
                    List<int> parameters = new List<int> { PWM1_CONTENT_INT, PWM2_CONTENT_INT, PWM3_CONTENT_INT, PWM4_CONTENT_INT, PWM5_CONTENT_INT, PWM6_CONTENT_INT, PWM7_CONTENT_INT, PWM8_CONTENT_INT };
                    /*********************************************处理ch_value的列表元素************************************************/
                    Imme_Deal_Configuration.ch_value.InsertRange(0, parameters);
                    Imme_Deal_Configuration.response = 0;
                    string single_Deal_configuration_Json = JsonConvert.SerializeObject(Imme_Deal_Configuration);
                    if (s.IsOpen)
                    {
                        s.Write(single_Deal_configuration_Json);
                        richTextBox1.Document.Blocks.Add(new Paragraph(new Run("Messages are being processed...")));
                    }
                    Imme_Deal_Configuration.ch_value.RemoveRange(0, 8);//不使用新实例化对象来解决列表删除问题，不然会导致处理速度降低

                }
            }
        }
        /****************************************************************画表格初始化**************************************************************************/

        /****************************************************************画表格记录数据**************************************************************************/

        private void CallToSaveThread()
        {
            if (Save_flag == 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    ///#####################################################
                    if (!Directory.Exists(File_name))
                    {
                        Directory.CreateDirectory(File_name);
                    }

                    StreamWriter rev_in = File.AppendText(File_name + rev_FileName);

                    if (!File.Exists(File_name + rev_FileName))
                    {
                        File.Create(File_name + rev_FileName).Close();
                    }
                    //###########################设置保存参数
                    string logi_Format = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9} ," +
                            " {10}, {11}, {12}, {13}, {14}, {15}, " +
                            "{16}, {17}, {18}, {19}, {20}, {21}, " +
                            "{22}, {23}, {24}, {25}, {26}, {27}, " +
                            "{28}, {29}, {30}, {31}, {32}, {33}, " +
                            "{34}, {35}, {36}, {37}, {38}, {39}, " +
                            "{40}, {41}, {42}, {43}, {44}, {45}, " +
                            "{46}, {47}, {48}, {49}, {50}, {51}, " +
                            "{52}, {53}, {54}, {55}, {56}, {57}, " +
                            "{58}, {59}",
                            state.timeStamp, cell.inside_all_vol_mV, cell.inside_cur_mA, cell.inside_ele_mW, cell.inside_fir_vol_mV, cell.inside_sec_vol_mV, cell.inside_thi_vol_mV, cell.inside_for_vol_mV, cell.inside_fif_vol_mV, cell.inside_six_vol_mV,
                            cell_one.output_vol, cell_one.output_cur, cell_one.output_pow, cell_one.output_consumption, cell_one.output_exp_PWM,
                            cell_two.output_vol, cell_two.output_cur, cell_two.output_pow, cell_two.output_consumption, cell_two.output_exp_PWM,
                            cell_three.output_vol, cell_three.output_cur, cell_three.output_pow, cell_three.output_consumption, cell_three.output_exp_PWM,
                            cell_four.output_vol, cell_four.output_cur, cell_four.output_pow, cell_four.output_consumption, cell_four.output_exp_PWM,
                            cell_five.output_vol, cell_five.output_cur, cell_five.output_pow, cell_five.output_consumption, cell_five.output_exp_PWM,
                            cell_six.output_vol, cell_six.output_cur, cell_six.output_pow, cell_six.output_consumption, cell_six.output_exp_PWM,
                            cell_seven.output_vol, cell_seven.output_cur, cell_seven.output_pow, cell_seven.output_consumption, cell_seven.output_exp_PWM,
                            cell_eight.output_vol, cell_eight.output_cur, cell_eight.output_pow, cell_eight.output_consumption, cell_eight.output_exp_PWM,
                            cell_nine.output_vol, cell_nine.output_cur, cell_nine.output_pow, cell_nine.output_consumption, cell_nine.output_exp_PWM,
                            cell_ten.output_vol, cell_ten.output_cur, cell_ten.output_pow, cell_ten.output_consumption, cell_ten.output_exp_PWM);

                    rev_in.WriteLine(logi_Format);
                    rev_in.Flush();
                    rev_in.Close();
                    rev_in.Dispose();

                    //##########################################################
                }));
            }
        }
        /**************************************************************************************************************************************************/
        struct cell_original_state
        {
            public string timeStamp;
            public string inside_all_vol_mV;
            public string inside_cur_mA;
            public string inside_ele_mW;
            public string inside_fir_vol_mV;
            public string inside_sec_vol_mV;
            public string inside_thi_vol_mV;
            public string inside_for_vol_mV;
            public string inside_fif_vol_mV;
            public string inside_six_vol_mV;
        }
        cell_original_state cell;
        /********************************************************************串口处理程序**************************************************************************/
        private void s_DataReceived(object sender, EventArgs e)
        {
            if (closing) return;
            if (mission_recheck == 1)
            {
                Thread.Sleep(50);
                if (s.IsOpen)
                {
                    timer_1.Start();
                    byte[] receiveData = new byte[s.BytesToRead];
                    if (receiveData != null)
                    {
                        s.Read(receiveData, 0, receiveData.Length);
                    }
                    string configure = Encoding.ASCII.GetString(receiveData, 0, receiveData.Length);

                    int firstIndex = configure.LastIndexOf("{");
                    int my_type_bytes = configure.LastIndexOf("type");
                    //configure = configure.Substring(firstIndex, 100);
                    int lastIndex = configure.IndexOf("}");
                    if (my_type_bytes != -1)
                    {
                        firstIndex = configure.LastIndexOf("{", my_type_bytes);
                        lastIndex = configure.IndexOf("}", my_type_bytes);
                    }

                    if (firstIndex >= 0 && lastIndex > 0 && my_type_bytes > 0)
                    {
                        configure = configure.Substring(firstIndex, lastIndex - firstIndex + 1);
                        configure = configure.Replace("\\", "");
                        configure = configure.Replace("\"{", "{");
                        configure = configure.Replace("}\"", "}");
                        try
                        {
                            configuration = JsonConvert.DeserializeObject<original_configuration>(configure);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("错误提示：" + ex.ToString());
                            return;
                        }

                        check_response();
                    }
                }
            }
            else
            {
                try
                {
                    Thread.Sleep(8);
                    listening = true;
                    int n = s.BytesToRead;
                    byte[] receiveData = new byte[n];

                    int flag = 0;

                    s.Read(receiveData, 0, n);

                    Data_length = receiveData.Length.ToString();
                    string temp = Encoding.ASCII.GetString(receiveData, 0, n);

                    int firstIndex = temp.IndexOf("{");
                    int lastIndex = temp.IndexOf("}");
                    if (firstIndex != -1)
                    {
                        if (lastIndex != -1)
                        {
                            if (lastIndex > firstIndex)
                            {
                                string[] arr = temp.Split(new string[] { "}" }, StringSplitOptions.RemoveEmptyEntries);
                                int lenth = arr.Length;
                                Data_FPS = lenth + Data_FPS;

                                temp = temp.Substring(firstIndex, lastIndex - firstIndex + 1);
                                temp = temp.Replace("\\", "");
                                temp = temp.Replace("\"{", "{");
                                temp = temp.Replace("}\"", "}");
                                try
                                {
                                    state = JsonConvert.DeserializeObject<power_state>(temp);
                                }
                                catch (Exception ex)
                                {
                                    flag = 1;
                                }
                                if (flag == 0)
                                {
                                    Value_assignment();
                                    Add_Cur_Cell_Output_State();
                                    CallToSaveThread();
                                }
                            }
                            if (lastIndex < firstIndex)
                            {
                                temp = "";
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        return;
                    }

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("错误提示：" + ex.ToString());
                    return;
                }
                finally
                {
                    listening = false;
                }
            }
        }
        /*************************************************************************************************************************************************************/

        private void Stop_Save_Click(object sender, EventArgs e)
        {
            if (Save_flag == 1)
            {
                Save_flag = 0;
                Save_btn.IsChecked = true;
                Stop_Save_btn.Header = "停止保存";
            }
            else if (Save_flag == 0)
            {
                Save_flag = 1;
                Save_btn.IsChecked = false;
                Stop_Save_btn.Header = "开始保存";
            }
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            richTextBox1.Document.Blocks.Clear();
          //  TextBox.Clear();
        }

        private void Portselect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            Portselect.ItemsSource = ports;
        }

        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            richTextBox1.ScrollToEnd();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

        }
        private void Data_Calibration(object sender, RoutedEventArgs e)
        {
            /*    if (!s.IsOpen)
                {
                    s.Open();
                }
                cur_cal_cmd.type = 4;
                cur_cal_cmd.cmd = 1;
                cur_cal_cmd.ch_value = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                cur_cal_cmd.response = 0;
                string cur_cal_cmd_Json = JsonConvert.SerializeObject(cur_cal_cmd);
                s.Write(cur_cal_cmd_Json);
                Thread.Sleep(10);
                s.Close();
                MessageBox.Show("校准命令已发送！");*/
            if (s.IsOpen)
            {
                s.Close();
            }
            calibration calibration1 = new calibration(s.PortName, s.BaudRate);
            calibration1.ShowDialog();
            s.Open();

        }
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            timer_1.Stop();
            mission_check = 0; 
            mission_recheck = 0;
            if (TextBox.Text == string.Empty)
            {
                MessageBox.Show("您没有加载文件");
            }
            TextBox.Clear();
            richTextBox1.Document.Blocks.Clear();
            Start_btn.IsEnabled = true;
            Send_doc.IsEnabled = false;

        }


        private void BPSselect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        private void checkBox_Copy_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void Open_Log(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(File_name);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public int count = 0;
        private void check_response()
        {
            Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        //代码块

                        string content = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd).Text;
                        if (configuration.response == 1)
                        {
                            timer_1.Stop();
                            count++;
                            if (count == 1)
                            {
                                //richTextBox1.Document.Blocks.Add(new Paragraph(new Run("system_response=" + configuration.response)));
                                richTextBox1.Document.Blocks.Add(new Paragraph(new Run("The bootloader is dealing with data...")));
                                Thread.Sleep(100);
                                s.Write(content);

                            }
                            if (count == 2)
                            {
                                //richTextBox1.Document.Blocks.Add(new Paragraph(new Run("system_response_Second=" + configuration.response)));
                                richTextBox1.Document.Blocks.Add(new Paragraph(new Run("Your data is already sent to the bootloader,Press the start button!")));
                                mission_check = 1;
                                Start_btn.IsEnabled = true;


                            }
                            if (count == 3)
                            {
                                //richTextBox1.Document.Blocks.Add(new Paragraph(new Run("system_response_Third=" + configuration.response)));
                                richTextBox1.Document.Blocks.Add(new Paragraph(new Run("The Mission is starting!")));
                                count = 0;
                                mission_recheck = 0;
                                timer_1.Stop();

                            }                       

                        }
                        else
                        {
                            richTextBox1.Document.Blocks.Add(new Paragraph(new Run("Sorry,sending error,please try again.")));
                            mission_check = 0;
                            mission_recheck = 0;
                            count = 0;
                            // if (s.IsOpen)
                            //  {
                            //      s.Close();
                            //  }
                            // button2.Content = "打开端口";
                            //  Portselect.IsEnabled = true;
                            // BPSselect.IsEnabled = true;
                        }
                    }
                )
            );

        }
        private void Error_report(object source, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(
               new Action(
                   delegate
                   {
                       timer_1.Stop();
                       richTextBox1.Document.Blocks.Add(new Paragraph(new Run("The host is not responding...")));
                       mission_recheck = 0;
                       mission_check = 0;
                       count = 0;
                       Send_doc.IsEnabled = false;
                   }
                )
            );
        }
        private void Add_Cur_Cell_Output_State()
        {
            Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        listView1.Items.Clear();
                        listView1.Items.Add(new BatteryInsideInFo(cell.inside_all_vol_mV, cell.inside_cur_mA, cell.inside_ele_mW, cell.inside_fir_vol_mV, cell.inside_sec_vol_mV, cell.inside_thi_vol_mV, cell.inside_for_vol_mV, cell.inside_fif_vol_mV, cell.inside_six_vol_mV));
                        /************************************************************剩余电量显示和预测剩余时间功能***************************************************************************/
                        double inside_ele_mW_INT = double.Parse(cell.inside_ele_mW);
                        double inside_cur_mA_INT = double.Parse(cell.inside_cur_mA);
                        inside_ele_mW_INT = (10000 - inside_ele_mW_INT) / 100;
                        battery_level_ui.Value = inside_ele_mW_INT;
                        battery_level_ui_info.Content = inside_ele_mW_INT.ToString() + "%";
                        estimate_hour = inside_ele_mW_INT * 100 / inside_cur_mA_INT;
                        estimate_minute = inside_ele_mW_INT * 100 % inside_cur_mA_INT * 60 / inside_cur_mA_INT;
                        estimate_second = inside_ele_mW_INT * 100 % inside_cur_mA_INT * 60 % inside_cur_mA_INT * 60 / inside_cur_mA_INT;

                        estimate_hour = (int)estimate_hour;
                        estimate_minute = (int)estimate_minute;
                        estimate_second = (int)estimate_second;
                        estimate_time.Content = estimate_hour.ToString() + ":" + estimate_minute.ToString() + ":" + estimate_second.ToString();
                        /********************************************************************************************************************************************************************/
                        listView2.Items.Clear();
                        listView2.Items.Add(new BatteryInFo("#1", cell_one.output_vol, cell_one.output_cur, cell_one.output_pow, cell_one.output_spd, cell_one.output_consumption, cell_one.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#2", cell_two.output_vol, cell_two.output_cur, cell_two.output_pow, cell_two.output_spd, cell_two.output_consumption, cell_two.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#3", cell_three.output_vol, cell_three.output_cur, cell_three.output_pow, cell_three.output_spd, cell_three.output_consumption, cell_three.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#4", cell_four.output_vol, cell_four.output_cur, cell_four.output_pow, cell_four.output_spd, cell_four.output_consumption, cell_four.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#5", cell_five.output_vol, cell_five.output_cur, cell_five.output_pow, cell_five.output_spd, cell_five.output_consumption, cell_five.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#6", cell_six.output_vol, cell_six.output_cur, cell_six.output_pow, cell_six.output_spd, cell_six.output_consumption, cell_six.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#7", cell_seven.output_vol, cell_seven.output_cur, cell_seven.output_pow, cell_seven.output_spd, cell_seven.output_consumption, cell_seven.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#8", cell_eight.output_vol, cell_eight.output_cur, cell_eight.output_pow, cell_eight.output_spd, cell_eight.output_consumption, cell_eight.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#9", cell_nine.output_vol, cell_nine.output_cur, cell_nine.output_pow, cell_nine.output_spd, cell_nine.output_consumption, cell_nine.output_exp_PWM));
                        listView2.Items.Add(new BatteryInFo("#10", cell_ten.output_vol, cell_ten.output_cur, cell_ten.output_pow, cell_ten.output_spd, cell_ten.output_consumption, cell_ten.output_exp_PWM));
                    })
                    );

        }
        private void Value_assignment()
        {
            /* 输入 详述*/
            check_data = 0;
            try
            {
                if (state.timeStamp != null)
                {
                    cell.timeStamp = state.timeStamp;
                    cell.inside_all_vol_mV = state.cellState[0];
                    cell.inside_cur_mA = state.cellState[1];
                    cell.inside_ele_mW = state.cellState[2];
                    cell.inside_fir_vol_mV = state.cellState[3];
                    cell.inside_sec_vol_mV = state.cellState[4];
                    cell.inside_thi_vol_mV = state.cellState[5];
                    cell.inside_for_vol_mV = state.cellState[6];
                    cell.inside_fif_vol_mV = state.cellState[7];
                    cell.inside_six_vol_mV = state.cellState[8];

                    /* 第一路 详述*/
                    cell_one.output_vol = state.outputState1[0][0];
                    cell_one.output_cur = state.outputState1[0][1];
                    cell_one.output_pow = state.outputState1[0][2];
                    cell_one.output_spd = state.outputState1[0][3];
                    cell_one.output_consumption = state.outputState1[0][4];
                    cell_one.output_exp_PWM = state.outputState1[0][5];
                    //cell_one.output_exp_CAN = state.outputState1[0][6];

                    /* 第二路 详述*/
                    cell_two.output_vol = state.outputState1[1][0];
                    cell_two.output_cur = state.outputState1[1][1];
                    cell_two.output_pow = state.outputState1[1][2];
                    cell_two.output_spd = state.outputState1[1][3];
                    cell_two.output_consumption = state.outputState1[1][4];
                    cell_two.output_exp_PWM = state.outputState1[1][5];
                    //cell_two.output_exp_CAN = state.outputState1[1][6];

                    /* 第三路 详述*/
                    cell_three.output_vol = state.outputState1[2][0];
                    cell_three.output_cur = state.outputState1[2][1];
                    cell_three.output_pow = state.outputState1[2][2];
                    cell_three.output_spd = state.outputState1[2][3];
                    cell_three.output_consumption = state.outputState1[2][4];
                    cell_three.output_exp_PWM = state.outputState1[2][5];
                   // cell_three.output_exp_CAN = state.outputState1[2][6];

                    /* 第四路 详述*/
                    cell_four.output_vol = state.outputState1[3][0];
                    cell_four.output_cur = state.outputState1[3][1];
                    cell_four.output_pow = state.outputState1[3][2];
                    cell_four.output_spd = state.outputState1[3][3];
                    cell_four.output_consumption = state.outputState1[3][4];
                    cell_four.output_exp_PWM = state.outputState1[3][5];
                    //cell_four.output_exp_CAN = state.outputState1[2][6];

                    /* 第五路 详述*/
                    cell_five.output_vol = state.outputState1[4][0];
                    cell_five.output_cur = state.outputState1[4][1];
                    cell_five.output_pow = state.outputState1[4][2];
                    cell_five.output_spd = state.outputState1[4][3];
                    cell_five.output_consumption = state.outputState1[4][4];
                    cell_five.output_exp_PWM = state.outputState1[4][5];
                    //cell_five.output_exp_CAN = state.outputState1[4][6];

                    /* 第六路 详述*/
                    cell_six.output_vol = state.outputState1[5][0];
                    cell_six.output_cur = state.outputState1[5][1];
                    cell_six.output_pow = state.outputState1[5][2];
                    cell_six.output_spd = state.outputState1[5][3];
                    cell_six.output_consumption = state.outputState1[5][4];
                    cell_six.output_exp_PWM = state.outputState1[5][5];
                    //cell_six.output_exp_CAN = state.outputState1[5][6];

                    /* 第七路 详述*/
                    cell_seven.output_vol = state.outputState1[6][0];
                    cell_seven.output_cur = state.outputState1[6][1];
                    cell_seven.output_pow = state.outputState1[6][2];
                    cell_seven.output_spd = state.outputState1[6][3];
                    cell_seven.output_consumption = state.outputState1[6][4];
                    cell_seven.output_exp_PWM = state.outputState1[6][5];
                    //cell_seven.output_exp_CAN = state.outputState1[6][6];

                    /* 第八路 详述*/
                    cell_eight.output_vol = state.outputState1[7][0];
                    cell_eight.output_cur = state.outputState1[7][1];
                    cell_eight.output_pow = state.outputState1[7][2];
                    cell_eight.output_spd = state.outputState1[7][3];
                    cell_eight.output_consumption = state.outputState1[7][4];
                    cell_eight.output_exp_PWM = state.outputState1[7][5];
                    //cell_eight.output_exp_CAN = state.outputState1[7][6];

                    /* 第九路 详述*/
                    cell_nine.output_vol = state.outputState1[8][0];
                    cell_nine.output_cur = state.outputState1[8][1];
                    cell_nine.output_pow = state.outputState1[8][2];
                    cell_nine.output_spd = state.outputState1[8][3];
                    cell_nine.output_consumption = state.outputState1[8][4];
                    cell_nine.output_exp_PWM = state.outputState1[8][5];
                    //cell_nine.output_exp_CAN = state.outputState1[8][6];

                    /* 第十路 详述*/
                    cell_ten.output_vol = state.outputState1[9][0];
                    cell_ten.output_cur = state.outputState1[9][1];
                    cell_ten.output_pow = state.outputState1[9][2];
                    cell_ten.output_spd = state.outputState1[9][3];
                    cell_ten.output_consumption = state.outputState1[9][4];
                    cell_ten.output_exp_PWM = state.outputState1[9][5];
                   // cell_ten.output_exp_CAN = state.outputState1[9][6];
                }
            }
            catch (Exception ee)
            {
                cell.timeStamp = state_last.timeStamp;
                cell.inside_all_vol_mV = state_last.cellState[0];
                cell.inside_cur_mA = state_last.cellState[1];
                cell.inside_ele_mW = state_last.cellState[2];
                cell.inside_fir_vol_mV = state_last.cellState[3];
                cell.inside_sec_vol_mV = state_last.cellState[4];
                cell.inside_thi_vol_mV = state_last.cellState[5];
                cell.inside_for_vol_mV = state_last.cellState[6];
                cell.inside_fif_vol_mV = state_last.cellState[7];
                cell.inside_six_vol_mV = state_last.cellState[8];

                /* 第一路 详述*/
                cell_one.output_vol = state_last.outputState1[0,0];
                cell_one.output_cur = state_last.outputState1[0,1];
                cell_one.output_pow = state_last.outputState1[0,2];
                cell_one.output_spd = state_last.outputState1[0,3];
                cell_one.output_consumption = state_last.outputState1[0,4];
                cell_one.output_exp_PWM = state_last.outputState1[0,5];
               // cell_one.output_exp_CAN = state_last.outputState1[0,6];

                /* 第二路 详述*/
                cell_two.output_vol = state_last.outputState1[1,0];
                cell_two.output_cur = state_last.outputState1[1,1];
                cell_two.output_pow = state_last.outputState1[1,2];
                cell_two.output_spd = state_last.outputState1[1,3];
                cell_two.output_consumption = state_last.outputState1[1,4];
                cell_two.output_exp_PWM = state_last.outputState1[1,5];
               // cell_two.output_exp_CAN = state_last.outputState1[1,6];

                /* 第三路 详述*/
                cell_three.output_vol = state_last.outputState1[2,0];
                cell_three.output_cur = state_last.outputState1[2,1];
                cell_three.output_pow = state_last.outputState1[2,2];
                cell_three.output_spd = state_last.outputState1[2,3];
                cell_three.output_consumption = state_last.outputState1[2,4];
                cell_three.output_exp_PWM = state_last.outputState1[2,5];
               // cell_three.output_exp_CAN = state_last.outputState1[2,6];

                /* 第四路 详述*/
                cell_four.output_vol = state_last.outputState1[3,0];
                cell_four.output_cur = state_last.outputState1[3,1];
                cell_four.output_pow = state_last.outputState1[3,2];
                cell_four.output_spd = state_last.outputState1[3,3];
                cell_four.output_consumption = state_last.outputState1[3,4];
                cell_four.output_exp_PWM = state_last.outputState1[3,5];
               // cell_four.output_exp_CAN = state_last.outputState1[3,6];

                /* 第五路 详述*/
                cell_five.output_vol = state_last.outputState1[4,0];
                cell_five.output_cur = state_last.outputState1[4,1];
                cell_five.output_pow = state_last.outputState1[4,2];
                cell_five.output_spd = state_last.outputState1[4,3];
                cell_five.output_consumption = state_last.outputState1[4,4];
                cell_five.output_exp_PWM = state_last.outputState1[4,5];
               // cell_five.output_exp_CAN = state_last.outputState1[4,6];

                /* 第六路 详述*/
                cell_six.output_vol = state_last.outputState1[5,0];
                cell_six.output_cur = state_last.outputState1[5,1];
                cell_six.output_pow = state_last.outputState1[5,2];
                cell_six.output_spd = state_last.outputState1[5,3];
                cell_six.output_consumption = state_last.outputState1[5,4];
                cell_six.output_exp_PWM = state_last.outputState1[5,5];
               // cell_six.output_exp_CAN = state_last.outputState1[5,6];

                /* 第七路 详述*/
                cell_seven.output_vol = state_last.outputState1[6,0];
                cell_seven.output_cur = state_last.outputState1[6,1];
                cell_seven.output_pow = state_last.outputState1[6,2];
                cell_seven.output_spd = state_last.outputState1[6,3];
                cell_seven.output_consumption = state_last.outputState1[6,4];
                cell_seven.output_exp_PWM = state_last.outputState1[6,5];
               // cell_seven.output_exp_CAN = state_last.outputState1[6,6];

                /* 第八路 详述*/
                cell_eight.output_vol = state_last.outputState1[7,0];
                cell_eight.output_cur = state_last.outputState1[7,1];
                cell_eight.output_pow = state_last.outputState1[7,2];
                cell_eight.output_spd = state_last.outputState1[7,3];
                cell_eight.output_consumption = state_last.outputState1[7,4];
                cell_eight.output_exp_PWM = state_last.outputState1[7,5];
               // cell_eight.output_exp_CAN = state_last.outputState1[7,6];

                /* 第九路 详述*/
                cell_nine.output_vol = state_last.outputState1[8,0];
                cell_nine.output_cur = state_last.outputState1[8,1];
                cell_nine.output_pow = state_last.outputState1[8,2];
                cell_nine.output_spd = state_last.outputState1[8,3];
                cell_nine.output_consumption = state_last.outputState1[8,4];
                cell_nine.output_exp_PWM = state_last.outputState1[8,5];
              //  cell_nine.output_exp_CAN = state_last.outputState1[8,6];

                /* 第十路 详述*/
                cell_ten.output_vol = state_last.outputState1[9,0];
                cell_ten.output_cur = state_last.outputState1[9,1];
                cell_ten.output_pow = state_last.outputState1[9,2];
                cell_ten.output_spd = state_last.outputState1[9,3];
                cell_ten.output_consumption = state_last.outputState1[9,4];
                cell_ten.output_exp_PWM = state_last.outputState1[9,5];
               // cell_ten.output_exp_CAN = state_last.outputState1[9,6];
                check_data = 1;

            }
            finally
            {
                if (check_data == 0)
                {
                    Value_Assignment_last();
                }
            }
        }

        private void Value_Assignment_last()
        {

            state_last.timeStamp = cell.timeStamp;
            state_last.cellState[0] = cell.inside_all_vol_mV;
            state_last.cellState[1] = cell.inside_cur_mA;
            state_last.cellState[2] = cell.inside_ele_mW;
            state_last.cellState[3] = cell.inside_fir_vol_mV;
            state_last.cellState[4] = cell.inside_sec_vol_mV;
            state_last.cellState[5] = cell.inside_thi_vol_mV;
            state_last.cellState[6] = cell.inside_for_vol_mV;
            state_last.cellState[7] = cell.inside_fif_vol_mV;
            state_last.cellState[8] = cell.inside_six_vol_mV;

            /* 第一路 详述*/
            state_last.outputState1[0,0] = cell_one.output_vol;
            state_last.outputState1[0,1] = cell_one.output_cur;
            state_last.outputState1[0,2] = cell_one.output_pow;
            state_last.outputState1[0,3] = cell_one.output_spd;
            state_last.outputState1[0,4] = cell_one.output_consumption;
            state_last.outputState1[0,5] = cell_one.output_exp_PWM;

            /* 第二路 详述*/
            state_last.outputState1[1,0] = cell_two.output_vol;
            state_last.outputState1[1,1] = cell_two.output_cur;
            state_last.outputState1[1,2] = cell_two.output_pow;
            state_last.outputState1[1,3] = cell_two.output_spd;
            state_last.outputState1[1,4] = cell_two.output_consumption;
            state_last.outputState1[1,5] = cell_two.output_exp_PWM;

            /* 第三路 详述*/
            state_last.outputState1[2,0] = cell_three.output_vol;
            state_last.outputState1[2,1] = cell_three.output_cur;
            state_last.outputState1[2,2] = cell_three.output_pow;
            state_last.outputState1[2,3] = cell_three.output_spd;
            state_last.outputState1[2,4] = cell_three.output_consumption;
            state_last.outputState1[2,5] = cell_three.output_exp_PWM;

            /* 第四路 详述*/
            state_last.outputState1[3,0] = cell_four.output_vol;
            state_last.outputState1[3,1] = cell_four.output_cur;
            state_last.outputState1[3,2] = cell_four.output_pow;
            state_last.outputState1[3,3] = cell_four.output_spd;
            state_last.outputState1[3,4] = cell_four.output_consumption;
            state_last.outputState1[3,5] = cell_four.output_exp_PWM;

            /* 第五路 详述*/
            state_last.outputState1[4,0] = cell_five.output_vol;
            state_last.outputState1[4,1] = cell_five.output_cur;
            state_last.outputState1[4,2] = cell_five.output_pow;
            state_last.outputState1[4,3] = cell_five.output_spd;
            state_last.outputState1[4,4] = cell_five.output_consumption;
            state_last.outputState1[4,5] = cell_five.output_exp_PWM;

            /* 第六路 详述*/
            state_last.outputState1[5,0] = cell_six.output_vol;
            state_last.outputState1[5,1] = cell_six.output_cur;
            state_last.outputState1[5,2] = cell_six.output_pow;
            state_last.outputState1[5,3] = cell_six.output_spd;
            state_last.outputState1[5,4] = cell_six.output_consumption;
            state_last.outputState1[5,5] = cell_six.output_exp_PWM;

            /* 第七路 详述*/
            state_last.outputState1[6,0] = cell_seven.output_vol;
            state_last.outputState1[6,1] = cell_seven.output_cur;
            state_last.outputState1[6,2] = cell_seven.output_pow;
            state_last.outputState1[6,3] = cell_seven.output_spd;
            state_last.outputState1[6,4] = cell_seven.output_consumption;
            state_last.outputState1[6,5] = cell_seven.output_exp_PWM;

            /* 第八路 详述*/
            state_last.outputState1[7,0] = cell_eight.output_vol;
            state_last.outputState1[7,1] = cell_eight.output_cur;
            state_last.outputState1[7,2] = cell_eight.output_pow;
            state_last.outputState1[7,3] = cell_eight.output_spd;
            state_last.outputState1[7,4] = cell_eight.output_consumption;
            state_last.outputState1[7,5] = cell_eight.output_exp_PWM;

            /* 第九路 详述*/
            state_last.outputState1[8,0] = cell_nine.output_vol;
            state_last.outputState1[8,1] = cell_nine.output_cur;
            state_last.outputState1[8,2] = cell_nine.output_pow;
            state_last.outputState1[8,3] = cell_nine.output_spd;
            state_last.outputState1[8,4] = cell_nine.output_consumption;
            state_last.outputState1[8,5] = cell_nine.output_exp_PWM;

            /* 第十路 详述*/
            state_last.outputState1[9,0] = cell_ten.output_vol;
            state_last.outputState1[9,1] = cell_ten.output_cur;
            state_last.outputState1[9,2] = cell_ten.output_pow;
            state_last.outputState1[9,3] = cell_ten.output_spd;
            state_last.outputState1[9,4] = cell_ten.output_consumption;
            state_last.outputState1[9,5] = cell_ten.output_exp_PWM; 
        }
        class BatteryInFo
        {
            public string LEV { set; get; }
            public string VOL { set; get; }
            public string CUR { set; get; }
            public string POW { set; get; }
            public string SPD { set; get; }
            public string COMSUMPTION { set; get; }
            public string PWM { set; get; }
           // public string CAN { set; get; }

            public BatteryInFo(string lev, string vol, string cur, string pow, string spd, string comsumption, string pwm)
            {
                this.LEV = lev;
                this.VOL = (double.Parse(vol)/1000).ToString("0.0");
                this.CUR = (double.Parse(cur) / 1000).ToString("0.0"); ;
                this.POW =  double.Parse(pow).ToString("0.0"); 
                this.SPD = (double.Parse(spd) / 1000).ToString("0.0"); ;
                this.COMSUMPTION = comsumption;
                this.PWM = pwm;
               // this.CAN = can;

            }
        }
        class BatteryInsideInFo
        {
            public string ALL_Vol { set; get; }
            public string ALL_Cur { set; get; }
            public string ALL_Ele { set; get; }
            public string Fir_Vol { set; get; }
            public string Sec_Vol { set; get; }
            public string Thi_Vol { set; get; }
            public string For_Vol { set; get; }
            public string Fif_Vol { set; get; }
            public string Six_Vol { set; get; }

            public BatteryInsideInFo(string all_vol, string all_cur, string all_ele, string fir_vol, string sec_vol, string thi_vol, string for_vol, string fif_vol, string six_vol)
            {
                this.ALL_Vol = all_vol;
                this.ALL_Cur = all_cur;
                this.ALL_Ele = all_ele;
                this.Fir_Vol = fir_vol;
                this.Sec_Vol = sec_vol;
                this.Thi_Vol = thi_vol;
                this.For_Vol = for_vol;
                this.Fif_Vol = fif_vol;
                this.Six_Vol = six_vol;
            }
        }
    }
}

