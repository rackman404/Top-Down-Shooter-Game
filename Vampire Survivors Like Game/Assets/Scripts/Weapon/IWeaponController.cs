using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponController
{
    public void Fire(Vector3 targetPos, GameObject targetObj);

    public IWeaponController Init(CharacterEntity parentE);

}
