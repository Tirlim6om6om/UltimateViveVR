// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.UI;

namespace Wave.Essence.Hand.Model.Demo
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Text))]
	public sealed class NaturalHandCurrentTrackerText : MonoBehaviour
	{
		private Text m_Text = null;
		void Start()
		{
			m_Text = gameObject.GetComponent<Text>();
		}

		float timeAcc = 0;

		void Update()
		{
			if (HandManager.Instance != null && m_Text != null)
			{
				timeAcc += Time.unscaledDeltaTime;
				if (timeAcc < 0.5f)
					return;
				timeAcc = 0;

				HandManager.TrackerType tracker = HandManager.TrackerType.Natural;
				if (HandManager.Instance.GetPreferTracker(ref tracker))
				{
					float scaleL = 1, scaleR = 1;
					Vector3 leftScale = Vector3.one, rightScale = Vector3.one;
					bool ret = false;
					ret |= HandManager.Instance.GetHandScale(ref leftScale, true);
					ret |= HandManager.Instance.GetHandScale(ref rightScale, false);
					if (ret)
					{
						scaleL = (leftScale.x + leftScale.y + leftScale.z) / 3;
						scaleR = (rightScale.x + rightScale.y + rightScale.z) / 3;
					}
					HandManager.Instance.GetHandTrackingTimestamp(tracker, out long timestamp);
					m_Text.text = "Tracker: " + tracker + " " + timestamp
						+ "\nHand Scale: L=" + scaleL + ", R=" + scaleR;
				}
			}
		}
	}
}
