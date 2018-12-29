using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TetBlockShape
{
    I,T,S,Z,L,J,O
};

public enum TetBlockRotation 
{
    UP=0,RIGHT,DOWN,LEFT,
    CLOCKWISE =0, COUNTERCLOCKWISE
}


public class TetBlock {


    public Vector2[] position;
    public Vector2 slotPosition;
    public Color color;
    public TetBlockShape shape;
    public int currRotation=(int)TetBlockRotation.UP;
    public bool isRotationable = true; // o블록은 회전을 못하게
    public int[] index = new int[4];


    public TetBlock(TetBlockShape shape)
    {
        this.shape = shape;
        this.position = TetBlockForm.GetForm(shape);
        slotPosition = new Vector2(4, 0);
        switch(shape)
        {
            case TetBlockShape.I:
                color = Color.cyan;
                break;
            case TetBlockShape.T:
                color = Color.magenta;
                break;
            case TetBlockShape.S:
                color = Color.green;
                break;
            case TetBlockShape.Z:
                color = Color.red;
                break;
            case TetBlockShape.L:
                color = new Color(1.0f, 0.368f, 0.0f);
                break;
            case TetBlockShape.J:
                color = Color.blue;
                break;
            case TetBlockShape.O:
                isRotationable = false;
                color = Color.yellow;
                break;
            default:
                Debug.Log("Unknown Block Type");
                break;
        }
        UpdateIndex();
    }

    public void GoDown()
    {
        slotPosition.y+=1;
        UpdateIndex();
    }

    public void GoDown(int i)
    {
        slotPosition.y += i;
        UpdateIndex();
    }

    public void GoRight()
    {
        foreach(var block in position)
        {
            if(block.x+slotPosition.x>=9)
            {
                return;
            }
        }
        slotPosition.x++;
        UpdateIndex();
    }

    public void GoLeft()
    {
        foreach (var block in position)
        {
            if (block.x + slotPosition.x <= 0)
            {
                return;
            }
        }
        slotPosition.x--;
        UpdateIndex();

    }

    public void RotateClockwise()
    {
        if (!isRotationable) return;

        float minX=0, maxX=9;

        for(int i=0;i<4;i++)
        {
            float temp = position[i].x;
            position[i].x = -position[i].y;
            position[i].y = temp;
            if (position[i].x + slotPosition.x < minX)
            {
                minX = position[i].x + slotPosition.x;
            }
            else if (position[i].x + slotPosition.x > maxX)
            {
                maxX = position[i].x + slotPosition.x;
            }
        }
        slotPosition.x += 9 - minX - maxX;
        currRotation = (currRotation + 1) % 4;        
        UpdateIndex();
    }

    public void RotateCounterClockwise()
    {
        if (!isRotationable) return;

        float minX = 0, maxX = 9;

        for (int i = 0; i < 4; i++)
        {
            float temp = position[i].x;
            position[i].x = position[i].y;
            position[i].y = -temp;
            if (position[i].x + slotPosition.x < minX)
            {
                minX = position[i].x + slotPosition.x;
            }
            if (position[i].x + slotPosition.x > maxX)
            {
                maxX = position[i].x + slotPosition.x;
            }
        }
        slotPosition.x += 9 - minX - maxX;
        currRotation = (4 + currRotation - 1) % 4;
        UpdateIndex();
    }
      
    public void UpdateIndex()
    {
        for (int i = 0; i < 4; i++)
        {
            index[i] = (int)(position[i].x + slotPosition.x + 10 * (position[i].y + slotPosition.y));
        }
    }
}

public static class TetBlockForm
{
    public static Vector2[] GetForm(TetBlockShape shape)
    {
        Vector2[] position = new Vector2[4];
        position[0].Set(0.0f, 0.0f);
        switch (shape)
        {
            case TetBlockShape.I:
                position[1].Set(-2, 0);
                position[2].Set(-1, 0);
                position[3].Set(1, 0);

                break;

            case TetBlockShape.T:
                position[1].Set(1, 0);
                position[2].Set(-1, 0);
                position[3].Set(0, -1);

                break;

            case TetBlockShape.S:
                position[1].Set(-1, 0);
                position[2].Set(0, -1);
                position[3].Set(1, -1);

                break;

            case TetBlockShape.Z:
                position[1].Set(-1, -1);
                position[2].Set(0, -1);
                position[3].Set(1, 0);

                break;

            case TetBlockShape.L:
                position[1].Set(-1,0);
                position[2].Set(1, 0);
                position[3].Set(1, -1);

                break;

            case TetBlockShape.J:
                position[1].Set(1, 0);
                position[2].Set(-1, 0);
                position[3].Set(-1, -1);
                break;

            case TetBlockShape.O:
                position[1].Set(1, 0);
                position[2].Set(0, 1);
                position[3].Set(1, 1);
                break;
            default:
                Debug.Log("Unknown Block Type");
                break;

        } // 모양 할당
        return position;
    }

