from JoyLib.Code.Entities.AI import RelationshipStatus
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType

class FamilyYes:
    def Interact(self, instigator, listener):
        listener.family.Add(instigator.GUID, RelationshipStatus.Spouse)
        instigator.family.Add(listener.GUID, RelationshipStatus.Spouse)
        ActionLog.AddText(instigator.name + "(" + str(instigator.GUID) + ") has become the spouse of " + listener.name + "(" + str(listener.GUID) + ").", LogType.Debug)
        return "I would love that."