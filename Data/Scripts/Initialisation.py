import clr
import sys
import System

sys.path.append(System.IO.Directory.GetCurrentDirectory())
sys.path.append(System.IO.Directory.GetCurrentDirectory() + '/Data/Scripts')

clr.AddReferenceToFileAndPath("JoyLib.dll")
from JoyLib.Code.Scripting import PythonEngine

#PythonEngine.PrintToConsole("IronPython engine initialised.")