using System;
using System.Collections;
using System.Globalization;
using DarkRift;
using DarkRift.Client.Unity;
using NiftyClub.Controllers;
using NiftyClub.Networking.Domain;
using NiftyClubPlugins.Common.Enums;
using UnityEngine;

namespace NiftyClub.Networking
{
	public class PlayerTransformSync : MonoBehaviour
	{
		[Header ("Parameters")]
		[SerializeField] private float recordPeriod;
		[SerializeField] private float notRecordingPeriod;
		[SerializeField] private float comeBackLivePeriod;

		[Space]
		[SerializeField] private float deltaDistance;
		[SerializeField] private float deltaLookDistance;
		[SerializeField] private float deltaAngle;

		[Header ("Links")]
		[SerializeField] private UnityClient networkingClient;
		
		[Space]
		[SerializeField] private Transform targetTransform;

		[Header ("Debug")]
		[SerializeField] private bool isDebugOn;

		private const string playerPositionString = "playerPositionPrefString";
		private const string playerRotationString = "playerRotationPrefString";
		private const string timestampString = "timestampPrefString";
		
		private const string loggedInRoomNamePref = "loggedInRoomName";
		private const string recordedRoomNameString = "recordedRoomNameString";
		private string loggedInRoomName;

		private float createTime;
		private float lastRecordTime;
		private Vector3 lastRecordedPosition;
		private Vector3 lastRecordedLookPosition;
		private Quaternion lastRecordedRotation;
		
		#region Unity Methods

		void Start ()
		{
			networkingClient = FindObjectOfType<UnityClient> ();

			createTime = Time.time;
			
			if (CheckTransformInPrefs ())
			{
				if (isDebugOn)
					Debug.Log ("Can reset back to the position");

				// TODO: Reset only if Space is pressed
				StartCoroutine (ResetTransformFromPrefs ());
			}
		}
		
		void Update ()
		{
			if (networkingClient == null)
				return;
			
			if (IsMovementSignificant ())
			{
				SyncPlayerTransform ();
			}
			
			if (Time.time < lastRecordTime + recordPeriod || Time.time < createTime + notRecordingPeriod)
				return;
			
			lastRecordTime = Time.time;
			RecordTransformToPrefs ();
		}

		#endregion

		private void RecordTransformToPrefs ()
		{
			Vector3 position = targetTransform.position;
			Quaternion rotation = targetTransform.rotation;
			
			PlayerPrefs.SetFloat ($"{playerPositionString}X", position.x);
			PlayerPrefs.SetFloat ($"{playerPositionString}Y", position.y);
			PlayerPrefs.SetFloat ($"{playerPositionString}Z", position.z);
			
			PlayerPrefs.SetFloat ($"{playerRotationString}W", rotation.w);
			PlayerPrefs.SetFloat ($"{playerRotationString}X", rotation.x);
			PlayerPrefs.SetFloat ($"{playerRotationString}Y", rotation.y);
			PlayerPrefs.SetFloat ($"{playerRotationString}Z", rotation.z);
			
			PlayerPrefs.SetString (timestampString, DateTime.Now.ToString (CultureInfo.InvariantCulture));
			
			PlayerPrefs.SetString (recordedRoomNameString, loggedInRoomName);
		}

		private bool CheckTransformInPrefs ()
		{
			if (!PlayerPrefs.HasKey (timestampString) || !PlayerPrefs.HasKey (recordedRoomNameString))
				return false;
			
			string recordedRoomName = PlayerPrefs.GetString (recordedRoomNameString);
			loggedInRoomName = PlayerPrefs.GetString (loggedInRoomNamePref);

			if (recordedRoomName != loggedInRoomName)
				return false;

			string timestamp = PlayerPrefs.GetString (timestampString);
			DateTime timestampTime = DateTime.Parse (timestamp);
			TimeSpan timeSpanSinceLastRecord = DateTime.Now - timestampTime;


			return timeSpanSinceLastRecord.TotalSeconds < comeBackLivePeriod;
		}

		private IEnumerator ResetTransformFromPrefs ()
		{
			yield return null;
			
			CharacterController controller = GetComponent<CharacterController>();
			PlayerController pController = GetComponent<PlayerController>();
			controller.enabled = false;
			pController.enabled = false;

			Vector3 oldPosition = new Vector3 (
				PlayerPrefs.GetFloat ($"{playerPositionString}X"),
				PlayerPrefs.GetFloat ($"{playerPositionString}Y") + 0.1f,
				PlayerPrefs.GetFloat ($"{playerPositionString}Z"));
			targetTransform.position = oldPosition;

			Quaternion oldRotation = new Quaternion (
				PlayerPrefs.GetFloat ($"{playerRotationString}X"),
				PlayerPrefs.GetFloat ($"{playerRotationString}Y"),
				PlayerPrefs.GetFloat ($"{playerRotationString}Z"),
				PlayerPrefs.GetFloat ($"{playerRotationString}W"));
			targetTransform.rotation = oldRotation;
			
			controller.enabled = true;
			pController.enabled = true;
		}

		private bool IsMovementSignificant ()
		{
			bool isDeltaPositionSignificant =
				Vector3.Distance (targetTransform.position, lastRecordedPosition) > deltaDistance;

			bool isDeltaRotationSignificant =
				Quaternion.Angle (targetTransform.rotation, lastRecordedRotation) > deltaAngle;

			return isDeltaPositionSignificant || isDeltaRotationSignificant;
		}

		private void SyncPlayerTransform ()
		{
			lastRecordedPosition = targetTransform.position;
			lastRecordedRotation = targetTransform.rotation;
			
			DarkRiftWriter writer = DarkRiftWriter.Create ();

			PlayerTransform playerTransform = new PlayerTransform (
				lastRecordedPosition,
				lastRecordedRotation);
			writer.Write (playerTransform);
			
			Message message = Message.Create (Tags.MovePlayer, writer);
			networkingClient.SendMessage (message, SendMode.Unreliable);
			
			if (isDebugOn)
				Debug.Log ("Synced Player Transform");
		}

		public void LookPositionSync (Vector3 lookPosition, float lookWeight)
		{
			bool isDeltaLookPositionSignificant =
				Vector3.Distance(lookPosition, lastRecordedLookPosition) > deltaLookDistance;

			if (isDeltaLookPositionSignificant)
			{
				DarkRiftWriter writer = DarkRiftWriter.Create();

				writer.Write(lookPosition.x);
				writer.Write(lookPosition.y);
				writer.Write(lookPosition.z);
				writer.Write((double)lookWeight);

				Message message = Message.Create(Tags.LookPosition, writer);
				networkingClient.SendMessage(message, SendMode.Unreliable);

				lastRecordedLookPosition = lookPosition;

				if (isDebugOn)
					Debug.Log("Synced look position.");
			}
		}

		public void JumpSync ()
		{
			DarkRiftWriter writer = DarkRiftWriter.Create ();

			writer.Write (true);

			Message message = Message.Create (Tags.Jump, writer);
			networkingClient.SendMessage (message, SendMode.Unreliable);
		}
	}
}