    public static Vector2[] GetRotationKick(TetBlockShape shape,TetBlockRotation prevRotation ,TetBlockRotation cw_ccw)
    {
        //rotation 0 clockwise 1 counterclockwise      
        
        Vector2[] kickVector=new Vector2[4];
        if(shape == TetBlockShape.I)
        {
            switch(prevRotation)
            {
                case TetBlockRotation.UP: //0
                    if(cw_ccw==TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(-2, 0);
                        kickVector[1].Set(1, 0);
                        kickVector[2].Set(-2, 1);
                        kickVector[3].Set(1, -2);
                    }
                    else // counter clock wise
                    {
                        kickVector[0].Set(-1, 0);
                        kickVector[1].Set(2, 0);
                        kickVector[2].Set(-1, -2);
                        kickVector[3].Set(2, 1);
                    }
                    break;

                case TetBlockRotation.RIGHT: //1
                    if (cw_ccw == TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(-1, 0);
                        kickVector[1].Set(2, 0);
                        kickVector[2].Set(-1, -2);
                        kickVector[3].Set(2, 1);
                    }
                    else // counter clock wise
                    {
                        kickVector[0].Set(2, 0);
                        kickVector[1].Set(-1, 0);
                        kickVector[2].Set(2, -1);
                        kickVector[3].Set(-1, 2);
                    }
                    break;

                case TetBlockRotation.DOWN: //2
                    if (cw_ccw == TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(2, 0);
                        kickVector[1].Set(-1, 0);
                        kickVector[2].Set(2, -1);
                        kickVector[3].Set(-1, 2);
                    }
                    else // counter clock wise
                    {
                        kickVector[0].Set(1, 0);
                        kickVector[1].Set(-2, 0);
                        kickVector[2].Set(1, 2);
                        kickVector[3].Set(-2, -1);
                    }
                    break;

                case TetBlockRotation.LEFT: //3
                    if (cw_ccw == TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(1, 0);
                        kickVector[1].Set(-2, 0);
                        kickVector[2].Set(1, 2);
                        kickVector[3].Set(-2, 1);
                    }
                    else // counter clock wise
                    {
                        kickVector[0].Set(-2, 0);
                        kickVector[1].Set(1, 0);
                        kickVector[2].Set(-2, 1);
                        kickVector[3].Set(1, -2);
                    }
                    break;

                default:
                    Debug.Log("unknown rotation form");
                    break;
            }
        }
        else // I를 제외한 블록
        {
            switch(prevRotation)
            {
                case TetBlockRotation.UP: //0
                    if(cw_ccw==TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(-1, 0);
                        kickVector[1].Set(-1, -1);
                        kickVector[2].Set(0, 2);
                        kickVector[3].Set(-1, 2);
                    }
                    else // counterclockwise
                    {
                        kickVector[0].Set(1, 0);
                        kickVector[1].Set(1, -1);
                        kickVector[2].Set(0, 2);
                        kickVector[3].Set(1, 2);
                    }
                    break;

                case TetBlockRotation.RIGHT: //1
                    if (cw_ccw == TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(1, 0);
                        kickVector[1].Set(1, 1);
                        kickVector[2].Set(0, -2);
                        kickVector[3].Set(1, -2);
                    }
                    else
                    {
                        kickVector[0].Set(1, 0);
                        kickVector[1].Set(1, 1);
                        kickVector[2].Set(0, -2);
                        kickVector[3].Set(1, -2);
                    }
                    break;

                case TetBlockRotation.DOWN: //2
                    if (cw_ccw == TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(1, 0);
                        kickVector[1].Set(1, -1);
                        kickVector[2].Set(0, 2);
                        kickVector[3].Set(1, 2);
                    }
                    else
                    {
                        kickVector[0].Set(-1, 0);
                        kickVector[1].Set(-1, -1);
                        kickVector[2].Set(0, 2);
                        kickVector[3].Set(-1, 2);
                    }
                    break;

                case TetBlockRotation.LEFT: //3
                    if (cw_ccw == TetBlockRotation.CLOCKWISE)
                    {
                        kickVector[0].Set(-1, 0);
                        kickVector[1].Set(-1, 1);
                        kickVector[2].Set(0, -2);
                        kickVector[3].Set(-1, -2);
                    }
                    else
                    {
                        kickVector[0].Set(-1, 0);
                        kickVector[1].Set(-1, 1);
                        kickVector[2].Set(0, 2);
                        kickVector[3].Set(-1, -2);
                    }
                    break;

                default:
                    Debug.Log("unknown rotation form");
                    break;
            }
        }

        return kickVector;
    }



}
