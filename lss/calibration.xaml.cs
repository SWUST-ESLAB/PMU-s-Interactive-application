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
using System.Windows.Shapes;
using System.IO.Ports;
using Newtonsoft.Json;
using System.Threading;

namespace lss
{
    /// <summary>
    /// calibration.xaml 的交互逻辑
    /// </summary>
    public partial class calibration : Window
    {
        SerialPort Cal_serial = new SerialPort();
        public calibration(string port_selected, int bps_selected)
        {
            InitializeComponent();
            parameter_Init();
            Cal_serial.PortName = port_selected;
            Cal_serial.BaudRate = bps_selected;

        }
        public class data_cal_information
        {
            public int type { get; set; }
            public int cmd { get; set; }

            public int value { get; set; }
            public int response { get; set; }
        }
        data_cal_information data_Cal_Information_R = new data_cal_information();
        data_cal_information data_Cal_Response = new data_cal_information();


        private void parameter_Init()
        {
            data_Cal_Information_R.type = 4;
            data_Cal_Information_R.response = 0;


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (CH_Combobox.Text.ToString() == "")
            {
                MessageBox.Show("校验错误！\r\n请选择校验通道");
            }
            else
            {
                try
                {
                    string Mid_data_CH = CH_Combobox.Text.ToString();
                    Mid_data_CH = Mid_data_CH.Substring(2);

                    data_Cal_Information_R.cmd = int.Parse(Mid_data_CH);
                    data_Cal_Information_R.value = (int.Parse(OHM_Cal.Text)) * 1000;

                    string data_Cal_Information_R_Json = JsonConvert.SerializeObject(data_Cal_Information_R); 
                    Cal_serial.Open();
                    Cal_serial.Write(data_Cal_Information_R_Json);
                    Thread.Sleep(50);
                    byte[] receiveData = new byte[Cal_serial.BytesToRead];
                    if (receiveData != null)
                    {
                        Cal_serial.Read(receiveData, 0, receiveData.Length);
                    }
                    string Cal_response = Encoding.ASCII.GetString(receiveData, 0, receiveData.Length);
                    int firstIndex = Cal_response.LastIndexOf("{");
                    int my_type_bytes = Cal_response.LastIndexOf("type");
                    int lastIndex = Cal_response.IndexOf("}");
                    if (my_type_bytes != -1)
                    {
                        firstIndex = Cal_response.LastIndexOf("{", my_type_bytes);
                        lastIndex = Cal_response.IndexOf("}", my_type_bytes);

                        if (firstIndex >= 0 && lastIndex > 0)
                        {
                            Cal_response = Cal_response.Substring(firstIndex, lastIndex - firstIndex + 1);
                            Cal_response = Cal_response.Replace("\\", "");
                            Cal_response = Cal_response.Replace("\"{", "{");
                            Cal_response = Cal_response.Replace("}\"", "}");
                            try
                            {
                                data_Cal_Response = JsonConvert.DeserializeObject<data_cal_information>(Cal_response);
                                if (data_Cal_Response.response == 1)
                                {
                                    MessageBox.Show("校验成功");
                                    Cal_serial.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("校验因为内部原因失败，请再次尝试");
                                Cal_serial.Close();
                            }

                        }
                    }
                }
                catch
                {
                    MessageBox.Show("校验失败，请仔细检查所填数字是否有误");
                }
            }

        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Cal_serial.Close();
            this.Close();

        }
    }
}
