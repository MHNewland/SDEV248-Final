using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneControler : MonoBehaviour
{
    [SerializeField]
    PlayableDirector cutsceneDirector;

    [SerializeField]
    TimelineAsset timelineAsset;

    [SerializeField]
    List<TrackAsset> timelineTracks;

    [SerializeField]
    CinemachineBrain MainCamera;

    [SerializeField]
    bool cutscenePlayed = false;

    CinemachineShot palyervirtualCamera;

    private void Start()
    {

        foreach (TrackAsset ta in timelineAsset.GetOutputTracks())
        {
            switch(ta.name)
            {

                case "Camera":
                    
                    MainCamera = PlayerController.Instance.GetComponentInChildren<CinemachineBrain>();
                    cutsceneDirector.SetGenericBinding(ta, MainCamera);
                    foreach (TimelineClip t in ta.GetClips())
                    {
                        if (t.displayName == "CM vcam1")
                        {
                            palyervirtualCamera = t.asset as CinemachineShot;
                            var test2 = PlayerController.Instance.GetComponentInChildren<CinemachineVirtualCamera>();
                            cutsceneDirector.SetReferenceValue(palyervirtualCamera.VirtualCamera.exposedName, test2);
                        }
                    }
                    break;
                
                default:
                    break;
                    
            }

            
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!cutscenePlayed && collision.gameObject.CompareTag("Player"))
        {
            cutsceneDirector.Play();
            cutscenePlayed = true;
        }
    }

}
