using System;
using System.Collections.Generic;

using System.IO;

using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonConverter
{

	public static void FromCsv()
	{

	}

#if UNITY_EDITOR
	public static void FromData(object data, string path)
	{
		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(path, json);
		AssetDatabase.Refresh();
	}
#endif
	public static string ToCsv()
	{

		return "";
	}

	public static object ToData(string filePath)
	{
		string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
		string typeName = fileName;
		try
		{
			using (FileStream fs = File.OpenRead(filePath))
			{

				using (BinaryReader sr = new BinaryReader(fs))
				{
					string jsonstring = sr.ReadString();
					
					Dictionary<string, object> jb = (Dictionary<string, object>)JsonConvert.DeserializeObject(jsonstring);

					if (jb.ContainsKey("typeName"))
					{
						typeName = (string)jb["typeName"];
					}

					Type type = typeName.GetAssemblyType();
					if (type == null)
					{
						sr.Close();
						return null;
					}
					var json = JsonUtility.FromJson(jsonstring, type);

					return json;
				}
			}
		}
		catch
		{
			string jsonstring = File.ReadAllText(filePath);
			Dictionary<string, object> jb = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonstring);
			if (jb.ContainsKey("typeName"))
			{
				typeName = (string)jb["typeName"];
			}

			Type type = typeName.GetAssemblyType();
			if (type == null)
			{
				return null;
			}

			var json = JsonUtility.FromJson(jsonstring, type);
			return json;
		}

	}
}

