using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillSettings))]
public class SkillSettingsInspector : Editor {

	private SerializedObject csObj;
	private SerializedProperty cs;

	public void OnEnable()
	{
		csObj = new SerializedObject(target);
		cs = csObj.FindProperty("skillProperties");
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		//EditorGUIUtility.LookLikeControls();

		csObj.Update();

		EditorGUILayout.PropertyField(cs);
		if(cs.isExpanded)
		{
			//EditorGUILayout.LabelField("Array Size", skills.arraySize.ToString());
			cs.arraySize = EditorGUILayout.IntField("Array Size", cs.arraySize);
			EditorGUI.indentLevel += 1;
			for (int i = 0; i < cs.arraySize; i++)
			{
				//EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));

				/*SerializedProperty currentTarget = skills.GetArrayElementAtIndex(i);
				currentTarget.FindPropertyRelative("skillType").enumValueIndex = EditorGUILayout.Popup("Skill Type", currentTarget.FindPropertyRelative("skillType").enumValueIndex, currentTarget.FindPropertyRelative("skillType").enumNames);

				//EditorGUILayout.LabelField("Array Size", currentTarget.FindPropertyRelative("duration").arraySize.ToString());
				currentTarget.FindPropertyRelative("duration").arraySize = EditorGUILayout.IntField("Array Size", currentTarget.FindPropertyRelative("duration").arraySize);
				
				for(int j = 0; j < currentTarget.FindPropertyRelative("duration").arraySize; j++)
				{
					//EditorGUILayout.TextField(list.GetArrayElementAtIndex(i));
					SerializedProperty subTarget = currentTarget.FindPropertyRelative("duration").GetArrayElementAtIndex(i);
					subTarget.floatValue = EditorGUILayout.FloatField("Level " + j, subTarget.floatValue);
					//EditorGUILayout.PropertyField(subTarget);
				}*/
			}
		}

		csObj.ApplyModifiedProperties();
	}
}

public static class EditorList2
{
	public static void Show(SerializedProperty list)
	{
		EditorGUILayout.PropertyField(list);
		if(list.isExpanded)
		{
			EditorGUILayout.LabelField("Array Size", list.arraySize.ToString());
			EditorGUI.indentLevel += 1;
			for (int i = 0; i < list.arraySize; i++)
			{
				//SkillProperties sp = (SkillProperties)list.GetArrayElementAtIndex(i);
				//EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
				//list.GetArrayElementAtIndex(i).FindPropertyRelative("skillType") = EditorGUILayout.Popup("Skill Type", 0, list.GetArrayElementAtIndex(i).FindPropertyRelative("skillType").enumNames);
				EditorGUILayout.LabelField("Array Size", list.GetArrayElementAtIndex(i).FindPropertyRelative("duration").arraySize.ToString());
				//sp.skillName = EditorGUILayout.TextField("Skill Name", sp.skillName);

				/*for(int j = 0; j < list.GetArrayElementAtIndex(i); j++)
				{
					EditorGUILayout.TextField(list.GetArrayElementAtIndex(i));
					//EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
				}*/
			}
		}
	}
}