

class QuestYes:
    def __init__(self):
        self.quest = None

    def Interact(self, instigator, listener):
        self.quest = listener.questOffered
        instigator.AddQuest(self.quest)
        return "Thanks!"