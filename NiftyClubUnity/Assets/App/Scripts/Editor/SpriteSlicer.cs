using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace NiftyClub.Editor
{
	public class SpriteSlicer : EditorWindow
	{
		private static int gridWidth = 128;

		[MenuItem ("Tools/Nifty League/Open Slicer Window")]
		static void OpenSlicer ()
		{
			GetWindow (typeof (SpriteSlicer));
		}

		void OnGUI ()
		{
			gridWidth = EditorGUILayout.IntField ("Grid: ", gridWidth);

			if (GUILayout.Button ("Slice"))
			{
				Slice ();
			}

			Repaint ();
		}
		
		static void Slice ()
		{        
			Texture2D[] textures = Selection.GetFiltered<Texture2D> (SelectionMode.Assets);

			int[] rowIndexLimits = new[]
			{
				0,
				4,
				4,
				4,
				4,
				4,
				4,
				4
			};
			
			foreach (Texture2D myTexture in textures)
			{
				string path = AssetDatabase.GetAssetPath(myTexture);
				TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
				ti.isReadable = true;
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
 
				if (ti.spriteImportMode != SpriteImportMode.Single)
					ti.spriteImportMode = SpriteImportMode.Single;
				ti.spriteImportMode = SpriteImportMode.Multiple;
 
				List<SpriteMetaData> newData = new List<SpriteMetaData>();
 
				int SliceWidth = gridWidth;
				int SliceHeight = gridWidth;

				int rowLimit = 7,
					columnLimit = 9;
				for (int rowIndex = rowLimit; rowIndex >= 0; rowIndex--)
				{
					for (int columnIndex = 0; columnIndex < columnLimit; columnIndex++)
					{
						int i = rowIndex * SliceHeight;
						int j = columnIndex * SliceWidth;
						
						SpriteMetaData smd = new SpriteMetaData();
						smd.pivot = new Vector2(0.5f, 0.5f);
						smd.alignment = 9;

						int x = (myTexture.height - j) / SliceHeight,
							y = i/SliceWidth;
						smd.name = $"{myTexture.name}_{x * 8 + y}";
						smd.rect = new Rect(i, j-SliceHeight, SliceWidth, SliceHeight);

						if (columnIndex > rowIndexLimits[rowIndex])
						{
							newData.Add(smd);
						}
					}
				}
 
				ti.spritesheet = newData.ToArray();
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			}
		}
	}
}