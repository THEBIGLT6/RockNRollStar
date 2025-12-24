using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLanguage", menuName = "ScriptableObjects/Language", order = 1)]
public class Language : ScriptableObject
{
    // Types

    [System.Serializable]
    public class Entry
    {
        public string key;
        public string value;
    }

    // Instance Variables
    [SerializeField]
    public List<Entry> m_entries = new List<Entry>();

    // Methods
    public string getValue(string key)
    {
        foreach (Entry entry in m_entries)
        {
            if (entry.key == key)
            {
                return entry.value;
            }
        }
        return "";
    
    } // End getValue

    public void createValue(string key, string value)
    {
        for (int i = 0; i < m_entries.Count; i++)
        {
            if (m_entries[i].key == key)
            {
                Entry entry = m_entries[i];
                entry.value = value;
                m_entries[i] = entry;
                return;
            }
        }
        Entry newEntry = new Entry { key = key, value = value };
        m_entries.Add(newEntry);
    
    } // End createValue

    public void deleteValue(string key)
    {
        for (int i = 0; i < m_entries.Count; i++)
        {
            if (m_entries[i].key == key)
            {
                m_entries.RemoveAt(i);
                return;
            }
        }

    } // End deleteValue

    public string[] missingValues()
    {
        List<string> missing = new List<string>();
        foreach (Entry entry in m_entries)
        {
            if (string.IsNullOrEmpty(entry.value))
            {
                missing.Add(entry.key);
            }
        }
        return missing.ToArray();
    }


    public bool isMissingValue()
    {
        string[] missing = missingValues();
        return missing.Length > 0;
    }

}
