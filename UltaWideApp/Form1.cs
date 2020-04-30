using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace UltaWideApp
{
    
    public partial class Form1 : Form
    {
        private class W
        {
            public String name;
            public int index = -1;
            public W(String _name)
            {
                name = _name;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hook, int nCode, IntPtr wp, IntPtr lp);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);

        private IntPtr ptrHook;

        private LowLevelKeyboardProc objKeyboardProcess;


        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        Process[] prArray;
        List<W> left;
        List<W> right;
        public Form1()
        {
            InitializeComponent();

            const string applicationName = "UltraWideApp";
            const string pathRegistryKeyStartup =
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            using (RegistryKey registryKeyStartup =
                        Registry.CurrentUser.OpenSubKey(pathRegistryKeyStartup, true))
            {
                registryKeyStartup.SetValue(
                    applicationName,
                    string.Format("\"{0}\"", System.Reflection.Assembly.GetExecutingAssembly().Location));
                registryKeyStartup.Flush();
            }

            notifyIcon1.Visible = false;
            this.notifyIcon1.MouseClick += new MouseEventHandler(notifyIcon1_MouseClick);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ProcessModule objCurrentModule =
                Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess,
                GetModuleHandle(objCurrentModule.ModuleName), 0);


            prArray = Process.GetProcesses();
            int i = 0;
            String pr = "brave";

            left = new List<W>();
            right = new List<W>();
            //left.Add(new W("brave"));
            //left.Add(new W("devenv"));
            //left.Add(new W("steam"));
            //right.Add(new W("Telegram"));
            ReadFromFile();

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();

            foreach (var a in left)
            {
                listBox2.Items.Add(a.name);
            }
            foreach (var a in right)
            {
                listBox3.Items.Add(a.name);
            }
            foreach (var a in prArray)
            {
                if (a.MainWindowHandle.ToString() != "0")
                {
                    listBox1.Items.Add(a.ToString().Substring(28).Remove(a.ToString().Length-1-28));
                    foreach (var b in left)
                    {
                        //Console.WriteLine(a.ToString());

                        if (a.ToString().IndexOf(b.name) >= 0)
                        {
                            b.index = i;
                        }
                    }
                    foreach (var b in right)
                    {
                       // Console.WriteLine(a.ToString());
                        if (a.ToString().IndexOf(b.name) >= 0)
                        {
                            b.index = i;
                        }
                    }
                }
                i++;

            }
            foreach (var b in left)
            {
                if (b.index != -1)
                {
                    IntPtr hWnd = prArray[b.index].MainWindowHandle;
                    Rect rct = new Rect();
                    GetWindowRect(hWnd, ref rct);
                    //Console.WriteLine("");
                    //Console.WriteLine(rct.Left.ToString() + " " + rct.Top.ToString() + " " + rct.Right.ToString() + " " + rct.Bottom.ToString());

                    SetWindowPos(hWnd, 0,
                        0,
                        0,
                        1920,
                        1040,
                        0x0040);
                    if (b.name == "brave")
                        SetWindowPos(hWnd, 0,
                        -7,
                        0,
                        1934,
                        1048,
                        0x0040);

                }
            }
            foreach (var b in right)
            {
                if (b.index != -1)
                {
                    IntPtr hWnd = prArray[b.index].MainWindowHandle;
                    Rect rct = new Rect();
                    GetWindowRect(hWnd, ref rct);
                    //Console.WriteLine("");
                    //Console.WriteLine(rct.Left.ToString() + " " + rct.Top.ToString() + " " + rct.Right.ToString() + " " + rct.Bottom.ToString());

                    SetWindowPos(hWnd, 0,
                    1920,
                    0,
                    640,
                    1040,
                    0x0040);

                }
            }

            this.WindowState = FormWindowState.Minimized;

            // прячем наше окно из панели
            this.ShowInTaskbar = false;
            // делаем нашу иконку в трее активной
            notifyIcon1.Visible = true;
        }
        void SetWindows()
        {
            foreach (var b in left)
            {
                if (b.index != -1)
                {
                    IntPtr hWnd = prArray[b.index].MainWindowHandle;
                    Rect rct = new Rect();
                    GetWindowRect(hWnd, ref rct);
                    //Console.WriteLine("");
                    //Console.WriteLine(rct.Left.ToString() + " " + rct.Top.ToString() + " " + rct.Right.ToString() + " " + rct.Bottom.ToString());
                    
                    SetWindowPos(hWnd, 0,
                        0,
                        0,
                        1920,
                        1040,
                        0x0040);
                    if (b.name == "brave")
                        SetWindowPos(hWnd, 0,
                        -7,
                        0,
                        1934,
                        1048,
                        0x0040);

                }
            }
            foreach (var b in right)
            {
                if (b.index != -1)
                {
                    IntPtr hWnd = prArray[b.index].MainWindowHandle;
                    Rect rct = new Rect();
                    GetWindowRect(hWnd, ref rct);

                    SetWindowPos(hWnd, 0,
                    1920,
                    0,
                    640,
                    1040,
                    0x0040);

                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox2.Items.IndexOf(listBox1.SelectedItem.ToString())==-1 && listBox3.Items.IndexOf(listBox1.SelectedItem.ToString()) == -1)
            {
                left.Add(new W(listBox1.SelectedItem.ToString()));
                listBox2.Items.Add(listBox1.SelectedItem.ToString());
                SaveToFile();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.IndexOf(listBox1.SelectedItem.ToString()) == -1 && listBox3.Items.IndexOf(listBox1.SelectedItem.ToString()) == -1)
            {
                right.Add(new W(listBox1.SelectedItem.ToString()));
                listBox3.Items.Add(listBox1.SelectedItem.ToString());
                SaveToFile();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox2.SelectedItem.ToString();
            for(int i=0;i<left.Count;i++)
            {
                if(left[i].name== listBox2.SelectedItem.ToString())
                {
                    left.RemoveAt(i);
                    break;
                }
            }
            listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            SaveToFile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox3.SelectedItem.ToString();
            for (int i = 0; i < right.Count; i++)
            {
                if (right[i].name == listBox3.SelectedItem.ToString())
                {
                    right.RemoveAt(i);
                    break;
                }
            }
            listBox3.Items.RemoveAt(listBox3.SelectedIndex);
            SaveToFile();
        }

        string pathLeft = @"left.txt";
        string pathRight = @"right.txt";
        void ReadFromFile()
        {
            using (StreamReader sr = new StreamReader(pathLeft, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    left.Add(new W(line));
                }
            }
            using (StreamReader sr = new StreamReader(pathRight, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    right.Add(new W(line));
                }
            }

        }
        void SaveToFile()
        {
            
            using (StreamWriter sw = new StreamWriter(pathLeft, false, System.Text.Encoding.Default))
            {
                foreach(var a in left)
                {
                    sw.WriteLine(a.name);
                }
             // sw.WriteLine(text);
            }
            using (StreamWriter sw = new StreamWriter(pathRight, false, System.Text.Encoding.Default))
            {
                foreach (var a in right)
                {
                    sw.WriteLine(a.name);
                }
                // sw.WriteLine(text);
            }



        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //if ($"{ e.KeyCode}" == "C" && $"{e.Modifiers}" == "Alt")
            //{
            //    label1.Text = "lol";
            //}
           // label1.Text = e.Modifiers.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }


        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo =
                    (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.Pause)
                {
                    //return (IntPtr)1;
                    //label1.Text = "lol";
                    SetWindows();
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            // проверяем наше окно, и если оно было свернуто, делаем событие        
            if (WindowState == FormWindowState.Minimized)
            {
                // прячем наше окно из панели
                this.ShowInTaskbar = false;
                // делаем нашу иконку в трее активной
                notifyIcon1.Visible = true;
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // делаем нашу иконку скрытой
            notifyIcon1.Visible = false;
            // возвращаем отображение окна в панели
            this.ShowInTaskbar = true;
            //разворачиваем окно
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            // делаем нашу иконку скрытой
            notifyIcon1.Visible = false;
            // возвращаем отображение окна в панели

            WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            //разворачиваем окно
        }

        private void button5_Click(object sender, EventArgs e)
        {
            prArray = Process.GetProcesses();
            int i = 0;
            listBox1.Items.Clear();
            foreach (var a in prArray)
            {
                if (a.MainWindowHandle.ToString() != "0")
                {
                    listBox1.Items.Add(a.ToString().Substring(28).Remove(a.ToString().Length - 1 - 28));
                    foreach (var b in left)
                    {
                        //Console.WriteLine(a.ToString());

                        if (a.ToString().IndexOf(b.name) >= 0)
                        {
                            b.index = i;
                        }
                    }
                    foreach (var b in right)
                    {
                        // Console.WriteLine(a.ToString());
                        if (a.ToString().IndexOf(b.name) >= 0)
                        {
                            b.index = i;
                        }
                    }
                }
                i++;

            }

        }
    }
}
