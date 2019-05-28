using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP_TEST
{
    

    public class CLogData
    {
        public string Log_msg { get; set; }
        public string Log_time { get; set; }
        public string Log_converted_msg { get; set; }


    }
    public class CHelper
    {
        private static Mutex mul = new Mutex();
        private ManualResetEvent _doneEvent;

        public CHelper(ManualResetEvent doneEvent)
        {
            _doneEvent = doneEvent;
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            _doneEvent.Set();
        }

        static public bool read_file_data(string read_file, ref List<string> data)
        {
            if (File.Exists(read_file))
            {
                // using문을 사용하면 Diposal를 자동 처리 즉 file close를 알아서 처리해줌
                using (StreamReader reader = new StreamReader(read_file))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                            return true;
                        data.Add(line);
                    }
                }
            }
            return false;

        }
        static public bool write_file_data(string write_file, string data)
        {
            bool result = true;

            using (var writer = new StreamWriter(write_file, true, Encoding.ASCII))
            {
                writer.NewLine = "\n";
                writer.WriteLine(data);
            }
            return result;
        }

        static public void analysis_log(List<string> log_data,bool all,List<string> concern_keys,ref Dictionary<string, List<CLogData>> param_dict)
        {
            foreach(string log in log_data)
            {
                string[] token = log.Split('#');
                string key_ = token[1];
                string time_ = token[0];
                string msg_ = token[2];
                if(all)
                {
                    CLogData data = new CLogData { Log_msg = msg_, Log_time = time_,Log_converted_msg=""};
                    if (param_dict.ContainsKey(key_))
                    {
                        param_dict[key_].Add(data);
                    }
                    else
                    {
                        param_dict[key_] = new List<CLogData>();
                        param_dict[key_].Add(data);
                    }
                }
                else
                {
                    if(concern_keys.Contains(key_))
                    {
                        CLogData data = new CLogData { Log_msg = msg_, Log_time = time_ };
                        if (param_dict.ContainsKey(key_))
                        {
                            param_dict[key_].Add(data);
                        }
                        else
                        {
                            param_dict[key_] = new List<CLogData>();
                            param_dict[key_].Add(data);
                        }
                    }
                }
            }
        }

        static public void make_report(bool all, List<string> param_keys,string file_name_, ref Dictionary<string, List<CLogData>> param_dict)
        {
            if (all)
            {
                List<string> keys_ = new List<string>(param_dict.Keys);
                foreach (string key in keys_)
                {
                    string data = key + "#";
                    data += param_dict[key].Count.ToString();
                    write_file_data(file_name_, data);
                    
                }
            }
            else
            {
                foreach(string key in param_keys)
                {
                    if (param_dict.ContainsKey(key))
                    {
                        string data = key + "#";
                        data += param_dict[key].Count.ToString();
                        write_file_data(file_name_, data);
                    }
                }
            }
        }

        static public string convert_msg(string param_s)
        {
            string result_s = "";
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"CODECONV.EXE";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            start.Arguments = param_s;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result_s = reader.ReadToEnd();
                    result_s = result_s.Replace("\r\n", string.Empty);
                    return result_s;
                }
            }
        }
        
        static public string convert_using_thread(string param_msg_)
        {
            object value = null;
            Thread thread = new Thread(() => { value = convert_msg(param_msg_); });
            thread.Start();
            //thread.Join();
            return (string)value;
        }
        static public void do_convert_all_msg(ref Dictionary<string, List<CLogData>> param_dict)
        {
            List<string> keys_ = new List<string>(param_dict.Keys);
            foreach (string key in keys_)
            {
                List<CLogData> each_log_data_ = param_dict[key];
                foreach (CLogData data in each_log_data_)
                {
                    data.Log_converted_msg = convert_msg(data.Log_msg);
                }
            }
        }
        static public void do_convert_all_msg_using_thread(ref Dictionary<string, List<CLogData>> param_dict)
        {
            List<string> keys_ = new List<string>(param_dict.Keys);
            foreach (string key in keys_)
            {
                List<CLogData> each_log_data_ = param_dict[key];
                foreach(CLogData data in each_log_data_)
                {
                    data.Log_converted_msg = convert_using_thread(data.Log_msg);
                }
            }
        }

        static public void write_log_data(int test_num, bool convert,bool all, List<string> param_keys, Dictionary<string, List<CLogData>> param_dict)
        {
            string prefix = "TYPELOG_";
            string postfix = ".TXT";
            if (all)
            {
                List<string> keys_ = new List<string>(param_dict.Keys);
                foreach (string key in keys_)
                {
                    string file_name = prefix + test_num + "_" + key + postfix;
                    List<CLogData> data_list = param_dict[key];
                    foreach(CLogData data in data_list)
                    {
                        string time_s = data.Log_time;
                        string msg_s = "";
                        if (convert)
                        {
                            msg_s = data.Log_converted_msg;
                        }
                        else
                        {
                            msg_s = data.Log_msg;
                        }
                        string log_s = time_s + "#" + key + "#" + msg_s;
                        write_file_data(file_name, log_s);
                    }

                }
            }
            else
            {
                foreach (string key in param_keys)
                {
                    if (param_dict.ContainsKey(key))
                    {
                        string file_name = prefix + test_num + "_" + key + postfix;
                        List<CLogData> data_list = param_dict[key];
                        foreach (CLogData data in data_list)
                        {
                            string time_s = data.Log_time;
                            string msg_s = "";
                            if (convert)
                            {
                                msg_s = data.Log_converted_msg;
                            }
                            else
                            {
                                msg_s = data.Log_msg;
                            }
                            string log_s = time_s + "#" + key + "#" + msg_s;
                            write_file_data(file_name, log_s);
                        }

                    }
                }
            }
        }

    }
    class CThread
    {
        static public Dictionary<string, List<CLogData>> _log_data = new Dictionary<string, List<CLogData>>();

        static public void preprocess_log_data()
        {
            List<string> input_log_data_ = new List<string>();
            List<string> keys = new List<string> { "EE", "SS", "WW" };
            
            CHelper.read_file_data("LOGFILE_A.TXT", ref input_log_data_);
            CHelper.analysis_log(input_log_data_, false, keys, ref _log_data);
            CHelper.make_report(false, keys, "REPORT_1.TXT", ref _log_data);
            CHelper.do_convert_all_msg_using_thread(ref _log_data);
            CHelper.write_log_data(1, true, true, keys, _log_data);
        }
        static public void process_msg_convert()
        {
            
        }
    }
}
