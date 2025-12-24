using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class LanguageEditor : EditorWindow
{
    //Types
    enum MenuState
    { 
        Blank,
        AddLanguage,
        RemoveLanguage,
        AddKey,
        RemoveKey,
        LanguageSelected
    }


    // Instance Variables
    private List<Language> languages;
    private List<string> languageKeys;
    private MenuState currentState = MenuState.Blank;
    private Language selectedLanguage;
    private Vector2 scrollPos;
    private Vector2 lowerScrollPos;
    private string textFieldInput = "";
    private Dictionary<string, string> updatedEntries;

    private const string KEY = "ROCKNROLLSTARKEYS";

    // Public methods

    // Private methods
    [MenuItem("Window/Language Editor")]
    private static void initWindow()
    {
        // Create window
        LanguageEditor window = (LanguageEditor)GetWindow(typeof(LanguageEditor));
        window.minSize = new Vector2(500, 500);
        window.Show();
    
    } // End initWindow

    private void OnEnable()
    {
        loadLanguages();

        // Load keys
        string saved = EditorPrefs.GetString( KEY, "" );
        if ( !string.IsNullOrEmpty(saved) )
            languageKeys = new List<string>(saved.Split('|'));
        else
            languageKeys = new List<string>();
        

    } //End OnEnable

    private void OnDisable()
    {
        EditorPrefs.SetString(KEY, string.Join("|", languageKeys.ToArray() ) );
    } // End OnDisable

    private void OnGUI()
    {


        // Add / Remove buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Language", GUILayout.Height(30)))
        {
            currentState = MenuState.AddLanguage;
            textFieldInput = "";
        }

        if (GUILayout.Button("Remove Language", GUILayout.Height(30)))
        {
            currentState = MenuState.RemoveLanguage;
        }

        if (GUILayout.Button("Add Key", GUILayout.Height(30)))
        {
            currentState = MenuState.AddKey;
            textFieldInput = "";
        }

        if (GUILayout.Button("Remove Key", GUILayout.Height(30)))
        {
            currentState = MenuState.RemoveKey;
        }
        GUILayout.EndHorizontal();


        GUILayout.Space(10);

        // list of languages
        GUILayout.BeginVertical(); 
        GUILayout.Label( "Languages", EditorStyles.boldLabel );
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(200));

        foreach (Language lang in languages)
        {
            GUIContent content= new GUIContent(lang.name, "Click to select this language"); ;
            if ( lang.isMissingValue() )
            {
                string msg = "This language is missing values for the following keys:\n";
                for ( int i = 0; i < lang.missingValues().Length; i++ )
                {
                    msg += lang.missingValues()[i];
                    msg += (i != lang.missingValues().Length - 1) ? "," : ".";
                }
                content = new GUIContent( lang.name, EditorGUIUtility.IconContent("console.warnicon.sml").image, msg );
            }

            if (GUILayout.Button(content, GUILayout.Height(25)))
            {
                selectedLanguage = lang;
                currentState = MenuState.LanguageSelected;

                updatedEntries = new Dictionary<string, string>();
                foreach ( string key in languageKeys )
                {
                    updatedEntries[key] = selectedLanguage.getValue(key);
                }
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // Changing area
        GUILayout.BeginVertical();

        bool reset = false;
        switch ( currentState )
        {
            
            case MenuState.AddLanguage:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Add Language", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(120))) reset = true;
                GUILayout.EndHorizontal();

                textFieldInput = EditorGUILayout.TextField( "Enter Language: ", textFieldInput);

                if (GUILayout.Button("Save Changes", GUILayout.Width(120)))
                {
                    addLanguage(textFieldInput);
                    reset = true;
                }
                break;

            case MenuState.RemoveLanguage:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Remove Language", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(120))) reset = true;
                GUILayout.EndHorizontal();

                GUILayout.Label("Select the language you would like to remove...");
                GUILayout.Space(5);


                lowerScrollPos = GUILayout.BeginScrollView(lowerScrollPos, false, true, GUILayout.Height(175));
                foreach (Language lang in languages)
                {
                    if (GUILayout.Button(lang.name, GUILayout.Height(25)))
                    {
                        removeLanguage(lang.name);
                        reset = true;
                        break;
                    }
                }

                GUILayout.EndScrollView();
                break;

            case MenuState.AddKey:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Add Key", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(120))) reset = true;
                GUILayout.EndHorizontal();

                textFieldInput = EditorGUILayout.TextField( "Enter New Key: ", textFieldInput);

                if (GUILayout.Button("Save Changes", GUILayout.Width(120)))
                { 
                    addKey(textFieldInput);
                    reset = true;
                }

                break;

            case MenuState.RemoveKey:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Remove Key", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(120))) reset = true;
                GUILayout.EndHorizontal();

                GUILayout.Label("Select the key you would like to remove...");
                GUILayout.Space(5);


                lowerScrollPos = GUILayout.BeginScrollView(lowerScrollPos, false, true, GUILayout.Height(175));
                foreach (string key in languageKeys)
                {
                    if (GUILayout.Button(key, GUILayout.Height(25)))
                    {
                        removeKey( key );
                        reset = true;
                        break;
                    }
                }
                
                GUILayout.EndScrollView();
                break;

            case MenuState.LanguageSelected:
                GUILayout.BeginHorizontal();
                GUILayout.Label( selectedLanguage.name, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(120))) reset = true;
                GUILayout.EndHorizontal();

                editLanguage(ref reset);

                break;

            default:
                break;
        }
        GUILayout.EndVertical();

        if ( reset )
        {
            currentState = MenuState.Blank;
            selectedLanguage = null;
        }

    } //end OnGUI

    private void loadLanguages()
    {
        // If languages is null, load all Language assets from Assets/Languages
        if ( languages == null )
        {
            languages = new List<Language>();

            string[] guids = AssetDatabase.FindAssets( "t:Language", new[] { "Assets/Resources/Languages" } );

            foreach ( string guid in guids )
            {
                var lang = AssetDatabase.LoadAssetAtPath<Language>( AssetDatabase.GUIDToAssetPath( guid ) );
                if (lang != null) languages.Add(lang);
            }
        }
    
    } // end loadLanguages

    private void addLanguage( string languageStr )
    {
        if (languageStr == null) return;

        foreach( Language lang in languages )
        {
            if( lang.name == languageStr )
            {
                return;
            }
        }

        string path = "Assets/Resources/Languages/" + languageStr + ".asset";

        Language newLanguage = ScriptableObject.CreateInstance<Language>();
        newLanguage.name = languageStr;
        AssetDatabase.CreateAsset( newLanguage, path );
        EditorUtility.SetDirty(newLanguage);
        AssetDatabase.SaveAssets(); 
        AssetDatabase.Refresh();   
        for ( int i = 0; i < languageKeys.Count; i++ )
        {
            newLanguage.createValue( languageKeys[i], "" );
        }   

        languages = null;
        loadLanguages();

    } // end addLanguage

    private void removeLanguage( string languageStr )
    {
        if (languageStr == null) return;

        Language selectedLanguage = null;
        foreach ( Language lang in languages )
        {
            if( languageStr == lang.name )
            {
                selectedLanguage = lang;
                break;
            }
        }

        if( selectedLanguage != null )
        {
            string path = AssetDatabase.GetAssetPath( selectedLanguage );
            AssetDatabase.DeleteAsset(path);
            EditorUtility.SetDirty(selectedLanguage);
            AssetDatabase.SaveAssets(); 
            AssetDatabase.Refresh();    
        }

        languages = null;
        loadLanguages();

    } // end removeLanguage

    private void addKey( string key )
    {
        if( key != null && !languageKeys.Contains( key ) )
        { 
            languageKeys.Add( key );
        }

        foreach ( Language lang in languages )
        {
            lang.createValue(key, "");
        }

    } // end addKey

    private void removeKey( string key )
    {
        if ( languageKeys.Contains(key) )
        {
            languageKeys.Remove(key);
        }

        foreach (Language lang in languages)
        {
            lang.deleteValue( key );
        }

    } // end removeKey

    private void editLanguage(ref bool reset)
    {
        List<Language.Entry> entries = selectedLanguage.m_entries;

        lowerScrollPos = GUILayout.BeginScrollView(lowerScrollPos, false, true, GUILayout.Height(175));
        foreach (string key in languageKeys)
        {
            if (!updatedEntries.ContainsKey(key))
                updatedEntries[key] = selectedLanguage.getValue(key);

            GUILayout.BeginHorizontal();

            GUIStyle richStyle = new GUIStyle(EditorStyles.label);
            richStyle.richText = true;
            GUILayout.Label( $" <b>KEY:</b> {key}   -   <b>VALUE:</b> {updatedEntries[key]}", richStyle);

            updatedEntries[key] = EditorGUILayout.TextField(updatedEntries[key], GUILayout.Width(100));

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if ( GUILayout.Button( "Save Changes", GUILayout.Width(120) ) )
        {
            reset = true;

            foreach ( var pair in updatedEntries )
            {
                selectedLanguage.createValue( pair.Key, pair.Value );
            }

            updatedEntries = null;
            EditorUtility.SetDirty(selectedLanguage);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

    } // end editLanguage


} // end LanguageEditor
