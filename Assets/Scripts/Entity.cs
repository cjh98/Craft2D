using UnityEngine;

[RequireComponent(typeof(EntityMovement))]
public class Entity : MonoBehaviour
{
    public float width, height;
    public float startHealth;
    float health;

    public bool isDead = false;

    void Start()
    {
        health = startHealth;
    }

    void Update()
    {
        
    }
}
