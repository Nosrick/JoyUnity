from JoyLib.Code.Quests import Quest
from JoyLib.Code.Quests import QuestProvider
from JoyLib.Code.States import WorldState

import clr

class QuestGet:
    def __init__(self):
        self.quest = None

    def Interact(self, instigator, listener):
        if(self.quest == None):
            self.quest = QuestProvider.MakeRandomQuest(instigator, listener)
            listener.questOffered = self.quest

        return self.quest.ToString() + "What do you say?"