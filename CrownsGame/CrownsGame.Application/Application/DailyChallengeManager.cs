/*
 * Scopul fisierului: Gestioneaza logica pentru modul de joc contra-cronometru (Daily Challenge).
 * Autor: Radani Antonia
 */

using System;
using System.Timers; 

namespace CrownsGame.Application
{
    /// <summary>
    /// Coordoneaza sesiunile de joc cu timp limita, monitorizand progresul si declansand evenimente de final de timp.
    /// </summary>
    public class DailyChallengeManager
    {
        private System.Timers.Timer _timer; 
        private int _secondsRemaining;

        /// <summary> Numarul de jocuri rezolvate cu succes in sesiunea curenta. </summary>
        public int GamesSolved { get; private set; }

        /// <summary> Indica daca cronometrul este activ in acest moment. </summary>
        public bool IsRunning { get; private set; }


        /// <summary> Eveniment lansat la fiecare secunda pentru actualizarea interfetei. </summary>
        public event Action<int>? OnTick;

        /// <summary> Eveniment lansat cand timpul a expirat complet. </summary>
        public event Action? OnTimeUp;

        /// <summary>
        /// Configureaza managerul cu o durata specifica pentru provocare.
        /// </summary>
        /// <param name="durationSeconds">Timpul total alocat, exprimat in secunde.</param>
        public DailyChallengeManager(int durationSeconds = 120)
        {
            _secondsRemaining = durationSeconds;
            _timer = new System.Timers.Timer(1000); // 1 secunda
            _timer.Elapsed += (s, e) => Tick();
            _timer.AutoReset = true;
        }


        /// <summary>
        /// Reseteaza scorul si porneste numaratoarea inversa.
        /// </summary>
        public void Start()
        {
            GamesSolved = 0;
            IsRunning = true;
            _timer.Start();
        }

        /// <summary>
        /// Opreste temporar sau definitiv cronometrul.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            _timer.Stop();
        }

        /// <summary>
        /// Incrementeaza numarul de victorii. Apelata cand un puzzle este finalizat corect.
        /// </summary>
        public void IncrementScore() => GamesSolved++;

        /// <summary>
        /// Logica interna executata la fiecare ticait de ceas pentru verificarea limitei de timp.
        /// </summary>
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


        /// <summary>
        /// Transforma secundele ramase intr-un format lizibil pentru utilizator (MM:SS).
        /// </summary>
        /// <returns>Un sir de caractere formatat.</returns>
        public string GetFormattedTime()
        {
            int minutes = _secondsRemaining / 60;
            int seconds = _secondsRemaining % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}