using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MM.Utils
{
    public static class TimeActionFactory {

		/// <summary>
		/// Creates a time action and registers it to TimeManager.
		/// </summary>
		/// <returns>The time action.</returns>
		/// <param name="duration">Duration of the action</param>
		/// <param name="group">Group of the action</param>
		/// <param name="update_callback">Update callback.</param>
		/// <param name="end_callback">End callback.</param>
		public static TimeAction CreateTimeAction(float duration, eTimeGroup group, Action<TimeAction, float> update_callback = null, Action<TimeAction> end_callback = null)
		{
			TimeAction new_action = new TimeAction (duration, update_callback, end_callback);
			TimeManager.instance.AddAction(new_action, group);
			return new_action;
		}
	}
}
