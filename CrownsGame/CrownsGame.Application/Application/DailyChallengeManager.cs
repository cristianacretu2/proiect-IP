using System;
using System.Timers; 

namespace CrownsGame.Application
{
    public class DailyChallengeManager
    {
        private System.Timers.Timer _timer; 
        private int _secondsRemaining;
        public int GamesSolved { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<int>? OnTick;
        public event Action? OnTimeUp;

        public DailyChallengeManager(int durationSeconds = 120)
        {
            _secondsRemaining = durationSeconds;
            _timer = new System.Timers.Timer(1000); // 1 secundă
            _timer.Elapsed += (s, e) => Tick();
            _timer.AutoReset = true;
        }

        public void Start()
        {
            GamesSolved = 0;
            IsRunning = true;
            _timer.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _timer.Stop();
        }

        public void IncrementScore() => GamesSolved++;

        private void Tick()
        {
            _secondsRemaining--;
            OnTick?.Invoke(_secondsRemaining);

            if (_secondsRemaining <= 0)
            {
                Stop();
                OnTimeUp?.Invoke();
            }
        }
        
        public string GetFormattedTime()
        {
            int minutes = _secondsRemaining / 60;
            int seconds = _secondsRemaining % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}