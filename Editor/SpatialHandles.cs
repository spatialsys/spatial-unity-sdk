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

        /// <summary>
        /// A flat gizmo that controls radius and a direction. 
        /// </summary>
        public static void RadiusAndDirectionHandle(Vector3 center, ref float radiusTarget, ref Vector3 directionTarget)
        {
            directionTarget = new Vector3(directionTarget.x, 0f, directionTarget.z).normalized;
            DrawRadiusAndDirectionHandle(center, radiusTarget, directionTarget, .2f);
            FloatAndFlatDirectionSlider(ref radiusTarget, ref directionTarget, center, .4f);
        }

        private static void DrawRadiusAndDirectionHandle(Vector3 center, float radius, Vector3 direction, float nubSize)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawRadiusAndDirectionElement(center, radius, direction, handleWhite, handleBlack, nubSize);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawRadiusAndDirectionElement(center, radius, direction, handleWhiteFade, handleBlackFade, nubSize);
        }

        private static void DrawRadiusAndDirectionElement(Vector3 center, float radius, Vector3 direction, Color foregroundColor, Color backgroundColor, float nubSize)
        {
            float largeNubSize = nubSize;
            float smallNubSize = nubSize - .05f;
            Vector3 nubPosition = center + direction * radius;

            Handles.color = backgroundColor;
            Handles.DrawWireDisc(center, Vector3.up, radius, 10f);
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * largeNubSize);
            Handles.ArrowHandleCap(-1, nubPosition, Quaternion.LookRotation(direction), HandleUtility.GetHandleSize(nubPosition) * largeNubSize * 4f, EventType.Repaint);

            Handles.color = foregroundColor;
            Handles.DrawWireDisc(center, Vector3.up, radius, 2f);
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * smallNubSize);
        }

        public static void RadiusHandle(Vector3 center, ref float target)
        {
            FloatHandleSlider(ref target, center, Vector3.right, .3f);
            FloatHandleSlider(ref target, center, Vector3.left, .3f);
            FloatHandleSlider(ref target, center, Vector3.forward, .3f);
            FloatHandleSlider(ref target, center, Vector3.back, .3f);
            DrawRadiusHandle(center, target, .15f);
        }

        private static void DrawRadiusHandle(Vector3 center, float radius, float nubSize)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawRadiusElement(center, radius, handleWhite, handleBlack, nubSize);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawRadiusElement(center, radius, handleWhiteFade, handleBlackFade, nubSize);
        }

        private static void DrawRadiusElement(Vector3 center, float radius, Color foregroundColor, Color backgroundColor, float nubSize)
        {
            float largeNubSize = nubSize;
            float smallNubSize = nubSize - .05f;

            Vector3 nubPosition;
            Handles.color = backgroundColor;
            Handles.DrawWireDisc(center, Vector3.up, radius, 10f);

            nubPosition = center + Vector3.forward * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * largeNubSize);
            nubPosition = center + Vector3.back * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * largeNubSize);
            nubPosition = center + Vector3.left * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * largeNubSize);
            nubPosition = center + Vector3.right * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * largeNubSize);

            Handles.color = foregroundColor;
            Handles.DrawWireDisc(center, Vector3.up, radius, 2f);

            nubPosition = center + Vector3.forward * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * smallNubSize);
            nubPosition = center + Vector3.back * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * smallNubSize);
            nubPosition = center + Vector3.left * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * smallNubSize);
            nubPosition = center + Vector3.right * radius;
            Handles.DrawSolidDisc(nubPosition, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nubPosition)).direction, HandleUtility.GetHandleSize(nubPosition) * smallNubSize);
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
            Vector3 startPos = origin + direction.normalized * target;
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.Slider(startPos, direction, HandleUtility.GetHandleSize(startPos) * handleSize, Handles.SphereHandleCap, 0f);
            if (EditorGUI.EndChangeCheck())
            {
                target = Vector3.Distance(origin, newPos);
            }
        }

        private static void FloatAndFlatDirectionSlider(ref float target, ref Vector3 direction, Vector3 origin, float handleSize)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = handleClear;
            Vector3 startPos = origin + direction.normalized * target;
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.Slider2D(startPos, Vector3.up, Vector3.forward, Vector3.right, HandleUtility.GetHandleSize(startPos) * handleSize, Handles.SphereHandleCap, Vector2.zero * .01f);
            newPos.y = startPos.y;
            if (EditorGUI.EndChangeCheck())
            {
                target = Vector3.Distance(origin, newPos);
                direction = (newPos - origin).normalized;
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
            Vector3 newPos = Handles.FreeMoveHandle(target.position, Quaternion.identity, HandleUtility.GetHandleSize(target.position) * handleSize, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change position");
                target.position = newPos;
            }
        }

        public static void EvenlyScaleableRectangleHandle(Vector3 position, ref Vector2 size, Matrix4x4 matrix)
        {
            DrawEvenlyScaleableRectangleHandle(position, size, matrix);

            Vector3 right = matrix.MultiplyVector(Vector3.right).normalized;
            Vector3 up = matrix.MultiplyVector(Vector3.up).normalized;

            Vector3 ne = position + (right * (size.x * .5f)) + (up * (size.y * .5f));
            Vector3 se = position + (right * (size.x * .5f)) + (-up * (size.y * .5f));
            Vector3 sw = position + (-right * (size.x * .5f)) + (-up * (size.y * .5f));
            Vector3 nw = position + (-right * (size.x * .5f)) + (up * (size.y * .5f));

            float distance = Vector3.Distance(position, ne);
            float startDistance = distance;
            FloatHandleSlider(ref distance, position, ne - position, .2f);
            FloatHandleSlider(ref distance, position, se - position, .2f);
            FloatHandleSlider(ref distance, position, sw - position, .2f);
            FloatHandleSlider(ref distance, position, nw - position, .2f);

            size = size * Mathf.LerpUnclamped(0f, 1f, distance / startDistance);
        }

        private static void DrawEvenlyScaleableRectangleHandle(Vector3 position, Vector2 size, Matrix4x4 matrix)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawEvenlyScaleableRectangleElement(position, size, matrix, handleWhite, handleBlack);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawEvenlyScaleableRectangleElement(position, size, matrix, handleWhiteFade, handleBlackFade);
        }

        //Evenly scalable has the handles on the corners. If we need non-even scaling do that with handles on the edges.
        private static void DrawEvenlyScaleableRectangleElement(Vector3 position, Vector2 size, Matrix4x4 matrix, Color foregroundColor, Color backgroundColor)
        {
            Vector3 pos = matrix.MultiplyPoint3x4(position);
            Vector3 right = matrix.MultiplyVector(Vector3.right).normalized;
            Vector3 up = matrix.MultiplyVector(Vector3.up).normalized;

            Vector3 ne = position + (right * (size.x * .5f)) + (up * (size.y * .5f));
            Vector3 se = position + (right * (size.x * .5f)) + (-up * (size.y * .5f));
            Vector3 sw = position + (-right * (size.x * .5f)) + (-up * (size.y * .5f));
            Vector3 nw = position + (-right * (size.x * .5f)) + (up * (size.y * .5f));

            float backgroundThickness = 10f;
            float foregroundThickness = 2f;

            Handles.color = backgroundColor;
            Handles.DrawLine(ne, se, backgroundThickness);
            Handles.DrawLine(se, sw, backgroundThickness);
            Handles.DrawLine(sw, nw, backgroundThickness);
            Handles.DrawLine(nw, ne, backgroundThickness);

            Handles.DrawSolidDisc(ne, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(ne)).direction, HandleUtility.GetHandleSize(ne) * (.15f));
            Handles.DrawSolidDisc(se, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(se)).direction, HandleUtility.GetHandleSize(se) * (.15f));
            Handles.DrawSolidDisc(sw, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(sw)).direction, HandleUtility.GetHandleSize(sw) * (.15f));
            Handles.DrawSolidDisc(nw, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nw)).direction, HandleUtility.GetHandleSize(nw) * (.15f));

            Handles.color = foregroundColor;
            Handles.DrawLine(ne, se, foregroundThickness);
            Handles.DrawLine(se, sw, foregroundThickness);
            Handles.DrawLine(sw, nw, foregroundThickness);
            Handles.DrawLine(nw, ne, foregroundThickness);

            Handles.DrawSolidDisc(ne, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(ne)).direction, HandleUtility.GetHandleSize(ne) * (.1f));
            Handles.DrawSolidDisc(se, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(se)).direction, HandleUtility.GetHandleSize(se) * (.1f));
            Handles.DrawSolidDisc(sw, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(sw)).direction, HandleUtility.GetHandleSize(sw) * (.1f));
            Handles.DrawSolidDisc(nw, HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(nw)).direction, HandleUtility.GetHandleSize(nw) * (.1f));
        }
    }
}
