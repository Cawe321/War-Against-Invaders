using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamCamera : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 200f;
    [SerializeField]
    float rotationSpeed = 10f;
    [SerializeField]
    float maxHeightAngle = 89f;


    [SerializeField]
    float minHeight = 0f;

    public bool isSpectate { get; private set; }
    PlaneEntity spectateEntity;

    public CinemachineVirtualCamera freeRoamCam;

    // Start is called before the first frame update
    void Start()
    {
        freeRoamCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.instance.gameplayPhase != GameplayManager.GAMEPLAY_PHASE.GAME)
        {
            // Override camera rotation for Intro Event
            transform.LookAt(GameplayManager.instance.GetSpaceshipEntity().transform);
        }
        else if (!isSpectate)
        {
            // No special events that require camera's attention.
            // Transform Rotation
            if (Input.GetMouseButton(1))
            {
                // Center the mouse
                MouseManager.instance.mouseLockState = CursorLockMode.Locked;
                float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * rotationSpeed;
                float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * rotationSpeed;
                if (newRotationY > 180f)
                    newRotationY -= 360f;
                transform.localEulerAngles = new Vector3(Mathf.Clamp(newRotationY, -maxHeightAngle, maxHeightAngle), newRotationX);
            }
            else
            {
                MouseManager.instance.mouseLockState = CursorLockMode.None;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y);
            }

            // Transform Movement
            if (Input.GetButton("Forward"))
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
            if (Input.GetButton("Backward"))
            {
                transform.position -= transform.forward * Time.deltaTime * moveSpeed;
            }
            if (Input.GetButton("Leftward"))
            {
                transform.position -= transform.right * Time.deltaTime * moveSpeed;
            }
            if (Input.GetButton("Rightward"))
            {
                transform.position += transform.right * Time.deltaTime * moveSpeed;
            }
            if (Input.GetButton("Upward"))
            {
                transform.position += Vector3.up * Time.deltaTime * moveSpeed;
            }
            if (Input.GetButton("Downward"))
            {
                transform.position -= Vector3.up * Time.deltaTime * moveSpeed;
            }

            if (transform.position.y < minHeight)
                transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
        }   // free roam mode
        else                        // spectate mode
        {
            if (Input.GetButtonDown("SpectateLeft"))
                SpectateLeft();
            if (Input.GetButtonDown("SpectateRight"))
                SpectateRight();


            
            if (spectateEntity == null || (spectateEntity.baseEntity != null && !spectateEntity.baseEntity.CheckHealth()))
            {
                // Assign one
                if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                {
                    if (GameplayManager.instance.defenderPlaneContainer.transform.childCount > 0)
                    {
                        spectateEntity = GameplayManager.instance.defenderPlaneContainer.transform.GetChild(0).GetComponent<PlaneEntity>();
                        // Enable the new camera
                        spectateEntity.cmCamera.SetActive(true);
                        TellPlayerSpectate(true);
                    }
                    else
                    {
                        TellPlayerSpectate(false);
                    }
                   
                }
                else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
                {
                    if (GameplayManager.instance.invaderPlaneContainer.transform.childCount > 0)
                    {
                        spectateEntity = GameplayManager.instance.invaderPlaneContainer.transform.GetChild(0).GetComponent<PlaneEntity>();
                        TellPlayerSpectate(true);
                    }
                    else
                    {
                        TellPlayerSpectate(false);
                    }
                }
            }


            
        }
        
    }

    void TellPlayerSpectate(bool hasPlanesToSpectate)
    {
        if (!hasPlanesToSpectate)
            MainHUDManager.instance.spectateInfoText.text = "No available planes to spectate.";
        else
        {
            if (spectateEntity.baseEntity.isAnyPlayerControlling)
                MainHUDManager.instance.spectateInfoText.text = "Currently operated by " + spectateEntity.baseEntity.playerControlling;
            else
                MainHUDManager.instance.spectateInfoText.text = "Currently operated by no one";
        }
    }

    public void SpectateLeft()
    {
        // DIsable the old spectate camera
        if (spectateEntity != null)
        {
            spectateEntity.cmCamera.SetActive(false);

            // Calculating ID
            int id = spectateEntity.transform.GetSiblingIndex();
            if (id > 0)
                --id;
            else
            {
                if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                    id = GameplayManager.instance.defenderPlaneContainer.transform.childCount - 1;
                else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
                    id = GameplayManager.instance.invaderPlaneContainer.transform.childCount - 1;
            }

            if (id < 0)
            {
                // No planes to spectate
                TellPlayerSpectate(false);
                spectateEntity = null;
                return;
            }
            else
            {
                // Assigning the id and camera
                if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                    spectateEntity = GameplayManager.instance.defenderPlaneContainer.transform.GetChild(id).GetComponent<PlaneEntity>();
                else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
                    spectateEntity = GameplayManager.instance.invaderPlaneContainer.transform.GetChild(id).GetComponent<PlaneEntity>();

                TellPlayerSpectate(true);
            }
            // Enable the new camera
            spectateEntity.cmCamera.SetActive(true);
        }



        
        
    }

    public void SpectateRight()
    {
        if (spectateEntity != null)
        {
            // DIsable the old spectate camera
            spectateEntity.cmCamera.SetActive(false);

            // Calculating ID
            int id = spectateEntity.transform.GetSiblingIndex();
            if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
            {
                if (GameplayManager.instance.defenderPlaneContainer.transform.childCount == 0)
                {
                    TellPlayerSpectate(false);
                    return;
                }

                if (id < GameplayManager.instance.defenderPlaneContainer.transform.childCount - 2)
                    ++id;
                else
                    id = 0;
            }
            else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
            {
                if (GameplayManager.instance.invaderPlaneContainer.transform.childCount == 0)
                {
                    TellPlayerSpectate(false);
                    return;
                }

                if (id < GameplayManager.instance.invaderPlaneContainer.transform.childCount - 2)
                    ++id;
                else
                    id = 0;
            }

            // Assigning the id and camera
            if (PlayerManager.instance.playerTeam == TEAM_TYPE.DEFENDERS)
                spectateEntity = GameplayManager.instance.defenderPlaneContainer.transform.GetChild(id).GetComponent<PlaneEntity>();
            else if (PlayerManager.instance.playerTeam == TEAM_TYPE.INVADERS)
                spectateEntity = GameplayManager.instance.invaderPlaneContainer.transform.GetChild(id).GetComponent<PlaneEntity>();

            // Enable the new camera
            spectateEntity.cmCamera.SetActive(true);
        }
       
    }

    public void StartSpectate()
    {
        isSpectate = true;
    }

    public void StopSpectate()
    {
        MainHUDManager.instance.spectateUI.SetActive(false);
        isSpectate = false;
        if (spectateEntity != null)
            spectateEntity.cmCamera.SetActive(false);

        if (spectateEntity != null)
        {
            transform.position = spectateEntity.cmCamera.transform.position;
            transform.rotation = spectateEntity.cmCamera.transform.rotation;
            spectateEntity = null;
        }

    }
}
