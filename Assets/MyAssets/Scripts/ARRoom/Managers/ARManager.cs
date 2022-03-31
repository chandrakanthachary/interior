using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class ARManager : MonoBehaviour
{
    ARSession aRSession;
    ARPlacementManager aRPlacementManager;
    ARModificationManager aRModificationManager;

    GameObject aRModel;

    Button resetButton;
    Button exitButton;

    GameObject floorTriggerPanel;

    int firstCount = 0;

    ARModel selectedARModel;

    void Start()
    {
        aRSession = GameObject.Find("/AR Session").GetComponent<ARSession>();

        aRPlacementManager = transform.Find("ARPlacementMode").gameObject.GetComponent<ARPlacementManager>();
        aRModificationManager = transform.Find("ARModificationMode").gameObject.GetComponent<ARModificationManager>();

        resetButton = GameObject.Find("/Canvas/ARBasicMode/TopLeftPanel/ResetButton").GetComponent<Button>();
        resetButton.onClick.AddListener(ResetARSession);

        exitButton = resetButton = GameObject.Find("/Canvas/ARBasicMode/TopLeftPanel/ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(ExitARRoom);

        floorTriggerPanel = GameObject.Find("/Canvas/ARModificationMode/FloorTriggerPanel");

        selectedARModel = MainManager.Instance.selectedARModelPrefab.GetComponent<ARModel>();
    }

    private void Update()
    {
        if (firstCount == 0 && aRModificationManager.gameObject.activeInHierarchy && aRPlacementManager.gameObject.activeInHierarchy)
        {
            InitializeUI();
            InitializeManagers();
            InitializeARModel();
            firstCount++;
        }
    }

    private void InitializeUI()
    {

        if (selectedARModel.modelType == ModelType.Studio)
        {
            floorTriggerPanel.SetActive(false);
        }
        else if (selectedARModel.modelType == ModelType.Loft)
        {
            floorTriggerPanel.SetActive(true);
        }
    }

    private void InitializeManagers()
    {
        SetActiveARPlacementMode(true);
        SetActiveARModificationMode(false);
    }

    private void InitializeARModel()
    {
        
        aRModel = Instantiate(selectedARModel.gameObject, new Vector3(0, 0, 0), selectedARModel.gameObject.transform.rotation);
        
        aRPlacementManager.aRModel = aRModel;

        if (selectedARModel.modelType == ModelType.Loft)
        {
            aRModificationManager.firstFloor = aRModel.transform.Find("FirstFloor").gameObject;
        }

        aRModificationManager.middleWallList = FindObjectsOfType<MiddleWall>();
        aRModificationManager.upperWallList = FindObjectsOfType<UpperWall>();

        aRModel.SetActive(false);
    }

    public void SetActiveARPlacementMode (bool isActive)
    {
        aRPlacementManager.gameObject.SetActive(isActive);
    }

    public void SetActiveARModificationMode (bool isActive)
    {
        aRModificationManager.gameObject.SetActive(isActive);
    }


    private void ResetARSession()
    {
        aRSession.Reset();
        aRModel.GetComponent<Lean.Touch.LeanPinchScale>().enabled = true;
        aRModel.GetComponent<Lean.Touch.LeanTwistRotateAxis>().enabled = true;
        aRModel.SetActive(false);
        ResetAllMode();
    }

    private void ResetAllMode()
    {
        aRPlacementManager.RestartUIFlow();
        aRModificationManager.RestartUIFlow();
        SetActiveARPlacementMode(true);
        SetActiveARModificationMode(false);
    }

    private void ExitARRoom ()
    {
        SceneManager.LoadScene(0);
    }

}