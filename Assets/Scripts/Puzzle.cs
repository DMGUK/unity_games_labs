using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{

    public NumberBox boxPrefab;
    public NumberBox[,] boxes = new NumberBox[4, 4];
    public Sprite[] sprites;
    public GameObject winTextObject;
    public GameObject welcomeTextObject;
    public InputField shuffleInputField;
    public Button startButton;
    public Button restartButton;

    void Start()
    {
        Init();
        EnablePuzzle(false);
        winTextObject.SetActive(false);
        welcomeTextObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
    }

    void Init()
    {
        int n = 0;
        for (int y = 3; y >= 0; y--)
            for (int x = 0; x < 4; x++) { 
                NumberBox box = Instantiate(boxPrefab, new Vector2(x, y), Quaternion.identity);
                box.Init(x, y, n + 1, sprites[n], ClickToSwap);
                boxes[x, y] = box;
                n++;
            }
    }

    void EnablePuzzle(bool enable)
    {
        foreach (NumberBox box in boxes)
        {
            box.GetComponent<Collider2D>().enabled = enable;
            box.GetComponent<SpriteRenderer>().enabled = enable;
        }
    }

    public void StartGame()
    {
        int numShuffles = int.Parse(shuffleInputField.text);
        Shuffle(numShuffles);
        EnablePuzzle(true);
        shuffleInputField.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        welcomeTextObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    void ClickToSwap(int x, int y) {
        int dx = getDx(x, y);
        int dy = getDy(x, y);

        Swap(x, y, dx, dy);
        CheckWin();
    }

    void Swap(int x, int y, int dx, int dy) {
        
        var from = boxes[x, y];
        var target = boxes[x+dx, y+dy];

        //swapping the boxes

        boxes[x, y] = target;
        boxes[x + dx, y + dy] = from;

        //update position
        from.UpdatePos(x + dx, y + dy);
        target.UpdatePos(x, y);
    }

    int getDx(int x, int y) 
    {
        // right side
        if (x < 3 && boxes[x + 1, y].IsEmpty()) { 
            return 1;
        }
        // left side
        if (x > 0 && boxes[x - 1, y].IsEmpty())
        {
            return -1;
        }
        return 0;
    }

    int getDy(int x, int y)
    {
        // top
        if (y < 3 && boxes[x, y + 1].IsEmpty())
        {
            return 1;
        }
        // bottom
        if (y > 0 && boxes[x, y - 1].IsEmpty())
        {
            return -1;
        }
        return 0;
    }

    void Shuffle(int n)
    {
        for (int x = 0; x < n; x++) { 
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (boxes[i, j].IsEmpty())
                    {
                        Vector2 pos = getValidMove(i, j);
                        Swap(i, j, (int)pos.x, (int)pos.y);
                    }
                }
            }
        }
    }

    private Vector2 lastMove;

    Vector2 getValidMove(int x, int y) {
        Vector2 pos = new Vector2();
        do
        {
            int n = Random.Range(0, 4);
            if (n == 0)
            {
                pos = Vector2.left;
            }
            else if (n == 1)
            {
                pos = Vector2.right;
            }
            else if (n == 2)
            {
                pos = Vector2.up;
            }
            else if (n == 3)
            {
                pos = Vector2.down;
            }
        } while (!(isValidRange(x+(int)pos.x) && isValidRange(y+(int)pos.y)) || isRepeatMove(pos));
        lastMove = pos;
        return pos;
    }

    bool isValidRange(int n) {
        return n >= 0 && n <= 3;
    }

    bool isRepeatMove(Vector2 pos) {
        return pos * -1 == lastMove;
    }

    void CheckWin()
    {
        int n = 0;
        for (int y = 3; y >= 0; y--)
        {
            for (int x = 0; x < 4; x++)
            {
                NumberBox box = boxes[x, y];
                if (box.index != n + 1)
                {
                    return;
                }
                n++;
            }
        }

        //Debug.Log("Congratulations! You have won!");
        winTextObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        foreach (NumberBox box in boxes)
        {
            Destroy(box.gameObject);
        }

        Init();

        EnablePuzzle(false);
        winTextObject.SetActive(false);
        welcomeTextObject.SetActive(true);
        shuffleInputField.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
    }



}
