using UnityEngine;
using System.Collections;

public class GridMove : MonoBehaviour {

	private enum PAUSE_TYPE
	{
		NONE,
		GAME_PAUSE,
		HITSTOP
	};
	private PAUSE_TYPE m_pause = PAUSE_TYPE.NONE;

	public float speed = 1.0f;

	private Vector3 m_dir;
	private Vector3 m_move_vec;
	private Vector3 m_current_grid;

	private const float CHECKHIT_H = .5f;
	private const int CHECKHIT_LAYER_MASH =1;

	// Use this for initialization
	void Start () {
		m_move_vec = Vector3.zero;
		m_dir = Vector3.forward;
		m_pause = PAUSE_TYPE.NONE;
	}

	public void onRestart(){
		m_move_vec = Vector3.zero;
		m_pause = PAUSE_TYPE.NONE;
	}

	public void onGameStart(){
		m_move_vec = Vector3.zero;
		m_pause = PAUSE_TYPE.NONE;
	}

	public void onStageStart(){
		m_move_vec = Vector3.zero;
		m_pause = PAUSE_TYPE.NONE;
	}

	public void onDead()
	{
		m_pause = PAUSE_TYPE.GAME_PAUSE;
	}

	public void onStageClear()
	{
		m_pause = PAUSE_TYPE.GAME_PAUSE;
	}

	public void onRebone()
	{
		m_pause = PAUSE_TYPE.NONE;
	}

	// Update is called once per frame
	void Update () {
		if (m_pause != PAUSE_TYPE.NONE) {
			m_move_vec = Vector3.zero;
			return;
		}

		if (Time.deltaTime <= .1f) {
			pMove (Time.deltaTime);
		} else {
			int n = (int)(Time.deltaTime / 0.1f) + 1;
			for (int i = 0; i < n; i++) {
				pMove (Time.deltaTime / (float)n);
			}
		}
	}

	public void pMove(float dt){
		Vector3 pos = transform.position;
		pos += m_dir * speed * dt;
		bool across = false;
		if ((int)pos.x != (int)transform.position.x) {
			across = true;
		}
		if ((int)pos.z != (int)transform.position.z) {
			across = true;
		}
		Vector3 near_grid = new Vector3 (Mathf.Round (pos.x), pos.y, Mathf.Round (pos.z));
		m_current_grid = near_grid;
		Vector3 forward_pos = pos + m_dir * 0.5f;
		if (Mathf.RoundToInt(forward_pos.x) != Mathf.RoundToInt(pos.x) ||
			Mathf.RoundToInt(forward_pos.z) != Mathf.RoundToInt(pos.z)) {
			Vector3 tpos =pos;
			tpos.y += CHECKHIT_H;
			bool collided = Physics.Raycast (tpos,m_dir,1.0f,CHECKHIT_LAYER_MASH);
			if (collided) {
				pos = near_grid;
				across = true;
			}
		}
		if (across || (pos-near_grid).magnitude < 0.00005f) {
			Vector3 direction_save = m_dir;
			SendMessage("OnGrid",pos);

			if (Vector3.Dot(direction_save,m_dir )< 0.00005f)
				pos = near_grid + m_dir * 0.001f;
		}

		m_move_vec = (pos - transform.position) / dt;
		transform.position = pos;
	}

	public void setDir(Vector3 dir){
		m_dir = dir;
	}

	public Vector3 getDir(){
		return m_dir;
	}

	public bool isReversDir(Vector3 v){
		if (Vector3.Dot (v, m_dir) < -0.99999f)
			return true;
		else
			return false;
	}

	public bool checkWall(Vector3 direction)
	{
		Vector3 tpos =m_current_grid;
		tpos.y += CHECKHIT_H;
		return Physics.Raycast(tpos,direction,1.0f,CHECKHIT_LAYER_MASH);
	}

	public bool isRunning()
	{
		if (m_move_vec.magnitude > 0.01f)
			return true;
		return false;
	}

	public void HitStop(bool enable)
	{
		if (enable)
			m_pause |= PAUSE_TYPE.HITSTOP;
		else
			m_pause &= ~PAUSE_TYPE.HITSTOP;
	}
}
