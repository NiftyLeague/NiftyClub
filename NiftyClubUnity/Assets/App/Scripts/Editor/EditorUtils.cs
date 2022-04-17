using UnityEditor;

namespace NiftyClub.Editor
{
	public static class EditorUtils
	{
		private static readonly string repoBasePath = "D:\\Projects\\Toptal\\NiftyLeague\\NiftyClub";
		private static readonly string darkRiftConsoleFolder =
			$"{repoBasePath}\\NiftyClubServer\\DarkRift.Server.Console.exe";
		private static readonly string darkRiftPluginsFolder =
			$"{repoBasePath}\\NiftyClubPlugins\\NiftyClubPlugins.sln";

		[MenuItem ("Tools/Nifty League/Open Folder: DarkRift Console")]
		public static void OpenFolderDarkRiftConsole ()
		{
			EditorUtility.RevealInFinder (darkRiftConsoleFolder);
		}

		[MenuItem ("Tools/Nifty League/Open Folder: DarkRift Plugins")]
		public static void OpenFolderDarkRiftPlugins ()
		{
			EditorUtility.RevealInFinder (darkRiftPluginsFolder);
		}
	}
}