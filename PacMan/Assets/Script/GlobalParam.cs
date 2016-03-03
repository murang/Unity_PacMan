using UnityEngine;
using System.Collections;

public class GlobalParam : MonoBehaviour {

	private bool m_isDemoMode = true;
	private static GlobalParam m_gpInstance = null;
	public static GlobalParam getGPInstance(){
		if(m_gpInstance == null){
			GameObject obj = new GameObject("GlobalParam");
			m_gpInstance = obj.AddComponent<GlobalParam>();
			DontDestroyOnLoad(obj);
		}
		return m_gpInstance;
	}
	
	public bool isDemoMode(){
		return m_isDemoMode;
	}

	public void setDemoMode(bool isDemoMode){
		m_isDemoMode = isDemoMode;
	}
}
