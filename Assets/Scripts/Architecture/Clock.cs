using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //needed for DateTime class

public class Clock : MonoBehaviour
{
    [SerializeField]
    public GameObject hoursHand;

    [SerializeField]
    public GameObject minutesHand;

    [SerializeField]
    public GameObject secondsHand;

    [SerializeField] private ClockDirection clockDirection;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DateTime currentTime = DateTime.Now; //get the current time
        float hour = currentTime.Hour % 12;
        float minute = currentTime.Minute;
        float second = currentTime.Second;

        float hourDegrees = (hour * (360f / 12f)); //scale to degrees: 12 hours per 360 degree cycle
        float minuteDegrees = (minute * (360f / 60f));
        float secondDegrees = (second * (360f / 60f)); //scale to degrees: 360 degrees per 60 seconds on a clock face cycle
        
        minuteDegrees += secondDegrees / 60;
        hourDegrees += minuteDegrees / 12;

        //Debug.Log(currentTime);
        //Debug.Log(hourDegrees + ":" + minuteDegrees + ":" + secondDegrees);

        if (clockDirection == ClockDirection.Z)
        {
            hoursHand.transform.localRotation = Quaternion.Euler(new Vector3(hourDegrees, 0, 0));
            minutesHand.transform.localRotation = Quaternion.Euler(new Vector3(minuteDegrees, 0, 0));
            secondsHand.transform.localRotation = Quaternion.Euler(new Vector3(secondDegrees, 0, 0));
        }
        else
        {
            hoursHand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, hourDegrees));
            minutesHand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, minuteDegrees));
            secondsHand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, secondDegrees));
        }


    }
}
