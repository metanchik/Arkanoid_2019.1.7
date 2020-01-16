using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class BonusController : MonoBehaviour {
    private bool isUnfrozen = false;
    private Bonus bonus;
    public float speed = 1;
    private LevelController level;
    private float platformCollectY;
    private float unfrozenRadius = 0.2f;
    private float frozenRadius = 0.05f;

    private void Awake() {
        level = FindObjectOfType<LevelController>();
        platformCollectY = level.platform.transform.position.y + level.platform.height / 2;
        transform.localScale = new Vector3(frozenRadius * 2, frozenRadius * 2, 1f);
    }

    public void SetBonus(Bonus bonus) {
        this.bonus = bonus;
        GetComponent<SpriteRenderer>().sprite = bonus.icon;
    }

    public void Unfreeze() {
        isUnfrozen = true;
        transform.localScale = new Vector3(unfrozenRadius * 2, unfrozenRadius * 2, 1f);
        level.unfrozenBonuses.Add(this);
    }

    private void Update() {
        if (isUnfrozen) {
            transform.position = Vector3.Lerp(transform.position,  transform.position + speed * Vector3.down, Time.deltaTime);
            if (transform.position.y <= platformCollectY) {
                var platformSides = level.platform.GetSidesForCheckingIntersect(unfrozenRadius);
                if (platformSides[BlockSide.Left][0].x <= transform.position.x &&
                    transform.position.x <= platformSides[BlockSide.Right][0].x) {
                    bonus.action.Invoke();
                    Destroy();
                }
            }

            if (transform.position.y <= level.field.bottom + unfrozenRadius) {
                Destroy();
            }
        }
    }

    private void Destroy() {
        level.unfrozenBonuses.Remove(this);
        Destroy(gameObject);
    }
}
