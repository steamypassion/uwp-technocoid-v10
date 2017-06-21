﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Windows.UI.Core;

namespace uwp_technocoid_v10
{
    /// <summary>
    /// Global Event Handler.
    /// This class manages the communication between the individual UIs
    /// and systems. Note that it also stores and provides information on
    /// the different windows and threads.
    /// </summary>
    class GlobalEventHandler
    {
        // This class is a singleton, allowing every UI class to access the same data.
        private static readonly GlobalEventHandler eventHandlerInstance = new GlobalEventHandler();
        public static GlobalEventHandler GetInstance()
        {
            return eventHandlerInstance;
        }

        // This will store the thread dispatchers for the controller and
        // fullscreen player windows. It can be used to control the pages running
        // within these threads.
        public CoreDispatcher controllerDispatcher;
        public CoreDispatcher playerDispatcher;

        // An event indicating that the current sequencer position has changed.
        // Classes can subscribe to this event and get notified.
        public event PropertyChangedEventHandler SequencerPositionChanged;
        private void NotifySequencerPositionChanged(int currentSequencerPosition)
        {
            if (SequencerPositionChanged != null)
            {
                SequencerPositionChanged(currentSequencerPosition, new PropertyChangedEventArgs("int currentSequencerPosition"));
            }
        }

        // The trigger used to indicate a position change which is then propagated
        // to all subscribed methods.
        public void TriggerSequencerPositionChanged(int currentSequencerPosiiton)
        {

            NotifySequencerPositionChanged(currentSequencerPosiiton);
        }

        // An event indicating that the sequencer has been started or stopped.
        // Classes can subscribe to this event and get notified.
        public event PropertyChangedEventHandler CurrentlyPlayingChanged;
        private void NotifyCurrentlyPlayingChanged(bool isCurrentlyPlaying)
        {
            if (CurrentlyPlayingChanged != null)
            {
                CurrentlyPlayingChanged(isCurrentlyPlaying, new PropertyChangedEventArgs("bool isCurrentlyPlaying"));
            }
        }

        // The trigger used to indicate a player change which is then propagated
        // to all subscribed methods.
        public void TriggerCurrentlyPlayingChanged(bool isCurrentlyPlaying)
        {

            NotifyCurrentlyPlayingChanged(isCurrentlyPlaying);
        }
    }
}