using System.IO;
using UnityEditor;
using UnityEngine;

namespace NiftyClub.Editor
{
	public static class EditorUtils
	{
		// The Unity project lives at <repoRoot>/NiftyClubUnity, so the repo
		// root is two levels above the Assets folder (Application.dataPath).
		private static readonly string repoBasePath =
			Directory.GetParent (Application.dataPath).Parent.FullName;
		private static readonly string darkRiftConsoleFolder =
			Path.Combine (repoBasePath, "NiftyClubServer", "DarkRift.Server.Console.exe");
		private static readonly string darkRiftPluginsFolder =
			Path.Combine (repoBasePath, "NiftyClubPlugins", "NiftyClubPlugins.sln");

		[MenuItem ("Tools/Nifty League/Open Folder: DarkRift Console")]
		public static void OpenFolderDarkRiftConsole ()
		{
			Reveal (darkRiftConsoleFolder);
		}

		[MenuItem ("Tools/Nifty League/Open Folder: DarkRift Plugins")]
		public static void OpenFolderDarkRiftPlugins ()
		{
			Reveal (darkRiftPluginsFolder);
		}

		private static void Reveal (string path)
		{
			if (string.IsNullOrEmpty (path) || !File.Exists (path) && !Directory.Exists (path))
			{
				Debug.LogWarning ($"Path does not exist, cannot reveal: {path}");
				return;
			}

			EditorUtility.RevealInFinder (path);
		}
	}
}