using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowStateEditor : EditorWindow
{
    public Vector2 scrollBar = new Vector2(0, 0);
    public bool eventFoldout;
    public bool attacksFoldout;
    public bool attackStats;
    public bool transitionFoldout;
    public bool hitboxFoldout;
    [MenuItem("Window/Citadel Deep/State Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow<WindowStateEditor>("State Editor", true, typeof(WindowStateEditor));
    }

    public State state;
    int currentFrame;
    public EngineVariablesObject gameEngineVariables;
    public static int copyEvent = -1;
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        state = EditorGUILayout.ObjectField(state, typeof(State), false) as State;

        gameEngineVariables = EditorGUILayout.ObjectField(gameEngineVariables, typeof(EngineVariablesObject), false, GUILayout.Width(150)) as EngineVariablesObject;
        EditorGUILayout.EndHorizontal();
        if (state != null)
        {
            scrollBar = EditorGUILayout.BeginScrollView(scrollBar);

            //State Base
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Length:", GUILayout.Width(50));
            state.length = EditorGUILayout.IntField(state.length, GUILayout.Width(60));
            GUILayout.Label("Animation State:", GUILayout.Width(100));
            state.animationState = EditorGUILayout.TextField(state.animationState, GUILayout.Width(120));
            state.hasAirVariantAnimation =
                GUILayout.Toggle(state.hasAirVariantAnimation, "Air Too?", EditorStyles.miniButton, GUILayout.Width(50));
            GUILayout.Label("Blend:", GUILayout.Width(40));
            state.animationBlend = EditorGUILayout.FloatField(state.animationBlend, GUILayout.Width(60));
            state.typicalStart =
                GUILayout.Toggle(state.typicalStart, "Typical Start?", EditorStyles.miniButton, GUILayout.Width(75));
            state.aim =
                GUILayout.Toggle(state.aim, "Aim?", EditorStyles.miniButton, GUILayout.Width(40));
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            state.useJumps =
                GUILayout.Toggle(state.useJumps, "Use Jump?", EditorStyles.miniButton, GUILayout.Width(80));
            state.cantLoop =
                GUILayout.Toggle(state.cantLoop, "Can't Loop?", EditorStyles.miniButton, GUILayout.Width(80));
            state.hitstunCancel =
                GUILayout.Toggle(state.hitstunCancel, "Hitstun Cancel", EditorStyles.miniButton, GUILayout.Width(80));
            state.breakCancel =
                GUILayout.Toggle(state.breakCancel, "Break Cancel", EditorStyles.miniButton, GUILayout.Width(80));
            state.bypassHitstunGuard =
                GUILayout.Toggle(state.bypassHitstunGuard, "Bypass HitGuard", EditorStyles.miniButton, GUILayout.Width(90));
            state.groundCancel =
                GUILayout.Toggle(state.groundCancel, "Ground Cancel", EditorStyles.miniButton, GUILayout.Width(90));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Class:", GUILayout.Width(40));
            //state.weaponClass = EditorGUILayout.ObjectField(state.weaponClass, typeof(WeaponClass), false, GUILayout.Width(150)) as WeaponClass;
            state.stateType = (State.StateType)EditorGUILayout.EnumPopup(state.stateType, GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label("TP:", GUILayout.Width(40));
            //state.tpCost = EditorGUILayout.FloatField(state.tpCost, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            if (eventFoldout = EditorGUILayout.Foldout(eventFoldout, "Events"))
            {
                int deleteEvent = -1;
                copyEvent = -1;
                for (int i = 0; i < state.events.Count; i++)
                {
                    StateEvent e = state.events[i];
                    //if(e.script)
                    if (deleteEvent > -1) { WinDrawHelper.DrawScript(e, i, state); } //FIX THIS LOL
                    else { deleteEvent = WinDrawHelper.DrawScript(e, i, state); }
                }
                if (deleteEvent > -1) { state.events.RemoveAt(deleteEvent); }
                if (copyEvent > -1) 
                {
                    StateEvent oldEvent = state.events[copyEvent];
                    copyEvent = -1;
                    StateEvent newEvent = new StateEvent();
                    newEvent.start = oldEvent.start;
                    newEvent.end = oldEvent.end;
                    newEvent.script = oldEvent.script;

                    newEvent.parameters = new List<float>();
                    for(int p = 0; p < oldEvent.parameters.Count; p++)
                    {
                        newEvent.parameters.Add(oldEvent.parameters[p]);
                    }

                    newEvent.args = new List<object>();
                    for (int a = 0; a < oldEvent.args.Count; a++)
                    {
                        newEvent.args.Add(oldEvent.parameters[a]);
                    }
                    newEvent.active = oldEvent.active;
                    newEvent.airCondition = oldEvent.airCondition;
                    newEvent.holdButton = oldEvent.holdButton;
                    newEvent.operation = oldEvent.operation;
                    newEvent.prefab = oldEvent.prefab;
                    state.events.Add(newEvent);
                    //state.events.RemoveAt(deleteEvent);
                }
                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(30))) { state.events.Add(new StateEvent()); }
                GUILayout.Label("");
            }
            

            float totDam = 0;
            float totLen = state.length;

            for(int i = 0; i < state.attacks.Count; i++)
            {
                if (state.attacks[i].active)
                {
                    totDam += state.attacks[i].damage;
                    totLen = (int)(state.attacks[i].cancelWindowStart * state.length);
                }
            }
            if (totLen == 0) { totLen = 1f; }
            float dps = totDam * (60f / totLen);
            float estDamage = 0;
            if (state.attacks.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("DPS:", GUILayout.Width(30));
                state.dps = EditorGUILayout.FloatField(state.dps, GUILayout.Width(60));
                GUILayout.Label("TP:", GUILayout.Width(20));
                state.tpCost = EditorGUILayout.FloatField(state.tpCost, GUILayout.Width(60));


                //float estDamage = state.dps * (state.attacks[0].startFrame * state.length) / 60f * 100f;
                estDamage = 100f * state.dps * ((1f + state.attacks[state.attacks.Count - 1].cancelWindowStart * state.length) / 60f);
                //_attacker.currentState.dps * ((float)_attacker.currentStateFrame / 60f);
                GUILayout.Label("Damage: " + estDamage.ToString("0"), GUILayout.Width(100));

                //float estRating = state.dps * 1f; //state.dps;
                float estRating = estDamage * 0.01f; //state.dps;
                if (state.tpCost > 0) { estRating /= 1f + (state.tpCost * 0.02f); }
                GUILayout.Label("Rating: " + estRating.ToString("0.00"), GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
            if (attacksFoldout = EditorGUILayout.Foldout(attacksFoldout, "Attacks (" + state.attacks.Count + ")"))
            {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.Label("", GUILayout.Width(30));
                
                EditorGUILayout.EndHorizontal();
                int deleteAttack = -1;
                deleteAttack = WinDrawHelper.DrawAttack(state);
                if (deleteAttack > -1) { state.attacks.RemoveAt(deleteAttack); }
                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(30))) { state.attacks.Add(new Attack()); }
            }

            GUILayout.Label("");

            if (transitionFoldout = EditorGUILayout.Foldout(transitionFoldout, "Transitions"))
            {
                int deleteTransition = -1;
                deleteTransition = WinDrawHelper.DrawTransition(state, gameEngineVariables);

                if (deleteTransition > -1) { state.transitions.RemoveAt(deleteTransition); }
                if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(30))) { state.transitions.Add(new StateTransition()); }

            }
            GUILayout.Label("");
            
            EditorGUILayout.EndScrollView();
            EditorUtility.SetDirty(state);
        }
    }
}
