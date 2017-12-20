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
        /// <summary>
        /// Game time: time since the Time Manager is running.
        /// </summary>
        public float game_time { get; private set; }

        public static TimeManager instance { get; private set; }

        private void Awake()
        {
            instance = this;
            _time_actions = new Dictionary<eTimeGroup, List<TimeAction>>();
        }

        /// <summary>
        /// Add a new TimeAction to a group.
        /// </summary>
        /// <param name="new_action">New TimeAction to add.</param>
        /// <param name="group">Group where the TimeAction will be added.</param>
        public void AddAction(TimeAction new_action, eTimeGroup group = eTimeGroup.common)
        {
            // If group doesn't exist, create it.
            if (_time_actions.ContainsKey(group) == false)
                _time_actions.Add(group, new List<TimeAction>());

            if (new_action != null)
            {
                _time_actions[group].Add(new_action);
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

        /// <summary>
        /// MonoBehaviour.Update()
        /// </summary>
        private void Update()
        {
            game_time += Time.deltaTime;

            List<TimeAction> finished_actions;
            // Get TimeAction List for every TimeAction group
            foreach (KeyValuePair<eTimeGroup, List<TimeAction>> pair in _time_actions)
            {
                finished_actions = new List<TimeAction>();
                foreach (TimeAction ta in pair.Value)
                {
                    // Update every TimeAction
                    if ( ta.Update(Time.deltaTime) == eTimeActionStatus.finished )
                    {
                        finished_actions.Add(ta);
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