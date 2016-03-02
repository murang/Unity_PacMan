using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {

	private const int PLAYER_HP_SIZE = 32;
	private GameControl m_gameControl;

	public Texture m_texGameOver;
	public Texture m_texStageClear;
	public Texture m_texGameStart;
	public Texture m_texPlayerHP;
	public GUIText m_demoHint;

	private bool m_isGameOver = false;
	private bool m_isStageClear = false;
	private bool m_isGameStart = false;

	// Use this for initialization
	void Start () {
		m_gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();


		GlobalParam gp = GlobalParam.getGPInstance();
		if(gp != null){
			if(gp.isDemoMode()){
				m_demoHint.enabled = true;
			}
			else{
				m_demoHint.enabled = false;
			}
		}
	}
	
	void onGameStart(){
		m_isGameOver = false;
		m_isStageClear = false;
	}

	public void drawGameStart(bool isShow){
		m_isGameStart = isShow;
	}

	public void drawGameOver(bool isShow){
		m_isGameOver = isShow;
	}

	public void drawStageClear(bool isShow){
		m_isStageClear = isShow;
	}

	void OnGUI(){
		float centerX = Screen.width/2.0f;
		float centerY = Screen.height/2.0f;
		int remainHP = m_gameControl.getRemainHP();

		for(int i=0; i<remainHP; i++){
			GUI.Label(new Rect(20 + i*PLAYER_HP_SIZE, 20, PLAYER_HP_SIZE, PLAYER_HP_SIZE), m_texPlayerHP);
		}

		if(m_isGameStart){
			GUI.DrawTexture(new Rect(centerX-m_texGameStart.width/2.0f, centerY-m_texGameStart.height/2.0f, m_texGameStart.width, m_texGameStart.height), m_texGameStart);
		}
		if(m_isGameOver){
			GUI.DrawTexture(new Rect(centerX-m_texGameOver.width/2.0f, centerY-m_texGameOver.height/2.0f, m_texGameOver.width, m_texGameOver.height), m_texGameOver);
		}
		if(m_isStageClear){
			GUI.DrawTexture(new Rect(centerX-m_texStageClear.width/2.0f, centerY-m_texStageClear.height/2.0f, m_texStageClear.width, m_texStageClear.height), m_texStageClear);
		}
	}
}
















