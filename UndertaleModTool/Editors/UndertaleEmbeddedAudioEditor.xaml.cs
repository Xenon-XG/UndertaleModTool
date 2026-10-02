using Microsoft.Win32;
using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using UndertaleModLib.Models;

namespace UndertaleModTool
{
    /// <summary>
    /// Logika interakcji dla klasy UndertaleEmbeddedAudioEditor.xaml
    /// </summary>
    public partial class UndertaleEmbeddedAudioEditor : DataUserControl, INotifyPropertyChanged
    {
        private WaveOutEvent waveOut;
        private WaveFileReader wavReader;
        private VorbisWaveReader oggReader;

        private static readonly MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        public string FileType
        {
            get
            {
                if (DataContext is not UndertaleEmbeddedAudio target)
                {
                    return "未知";
                }

                if (IsWav(target.Data))
                {
                    return "WAV";
                }
                if (IsOgg(target.Data))
                {
                    return "OGG";
                }
                return "未知";
            }
        }

        public UndertaleEmbeddedAudioEditor()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileType)));
            };
            this.Unloaded += Unload;
        }

        public void Unload(object sender, RoutedEventArgs e)
        {
            waveOut?.Stop();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            UndertaleEmbeddedAudio target = DataContext as UndertaleEmbeddedAudio;

            OpenFileDialog dlg = new();

            if (IsWav(target.Data))
            {
                dlg.DefaultExt = ".wav";
                dlg.Filter = "WAV 文件|*.wav|所有文件|*";
            }
            else if (IsOgg(target.Data))
            {
                dlg.DefaultExt = ".ogg";
                dlg.Filter = "OGG 文件|*.ogg|所有文件|*";
            }
            else
            {
                dlg.DefaultExt = "";
                dlg.Filter = "所有文件|*";
            }

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    byte[] data = File.ReadAllBytes(dlg.FileName);
                    if (!IsWav(data) && !IsOgg(data)) 
                    {
                        if (mainWindow.ShowQuestionWithCancel("警告：要导入的文件不是 WAV 或 OGG 格式。仍要导入吗？\r\n\r\n这可能会损坏该声音。", MessageBoxImage.Warning, "未知格式") != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }
                    else if ((IsWav(target.Data) && IsOgg(data)) || (IsOgg(target.Data) && IsWav(data)))
                    {
                        if (mainWindow.ShowQuestionWithCancel(
                            "警告：要导入的文件类型与现有文件类型不匹配。仍要导入吗？\r\n\r\n" +
                            "除非同时调整声音资源的压缩设置，否则这可能会损坏该声音。", MessageBoxImage.Warning, "格式不匹配") != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    } 
                    target.Data = data;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FileType)));
                }
                catch (Exception ex)
                {
                    mainWindow.ShowError("导入文件失败：" + ex.Message, "导入文件失败");
                }
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            UndertaleEmbeddedAudio target = DataContext as UndertaleEmbeddedAudio;

            SaveFileDialog dlg = new SaveFileDialog();

            if (IsWav(target.Data))
            {
                dlg.DefaultExt = ".wav";
                dlg.Filter = "WAV 文件|*.wav|所有文件|*";
            }
            else if (IsOgg(target.Data))
            {
                dlg.DefaultExt = ".ogg";
                dlg.Filter = "OGG 文件|*.ogg|所有文件|*";
            }
            else
            {
                dlg.DefaultExt = "";
                dlg.Filter = "所有文件|*";
            }

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllBytes(dlg.FileName, target.Data);
                }
                catch (Exception ex)
                {
                    mainWindow.ShowError("导出文件失败：" + ex.Message, "导出文件失败");
                }
            }
        }

        private void InitAudio()
        {
            if (waveOut == null)
            {
                waveOut = new WaveOutEvent() { DeviceNumber = 0 };
            }
            else if (waveOut.PlaybackState != PlaybackState.Stopped)
            {
                waveOut.Stop();
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            UndertaleEmbeddedAudio target = DataContext as UndertaleEmbeddedAudio;

            try
            {
                if (IsWav(target.Data))
                {
                    wavReader = new WaveFileReader(new MemoryStream(target.Data));
                    InitAudio();
                    waveOut.Init(wavReader);
                    waveOut.Play();
                }
                else if (IsOgg(target.Data))
                {
                    oggReader = new VorbisWaveReader(new MemoryStream(target.Data));
                    InitAudio();
                    waveOut.Init(oggReader);
                    waveOut.Play();
                }
                else
                {
                    mainWindow.ShowError("播放音频失败！\r\n不是 WAV 或 OGG 格式。", "音频错误");
                }
            }
            catch (Exception ex)
            {
                waveOut = null;
                mainWindow.ShowError("播放音频失败！\r\n" + ex.Message, "音频错误");
            }
        }


        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Stop();
        }
        
        private static bool IsWav(byte[] data) 
        {
            return data.Length >= 4 && data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F';
        }

        private static bool IsOgg(byte[] data) 
        {
            return data.Length >= 4 && data[0] == 'O' && data[1] == 'g' && data[2] == 'g' && data[3] == 'S';
        }
    }
}