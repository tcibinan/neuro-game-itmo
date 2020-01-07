using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static Vector2 bottomLeft;
    public static Vector2 topRight;

    public Ball ball;
    public Paddle paddle;
    public Text txt;
    AndroidJavaObject sensorHeartRate;

    // Start is called before the first frame update
    void Start()
    {
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        Instantiate(ball);

        Paddle paddle1 = Instantiate(paddle) as Paddle;
        Paddle paddle2 = Instantiate(paddle) as Paddle;
        paddle1.Init(true);
        paddle2.Init(false);

        sensorHeartRate = new AndroidJavaObject("com.itmo.heartrate.SensorHeartRate");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            int heartRate = sensorHeartRate.Call<AndroidJavaObject>("get").Call<int>("intValue");
            if (heartRate > 0) {
                txt.text = heartRate.ToString();
            } else {
                txt.text = "No finger detected.";
            }
        }
        catch
        {
            txt.text = "No heart rate sensor detected.";
        }
    }
}
