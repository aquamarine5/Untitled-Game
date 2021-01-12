using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Strata
{
    public class PathVisualizer : MonoBehaviour
    {
        public bool visualizePath = true;

        public Vector3 nodePosition;
        public Vector3 nodeDirection;
        public string debugString;
        public RoomChain.Direction pathDirection;

        private Vector3 nodeTextOffset = new Vector3(-3f, -3f, 0);

        private void OnDrawGizmos()
        {
            if (visualizePath)
            {
                DrawArrow.ForGizmo(nodePosition, nodeDirection, Color.yellow, 2.5f, 20);
#if UNITY_EDITOR
                Handles.Label((nodePosition + nodeTextOffset), debugString);
#endif
            }
        }

        public void SetupNode(Vector3 pos, Vector3 dir, string debugTxt)
        {
            nodePosition = pos;
            nodeDirection = dir;
            debugString = debugTxt;
        }
    }

}
