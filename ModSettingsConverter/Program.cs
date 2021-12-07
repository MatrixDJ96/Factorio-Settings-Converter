using System;
using System.IO;
using System.Windows.Forms;

namespace ModSettingsConverter
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "dat files (*.dat)|*.dat|json files (*.json)|*.json";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var dir = Path.GetDirectoryName(openFileDialog.FileName);
                    var extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                    var filename = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    var data = Array.Empty<byte>();
                    var json = string.Empty;

                    try
                    {
                        switch (extension)
                        {
                            case ".json":
                                json = File.ReadAllText(openFileDialog.FileName);
                                data = JsonUtil.JsonStringToData(json);
                                File.WriteAllBytes(Path.Combine(dir, filename + ".dat"), data);
                                break;
                            case ".dat":
                                data = File.ReadAllBytes(openFileDialog.FileName);
                                json = JsonUtil.DataToJsonString(data);
                                File.WriteAllText(Path.Combine(dir, filename + ".json"), json);
                                break;
                            default:
                                throw new Exception("Unable to convert selected file...");
                        }

                        MessageBox.Show("Successfully converted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
            }
        }
    }
}
