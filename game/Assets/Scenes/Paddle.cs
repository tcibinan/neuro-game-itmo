using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{

    [SerializeField]
    float speed;
    float height;
    string input;
    public bool isRight;
    public Ball ball;

    public void Init(bool isRightPaddle, Ball ball)
    {
        this.isRight = isRightPaddle;
        this.ball = ball;
        Vector2 pos = Vector2.zero;
        if (isRightPaddle) {
            pos = new Vector2(GameManager.topRight.x, 0);
            pos -= Vector2.right * transform.localScale.x;
            input = "PaddleRight";
        } else {
            pos = new Vector2(GameManager.bottomLeft.x, 0);
            pos += Vector2.right * transform.localScale.x;
            input = "PaddleLeft";
        }
        transform.position = pos;
        transform.name = input;
    }

    // Start is called before the first frame update
    void Start()
    {
        height = transform.localScale.y;
        speed = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        float move = Input.GetAxis(input);
        Touch[] touches = Input.touches;
        Vector2? fingerPosition;
        if (touches.Length > 0) {
            fingerPosition = Camera.main.ScreenToWorldPoint(touches[0].position);
        } else {
            fingerPosition = null;
        }

        if (isRight) {
            if (fingerPosition.HasValue) {
                if (fingerPosition.Value.y > transform.position.y) {
                    move = 1;
                } else if (fingerPosition.Value.y < transform.position.y) {
                    move = -1;
                } else {
                    move = 0;
                }
            }
        } else {
            float ballPositionY = ball.transform.position.y;
            if (transform.position.y > ballPositionY) {
                move = -1;
            } else if (transform.position.y < ballPositionY) {
                move = 1;
            } else {
                move = 0;
            }
        }

        if (transform.position.y < GameManager.bottomLeft.y + height / 2 && move < 0) {
            move = 0;
        }

        if (transform.position.y > GameManager.topRight.y - height / 2 && move > 0) {
            move = 0;
        }

        transform.Translate(move * Time.deltaTime * speed * Vector2.up);
    }
}
