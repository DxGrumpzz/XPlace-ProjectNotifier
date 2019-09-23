﻿namespace ProjectNotifier.XPlace.Client
{
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System.IO;

	/// <summary>
	/// A config file manager specifically for Json type files
	/// </summary>
	public class JsonConfigManager
	{

		#region Private fields

		/// <summary>
		/// The location of the config file
		/// </summary>
		private string _configFileLocation;

		/// <summary>
		/// A string that contains the data inside the config file
		/// </summary>
		private string _configData;

		#endregion

		#region Public properties

		public JObject JsonData { get; private set; }

		#endregion


		public JsonConfigManager(string configLocation)
		{
			_configFileLocation = configLocation;

			UpdateConfigData(configLocation);
		}


		/// <summary>
		/// Reads a setting's value from a json config file
		/// </summary>
		/// <typeparam name="T"> The type of the setting </typeparam>
		/// <param name="settingName"> The name of the setting </param>
		/// <returns></returns>
		public T ReadSetting<T>(string settingName)
		{
			T setting = JsonData.Value<T>(settingName);

			return setting;
		}


		/// <summary>
		/// Modyfies a setting's value 
		/// </summary>
		/// <typeparam name="T"> The type of value </typeparam>
		/// <param name="settingName"> The name of the setting </param>
		/// <param name="settingValue"> The value of the setting </param>
		public void WriteSetting<T>(string settingName, T settingValue)
		{
			// Desrialize the json file
			dynamic jsonObj = JsonConvert.DeserializeObject(_configData);

			// Find the appropriate setting and set it's value
			jsonObj[settingName] = settingValue;

			// Serialize data
			string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

			// Wirte data to file
			File.WriteAllText(_configFileLocation, output);

			// Update data
			UpdateConfigData(_configFileLocation);
		}


		#region Private helpers

		/// <summary>
		/// Updates this calss' json file data
		/// </summary>
		private void UpdateConfigData(string configLocation)
		{
			_configData = File.ReadAllText(configLocation);

			JsonData = JObject.Parse(_configData);
		}

		#endregion

	};
};
