using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP_TEST
{
    class CTask
    {
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

        static public void line_zip(string zip_file, List<string> data)
        {
            if (File.Exists(zip_file)) File.Delete(zip_file);
            int current = 0;
            string pibot = "";
            int total = data.Count;
            do
            {
                pibot = data[current];
                int num = 1;

                while ((++current < total) && (pibot.Equals(data[current]))) num++;

                string zip_data = (num > 1) ? (num.ToString() + "#" + pibot) : pibot;
                write_file_data(zip_file, zip_data);
            } while (total > current);
        }

        static public string char_zip(string data)
        {
            string results = "";
            int len = data.Length;
            int current = 0;
            char pibot = ' ';
            do
            {
                pibot = data[current];
                int num = 1;
                while ((++current < len) && (pibot == data[current])) num++;
                results += (num > 2) ? (num.ToString() + pibot.ToString()) : string.Concat(Enumerable.Repeat(pibot.ToString(), num));
            } while (len > current);
            return results;
        }
        static public void char_zip(string path, List<string> data)
        {
            if (File.Exists(path)) File.Delete(path);
            foreach (string line in data)
            {
                string zip = char_zip(line);
                write_file_data(path, zip);
            }
        }

        static public string caesar(string data)
        {
            string results = "";
            string enc_table = "VWXYZABCDEFGHIJKLMNOPQRSTU";
            string org_table = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (char a in data)
            {
                results += org_table.Contains(a) ? enc_table[a - 'A'] : a;
            }
            return results;
        }
        static public void caesar(string path, List<string> data)
        {
            if (File.Exists(path)) File.Delete(path);
            foreach (string line in data)
            {
                string enc_str = caesar(line);
                write_file_data(path, enc_str);
            }
        }
        static public string key_enc(string data, string key)
        {
            string results = "";
            string org_tbl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string tmp_tbl = new string(org_tbl.Except(key).ToArray());
            string enc_tbl = key + tmp_tbl;
            foreach (char a in data)
            {
                results += org_tbl.Contains(a) ? enc_tbl[a - 'A'] : a;
            }
            return results;
        }

        static public void key_enc(string path, string key, List<string> data)
        {
            if (File.Exists(path)) File.Delete(path);
            foreach (string line in data)
            {
                string enc_str = key_enc(line, key);
                write_file_data(path, enc_str);
            }
        }
    }

    public class ServerTask
    {
        static Socket listener;

        public int FindMatchedFilesFromSubDirectory(string path, string file_name, out string[] result)
        {
            // find all files which is matched given file_name
            result = Directory.GetFiles(path, file_name, SearchOption.AllDirectories);
            return result.Length;
        }

        public bool enc_data(string file_name, ref List<string> results, string key_val, int count = 0)
        {
            string folder = "./BIGFILE/";

            string[] result;
            if (FindMatchedFilesFromSubDirectory(folder, file_name, out result) > 0)
            {
                string path = result[0];
                string input_file_path = folder + file_name;
                string folder_path = Path.GetDirectoryName(path);
                string zip_file = folder_path + "\\CMP_" + file_name;
                List<string> read = new List<string>();
                if (CTask.read_file_data(path, ref read))
                {
                    if (count > 0)
                    {
                        read.RemoveRange(0, count);
                    }
                    CTask.line_zip(zip_file, read);
                }

                List<string> zip_data = new List<string>();
                if (CTask.read_file_data(zip_file, ref zip_data))
                {
                    CTask.char_zip(zip_file, zip_data);
                }

                List<string> key_data = new List<string>();
                if (CTask.read_file_data(zip_file, ref key_data))
                {
                    CTask.key_enc(zip_file, key_val, key_data);
                }
                CTask.read_file_data(zip_file, ref results);
                return true;
            }
            return false;
        }
        // This method will be called when the thread is started. 
        public void server_start()
        {
            const int DEFAULT_BUFLEN = 4096;
            const int DEFAULT_PORT = 9876;

            // Data buffer for incoming data.
            byte[] recvbuf = new byte[DEFAULT_BUFLEN];
            int recvLen;

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, DEFAULT_PORT);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    if ((recvLen = handler.Receive(recvbuf)) > 0)
                    {
                        int index = 0;
                        var input_str = System.Text.Encoding.Default.GetString(recvbuf);
                        input_str = input_str.Trim('\0');
                        string[] tokens = input_str.Split('#');
                        string str = tokens[0];
                        string keyVal = tokens[1];
                        List<string> out_data = new List<string>();
                        if (enc_data(str, ref out_data, keyVal))
                        {
                            byte[] byData = System.Text.Encoding.ASCII.GetBytes(out_data[index]);
                            handler.Send(byData);
                        }
                        Array.Clear(recvbuf, 0, recvbuf.Length);
                        while ((recvLen = handler.Receive(recvbuf)) > 0)
                        {
                            var result_str = System.Text.Encoding.Default.GetString(recvbuf);
                            result_str = result_str.Trim('\0');
                            if ("ACK".Equals(result_str))
                            {
                                index++;
                                if (index >= out_data.Count) break;
                                byte[] byData = System.Text.Encoding.ASCII.GetBytes(out_data[index]);
                                handler.Send(byData);
                            }
                            else if ("ERR".Equals(result_str))
                            {
                                byte[] byData = System.Text.Encoding.ASCII.GetBytes(out_data[index]);
                                handler.Send(byData);
                            }
                            else
                            {
                                int num = Convert.ToInt32(result_str) - 1;
                                out_data.Clear();
                                if (enc_data(str, ref out_data, keyVal, num))
                                {
                                    index = 0;
                                    byte[] byData = System.Text.Encoding.ASCII.GetBytes(out_data[index]);
                                    handler.Send(byData);
                                }
                            }
                            Array.Clear(recvbuf, 0, recvbuf.Length);
                        }
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        break; // thread exit
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();
        }
    }
    class CNetworkTest
    {
        public static void runTask(string[] args)
        {
            ServerTask workerObject = new ServerTask();
            Thread workerThread = new Thread(workerObject.server_start);
            workerThread.Start();
            workerThread.Join();
        }
    }
    
}
