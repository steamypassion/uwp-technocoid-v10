﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Windows.System.Threading;
using Windows.UI.Core;

namespace uwp_technocoid_v10
{
    /// <summary>
    /// Global Sequencer Controller.
    /// This class provides the basic sequencer logic and is also responsible for
    /// managing the timer threads.
    /// </summary>
    class GlobalSequencerController
    {
        // This class is a singleton, allowing every UI class to access the same data.
        private static readonly GlobalSequencerController sequencerControllerInstance = new GlobalSequencerController();
        public static GlobalSequencerController GetInstance()
        {
            return sequencerControllerInstance;
        }

        // Use the global event handler to propagate certain states and events triggered
        // by the timers.
        GlobalEventHandler globalEventHandlerInstance;

        // The master timer, later running as a task in the thread pool
        static ThreadPoolTimer masterTimer;

        // The currently selected BPM count in different units
        private int currentBPM = 60;
        private int currentBPMinMS = 1000;
        private TimeSpan currentBPMasTimeSpan = TimeSpan.FromSeconds(1);

        // The current sequencer position.
        // Note that we propagate this as an event once the sequencer position
        // is changed.
        private int _currentSequencerPosition;
        public int currentSequencerPosition
        {
            get { return _currentSequencerPosition; }
            set
            {
                _currentSequencerPosition = value;
                globalEventHandlerInstance.TriggerSequencerPositionChanged(value);
            }
        }

        // Flag if the sequencer is currently running
        public bool isSequencerRunning = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GlobalSequencerController()
        {
            // Get an instance of the event handler.
            this.globalEventHandlerInstance = GlobalEventHandler.GetInstance();

            // Bind the StartSequencer() function to the CurrentlyPlayingChanged event.
            this.globalEventHandlerInstance.CurrentlyPlayingChanged += this.StartSequencer;
        }

        /// <summary>
        /// Start and stop the sequencer based on the parameter given.
        /// Note that this subscribes to the CurrentlyPlayingChanged in the event handler.
        /// </summary>
        /// <param name="isCurrentlyPlaying">This starts / stops the timer based on a bool.</param>
        /// <param name="e">PropertyChangedEventArgs.</param>
        private void StartSequencer(object isCurrentlyPlaying, PropertyChangedEventArgs e)
        {
            isSequencerRunning = (bool)isCurrentlyPlaying;

            if (isSequencerRunning)
            {
                masterTimer = ThreadPoolTimer.CreatePeriodicTimer(SequencerMainLoop, this.currentBPMasTimeSpan);
            }
            else
            {
                masterTimer.Cancel();
                this.currentSequencerPosition = 0;
            }
        }

        /// <summary>
        /// This is called by the timer on every trigger.
        /// It just increases the sequencer position, so other classes should
        /// subscribe to the SequencerPositionChanged event in the event handler.
        /// </summary>
        /// <param name="timer">Timer object.</param>
        private async void SequencerMainLoop(ThreadPoolTimer timer)
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(
             CoreDispatcherPriority.Normal, () =>
             {
                 if (this.currentSequencerPosition < 7)
                 {
                     this.currentSequencerPosition += 1;
                 }
                 else
                 {
                     this.currentSequencerPosition = 0;
                 }
             });
        }


        /// <summary>
        /// Update the BPM counter, setting a new timing for the master timer.
        /// If the timer is running, it will be updated directly.
        /// </summary>
        /// <param name="newBPM">New BPM as int.</param>
        public void UpdateBPM(int newBPM)
        {
            this.currentBPM = newBPM;
            this.currentBPMinMS = Convert.ToInt32(60000 / this.currentBPM);
            this.currentBPMasTimeSpan = new TimeSpan(0, 0, 0, 0, this.currentBPMinMS);

            if (this.isSequencerRunning)
            {
                masterTimer.Cancel();
                masterTimer = ThreadPoolTimer.CreatePeriodicTimer(SequencerMainLoop, this.currentBPMasTimeSpan);
            }
        }
    }
}
