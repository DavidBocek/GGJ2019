using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CommonEnemyLogic : MonoBehaviour
{
    public GameObject enemyVisualsObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSpawnFromSpawner()
    {
        Transform modelTrans = enemyVisualsObject.transform;
        float targetScale = modelTrans.localScale.x; // please don't make this non-uniform

        modelTrans.localScale = Vector3.zero;
        Sequence spawnAnimSequence = DOTween.Sequence();
        spawnAnimSequence.Append( modelTrans.DOScale( targetScale + 0.4f, 0.2f ).SetEase( Ease.OutQuad ) );
        spawnAnimSequence.Append( modelTrans.DOScale( targetScale, 0.2f ).SetEase( Ease.OutQuad ) );
    }
}
