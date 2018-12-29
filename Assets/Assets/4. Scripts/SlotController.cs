using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TetrisSfx
{
    public AudioClip moveBlock;
    public AudioClip rotateBlock;
    public AudioClip spaceBlock;
    public AudioClip clearRows;
}

public class SlotController : MonoBehaviour {

    public TetrisSfx tetrisSfx;
    private AudioSource mAudioSource;

    public TetSlot[] slotList = new TetSlot[200];  // 슬롯 배열
    public TetBlock tetBlock=null; //

    private int[] shadowindex = new int[4];
    private bool bdonremoveshadow = false;
    private float delay;
    public int[] countBlocks = new int[20];
    private Vector2[] kickPosition;

    private List<TetBlock> blockOrder = new List<TetBlock>();

    public PanelNextSlot[] listNextSlots = new PanelNextSlot[6];
    public GameObject gameOverImg;
    public bool isGameEnd = false;

    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        int[] startingArray = GetRandomBlockShapeArray();
        for (int i = 0; i < 7; i++)
        {
            blockOrder.Add(new TetBlock((TetBlockShape)startingArray[i]));
        }
        startingArray = GetRandomBlockShapeArray();
        for (int i = 0; i < 7; i++)
        {
            blockOrder.Add(new TetBlock((TetBlockShape)startingArray[i]));
        }
        tetBlock = blockOrder[0];
        SetPanelNext();

    }

    private void Update()
    {
        if (isGameEnd) return;

        ChangeSlotStatusToBlack(tetBlock);

        //떨어질곳을 예상할 수 있게 해주는 block을 그리는 알고리즘
        {
            if (!bdonremoveshadow)
            {
                foreach (var rsindex in shadowindex)
                {
                    if((rsindex>=0)&&(rsindex<200))
                        slotList[rsindex].UpdateColor(Color.black);
                }
            }
            else
                bdonremoveshadow=false;
            

            shadowindex = new int[]{ tetBlock.index[0], tetBlock.index[1], tetBlock.index[2], tetBlock.index[3] };
            bool isbreak=false;
            while (tetBlock != null)
            {
                foreach(var sindex in shadowindex)
                {
                    if(((sindex+10)>199)||(slotList[sindex+10].isFilled))
                    {
                        isbreak = true;
                        break;
                    }
                }
                if(isbreak)
                {
                    foreach(var sindex in shadowindex)
                    {
                        if((sindex>0)&&(sindex<200))
                            slotList[sindex].UpdateColor(Color.gray);
                    }
                    break;
                }
                else
                {
                    shadowindex[0]+=10;
                    shadowindex[1] += 10;
                    shadowindex[2] += 10;
                    shadowindex[3] += 10;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            List<int> forCheckTetris = new List<int>();
            foreach(var sindex in shadowindex)
            {
                if((slotList[4].isFilled)||(sindex < 0))
                {
                    isGameEnd = true;
                    gameOverImg.SetActive(true);
                    return;
                }

                slotList[sindex].UpdateToFilled();
                slotList[sindex].UpdateColor(tetBlock.color);
                int row = (int)sindex / 10;
                countBlocks[row]++;
                if(countBlocks[row]>=10)
                {
                    forCheckTetris.Add(row);
                }       
            }
            CheckTetris(forCheckTetris);
            bdonremoveshadow = true;
            blockOrder.RemoveAt(0);
            tetBlock = blockOrder[0];
            SetPanelNext();
            if (blockOrder.Count < 8)
            {
                int[] orderArray = GetRandomBlockShapeArray();
                for (int i = 0; i < 7; i++)
                {
                    blockOrder.Add(new TetBlock((TetBlockShape)orderArray[i]));
                }
            }
            mAudioSource.PlayOneShot(tetrisSfx.spaceBlock, 1.0f);
            return;
        } // 스페이스 키

        if ((Input.GetKey(KeyCode.S)&&(delay>0.1f))||(delay > 0.5f)) // 아래로 내려감, 더 이상 내려갈 수 없으면 고정시키고 다음 블럭으로 바꿈
        {
            foreach (var bindex in tetBlock.index)
            {
                if ((bindex >= 0) && (bindex < 200))
                {
                    if (((bindex + 10) > 199) || slotList[bindex + 10].isFilled)
                    {
                        List<int> forCheckTetris = new List<int>();
                        foreach (var cindex in tetBlock.index)
                        {
                            if (slotList[4].isFilled || (cindex < 0))
                            {
                                isGameEnd = true;
                                gameOverImg.SetActive(true);
                                return;
                            } // game over조건 채크

                            slotList[cindex].UpdateToFilled();
                            int row = (int)cindex / 10;
                            countBlocks[row]++;
                            if (countBlocks[row] >= 10)
                            {
                                forCheckTetris.Add(row);
                            }
                        }
                        ChangeSlotStatus(tetBlock);
                        CheckTetris(forCheckTetris);
                        bdonremoveshadow = true;
                        blockOrder.RemoveAt(0);
                        tetBlock = blockOrder[0];
                        SetPanelNext();
                        if (blockOrder.Count < 8)
                        {
                            int[] orderArray = GetRandomBlockShapeArray();
                            for (int i = 0; i < 7; i++)
                            {
                                blockOrder.Add(new TetBlock((TetBlockShape)orderArray[i]));
                            }
                        }
                        return;
                    }
                }
            }
            tetBlock.GoDown();
            delay = 0.0f;
        }
        else
        {
            delay += Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.D)&&delay>0.15f)
        {
            delay = 0;
            foreach (var index in tetBlock.index)
            {
                if (((int)(index%10) <=8)&&(index>0))
                {
                    if (slotList[index + 1].isFilled)
                    {
                        ChangeSlotStatus(tetBlock);
                        return;
                    }
                }
            }
            tetBlock.GoRight();
            mAudioSource.PlayOneShot(tetrisSfx.moveBlock, 1.0f);
        }
        else if (Input.GetKey(KeyCode.A)&&delay>0.15f)
        {
            delay = 0;
            foreach (var index in tetBlock.index)
            {
                if ((int)(index%10) > 0)
                {
                    if (slotList[index - 1].isFilled)
                    {
                        ChangeSlotStatus(tetBlock);
                        return;
                    }
                }
            }
            tetBlock.GoLeft();
            mAudioSource.PlayOneShot(tetrisSfx.moveBlock, 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            delay = 0;

            TetBlockRotation prevRot = (TetBlockRotation)tetBlock.currRotation;

            bool TempFill = false;
            tetBlock.RotateClockwise();
            mAudioSource.PlayOneShot(tetrisSfx.rotateBlock, 1.0f);

            foreach (var tindex in tetBlock.index)
            {
                if (!IsInBoundary(tindex)||slotList[tindex].isFilled)
                {
                    TempFill = true;
                }
            }
            if (TempFill)
            {
                kickPosition = TetBlockForm.GetRotationKick(tetBlock.shape, prevRot, TetBlockRotation.CLOCKWISE);
                for (int i = 0; i < 4; i++)
                {
                    TempFill = false;
                    int kickIndex = (int)(kickPosition[i].x + 10 * kickPosition[i].y);
                    foreach (var pos in tetBlock.index)
                    {
                        if (!IsInBoundary(pos + kickIndex) 
                            ||(((pos%10+kickIndex%10)>9)&&(pos % 10 + kickIndex % 10) <0)
                            || slotList[pos + kickIndex].isFilled)
                        {
                            TempFill = true;
                            break;
                        }                            
                    }
                    if (!TempFill)
                    {
                        tetBlock.slotPosition += kickPosition[i];
                        tetBlock.UpdateIndex();
                        break;
                    }
                }
                if (TempFill)
                {
                    tetBlock.RotateCounterClockwise();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            delay = 0;
            
            TetBlockRotation prevRot = (TetBlockRotation)tetBlock.currRotation;

            bool TempFill = false;
            tetBlock.RotateCounterClockwise();
            mAudioSource.PlayOneShot(tetrisSfx.rotateBlock, 1.0f);

            foreach (var tindex in tetBlock.index)
            {
                if (!IsInBoundary(tindex)||slotList[tindex].isFilled)
                {
                    TempFill = true;
                }
            }
            if (TempFill)
            {
                kickPosition = TetBlockForm.GetRotationKick(tetBlock.shape, prevRot, TetBlockRotation.COUNTERCLOCKWISE);
                for (int i = 0; i < 4; i++)
                {
                    TempFill = false;
                    int kickIndex = (int)(kickPosition[i].x + 10 * kickPosition[i].y);
                    foreach (var pos in tetBlock.index)
                    {
                        if (!IsInBoundary(pos + kickIndex)
                            || (((pos % 10 + kickIndex % 10) > 9) && (pos % 10 + kickIndex % 10) < 0)
                            || slotList[pos + kickIndex].isFilled)
                        {
                            TempFill = true;
                            break;
                        }                            
                    }
                    if (!TempFill)
                    {
                        tetBlock.slotPosition += kickPosition[i];
                        tetBlock.UpdateIndex();
                        break;
                    }
                }
                if (TempFill)
                {
                    tetBlock.RotateClockwise();
                }
            }
        }

        ChangeSlotStatus(tetBlock);
    }



    private int[] GetRandomBlockShapeArray()
    {
        int[] randnum = new int[] { 0, 1, 2, 3, 4, 5, 6 };
        for (int i = 6; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            int temp = randnum[i];
            randnum[i] = randnum[rand];
            randnum[rand] = temp;
        }
        return randnum;
    }

    private void SetPanelNext()
    {
        for(int i=0;i<6;i++)
        {
            listNextSlots[i].UpdateTetBlock(blockOrder[i+1]);
        }
    }

    private void CheckTetris(List<int> rows)
    {
        //for문 사용을 위한 변수        
        rows.Sort();
        //rows[0] 은 가득 찬 행 중에 가장 위의 행이다
        for (int i=0;i<rows.Count;i++)
        {
            for(int j = 0; j<10;j++) 
            {
                slotList[10 * rows[i] + j].UpdateToBlack();
            }
            for(int j= rows[i];j>0;j--)
            {
                if (countBlocks[j] == 0)
                    break;
                for(int k =0;k<10;k++)
                {
                    slotList[10 * j + k].isFilled = slotList[10 * j + k-10].isFilled;
                    slotList[10 * j + k].UpdateColor(slotList[10 * j + k-10].GetComponent<RawImage>().color);
                }
                countBlocks[j] = countBlocks[j-1];
            }
            mAudioSource.PlayOneShot(tetrisSfx.clearRows, 1.0f);            
        }
        

    }

    public void ChangeSlotStatus(TetBlock block)
    {
        foreach (var index in block.index)
        {
            if (IsInBoundary(index))
            {
                slotList[index].UpdateColor(block.color);
            }
        }
    }

    public void ChangeSlotStatusToBlack(TetBlock block)
    {
        foreach(var index in block.index)
        {
            if((index>=0)&&(index<200))
           {
                if(!slotList[index].isFilled)
                {
                    slotList[index].UpdateToBlack();
                }                
            }
        }
    }

    private void OnDisable()
    {
        Debug.Log("Game Ended");
    }

    private bool IsInBoundary(int i)
    {
        if (
            (i >= 0) &&
            (i < 200)
            )
            return true;
        else
            return false;
    }
}

