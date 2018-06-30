from JoyLib.Code.States import WorldState
from JoyLib.Code.Quests import QuestTracker

class FindPerson:
    def __init__(self):
        pass

    def Interact(self, instigator, listener):
        quest = QuestTracker.GetPrimaryQuestForEntity(instigator.GUID)
        if(quest == None):
            return ""

        if(quest.steps[0].actors.Count > 0):
            subject = quest.steps[0].actors[0]
            return "Where can I find " + subject.name + "?"
        else:
            return ""