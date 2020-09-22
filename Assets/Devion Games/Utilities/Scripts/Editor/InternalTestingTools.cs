using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames
{
    public class InternalTestingTools 
    {
        [MenuItem("Tools/Devion Games/Internal/Delete PlayerPrefs")]
        public static void DeletePlayerPrefs() {
            PlayerPrefs.DeleteAll();
        }
       
    }
}