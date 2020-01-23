using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{

    [SerializeField]
    public float speed = 6;

    private float initialSpeed;
    private Text text;
    private int leftScore;
    private int rightScore;
    private float radius;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector2.one.normalized;
        radius = transform.localScale.x / 2;
        initialSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (transform.position.y < GameManager.bottomLeft.y + radius && direction.y < 0) {
            direction.y = - direction.y;
        }

        if (transform.position.y > GameManager.topRight.y - radius && direction.y > 0) {
            direction.y = - direction.y;
        }

        if (transform.position.x < GameManager.bottomLeft.x + radius && direction.x < 0) {
//            Debug.Log("Right has won");
//            Time.timeScale = 0;
//            enabled = false;
            rightScore += 1;
            transform.position = new Vector2(GameManager.bottomLeft.x + (GameManager.topRight.x - GameManager.bottomLeft.x) / 2,
                GameManager.bottomLeft.y + (GameManager.topRight.y - GameManager.bottomLeft.y) / 2);
        }

        if (transform.position.x > GameManager.topRight.x - radius && direction.x > 0) {
//            Debug.Log("Left has won");
//            Time.timeScale = 0;
//            enabled = false;
            leftScore += 1;
            transform.position = new Vector2(GameManager.bottomLeft.x + (GameManager.topRight.x - GameManager.bottomLeft.x) / 2,
                GameManager.bottomLeft.y + (GameManager.topRight.y - GameManager.bottomLeft.y) / 2);
        }
        text.text = "Score: " + leftScore.ToString() + "-" + rightScore.ToString();
    }

    public void Init(Text text) {
        this.text = text;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Paddle") {
            bool isRight = other.GetComponent<Paddle>().isRight;

            if (isRight && direction.x > 0) {
                direction.x = - direction.x;
            }
            if (!isRight && direction.x < 0) {
                direction.x = - direction.x;
            }
        }
    }

    public void AdjustSpeed(int heartRate) {
        int minHeartRate = 50;
        int maxHeartRate = 100;
        if (heartRate < minHeartRate) {
            heartRate = minHeartRate;
        } else if (heartRate > maxHeartRate) {
            heartRate = maxHeartRate;
        }
        float heatRateRatio = 1.0F - ((float) heartRate - minHeartRate) / ((float) maxHeartRate - minHeartRate);
        float minSpeed = 4.0F;
        float maxSpeed = 14.0F;
        speed = minSpeed + (maxSpeed - minSpeed) * heatRateRatio;
    }
}
