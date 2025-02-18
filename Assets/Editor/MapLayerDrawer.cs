using UnityEditor;
using UnityEngine;
using RoguelikeMap;
[CustomPropertyDrawer(typeof(MapLayer))]
public class MapLayerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUI.GetPropertyHeight(property, label,false);

        if (!property.isExpanded)
        {
            return height; // Only the foldout is visible
        }

        height += EditorGUIUtility.singleLineHeight; // RandomizedLayer toggle

        SerializedProperty randomizedLayerProp = property.FindPropertyRelative("randomizedLayer");

        if (!randomizedLayerProp.boolValue)
        {
            // Only count height for "layerNodes"
            SerializedProperty layerNodesProp = property.FindPropertyRelative("layerNodes");
            height += EditorGUI.GetPropertyHeight(layerNodesProp, true);
        }
        else
        {
            // Count height for the properties that are drawn
            SerializedProperty weightedPotentialNodeTypesProp = property.FindPropertyRelative("weightedPotentialNodeTypes");
            SerializedProperty minNodeSizeProp = property.FindPropertyRelative("minNodeSize");
            SerializedProperty maxNodeSizeProp = property.FindPropertyRelative("maxNodeSize");

            height += EditorGUIUtility.singleLineHeight; // WeightedPotentialNodeTypes label

            if (weightedPotentialNodeTypesProp.isExpanded)
            {
                height += GetListHeight(weightedPotentialNodeTypesProp); // List items
            }

            height += EditorGUIUtility.singleLineHeight * 2; // MinNodeSize & MaxNodeSize
        }
        height += EditorGUIUtility.singleLineHeight;
        return height;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin drawing the property (the overall MapLayer)
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout for the MapLayer element
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

        // If the property is expanded, draw the individual properties inside
        if (property.isExpanded)
        {
            // Indent the next fields to create some spacing from the foldout
            EditorGUI.indentLevel++;

            // Get the property "randomizedLayer"
            SerializedProperty randomizedLayerProp = property.FindPropertyRelative("randomizedLayer");

            // Display the randomizedLayer toggle
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), randomizedLayerProp);

            // Conditionally show properties based on randomizedLayer value
            if (!randomizedLayerProp.boolValue)
            {
                // Show only layerNodes if randomizedLayer is not true
                SerializedProperty layerNodesProp = property.FindPropertyRelative("layerNodes");
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), layerNodesProp, new GUIContent("Layer Nodes"));
            }
            else
            {
                // If randomizedLayer is true, show other properties
                SerializedProperty weightedPotentialNodeTypesProp = property.FindPropertyRelative("weightedPotentialNodeTypes");
                SerializedProperty minNodeSizeProp = property.FindPropertyRelative("minNodeSize");
                SerializedProperty maxNodeSizeProp = property.FindPropertyRelative("maxNodeSize");

                // Draw the WeightedPotentialNodeTypes list and calculate height
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), weightedPotentialNodeTypesProp, new GUIContent("Weighted Potential Node Types"));

                // Ensure the height for list items is added properly
                if (weightedPotentialNodeTypesProp.isExpanded)
                {
                    position.y += GetListHeight(weightedPotentialNodeTypesProp);
                    
                }

                // Draw other properties
                position.y += EditorGUIUtility.singleLineHeight*2;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), minNodeSizeProp, new GUIContent("Min Node Size"));

                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), maxNodeSizeProp, new GUIContent("Max Node Size"));
            }

            // Reset the indent level after drawing
            EditorGUI.indentLevel--;
        }

        // End drawing the property
        EditorGUI.EndProperty();
    }

    // Helper function to recursively calculate the height of a property (including list elements)
    private float GetListHeight(SerializedProperty property)
    {
        // If the property is a list or array, calculate height recursively for each element
        if (property.isArray)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            int elementCount = property.arraySize;

            // Add space for each item in the array/list
            for (int i = 0; i < elementCount; i++)
            {
                SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                totalHeight += EditorGUI.GetPropertyHeight(elementProperty, true);  // Use EditorGUI to get the height of each element
            }
            totalHeight += EditorGUIUtility.singleLineHeight;
            return totalHeight;
        }
        else
        {
            // If it's a regular property, return its height
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}