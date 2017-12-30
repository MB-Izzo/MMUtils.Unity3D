using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MM.Utils
{
    public enum eTimeActionStatus
    {
        running,
        paused,
        interrupted,
        finished
    }

    /// <summary>
    /// Define the state of an action to be done in time.
    /// Given a start time and a duration this object is updated (if running) and call
    /// Actions when it's updated or finished.
    /// </summary>
    public class TimeAction
    {
        /// <summary>
        /// Action to be called when TimeAction is updated.
        /// Given parameters: updated TimeAction and status ratio value.
        /// </summary>
        private Action<TimeAction, float> _update_callback;
        /// <summary>
        /// Action called when TimeAction is over.
        /// Given parameters: ended TimeAction.
        /// </summary>
        private Action<TimeAction> _end_callback;
        private float _time_elapsed;

        public float start_time { get; private set; }
        public float duration { get; private set; }
        public float ratio { get { return _time_elapsed / duration; } }
        public eTimeActionStatus status { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="duration">Duration of the action.</param>
        /// <param name="update_callback">Action to be called when the object is updated.</param>
        /// <param name="end_callback">Action to be called when the action is finished.</param>
        public TimeAction(float duration, Action<TimeAction, float> update_callback = null, Action<TimeAction> end_callback = null)
        {
            this.duration = duration;
            start_time = TimeManager.instance.game_time;
            status = eTimeActionStatus.running;
            _time_elapsed = 0.0f;
            _update_callback = update_callback;
            _end_callback = end_callback;
        }

        /// <summary>
        /// Called by Time Manager to update the object if needed.
        /// Only "running" TimeAction will be updated.
        /// </summary>
        /// <param name="delta">Delta time to update the TimeAction with.</param>
        /// <returns>Current status of the TimeAction object.</returns>
        public eTimeActionStatus Update(float delta)
        {
            // if the TimeAction is running...
            if (status == eTimeActionStatus.running)
            {
                _time_elapsed += delta;
                // clamp _elapsed_time to duration if needed.
                if (_time_elapsed > duration)
                    _time_elapsed = duration;

                // if the action is not finished...
                if (ratio != 1.0f)
                {
                    // call the update_callback action if needed.
                    if (_update_callback != null)
                        _update_callback(this, ratio);
                }
                else
                {
                    Stop();
                }
            }
            return status;
        }

        public void Reset()
        {
            _time_elapsed = 0.0f;
            status = eTimeActionStatus.running;
        }

        /// <summary>
        /// Pause the TimeAction.
        /// </summary>
        public void Pause()
        {
            if (status != eTimeActionStatus.finished)
                status = eTimeActionStatus.paused;
        }

        /// <summary>
        /// Resume the TimeAction.
        /// </summary>
        public void Resume()
        {
            if (status != eTimeActionStatus.finished)
                status = eTimeActionStatus.running;
        }

        /// <summary>
        /// Stop the TimeAction.
        /// If the ratio value is not 1.0f (action has not finished by itself), status of the TimeAction will be set 
        /// to `interrupted`. Otherwise, status of the TimeAction will be set to `finished`.
        /// </summary>
        public void Stop()
        {
            status = ratio == 1.0f ? eTimeActionStatus.finished : eTimeActionStatus.interrupted;
            if (_end_callback != null)
                _end_callback(this);
        }
    }
}
