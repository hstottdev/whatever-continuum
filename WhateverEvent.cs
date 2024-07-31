using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WhateverEvent : MonoBehaviour
{
    [SerializeField] AudioClip effectMusic;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] PostProcessShuffler shuffler;
    [SerializeField] Animator whateverMachine;
    [SerializeField] GameObject machineTeleportFX;
    [SerializeField] List<RuptureManager> ruptures;
    [Header("Camera")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float cameraZoomSpeed;
    public void StartTheEvent()
    {
        StartCoroutine(TheEvent());
    }


    public IEnumerator TheEvent()
    {
        LevelManager.disableInputs = true;
        AudioManager.SetMusic(effectMusic);
        AudioManager.inst.music.loop = false;
        yield return new WaitForSeconds(0.2f);
        whateverMachine.Play("active");
        yield return new WaitWhile(() => AudioManager.inst.music.time < 5);
        shuffler.StartShuffling();

        yield return new WaitWhile(() => AudioManager.inst.music.time < 17);
        machineTeleportFX.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        whateverMachine.gameObject.SetActive(false);
        shuffler.StopShuffling();
        yield return new WaitWhile(() => AudioManager.inst.music.time < 19);
        
        foreach(RuptureManager rupture in ruptures)
        {
            virtualCamera.m_Lens.Dutch = Random.Range(0,1f);
            rupture.StartCoroutine(rupture.Open());
            shuffler.StartShuffling();
            yield return new WaitForSeconds(0.5f);
            virtualCamera.m_Lens.Dutch = Random.Range(0, 1f);
            shuffler.StopShuffling();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitWhile(() => AudioManager.inst.music.time < 24);
        //camera zoom

        float targetZoom = 0.05f;

        yield return new WaitForSeconds(0.5f);

        while (Mathf.Abs(virtualCamera.m_Lens.OrthographicSize - targetZoom) > 0.01f)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetZoom, Time.deltaTime * cameraZoomSpeed);

            yield return new WaitForEndOfFrame();
            virtualCamera.m_Lens.Dutch = Random.Range(0, 2f);
        }

        virtualCamera.m_Lens.OrthographicSize = targetZoom;

         yield return new WaitWhile(() => !AudioManager.inst.music.isPlaying);//wait until music stops

        AudioManager.SetMusic(menuMusic);
        LevelManager.disableInputs = false;
        SceneManager.LoadScene("Main Menu");

    }
}
