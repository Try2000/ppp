using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCatcher : MonoBehaviour
{
    [SerializeField] GrabbingStickRaycast grabbingStick;
    [SerializeField] Transform handParent;
    private void Awake()
    {
        grabbingStick.onStickHit += OnStickHit;
    }
    public void OnStickHit(GameObject air,Vector3 offset)
    {
        if(air.TryGetComponent<EatableAir>(out EatableAir eatableAir))
        {
            if (eatableAir._airState != EatableAir.AirState.Idle) return; 
            eatableAir.airParent = handParent;
            eatableAir.offset = offset;
            eatableAir.transform.position = handParent.position + offset;
            eatableAir.ChangeState(EatableAir.AirState.Moving);
        }
    }
}
