using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	public Hud m_hud;
	public Stage m_stage;
	public int RETRY = 2;
	public GameObject m_prefab_enemy;
	public GameObject m_prefab_treasure;

	public AudioChannels m_audio;
	public AudioClip m_bgmClip;

	public AudioClip m_seStageClear;
	public AudioClip m_seGameOver;

	public FollowCamera m_camera;
	private int m_retry_remain;
	private ArrayList m_objList = new ArrayList();
	private int m_stageNo = 0;

	// Use this for initialization
	void Start () {
		gameStart();
	}

	// Update is called once per frame
	void Update () {

	}

	public void gameStart(){
		m_retry_remain = RETRY;
		m_hud.drawStageClear(false);
		m_stageNo = 1;
		GameObject.Find("Player").SendMessage("onGameStart");
		GameObject.Find("Score").SendMessage("onGameStart");
		onGameStart();
	}

	private void onGameStart(){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i=0; i<enemies.Length; i++) {
			Destroy (enemies [i]);
		}

		TreasureGenerator treasureGenerator = FindObjectOfType(typeof(TreasureGenerator)) as TreasureGenerator;
		if(treasureGenerator != null){
			Destroy(treasureGenerator);
		}

		GameObject.Find("Stage").SendMessage("onStageStart");
	}

	public int getRemainHP(){
		return m_retry_remain;
	}

	public int getStageNo(){
		return m_stageNo;
	}

	public void AddObjectToList(GameObject o)
	{
		m_objList.Add(o);
	}

	public void OnEatAll()
	{
		GameObject.Find("Player").SendMessage("OnStageClear");
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemys.Length; i++)
			enemys[i].SendMessage("OnStageClear");
		GameObject.Find("Player").SendMessage("OnStageClear");
		m_hud.drawStageClear(true);
		StartCoroutine("StageClear");

	}
}








