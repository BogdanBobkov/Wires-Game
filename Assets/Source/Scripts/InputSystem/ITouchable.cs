using UnityEngine;

namespace Inputs
{
	public interface ITouchable
	{
		void OnTouchDown(Vector3 downPosition);
		void OnTouchUp(Vector3 upPosition);
	}
}
