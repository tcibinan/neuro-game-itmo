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
    public Text leftTxt;
    public Text rightTxt;
    public Canvas canvas;
    
    private Ball actualBall;
    private Text statusText;
    private Text scoreText;
    private AndroidJavaObject sensorHeartRate;

    // Start is called before the first frame update
    void Start()
    {
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        Canvas actualCanvas = Instantiate(canvas) as Canvas;
        statusText = Instantiate(leftTxt) as Text;
        statusText.transform.SetParent(actualCanvas.transform, false);
        scoreText = Instantiate(rightTxt) as Text;
        scoreText.transform.SetParent(actualCanvas.transform, false);

        actualBall = Instantiate(ball) as Ball;
        actualBall.Init(scoreText);

        Paddle paddle1 = Instantiate(paddle) as Paddle;
        Paddle paddle2 = Instantiate(paddle) as Paddle;
        paddle1.Init(true, actualBall);
        paddle2.Init(false, actualBall);

        sensorHeartRate = new AndroidJavaObject("com.itmo.heartrate.SensorHeartRate");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            int heartRate = sensorHeartRate.Call<AndroidJavaObject>("get").Call<int>("intValue");
            if (heartRate > 0 && heartRate < 150) {
                statusText.text = "Heart rate: " + heartRate.ToString();
                actualBall.AdjustSpeed(heartRate);
            } else if (heartRate >= 150) {
                statusText.text = "Bad finger position. Put only your fingertip on the camera.";
            } else {
                statusText.text = "No finger detected. Put your finger on the camera.";
            }
            if (heartRate > 0 && heartRate < 50) {
                statusText.text = "Your heart rate is too low: " + heartRate + ". Please take a walk.";
            } else if (heartRate > 100) {
                statusText.text = "Your heart rate is too high: " + heartRate + ". Please take a break.";
            }
        }
        catch
        {
            statusText.text = "No heart rate sensor detected.";
        }
    }
}
