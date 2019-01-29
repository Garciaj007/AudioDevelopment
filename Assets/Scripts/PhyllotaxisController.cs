using UnityEngine;

public class PhyllotaxisController : MonoBehaviour {

    public AudioVisualiser audioVisualiser;
    public float degree, scale;
    public int startNumber;
    public int stepSize;
    public int maxIteration;
    public int audioBand;

    [Header("Lerp")]
    public bool useLerping;
    public bool repeat, invert;
    public Vector2 lerpPosSpeedMinMax;
    public AnimationCurve lerpPosAnimCurve;

    [Header("Audio Scaling")]
    public bool useScaleAnimation;
    public bool useScaleCurve;
    public Vector2 scaleAnimMinMax;
    public AnimationCurve scaleAnimCurve;
    public float scaleAnimSpeed;

    [Header("Animate Shape")]
    public bool animateShape;
    [Range(-0.01f, 0.01f)]
    public float animShapeSpeed; 

    private Vector3 startPos, endPos;
    private Vector2 phyllotaxisPos;
    private int number, currentIteration;
    private bool isLerping, forward;
    private float lerpPosTimer, lerpPosSpeed, scaleTimer, currentScale;
    

    void SetLerpPositions()
    {
        phyllotaxisPos = CalculatePhyllotaxis(degree, currentScale, number);
        startPos = transform.localPosition;
        endPos = new Vector3(phyllotaxisPos.x, phyllotaxisPos.y, 0);
    }

    private Vector2 CalculatePhyllotaxis(float degree, float scale, int count)
    {
        double angle = count * (degree * Mathf.Deg2Rad);
        float r = scale * Mathf.Sqrt(count);
        return new Vector2(r * (float)System.Math.Cos(angle),r * (float)System.Math.Sin(angle));
    }

    private void Awake()
    {
        forward = true;
        currentScale = scale;
        number = startNumber;
        transform.localPosition = CalculatePhyllotaxis(degree, currentScale, number);

        if (useLerping)
        {
            isLerping = true;
            SetLerpPositions();
        }
    }

    private void Update()
    {
        if (animateShape)
        {
            degree += audioVisualiser.amplitudeBuffer * animShapeSpeed;
        }

        if (useScaleAnimation)
        {
            if (useScaleCurve)
            {
                scaleTimer += (scaleAnimSpeed * audioVisualiser.audioBand[audioBand]) * Time.deltaTime;
                if(scaleTimer >= 1)
                {
                    scaleTimer -= 1;
                }
                currentScale = Mathf.Lerp(scaleAnimMinMax.x, scaleAnimMinMax.y, scaleAnimCurve.Evaluate(scaleTimer));
            }
            else
            {
                currentScale = Mathf.Lerp(scaleAnimMinMax.x, scaleAnimMinMax.y, audioVisualiser.audioBand[audioBand]);
            }
        }

        if (useLerping)
        {
            if (isLerping)
            {
                lerpPosSpeed = Mathf.Lerp(lerpPosSpeedMinMax.x, lerpPosSpeedMinMax.y, lerpPosAnimCurve.Evaluate(audioVisualiser.audioBand[audioBand]));
                lerpPosTimer += Time.deltaTime * lerpPosSpeed;
                transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(lerpPosSpeed));
                if(lerpPosTimer >= 1)
                {
                    lerpPosTimer -= 1;

                    if (forward)
                    {
                        number += stepSize;
                        currentIteration++;
                    } else
                    {
                        number -= stepSize;
                        currentIteration--;
                    }

                    if (currentIteration > 0 && currentIteration < maxIteration)
                    {
                        SetLerpPositions();
                    }
                    else
                    {
                        if (repeat)
                        {
                            if (invert)
                            {
                                forward = !forward;
                                SetLerpPositions();
                            }
                            else
                            {
                                number = startNumber;
                                currentIteration = 0;
                                SetLerpPositions();
                            }
                        }
                        else
                        {
                            isLerping = false;
                        }
                    } 
                        
                }
            }
        }
        else
        {
            transform.localPosition = CalculatePhyllotaxis(degree, currentScale, number);
            number += stepSize;
            currentIteration++;
        }
    }
}
