using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {

	public Texture m_titleTex;
	public float m_domoDelay = 10.0f;
	private bool m_isStartPush = false;

//	test
//	public TextAsset _txtass;
	
	// Use this for initialization
	void Start () {
//		if(_txtass != null){
//			string txtData = _txtass.text;
//			System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
//			string[] lines = txtData.Split(new char[]{'\r', '\n'}, option);
//			string[] chars = lines[2].Split(new char[]{','}, option);
//			for(int i=0; i<chars.Length; i++){
//				print(chars[i]);
//			}
//		}
	}
	
	// Update is called once per frame
	void Update () {
		this.m_domoDelay -= Time.deltaTime;
		if (this.m_domoDelay < .0f && !this.m_isStartPush) {
			GlobalParam.getGPInstance().setDemoMode(true);
			Application.LoadLevel("GameScene");
		}
	}

	void OnGUI(){
		float sw = Screen.width;
		float sh = Screen.height;
		float tw = m_titleTex.width;
		float th = m_titleTex.height;
		float r = th / tw;
		GUI.Label (new Rect(0, (sh-sw*r)/2.0f, sw, sw*r), m_titleTex);

		float btn_w = 200;
		float btn_h = 80;
		if(GUI.Button(new Rect((sw-btn_w)/2.0f, sh-(btn_h+50), btn_w, btn_h), "Start")){
			if(!m_isStartPush){
				m_isStartPush = true;
				GetComponent<AudioSource>().Play();
				GlobalParam.getGPInstance().setDemoMode(false);
				StartCoroutine(startGame());
			}
		}

		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Copyright (C) 2012 METAL BRAGE. All Rights Reserved.");
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	IEnumerator startGame(){
		yield return new WaitForSeconds(2);
		Application.LoadLevel("GameScene");
	}
}





























