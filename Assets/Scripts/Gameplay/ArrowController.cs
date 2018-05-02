using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowController : MonoBehaviour
{

    public int lengthNormal, lengthExtend;
    public GameObject DotObject;

    public Sprite[] colorPack;
    public List<GameObject> ArrowParts = new List<GameObject>();
    public GameplayController gameplayController;
    public GameObject extender;
    public List<GameObject> extenderParts = new List<GameObject>();

    public GameObject startPoint;

    public int currentID = -1;
    public int LimitLevel = 0; // Lowest Level to show full arrow
    void Awake()
    {
        transform.position = startPoint.transform.position;
    }

    public void CreateDots()
    {

        if (GlobalData.GetCurrentLevel() < LimitLevel)
        {
            lengthExtend = 2;
        }
        else
        {
            lengthExtend = 20;
        }

        if (GlobalData.gameMode == GlobalData.GameMode.ENDLESS_MODE)
        {
            lengthExtend = 8;
        }

        for (int i = 0; i < lengthNormal; i++)
        {
            GameObject newpart = Instantiate(DotObject) as GameObject;
            newpart.transform.SetParent(transform);
            newpart.transform.localPosition = new Vector3(0.2f + i * 0.2f, 0, 0);

            ArrowParts.Add(newpart);
        }

        for (int i = 0; i < lengthExtend; i++)
        {
            GameObject newpart = Instantiate(DotObject) as GameObject;
            newpart.transform.SetParent(extender.transform);
            newpart.transform.localPosition = new Vector3(i * 0.2f, 0, 0);

            extenderParts.Add(newpart);
        }
    }

    bool stopped = false;
    int needExtend = 0;
    float bounceY = 0;//, bounceX = 0;
    public void UpdateArrow()
    {
        stopped = false;
        needExtend = 0;
        bounceY = 0; //bounceX = 0;

        for (int i = 0; i < ArrowParts.Count; i++)
        {
            GameObject arrowpart = ArrowParts[i];

            if (stopped)
            {
                arrowpart.SetActive(false);
                continue;
            }

            arrowpart.SetActive(CheckPos(arrowpart.transform.position));
        }

        if (needExtend != 0)
        {
            bool extendStopped = false;
            for (int i = 0; i < extenderParts.Count; i++)
            {
                GameObject extenderpart = extenderParts[i];
                if (extendStopped)
                {
                    extenderpart.SetActive(false);
                    continue;
                }

                if (gameplayController._bubbleBoard.MeetAnyBubble(extenderpart.transform.position))
                {
                    extenderpart.SetActive(false);
                    extendStopped = true;
                }
                else if (extenderpart.transform.position.x < GameplayController.LeftWall + Bubble.BUBBLE_RADIUS / 2)
                {
                    extenderpart.SetActive(false);
                }
                else if (extenderpart.transform.position.x > GameplayController.RightWall - Bubble.BUBBLE_RADIUS / 2)
                {
                    extenderpart.SetActive(false);
                }
                else
                {
                    extenderpart.SetActive(true);
                }
            }

            if (needExtend == 1)
            {
                extender.transform.position = new Vector3(GameplayController.RightWall - Bubble.BUBBLE_RADIUS / 2, bounceY, extender.transform.position.z);
                extender.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 180 - transform.eulerAngles.z);
                extender.SetActive(true);
            }
            else
            {
                extender.transform.position = new Vector3(GameplayController.LeftWall + Bubble.BUBBLE_RADIUS / 2, bounceY, extender.transform.position.z);
                extender.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 180 - transform.eulerAngles.z);
                extender.SetActive(true);
            }
        }
        else
        {
            extender.SetActive(false);
        }

    }

    public void UpdateDotColor(int id)
    {
        if (id < 1 || id > 10) return;

        if (id == currentID) return;

        currentID = id;
        
        for (int i = 0; i < ArrowParts.Count; i++)
        {
            GameObject part = ArrowParts[i];
            SpriteRenderer srenderer = part.GetComponent<SpriteRenderer>();
            srenderer.sprite = colorPack[id-1];

        }
        for (int i = 0; i < extenderParts.Count; i++)
        {
            GameObject part = extenderParts[i];
            SpriteRenderer srenderer = part.GetComponent<SpriteRenderer>();
            srenderer.sprite = colorPack[id - 1];
        }
    }

    bool CheckPos(Vector3 pos)
    {
        bool result = true;
        if (gameplayController._bubbleBoard.MeetAnyBubble(pos))
        {
            result = false;
            stopped = true;
        }
        else if (pos.x < GameplayController.LeftWall + Bubble.BUBBLE_RADIUS / 2)
        {
            result = false;
            stopped = true;
            needExtend = -1;
            bounceY = pos.y;
        }
        else if (pos.x > GameplayController.RightWall - Bubble.BUBBLE_RADIUS / 2)
        {
            result = false;
            stopped = true;
            needExtend = 1;
            bounceY = pos.y;
        }
        return result;
    }
}
