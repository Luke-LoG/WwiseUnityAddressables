#if AK_WWISE_ADDRESSABLES && UNITY_ADDRESSABLES

namespace AK.Wwise.Unity.WwiseAddressables
{
	public class AkWwiseAddressablesInitializationSettings : AkWwiseInitializationSettings
	{
		private static AkWwiseAddressablesInitializationSettings m_AddressablesInstance;
		public new static AkWwiseAddressablesInitializationSettings Instance
		{
			get
			{
				if (m_AddressablesInstance == null)
				{
					if (m_Instance != null && m_Instance is AkWwiseAddressablesInitializationSettings addressableSettings)
					{
						m_AddressablesInstance = addressableSettings;
					}
					else
					{
#if UNITY_EDITOR
						var name = typeof(AkWwiseInitializationSettings).Name;
						var className = typeof(AkWwiseAddressablesInitializationSettings).Name;
						m_AddressablesInstance = ReplaceOrCreateAsset(className, name);
#else
						m_AddressablesInstance =(AkWwiseAddressablesInitializationSettings) CreateInstance<AkWwiseAddressablesInitializationSettings>();
						UnityEngine.Debug.LogWarning("WwiseUnity: No platform specific settings were created. Default initialization settings will be used.");
#endif
						m_Instance = m_AddressablesInstance;
					}
				}
				return m_AddressablesInstance;
			}
		}

		public static void SetInstance(AkWwiseAddressablesInitializationSettings existingSettings)
		{
			m_Instance = existingSettings;
			m_AddressablesInstance = existingSettings;
		}

		protected override void LoadInitBank()
		{
			AkAddressableBankManager.Instance.LoadInitBank();
		}

		protected override void ClearBanks()
		{
			AkAddressableBankManager.Instance.UnloadAllBanks(clearBankDictionary: false);
			AkAddressableBankManager.Instance.UnloadInitBank();
		}

#if UNITY_EDITOR
		public static AkWwiseAddressablesInitializationSettings ReplaceOrCreateAsset(string className, string fileName)
		{
			var path = System.IO.Path.Combine(AkWwiseEditorSettings.WwiseScriptableObjectRelativePath, fileName + ".asset");
			var assetExists = string.IsNullOrEmpty(UnityEditor.AssetDatabase.AssetPathToGUID(path));
			if (assetExists)
			{
				var loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<AkWwiseAddressablesInitializationSettings>(path);
				if (loadedAsset != null)
				{
					return loadedAsset;
				}
				else //overwrite current InitializationSettings asset with new one adressables one
				{
					UnityEditor.AssetDatabase.DeleteAsset(path);
					var newAsset = CreateInstance<AkWwiseAddressablesInitializationSettings>();
					UnityEditor.AssetDatabase.CreateAsset(newAsset, path);
					return newAsset;
				}
			}

			var guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(AkWwiseAddressablesInitializationSettings).Name);
			foreach (var assetGuid in guids)
			{
				var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGuid);
				var foundAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<AkWwiseAddressablesInitializationSettings>(assetPath);
				if (foundAsset)
					return foundAsset;
			}

			var createdAsset = CreateInstance<AkWwiseAddressablesInitializationSettings>();
			AkUtilities.CreateFolder(AkWwiseEditorSettings.WwiseScriptableObjectRelativePath);
			UnityEditor.AssetDatabase.CreateAsset(createdAsset, path);
			return createdAsset;
		}
#endif
	}
}
#endif // AK_WWISE_ADDRESSABLES
