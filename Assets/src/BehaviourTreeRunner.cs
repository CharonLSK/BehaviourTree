using System;
using TreeEditor;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    private void Start()
    {
        tree = tree.Clone();
    }

    private void Update()
    {
        if (tree != null)
        {
            tree.Update();
        }
    }
}
