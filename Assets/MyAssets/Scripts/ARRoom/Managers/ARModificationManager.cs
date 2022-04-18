using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARModificationManager : MonoBehaviour
{


    #region Wall&Floor


    [HideInInspector] public GameObject firstFloor;

    Button firstFloorButton;
    Button groundFloorButton;
    Button fullWallButton;
    Button halfWallButton;

    Image triggerWallButtonBg;
    Image triggerFloorButtonBg;
    
    
    bool isFullWall = true;

    [HideInInspector] public UpperWall[] upperWallList;
    [HideInInspector] public MiddleWall[] middleWallList;


    #endregion


    #region UI GameObject


    ObjectListHandler objectListHandler;
    GameObject objectListPanel;

    Image moveGuidePanel;
    Image pinchGuidePanel;
    Image modificationGuidePanel;

    Button closeModificationGuidePanelButton;


    #endregion


    #region ARModification


    public PlayerManager playerManager;
    private Lean.Touch.LeanSelectByFinger leanSelectByFinger;
    private float yBoundary;
    //public GameObject desiredObjectPrefab = null;


    #endregion


    #region MonoBehaviourCallbacks


    void Start()
    {

        // Wall&Floor
        firstFloorButton = GameObject.Find("/Canvas/ARModificationMode/FloorTriggerPanel/FirstFloorButton").gameObject.GetComponent<Button>();
        firstFloorButton.onClick.AddListener(ViewFirstFloor);

        groundFloorButton = GameObject.Find("/Canvas/ARModificationMode/FloorTriggerPanel/GroundFloorButton").gameObject.GetComponent<Button>();
        groundFloorButton.onClick.AddListener(ViewGroundFloor);

        fullWallButton = GameObject.Find("/Canvas/ARModificationMode/WallTriggerPanel/FullWallButton").gameObject.GetComponent<Button>();
        fullWallButton.onClick.AddListener(TriggerFullWall);

        halfWallButton = GameObject.Find("/Canvas/ARModificationMode/WallTriggerPanel/HalfWallButton").gameObject.GetComponent<Button>();
        halfWallButton.onClick.AddListener(TriggerHalfWall);

        triggerWallButtonBg = GameObject.Find("/Canvas/ARModificationMode/WallTriggerPanel/ButtonBg").gameObject.GetComponent<Image>();
        triggerFloorButtonBg = GameObject.Find("/Canvas/ARModificationMode/FloorTriggerPanel/ButtonBg").gameObject.GetComponent<Image>();


        // UI
        objectListHandler = GameObject.Find("/Canvas/ARModificationMode/ObjectListPanel/Scroll/Panel").GetComponent<ObjectListHandler>();
        objectListPanel = GameObject.Find("/Canvas/ARModificationMode/ObjectListPanel");
        objectListPanel.transform.localScale = new Vector3(0, 1, 0);

        moveGuidePanel = GameObject.Find("/Canvas/ARModificationMode/MoveGuidePanel").gameObject.GetComponent<Image>();
        moveGuidePanel.gameObject.transform.localScale = Vector2.zero;

        pinchGuidePanel = GameObject.Find("/Canvas/ARModificationMode/PinchGuidePanel").gameObject.GetComponent<Image>();
        pinchGuidePanel.gameObject.transform.localScale = Vector2.zero;

        modificationGuidePanel = GameObject.Find("/Canvas/ARModificationMode/ModificationGuidePanel").gameObject.GetComponent<Image>();
        modificationGuidePanel.gameObject.transform.localScale = Vector2.zero;

        closeModificationGuidePanelButton = GameObject.Find("/Canvas/ARModificationMode/ModificationGuidePanel/CloseButton").gameObject.GetComponent<Button>();
        closeModificationGuidePanelButton.onClick.AddListener(CloseModificationGuidePanel);


        //ARModification
        leanSelectByFinger = GameObject.Find("Tap To Select").GetComponent<Lean.Touch.LeanSelectByFinger>();
        leanSelectByFinger.OnSelected.AddListener(SelectObject);
        leanSelectByFinger.OnDeselected.AddListener(DeselectObject);
        leanSelectByFinger.OnNothing.AddListener(DeselectObject);
    }

    private void Update()
    {
        if (playerManager.myCurrentSelectedObject == null)
            return;

        ConstraintObjectYPosition();
    }

    private void ConstraintObjectYPosition()
    {
        if (playerManager.myCurrentSelectedObject.transform.position.y != yBoundary)
        {
            playerManager.myCurrentSelectedObject.transform.position = new Vector3(playerManager.myCurrentSelectedObject.transform.position.x, yBoundary, playerManager.myCurrentSelectedObject.transform.position.z);
        }
    }


    #endregion


    #region Walls&Floor Functions


    private void TriggerFullWall()
    {
        isFullWall = true;
        SearchAndTriggerAllWalls(isFullWall);

        LeanTween.moveLocalY(triggerWallButtonBg.gameObject, 50.0f, 0.25f).setEaseInOutQuart();
    }

    private void TriggerHalfWall()
    {
        isFullWall = false;
        SearchAndTriggerAllWalls(isFullWall);

        LeanTween.moveLocalY(triggerWallButtonBg.gameObject, -50.0f, 0.25f).setEaseInOutQuart();
    }

    private void SearchAndTriggerAllWalls (bool isFullWall)
    {
        foreach (UpperWall upperWall in upperWallList)
        {
            
            if (firstFloor != null)
            {
                if (firstFloor.activeInHierarchy)
                {
                    if (upperWall.onFloor == FloorType.FirstFloor)
                    {
                        upperWall.gameObject.SetActive(isFullWall);
                        Lean.Touch.LeanSelectableByFinger parentPaintLeanSelectable = upperWall.transform.parent.GetComponent<Lean.Touch.LeanSelectableByFinger>();
                        if (parentPaintLeanSelectable != null)
                            parentPaintLeanSelectable.enabled = isFullWall;
                    }

                }
                else
                {
                    if (upperWall.onFloor == FloorType.GroundFloor)
                    {
                        upperWall.gameObject.SetActive(isFullWall);
                        Lean.Touch.LeanSelectableByFinger parentPaintLeanSelectable = upperWall.transform.parent.GetComponent<Lean.Touch.LeanSelectableByFinger>();
                        if (parentPaintLeanSelectable != null)
                            parentPaintLeanSelectable.enabled = isFullWall;
                    }
                }
            }
            else
            {
                upperWall.gameObject.SetActive(isFullWall);
                Lean.Touch.LeanSelectableByFinger parentPaintLeanSelectable = upperWall.transform.parent.GetComponent<Lean.Touch.LeanSelectableByFinger>();
                if (parentPaintLeanSelectable != null)
                    parentPaintLeanSelectable.enabled = isFullWall;
            }
        }

        foreach (MiddleWall middleWall in middleWallList)
        {
            if (firstFloor != null)
            {
                if (firstFloor.activeInHierarchy)
                {
                    if (middleWall.onFloor == FloorType.FirstFloor)
                    {
                        middleWall.gameObject.SetActive(isFullWall);
                    }
                }
                else
                {
                    if (middleWall.onFloor == FloorType.GroundFloor)
                    {
                        middleWall.gameObject.SetActive(isFullWall);
                    }
                }
            }
            else
            {
                middleWall.gameObject.SetActive(isFullWall);
            }
        }

    }

    private void ViewFirstFloor()
    {
        SearchAndTriggerAllWalls(true);

        firstFloor.SetActive(true);
        LeanTween.moveLocalY(triggerFloorButtonBg.gameObject, 50.0f, 0.25f).setEaseInOutQuart();

        SearchAndTriggerAllWalls(isFullWall);
    }

    private void ViewGroundFloor()
    {
        firstFloor.SetActive(false);
        LeanTween.moveLocalY(triggerFloorButtonBg.gameObject, -50.0f, 0.25f).setEaseInOutQuart();

        SearchAndTriggerAllWalls(isFullWall);
    }


    #endregion


    #region UI Functions


    public void ShowObjectListPanel(GameObject myCurrentSelectedObject)
    {
        if (!myCurrentSelectedObject.CompareTag("Toilet") && !myCurrentSelectedObject.CompareTag("Shower"))
        {
            LeanTween.scale(objectListPanel.gameObject, new Vector2(1, 1), 0.25f).setEaseInOutQuart();
            objectListHandler.CreateObjectList(myCurrentSelectedObject);
        }
    }

    public void HideObjectListPanel(GameObject myCurrentSelectedObject)
    {
        if (!myCurrentSelectedObject.CompareTag("Toilet") && !myCurrentSelectedObject.CompareTag("Shower"))
        {
            LeanTween.scale(objectListPanel.gameObject, new Vector2(0, 1), 0.25f).setEaseInOutQuart();
            objectListHandler.EmptyObjectList();
        }
    }

    public void ShowGuidePanels(GameObject myCurrentSelectedObject)
    {
        if (!myCurrentSelectedObject.CompareTag("Paint") && !myCurrentSelectedObject.CompareTag("Floor"))
        {
            LeanTween.scale(moveGuidePanel.gameObject, new Vector2(1, 1), 0.25f).setEaseOutBack();
            LeanTween.scale(pinchGuidePanel.gameObject, new Vector2(1, 1), 0.25f).setEaseOutBack();
        }
    }

    public void HideGuidePanels(GameObject myCurrentSelectedObject)
    {
        if (!myCurrentSelectedObject.CompareTag("Paint") && !myCurrentSelectedObject.CompareTag("Floor"))
        {
            LeanTween.scale(moveGuidePanel.gameObject, new Vector2(0, 0), 0.25f).setEaseInBack();
            LeanTween.scale(pinchGuidePanel.gameObject, new Vector2(0, 0), 0.25f).setEaseInBack();
        }
    }

    private void CloseModificationGuidePanel()
    {
        LeanTween.scale(modificationGuidePanel.gameObject, new Vector2(0, 0), 0.1f);
    }


    #endregion


    #region ARModification


    public void SelectObject(Lean.Common.LeanSelectable leanSelectable)
    {
        
        StartCoroutine(WaitAWhileThenSelectObject(leanSelectable.gameObject));

    }

    IEnumerator WaitAWhileThenSelectObject(GameObject selectedObject)
    {
        yield return new WaitForSeconds(0.1f);

        playerManager.myCurrentSelectedObject = selectedObject;

        yBoundary = playerManager.myCurrentSelectedObject.transform.position.y;

        ShowObjectListPanel(playerManager.myCurrentSelectedObject);
        ShowGuidePanels(playerManager.myCurrentSelectedObject);
    }

    public void DeselectObject(Lean.Common.LeanSelectable leanSelectable)
    {
        if (playerManager.myCurrentSelectedObject == null)
            return;

        HideGuidePanels(playerManager.myCurrentSelectedObject);
        HideObjectListPanel(playerManager.myCurrentSelectedObject);

        playerManager.myCurrentSelectedObject = null;
    }

    public void DeselectObject()
    {
        if (playerManager.myCurrentSelectedObject == null)
            return;

        HideGuidePanels(playerManager.myCurrentSelectedObject);
        HideObjectListPanel(playerManager.myCurrentSelectedObject);

        playerManager.myCurrentSelectedObject = null;
    }

    public void DeselectObjectByOther()
    {
        if (playerManager.myCurrentSelectedObject == null)
            return;

        HideGuidePanels(playerManager.myCurrentSelectedObject);
        HideObjectListPanel(playerManager.myCurrentSelectedObject);

        playerManager.myCurrentSelectedObject = null;
    }

    public void ChangeObjectModel()
    {
        //desiredObjectPrefab = roomManager.sofaPrefabs[0];
    }


    #endregion

    //public void SelectARObject (Lean.Common.LeanSelectable leanSelectable)
    //{
    //    currentSelectable = leanSelectable.gameObject;
    //    yBoundary = currentSelectable.transform.position.y;

    //    TriggerOutline(true);
    //    ShowObjectListPanel();
    //    ShowGuidePanels();
    //}

    //public void DeselectARObject()
    //{
    //    HideGuidePanels();
    //    HideObjectListPanel();
    //    TriggerOutline(false);

    //    currentSelectable = null;
    //}

    //private void TriggerOutline (bool isEnabled)
    //{
    //    if (currentSelectable.CompareTag("Paint") || currentSelectable.CompareTag("Floor"))
    //    {
    //        GameObject room = currentSelectable.transform.parent.gameObject;
    //        Outline[] childrenOutline = room.GetComponentsInChildren<Outline>();
    //        foreach (Outline outline in childrenOutline)
    //        {
    //            outline.enabled = isEnabled;
    //        }
    //    }
    //    else
    //    {
    //        currentSelectable.GetComponent<Outline>().enabled = isEnabled;
    //    }
    //}

    //private void ShowObjectListPanel ()
    //{
    //    if (!currentSelectable.CompareTag("Toilet") && !currentSelectable.CompareTag("Shower"))
    //    {
    //        LeanTween.scale(objectListPanel.gameObject, new Vector2(1, 1), 0.25f).setEaseInOutQuart();
    //        objectListHandler.CreateObjectList(currentSelectable);
    //    }
    //}

    //private void HideObjectListPanel()
    //{
    //    if (!currentSelectable.CompareTag("Toilet") && !currentSelectable.CompareTag("Shower"))
    //    {
    //        LeanTween.scale(objectListPanel.gameObject, new Vector2(0, 1), 0.25f).setEaseInOutQuart();
    //        objectListHandler.EmptyObjectList();
    //    }
    //}

    //private void ShowGuidePanels ()
    //{
    //    if (!currentSelectable.CompareTag("Paint") && !currentSelectable.CompareTag("Floor"))
    //    {
    //        LeanTween.scale(moveGuidePanel.gameObject, new Vector2(1, 1), 0.25f).setEaseOutBack();
    //        LeanTween.scale(pinchGuidePanel.gameObject, new Vector2(1, 1), 0.25f).setEaseOutBack();
    //    }
    //}

    //private void HideGuidePanels()
    //{
    //    if (!currentSelectable.CompareTag("Paint") && !currentSelectable.CompareTag("Floor"))
    //    {
    //        LeanTween.scale(moveGuidePanel.gameObject, new Vector2(0, 0), 0.25f).setEaseInBack();
    //        LeanTween.scale(pinchGuidePanel.gameObject, new Vector2(0, 0), 0.25f).setEaseInBack();
    //    }
    //}

    //private void CloseModificationGuidePanel()
    //{
    //    LeanTween.scale(modificationGuidePanel.gameObject, new Vector2(0, 0), 0.1f);
    //}


    public void RestartUIFlow()
    {
        //moveGuidePanel.gameObject.transform.localScale = Vector2.zero;
        //pinchGuidePanel.gameObject.transform.localScale = Vector2.zero;

        //if (currentSelectable != null)
        //{
        //    DeselectARObject();
        //}

    }
}
