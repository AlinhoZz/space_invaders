using NAudio.Wave;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceInvaders.Services
{
    public class SoundManager
    {
        private readonly string _soundsPath;
        public SoundManager()
        {
            _soundsPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds");
        }

        public void PlaySound(string fileName)
        {
            string fullPath = Path.Combine(_soundsPath, fileName);
            if (!File.Exists(fullPath)) return;

            Task.Run(() =>
            {
                try
                {
                    using var audioFile = new AudioFileReader(fullPath);
                    using var outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(50);
                    }
                }
                catch { /* ignora */ }
            });
        }

        private WaveOutEvent? _ufoLoopWaveOut;
        private LoopStream? _ufoLoopStream;

        public void StartUFOLoop(string fileName)
        {
            StopUFOLoop();
            string fullPath = Path.Combine(_soundsPath, fileName);
            if (!File.Exists(fullPath)) return;

            try
            {
                var audioFile = new AudioFileReader(fullPath);
                _ufoLoopStream = new LoopStream(audioFile);
                _ufoLoopWaveOut = new WaveOutEvent();
                _ufoLoopWaveOut.Init(_ufoLoopStream);
                _ufoLoopWaveOut.Play();
            }
            catch { /* ignora */ }
        }

        public void StopUFOLoop()
        {
            try
            {
                if (_ufoLoopWaveOut != null)
                {
                    _ufoLoopWaveOut.Stop();
                    _ufoLoopWaveOut.Dispose();
                    _ufoLoopWaveOut = null;
                }
                if (_ufoLoopStream != null)
                {
                    _ufoLoopStream.Dispose();
                    _ufoLoopStream = null;
                }
            }
            catch { /* ignora */ }
        }
    }
}
