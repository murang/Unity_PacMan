using UnityEngine;
using System.Collections;

public class AudioChannels : MonoBehaviour {

	private const int AUDID_CHANNEL_NUM = 8;
	private struct CHANNEL{
		public AudioSource c_audio_source;
		public float key_on_time;
	}
	private CHANNEL[] m_channels;

	void Awake(){
		m_channels = new CHANNEL[AUDID_CHANNEL_NUM];
		for (int i=0; i<AUDID_CHANNEL_NUM; i++) {
			m_channels[i].c_audio_source = gameObject.AddComponent<AudioSource>();
			m_channels[i].key_on_time = 0;
		}
	}

	public int playOneShot(AudioClip clip, float volume, float pan, float pitch = 1.0f){
		for (int i=0; i<m_channels.Length; i++) {
			if(m_channels[i].c_audio_source.isPlaying && 
			   m_channels[i].c_audio_source.clip == clip &&
			   m_channels[i].key_on_time >= Time.time - 0.03f
			   ){
				return -1;
			}
		}

		int oldest = -1;
		float time = 100000000.0f;
		for (int i=0; i<m_channels.Length; i++) {
			if(m_channels[i].c_audio_source.loop == false &&
			   m_channels[i].c_audio_source.isPlaying &&
			   m_channels[i].key_on_time < time
			   ){
				oldest = i;
				time = m_channels[i].key_on_time;
			}
			if(!m_channels[i].c_audio_source.isPlaying) {
				m_channels[i].c_audio_source.clip = clip;
				m_channels[i].c_audio_source.volume = volume;
				m_channels[i].c_audio_source.panStereo = pan;
				m_channels[i].c_audio_source.loop = false;
				m_channels[i].c_audio_source.pitch = pitch;
				m_channels[i].c_audio_source.Play();
				m_channels[i].key_on_time = Time.time;
				return i;
			}
		}

		if (oldest != -1) {
			m_channels[oldest].c_audio_source.clip = clip;
			m_channels[oldest].c_audio_source.volume = volume;
			m_channels[oldest].c_audio_source.panStereo = pan;
			m_channels[oldest].c_audio_source.loop = false;
			m_channels[oldest].c_audio_source.pitch = pitch;
			m_channels[oldest].c_audio_source.Play();
			m_channels[oldest].key_on_time = Time.time;
			return oldest;
		}

		return -1;
	}

	public int playLoop(AudioClip clip, float volume, float pan, float pitch = 1.0f){
		for (int i = 0; i < m_channels.Length ; i++) {
			if (!m_channels[i].c_audio_source.isPlaying) {
				m_channels[i].c_audio_source.clip = clip;
				m_channels[i].c_audio_source.volume = volume;
				m_channels[i].c_audio_source.panStereo = pan;
				m_channels[i].c_audio_source.loop = true;
				m_channels[i].c_audio_source.pitch = pitch;
				m_channels[i].c_audio_source.Play();
				m_channels[i].key_on_time = Time.time;
				return i;
			}
		}
		return -1;
	}

	public void stopAll(){
		foreach (CHANNEL channel in m_channels) {
			channel.c_audio_source.Stop();
		}
	}

	public void stopByID(int id){
		if (id >= 0 && id < m_channels.Length) {
			m_channels[id].c_audio_source.Stop();
		}
	}
}










