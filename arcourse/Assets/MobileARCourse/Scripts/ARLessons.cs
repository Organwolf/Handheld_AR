using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARLessons : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text stateText;
    [SerializeField] TMPro.TMP_Text numberOfPlanesText;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] ARPointCloudManager pointCloudManager;
    //[SerializeField] Transform transformVariable;
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] GameObject prefabRobot;
    [SerializeField] ARCameraManager cameraManager;
    [SerializeField] GameObject ballPrefab;

    private GameObject robot;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    private Vector3 fingerPostion;
    private int planeAdded = 0;
    private int planeRemoved = 0;
    private bool placementPoseIsValid = false;

    // GPS variables
    private float Latitude;
    private float Longitude;

    void Start()
    {
        ARSession.stateChanged += ARSession_StateChanged;
        planeManager.planesChanged += PlaneManager_PlanesChanged;
        pointCloudManager.pointCloudsChanged += PointCloudManager_PointCloudsChanged;
        
        // GPS
        //if (Input.location.isEnabledByUser)
        StartCoroutine(GetLocation());

    }

    private IEnumerator GetLocation()
    {
        Input.location.Start();
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Latitude = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
        yield break;

    }
    public void ShootBall()
    {
        GameObject theball = Instantiate(ballPrefab);
        theball.transform.position = Camera.main.transform.position;
        var rb = theball.GetComponent<Rigidbody>();
        rb.AddForce(5000 * Camera.main.transform.forward);
    }

    void PointCloudManager_PointCloudsChanged(ARPointCloudChangedEventArgs obj)
    {
        //Debug.Log("point cloud");
    }

    // Added planes accumulate as the app runs
    // https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/api/UnityEngine.XR.ARFoundation.ARPlaneManager.html?q=planes

    void PlaneManager_PlanesChanged(ARPlanesChangedEventArgs obj)
    {
        //Debug.Log("planes changed");
        planeAdded += obj.added.Count;
        planeRemoved += obj.removed.Count;
        //numberOfPlanesText.text = "planes added: " + planeAdded + "\nplanes removed: " + planeRemoved;
    }


    void ARSession_StateChanged(ARSessionStateChangedEventArgs obj)
    {
        //Debug.Log("state");
        stateText.text = obj.state.ToString();  
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Touch touch = Input.GetTouch(0);
            bool hitsFound = raycastManager.Raycast(touch.position, s_Hits, TrackableType.Planes);

            // https://github.com/Unity-Technologies/arfoundation-samples/blob/master/Assets/Scripts/PlaceOnPlane.cs
            // line 46-61
            // currently at: Step 2 -> 3.
            
            // light estimation callbacks!

            if (hitsFound)
            {
                var hit = s_Hits[0];
                Vector3 newRobotPosition = hit.pose.position;
                Quaternion newRobotRotation = hit.pose.rotation;

                //robot = Instantiate(prefabRobot, newRobotPosition, newRobotRotation);
                if (robot == null)
                {
                    robot = Instantiate(prefabRobot, newRobotPosition, newRobotRotation);
                    var rb = robot.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    // rb.detectCollisions = true;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                else
                {
                    robot.transform.position = newRobotPosition;
                    robot.transform.rotation = newRobotRotation;
                }

            }
            // test som skriver ut x, y av finger på skärm
            //Vector2 touchPosition = touch.position;
            //numberOfPlanesText.text = $"x: {touchPosition.x} \ny: {touchPosition.y}";
            //numberOfPlanesText.text = $"hits baby: {s_Hits.Count}";
        }
        Latitude = Input.location.lastData.latitude;
        Longitude = Input.location.lastData.longitude;
        numberOfPlanesText.text = $"Latitude: {Latitude}\nLongitude: {Longitude}";

    }
}
