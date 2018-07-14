using UnityEngine;
using System.Collections;

public class Person
{
    private Transform transform;
    private GameObject go;
    private Card card;

    public Person(Transform transform) {
        this.transform = transform;
    }
	
    public void Spawn() {
        go = new GameObject("Person");
        go.transform.parent = transform;
        card = go.AddComponent<Card>();
    }
}
