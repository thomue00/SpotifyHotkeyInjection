using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SpotifyHotkeys {
    static class Program {

        private enum KeyMapper {
            PausePlay = 0,
            Next = 1,
            Previous = 2
        }

        //-----------------------------------------------------------------------------

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //-----------------------------------------------------------------------------
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (ProcessIcon pi = new ProcessIcon()) {

                pi.Display();
                LowLevelKeyboardHook hook = new LowLevelKeyboardHook();
                hook.OnKeyUnpressed += OnKeyUnpressed;
                hook.OnKeyPressed += OnKeyPressed;

                Application.Run();

                hook.UnHookKeyboard();
            }
        }

        //-----------------------------------------------------------------------------

        private static void OnKeyPressed(object sender, Keys e) {
        }

        //-----------------------------------------------------------------------------

        private static void gkh_KeyUp(object sender, KeyEventArgs e) {
            e.Handled = true;
            if (e.KeyCode == Keys.MediaPlayPause) {
                // Play Pause
                SendHotKey(KeyMapper.PausePlay);
            }
            else if (e.KeyCode == Keys.MediaNextTrack) {
                // Next Track
                SendHotKey(KeyMapper.Next);
            }
            else if (e.KeyCode == Keys.MediaPreviousTrack) {
                // Next Track
                SendHotKey(KeyMapper.Previous);
            }
        }

        private static void OnKeyUnpressed(object sender, Keys e) {

            if (e == Keys.MediaPlayPause) {
                // Play Pause
                SendHotKey(KeyMapper.PausePlay);
            }
            else if (e == Keys.MediaNextTrack) {
                // Next Track
                SendHotKey(KeyMapper.Next);
            }
            else if (e == Keys.MediaPreviousTrack) {
                // Next Track
                SendHotKey(KeyMapper.Previous);
            }

        }

        //-----------------------------------------------------------------------------

        private static void SendHotKey(KeyMapper key) {

            // Get Process
            Process current = Process.GetCurrentProcess();
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0) {
                Console.WriteLine("Chrome is not running");
            }
            else {

                foreach (Process proc in procsChrome) {

                    // the chrome process must have a window 
                    if (proc.MainWindowHandle == IntPtr.Zero) {
                        continue;
                    }
                    // Set to Foreground
                    SetForegroundWindow(proc.MainWindowHandle);

                    // Send Hotkeys
                    string keycode = "%+";
                    switch (key) {
                        case KeyMapper.PausePlay:
                            keycode += "P";
                            break;
                        case KeyMapper.Next:
                            keycode += ";";
                            break;
                        case KeyMapper.Previous:
                            keycode += "{,}";
                            break;
                    }

                    SendKeys.SendWait(keycode);
                }
            }

            if (current.MainWindowHandle != IntPtr.Zero) {
                SetForegroundWindow(current.MainWindowHandle);
            }

        }
    }
}
