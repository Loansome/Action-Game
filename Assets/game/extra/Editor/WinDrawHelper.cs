using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class WinDrawHelper
{

    static int deleteEvent;
    static int deleteScript;
    static int deleteFunction;

    public static int DrawScript(StateEvent e, int i, State s)
    {
        deleteEvent = -1;
        if (e != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(20));

            GUILayout.Label(i.ToString() + " :", GUILayout.Width(20));
            e.active = EditorGUILayout.Toggle(e.active, GUILayout.Width(15));
            GUILayout.Label(((int)(e.start * s.length)).ToString() + "~" +
                            ((int)(e.end * s.length)).ToString(), GUILayout.Width(50));
            EditorGUILayout.MinMaxSlider(ref e.start, ref e.end, 0f, 1f, GUILayout.Width(225));

            e.airCondition = (StateEvent.AirCondition)EditorGUILayout.EnumPopup(e.airCondition, GUILayout.Width(45));
            e.holdButton = GUILayout.Toggle(e.holdButton, " Hold?", EditorStyles.miniButton, GUILayout.Width(42));
            //if(EditorGUILayout.DropdownButton(FocusType.Keyboard);
            string nextString = e.script;
            if (e.script == "") { nextString = "<Select Script>"; }
            string lastScript = e.script;
            DrawDropdown(EditorGUILayout.GetControlRect(GUILayout.Width(100)), new GUIContent(nextString), e);
            bool refresh = false;
            if (lastScript != e.script) { refresh = true; Debug.Log(e.script); }

            if (nextString != "<Select Script>")
            {
                MethodInfo mInfo =
                    typeof(CharacterControl)
                .GetMethod(e.script);
                if (mInfo.GetParameters().Length > 0)
                {
                    ParameterInfo[] mParams = mInfo.GetParameters();
                    args =
                        mParams
                        .Select(x => x.Name)
                        .ToArray();
                    if (refresh || e.parameters.Count != args.Length)
                    {
                        Debug.Log("Refresh parameters");

                        List<float> refreshList = new List<float>();
                        for (int r = 0; r < args.Length; r++)
                        {
                            if (r <= e.parameters.Count - 1) { refreshList.Add(e.parameters[r]); }
                            else { refreshList.Add(0); }
                        }

                        e.parameters = new List<float>();

                        for (int g = 0; g < args.Length; g++)
                        {
                            e.parameters.Add(refreshList[g]);
                            //e.args.Add(new object()); 
                        }
                    }
                    e.args = new List<object>();
                    if (e.script == "VFX" || e.script ==  "FireProjectile")
                    {
                        e.prefab = EditorGUILayout.ObjectField(e.prefab, typeof(GameObject), false, GUILayout.Width(200)) as GameObject;
                    }
                    if (args.Length>2)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("", GUILayout.Width(50));
                    }
                    for (int a = 0; a < args.Length; a++)
                    {
                        ParameterInfo pI = mParams[a];
                        if (pI.ParameterType == typeof(float))
                        {
                            GUILayout.Label(args[a] + " : ", GUILayout.Width(50));
                            e.parameters[a] = EditorGUILayout.FloatField(e.parameters[a], GUILayout.Width(40));
                            e.args.Add(e.parameters[a]);
                        }
                    }
                    
                }
            }
            if (GUILayout.Button("copy", EditorStyles.miniButton, GUILayout.Width(35))) 
            {
                WindowStateEditor.copyEvent = i;
            }
            if (GUILayout.Button(" x", EditorStyles.miniButton, GUILayout.Width(20))) { deleteEvent = i; }

            //GUILayout.Label("", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();
        }
        return deleteEvent;
    }
    static string[] args;
    static string[] methods;
    static string[] ignoreMethods = new string[] { "Start", "Update", "OnGUI" };

    
    public static void DrawDropdown(Rect position, GUIContent label, StateEvent e)
    {
        if (!EditorGUI.DropdownButton(position, label, FocusType.Passive))
        {
            return;
        }

        void handleItemClicked(object parameter)
        {
            e.script = parameter.ToString();
            //Debug.Log(parameter);
        }

        //MethodInfo[] mInfo = typeof(CharacterControl).GetType().
        //    GetMethods().Where(x => x.DeclaringType == typeof(CharacterControl));

        methods =
            typeof(CharacterControl)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public) // Instance methods, both public and private/protected
            .Where(x => x.DeclaringType == typeof(CharacterControl)) // Only list methods defined in our own class
                                                                     //.Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
            .Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
            .Select(x => x.Name)
            .ToArray();

        //SerializedProperty newProp = new SerializedProperty();
        if (methods.Length <= 0) { Debug.Log("No Members Found"); return; }
        List<string> methodList = methods.ToList();
        methodList.Sort();
        methods = methodList.ToArray();
        GenericMenu menu = new GenericMenu();
        for (int i = 0; i < methods.Length; i++)
        {

            menu.AddItem(new GUIContent(methods[i]), false, handleItemClicked, methods[i]);

        }
        //menu.AddItem(new GUIContent("Item 1"), false, handleItemClicked, "Item 1");
        //menu.AddItem(new GUIContent("Item 2"), false, handleItemClicked, "Item 2");
        //menu.AddItem(new GUIContent("Item 3"), false, handleItemClicked, "Item 3");
        menu.DropDown(position);
    }

    public static int DrawAttack(State s)
    {
        int deleteAttack = -1;

        for (int i = 0; i < s.attacks.Count; i++)
        {
            Attack atk = s.attacks[i];

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(20));
            GUILayout.Label(i.ToString() + ":", GUILayout.Width(20));
            

            atk.active = EditorGUILayout.Toggle(atk.active, GUILayout.Width(15));
            GUILayout.Label(((int)(atk.startFrame * s.length)).ToString() + "~" +
                            ((int)(atk.endFrame * s.length)).ToString(), GUILayout.Width(50));

            EditorGUILayout.MinMaxSlider(ref atk.startFrame, ref atk.endFrame, 0f, 1f, GUILayout.Width(225));
            //atk.cancelWindowStart = EditorGUILayout.Slider(atk.cancelWindowStart, atk.startFrame, 1f, GUILayout.Width(175));
            Rect r = GUILayoutUtility.GetLastRect();
            r.x += 140; r.width -= 150;
            atk.cancelWindowStart = GUILayout.HorizontalSlider(atk.cancelWindowStart, atk.startFrame, 1f, GUILayout.Width(100));
            GUILayout.Label(Mathf.Round(atk.cancelWindowStart * s.length).ToString() + ":" + Mathf.Round((atk.cancelWindowStart - atk.startFrame) * s.length).ToString(), GUILayout.Width(47));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(30));

            GUILayout.Label("Damage:", GUILayout.Width(70));
            atk.damage = EditorGUILayout.FloatField(atk.damage, GUILayout.Width(75));

            GUILayout.Label("Hit Pause:", GUILayout.Width(70));
            atk.hitPause = EditorGUILayout.IntField(atk.hitPause, GUILayout.Width(75));

            GUILayout.Label("Hit Stun:", GUILayout.Width(70));
            atk.hitStun = EditorGUILayout.IntField(atk.hitStun, GUILayout.Width(75));

            

            //GUILayout.Label("Stun:", GUILayout.Width(70));
            //atk.dizzy = EditorGUILayout.FloatField(atk.dizzy, GUILayout.Width(75));

            EditorGUILayout.EndHorizontal();

            //Hit Effect
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(30));
            atk.hitPower = (Attack.HitPower)EditorGUILayout.EnumPopup(atk.hitPower, GUILayout.Width(75));
            atk.hitDirection = (Attack.HitDirection)EditorGUILayout.EnumPopup(atk.hitDirection, GUILayout.Width(75));
            atk.knockbackType = (Attack.KnockbackType)EditorGUILayout.EnumPopup(atk.knockbackType, GUILayout.Width(75));
            atk.knockBack = EditorGUILayout.Vector2Field("Knockback:", atk.knockBack, GUILayout.Width(125));
            atk.ignoreChainKnockback =
                GUILayout.Toggle(atk.ignoreChainKnockback, "Ignore Chain", EditorStyles.miniButton, GUILayout.Width(80));
            atk.guardBreak =
                GUILayout.Toggle(atk.guardBreak, "Guard Break", EditorStyles.miniButton, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            //Hitbox
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(30));
            GUILayout.Label("Hitbox : ", GUILayout.Width(50));
            atk.hitboxType = (Attack.HitboxType)EditorGUILayout.EnumPopup(atk.hitboxType, GUILayout.Width(75));
            atk.hitBoxPos = EditorGUILayout.Vector3Field("Position:", atk.hitBoxPos, GUILayout.Width(200));
            atk.hitBoxScale = EditorGUILayout.Vector3Field("Scale:", atk.hitBoxScale, GUILayout.Width(200));
            atk.hitBoxRot = EditorGUILayout.Vector3Field("Rotation:", atk.hitBoxRot, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

        }

        return deleteAttack;
    }

    

    public static int DrawTransition(State s, EngineVariablesObject ge)
    {
        int deleteTransition = -1;

        for (int t = 0; t < s.transitions.Count; t++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(20));
            GUILayout.Label(t.ToString() + ":", GUILayout.Width(20));
            //GUILayout.Label("Press:", GUILayout.Width(50));
            s.transitions[t].active = EditorGUILayout.Toggle(s.transitions[t].active, GUILayout.Width(15));
            s.transitions[t].priority = EditorGUILayout.IntField(s.transitions[t].priority, GUILayout.Width(20));
            if (s.transitions[t].pressButton.active = 
                GUILayout.Toggle(s.transitions[t].pressButton.active, "Press:", EditorStyles.miniButtonLeft, GUILayout.Width(40)))
            {
                s.transitions[t].pressButton.buttonIndex = 
                    EditorGUILayout.Popup(s.transitions[t].pressButton.buttonIndex, ge.rawInputBindings.ToArray(), EditorStyles.miniButtonRight, GUILayout.Width(35));
            }
            if (s.transitions[t].holdButton.active = 
                GUILayout.Toggle(s.transitions[t].holdButton.active, "Hold:", EditorStyles.miniButtonLeft, GUILayout.Width(40)))
            {
                s.transitions[t].holdButton.buttonIndex = 
                    EditorGUILayout.Popup(s.transitions[t].holdButton.buttonIndex, ge.rawInputBindings.ToArray(), EditorStyles.miniButtonRight, GUILayout.Width(35));//EditorGUILayout.TextField(s.transitions[t].pressButton, GUILayout.Width(40));
            }
            s.transitions[t].airState = (StateTransition.AirState)EditorGUILayout.EnumPopup(s.transitions[t].airState, GUILayout.Width(60));
            s.transitions[t].useJumps =
                GUILayout.Toggle(s.transitions[t].useJumps, "Jump?", EditorStyles.miniButton, GUILayout.Width(40));
            s.transitions[t].cantLoop =
                GUILayout.Toggle(s.transitions[t].cantLoop, "Loop?", EditorStyles.miniButton, GUILayout.Width(40));
            s.transitions[t].state = EditorGUILayout.ObjectField(s.transitions[t].state, typeof(State), false, GUILayout.Width(200)) as State;
            if (GUILayout.Button(" x", EditorStyles.miniButton, GUILayout.Width(20))) { deleteTransition = t; }
            EditorGUILayout.EndHorizontal();
        }

        return deleteTransition;
    }

}
