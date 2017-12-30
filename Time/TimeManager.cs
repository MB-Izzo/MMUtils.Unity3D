using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// TODO
/// + speed factor for a given group
/// + remove a group if empty

namespace MM.Utils
{
    public enum eTimeGroup
    {
        common,
        map,
        map_ui,
    }

    /// <summary>
    /// Main access point to Time.
    /// Allows one to use several / different in game timers.
    /// Allows to pause/resume/stop an entire group of TimeActions.
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        /// <summary>
        /// TimeActions grouped by group type.
        /// </summary>
        private Dictionary<eTimeGroup, List<TimeAction>> _time_actions;
        private Dictionary<eTimeGroup, List<TimeAction>> _next_actions;
        /// <summary>
        /// Game time: time since the Time Manager is running.
        /// </summary>
        public float game_time { get; private set; }

        public static TimeManager instance { get; private set; }

        private void Awake()
        {
            instance = this;
            _time_actions = new Dictionary<eTimeGroup, List<TimeAction>>();
            _next_actions = new Dictionary<eTimeGroup, List<TimeAction>>();
        }

        /// <summary>
        /// Add a new TimeAction to a group.
        /// </summary>
        /// <param name="new_action">New TimeAction to add.</param>
        /// <param name="group">Group where the TimeAction will be added.</param>
        public void AddAction(TimeAction new_action, eTimeGroup group = eTimeGroup.common)
        {
            // If group doesn't exist, create it.
            if (_next_actions.ContainsKey(group) == false)
                _next_actions.Add(group, new List<TimeAction>());

            if (new_action != null)
            {
                _next_actions[group].Add(new_action);
            }
            else
            {
                Debug.LogError("Can't add null TimeAction to TimeManager.");
            }
        }

        /// <summary>
        /// Pauses an entire group of TimeAction.
        /// </summary>
        /// <param name="group">Group to pause.</param>
        public void PauseActionGroup(eTimeGroup group)
        {
            if (_time_actions.ContainsKey(group) == true)
            {
                List<TimeAction> actions = _time_actions[group];
                foreach (TimeAction ta in actions)
                {
                    ta.Pause();
                }
            }
        }

        /// <summary>
        /// Resumes an entire group of TimeAction.
        /// </summary>
        /// <param name="group">Group to resume.</param>
        public void ResumeActionGroup(eTimeGroup group)
        {
            if (_time_actions.ContainsKey(group) == true)
            {
                List<TimeAction> actions = _time_actions[group];
                foreach (TimeAction ta in actions)
                {
                    ta.Resume();
                }
            }
        }

        /// <summary>
        /// Stops an entire group of TimeAction.
        /// Will remove them from the dictionnary.
        /// </summary>
        /// <param name="group"></param>
        public void StopActionGroup(eTimeGroup group)
        {
            if (_time_actions.ContainsKey(group) == true)
            {
                List<TimeAction> actions = _time_actions[group];
                foreach (TimeAction ta in actions)
                {
                    ta.Stop();
                }
            }
        }

        private void AddNextActions()
        {
            if ( _next_actions.Count != 0 )
            {
                foreach ( KeyValuePair<eTimeGroup, List<TimeAction>> pair in _next_actions)
                {
                    List<TimeAction> actions = pair.Value;
                    foreach (TimeAction action in actions)
                    {
                        if ( _time_actions.ContainsKey(pair.Key) == false )
                        {
                            _time_actions.Add(pair.Key, actions);
                        }
                        else
                        {
                            _time_actions[pair.Key].AddRange(actions);
                        }
                    }
                }
            }
            _next_actions.Clear();
        }

        /// <summary>
        /// MonoBehaviour.Update()
        /// </summary>
        private void Update()
        {
            game_time += Time.deltaTime;

            AddNextActions();


            List<TimeAction> finished_actions;
            TimeAction current_timeaction;
            // Get TimeAction List for every TimeAction group
            foreach (KeyValuePair<eTimeGroup, List<TimeAction>> pair in _time_actions)
            {
                finished_actions = new List<TimeAction>();
                IEnumerator<TimeAction> enumerator = pair.Value.GetEnumerator();
                while (enumerator.MoveNext() == true)
                {
                    current_timeaction = enumerator.Current;
                    // Update every TimeAction
                    if (current_timeaction.Update(Time.deltaTime) == eTimeActionStatus.finished)
                    {
                        finished_actions.Add(current_timeaction);
                    }
                }

                // Remove finished actions
                foreach( TimeAction ta in finished_actions )
                {
                    pair.Value.Remove(ta);
                }
            }
        }
    }
}