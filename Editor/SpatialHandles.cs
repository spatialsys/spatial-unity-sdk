using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace SpatialSys.UnitySDK.Editor
{
    public class SpatialHandles
    {
        // Naming:
        // Draw__Element = Draw a handle element once (black & white), private
        // Draw__Handle = Draw 2 elements with different zTest's, sometimes public
        // __Handle = Draw the handle w/ funcitonality, public

        //for some handles an alpha of 1 looks like alpha .7~
        //using colors with alpha 2 generally gets around this.
        public static Color handleBlack = new Color(0, 0, 0, 2);
        public static Color handleWhite = new Color(1, 1, 1, 2);
        //use lower alpha when handles fail ztest
        public static Color handleBlackFade = new Color(0, 0, 0, .5f);
        public static Color handleWhiteFade = new Color(1, 1, 1, .5f);
        //i find most of the handle CAPS ugly so i hide them.
        public static Color handleClear = new Color(0, 0, 0, 0);

        public static void RadiusHandle(Vector3 center, ref float target)
        {
            //TODO: maybe expose the size of the "nub", would add context to the .31 below...
            DrawRadiusHandle(center, target);
            FloatHandleSlider(ref target, center, Vector3.right, .31f);//radius of white nub * 2 + .1
        }

        private static void DrawRadiusHandle(Vector3 center, float radius)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawRadiusElement(center, radius, handleWhite, handleBlack);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawRadiusElement(center, radius, handleWhiteFade, handleBlackFade);
        }

        private static void DrawRadiusElement(Vector3 center, float radius, Color foregroundColor, Color backgroundColor)
        {
            Vector3 handlePos = center + Vector3.right * radius;
            Handles.color = backgroundColor;
            Handles.DrawWireDisc(center, Vector3.up, radius, 10f);
            Handles.DrawSolidDisc(handlePos, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(handlePos)).direction, HandleUtility.GetHandleSize(handlePos) * .2f);
            Handles.color = foregroundColor;
            Handles.DrawSolidDisc(handlePos, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(handlePos)).direction, HandleUtility.GetHandleSize(handlePos) * .15f);
            Handles.DrawWireDisc(center, Vector3.up, radius, 2f);
        }

        public static void TargetTransformHandle(Transform parent, ref Transform target)
        {
            if (parent == null || target == null)
            {
                return;
            }

            DrawTransformTargetHandle(parent, target);

            //transform controls
            float axisOffset = HandleUtility.GetHandleSize(target.position) * .75f;

            DrawSolidCircleHandle(target.position + Vector3.right * axisOffset, .075f, Handles.xAxisColor);
            DrawSolidCircleHandle(target.position + Vector3.up * axisOffset, .075f, Handles.yAxisColor);
            DrawSolidCircleHandle(target.position + Vector3.forward * axisOffset, .075f, Handles.zAxisColor);

            PositionHandleSlider(ref target, Vector3.right, axisOffset, .15f);
            PositionHandleSlider(ref target, Vector3.up, axisOffset, .15f);
            PositionHandleSlider(ref target, Vector3.forward, axisOffset, .15f);

            PositionFreeMoveHandle(ref target, .31f);
        }

        private static void DrawTransformTargetHandle(Transform parent, Transform target)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            SpatialHandles.DrawTransformTargetElement(parent, target, handleWhite, handleBlack);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            SpatialHandles.DrawTransformTargetElement(parent, target, handleWhiteFade, handleBlackFade);
        }

        private static void DrawTransformTargetElement(Transform parent, Transform target, Color foregroundColor, Color backgroundColor)
        {
            Handles.color = backgroundColor;
            Handles.DrawSolidDisc(parent.position, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(parent.position)).direction, HandleUtility.GetHandleSize(parent.position) * .1f);
            Handles.DrawSolidDisc(target.position, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(target.position)).direction, HandleUtility.GetHandleSize(target.position) * .2f);
            Handles.DrawLine(parent.position, target.position, 10f);

            Handles.color = foregroundColor;
            Handles.DrawSolidDisc(parent.position, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(parent.position)).direction, HandleUtility.GetHandleSize(parent.position) * .05f);
            Handles.DrawSolidDisc(target.position, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(target.position)).direction, HandleUtility.GetHandleSize(target.position) * .15f);
            Handles.DrawLine(parent.position, target.position, 2f);
        }

        public static void UnityEventHandle(Transform parent, UnityEvent unityEvent)
        {
            if (unityEvent == null || parent == null)
            {
                return;
            }

            float minimumConnectionDistance = .75f;

            //all black first, then white.

            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                UnityEngine.Object o = unityEvent.GetPersistentTarget(i);
                Transform t = null;
                if (o is GameObject g)
                {
                    t = g.transform;
                }
                else if (o is Component c)
                {
                    t = c.transform;
                }
                else
                {
                    continue;
                }

                if (Vector3.Distance(parent.position, t.position) < minimumConnectionDistance)
                {
                    continue;
                }

                DrawConnectionHandle(parent.position, t.position);
            }
        }

        /// <summary>
        /// Draw a ring below the given position. Ring is placed on first colider under. If there is
        /// no collider no ring will be drawn.
        /// </summary>
        public static void DrawGroundPoint(Vector3 position, float radius)
        {
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit))
            {
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                DrawGroundPointElement(hit.point, radius, handleWhite, handleBlack);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                DrawGroundPointElement(hit.point, radius, handleWhiteFade, handleBlackFade);
            }
        }

        private static void DrawGroundPointElement(Vector3 position, float radius, Color foregroundColor, Color backgroundColor)
        {
            Handles.color = backgroundColor;
            Handles.DrawWireDisc(position, Vector3.up, radius, 4f);
            Handles.color = foregroundColor;
            Handles.DrawWireDisc(position, Vector3.up, radius * .5f, 2f);
        }

        /// <summary>
        /// Draw a line between start and end position. Ends have a pretty cap
        /// </summary>
        public static void DrawConnectionHandle(Vector3 start, Vector3 end)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawConnectionElement(start, end, handleBlack, handleWhite);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawConnectionElement(start, end, handleBlackFade, handleWhiteFade);
        }

        private static void DrawConnectionElement(Vector3 start, Vector3 end, Color backgroundColor, Color foregroundColor)
        {
            Handles.color = backgroundColor;
            Handles.DrawSolidDisc(start, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(start)).direction, HandleUtility.GetHandleSize(start) * .1f);
            Handles.DrawSolidDisc(end, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(end)).direction, HandleUtility.GetHandleSize(end) * .1f);
            Handles.DrawLine(start, end, 10f);

            Handles.color = foregroundColor;
            Handles.DrawSolidDisc(start, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(start)).direction, HandleUtility.GetHandleSize(start) * .05f);
            Handles.DrawSolidDisc(end, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(end)).direction, HandleUtility.GetHandleSize(end) * .05f);
            Handles.DrawLine(start, end, 2f);
        }

        /// <summary>
        /// Draws a black and white circle at the position.
        /// </summary>
        public static void DrawSolidCircleHandle(Vector3 position, float radius)
        {
            DrawSolidCircleHandle(position, radius, handleWhite);
        }
        public static void DrawSolidCircleHandle(Vector3 position, float radius, Color color)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawSolidCircleElement(position, radius, color, handleBlack);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawSolidCircleElement(position, radius, new Color(color.r, color.g, color.b, color.a * .5f), handleBlackFade);
        }

        private static void DrawSolidCircleElement(Vector3 position, float radius, Color foregroundColor, Color backgroundColor)
        {
            Handles.color = backgroundColor;
            Handles.DrawSolidDisc(position, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(position)).direction, HandleUtility.GetHandleSize(position) * (radius + .05f));
            Handles.color = foregroundColor;
            Handles.DrawSolidDisc(position, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(position)).direction, HandleUtility.GetHandleSize(position) * radius);
        }

        private static void FloatHandleSlider(ref float target, Vector3 origin, Vector3 direction, float handleSize)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = handleClear;
            Vector3 startPos = origin + direction * target;
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.Slider(startPos, direction, HandleUtility.GetHandleSize(startPos) * handleSize, Handles.SphereHandleCap, 0f);
            if (EditorGUI.EndChangeCheck())
            {
                target = Vector3.Distance(origin, newPos);
            }
        }

        private static void PositionHandleSlider(ref Transform target, Vector3 direction, float handleOffset, float handleSize = .15f)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = handleClear;
            EditorGUI.BeginChangeCheck();
            Vector3 startPos = target.position + direction * handleOffset;
            Vector3 newPos = Handles.Slider(startPos, direction, HandleUtility.GetHandleSize(startPos) * handleSize, Handles.SphereHandleCap, 0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change position");
                target.position = target.position + (newPos - startPos);
            }
        }

        private static void PositionFreeMoveHandle(ref Transform target, float handleSize)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = handleClear;
            EditorGUI.BeginChangeCheck();
            Vector4 newPos = Handles.FreeMoveHandle(target.position, Quaternion.identity, HandleUtility.GetHandleSize(target.position) * handleSize, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change position");
                target.position = newPos;
            }
        }
    }
}
