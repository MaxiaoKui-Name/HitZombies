using System.Collections;
using UnityEngine;

namespace RootMotion.FinalIK
{
	public class AimController : MonoBehaviour
	{
		[Tooltip("Reference to the AimIK component.")]
		public AimIK ik;

		[Tooltip("Master weight of the IK solver.")]
		[Range(0f, 1f)]
		public float weight = 1f;

		[Header("Target Smoothing")]
		[Tooltip("The target to aim at. Do not use the Target transform that is assigned to AimIK. Set to null if you wish to stop aiming.")]
		public Transform target;

		[Tooltip("The time it takes to switch targets.")]
		public float targetSwitchSmoothTime = 0.3f;

		[Tooltip("The time it takes to blend in/out of AimIK weight.")]
		public float weightSmoothTime = 0.3f;

		[Header("Turning Towards The Target")]
		[Tooltip("Enables smooth turning towards the target according to the parameters under this header.")]
		public bool smoothTurnTowardsTarget = true;

		[Tooltip("Speed of turning towards the target using Vector3.RotateTowards.")]
		public float maxRadiansDelta = 3f;

		[Tooltip("Speed of moving towards the target using Vector3.RotateTowards.")]
		public float maxMagnitudeDelta = 3f;

		[Tooltip("Speed of slerping towards the target.")]
		public float slerpSpeed = 3f;

		[Tooltip("The position of the pivot that the aim target is rotated around relative to the root of the character.")]
		public Vector3 pivotOffsetFromRoot = Vector3.up;

		[Tooltip("Minimum distance of aiming from the first bone. Keeps the solver from failing if the target is too close.")]
		public float minDistance = 1f;

		[Tooltip("Offset applied to the target in world space. Convenient for scripting aiming inaccuracy.")]
		public Vector3 offset;

		[Header("RootRotation")]
		[Tooltip("Character root will be rotate around the Y axis to keep root forward within this angle from the aiming direction.")]
		[Range(0f, 180f)]
		public float maxRootAngle = 45f;

		[Tooltip("If enabled, aligns the root forward to target direction after 'Max Root Angle' has been exceeded.")]
		public bool turnToTarget;

		[Tooltip("The time of turning towards the target direction if 'Max Root Angle has been exceeded and 'Turn To Target' is enabled.")]
		public float turnToTargetTime = 0.2f;

		[Header("Mode")]
		[Tooltip("If true, AimIK will consider whatever the current direction of the weapon to be the forward aiming direction and work additively on top of that. This enables you to use recoil and reloading animations seamlessly with AimIK. Adjust the Vector3 value below if the weapon is not aiming perfectly forward in the aiming animation clip.")]
		public bool useAnimatedAimDirection;

		[Tooltip("The direction of the animated weapon aiming in character space. Tweak this value to adjust the aiming. 'Use Animated Aim Direction' must be enabled for this property to work.")]
		public Vector3 animatedAimDirection = Vector3.forward;

		private Transform lastTarget;

		private float switchWeight;

		private float switchWeightV;

		private float weightV;

		private Vector3 lastPosition;

		private Vector3 dir;

		private bool lastSmoothTowardsTarget;

		private bool turningToTarget;

		private float turnToTargetMlp = 1f;

		private float turnToTargetMlpV;

		private Vector3 pivot
		{
			get
			{
				return ik.transform.position + ik.transform.rotation * pivotOffsetFromRoot;
			}
		}

		private void Start()
		{
			lastPosition = ik.solver.IKPosition;
			dir = ik.solver.IKPosition - pivot;
			ik.solver.target = null;
		}

		private void LateUpdate()
		{
			if (target != lastTarget)
			{
				if (lastTarget == null && target != null)
				{
					lastPosition = target.position;
					dir = target.position - pivot;
					ik.solver.IKPosition = target.position + offset;
				}
				else
				{
					lastPosition = ik.solver.IKPosition;
					dir = ik.solver.IKPosition - pivot;
				}
				switchWeight = 0f;
				lastTarget = target;
			}
			ik.solver.IKPositionWeight = Mathf.SmoothDamp(ik.solver.IKPositionWeight, (!(target != null)) ? 0f : weight, ref weightV, weightSmoothTime);
			if (ik.solver.IKPositionWeight >= 0.999f)
			{
				ik.solver.IKPositionWeight = 1f;
			}
			if (ik.solver.IKPositionWeight <= 0.001f)
			{
				ik.solver.IKPositionWeight = 0f;
			}
			if (!(ik.solver.IKPositionWeight <= 0f))
			{
				switchWeight = Mathf.SmoothDamp(switchWeight, 1f, ref switchWeightV, targetSwitchSmoothTime);
				if (switchWeight >= 0.999f)
				{
					switchWeight = 1f;
				}
				if (target != null)
				{
					ik.solver.IKPosition = Vector3.Lerp(lastPosition, target.position + offset, switchWeight);
				}
				if (smoothTurnTowardsTarget != lastSmoothTowardsTarget)
				{
					dir = ik.solver.IKPosition - pivot;
					lastSmoothTowardsTarget = smoothTurnTowardsTarget;
				}
				if (smoothTurnTowardsTarget)
				{
					Vector3 b = ik.solver.IKPosition - pivot;
					dir = Vector3.Slerp(dir, b, Time.deltaTime * slerpSpeed);
					dir = Vector3.RotateTowards(dir, b, Time.deltaTime * maxRadiansDelta, maxMagnitudeDelta);
					ik.solver.IKPosition = pivot + dir;
				}
				ApplyMinDistance();
				RootRotation();
				if (useAnimatedAimDirection)
				{
					ik.solver.axis = ik.solver.transform.InverseTransformVector(ik.transform.rotation * animatedAimDirection);
				}
			}
		}

		private void ApplyMinDistance()
		{
			Vector3 vector = pivot;
			Vector3 vector2 = ik.solver.IKPosition - vector;
			vector2 = vector2.normalized * Mathf.Max(vector2.magnitude, minDistance);
			ik.solver.IKPosition = vector + vector2;
		}

		private void RootRotation()
		{
			float num = Mathf.Lerp(180f, maxRootAngle * turnToTargetMlp, ik.solver.IKPositionWeight);
			if (!(num < 180f))
			{
				return;
			}
			Vector3 vector = Quaternion.Inverse(ik.transform.rotation) * (ik.solver.IKPosition - pivot);
			float num2 = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			float angle = 0f;
			if (num2 > num)
			{
				angle = num2 - num;
				if (!turningToTarget && turnToTarget)
				{
					StartCoroutine(TurnToTarget());
				}
			}
			if (num2 < 0f - num)
			{
				angle = num2 + num;
				if (!turningToTarget && turnToTarget)
				{
					StartCoroutine(TurnToTarget());
				}
			}
			ik.transform.rotation = Quaternion.AngleAxis(angle, ik.transform.up) * ik.transform.rotation;
		}

		private IEnumerator TurnToTarget()
		{
			turningToTarget = true;
			while (turnToTargetMlp > 0f)
			{
				turnToTargetMlp = Mathf.SmoothDamp(turnToTargetMlp, 0f, ref turnToTargetMlpV, turnToTargetTime);
				if (turnToTargetMlp < 0.01f)
				{
					turnToTargetMlp = 0f;
				}
				yield return null;
			}
			turnToTargetMlp = 1f;
			turningToTarget = false;
		}
	}
}