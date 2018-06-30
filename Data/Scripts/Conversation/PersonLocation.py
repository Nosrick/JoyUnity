from JoyLib.Code.States import WorldState
from JoyLib.Code.Quests import QuestTracker
from JoyLib.Code.Entities import Gender
from JoyLib.Code.Helpers import SectorStringConvertor
from JoyLib.Code.Entities.AI import Pathfinder

class PersonLocation:
    def __init__(self):
        pass

    def Interact(self, instigator, listener):
        quest = QuestTracker.GetPrimaryQuestForEntity(instigator.GUID)

        if(quest == None):
            return ""

        if(quest.steps[0].actors.Count > 0):
            subject = quest.steps[0].actors[0]

            if(subject.GUID == listener.GUID):
                return "That's me!"

            world = WorldState.overworld.GetWorldOfEntity(subject.GUID)
            subjectGender = "they're"
            if(subject.gender == Gender.Male):
                subjectGender = "he's"
            elif(subject.gender == Gender.Female):
                subjectGender = "she's"

            if(WorldState.playerWorld.GUID == world.GUID):
                sector = Pathfinder.DetermineSector(listener.position, subject.position)
                sectorString = SectorStringConvertor.ConvertSector(sector)
                return "Last I saw them, they were " + sectorString + "."
            else:
                return "I hear " + subjectGender + " is in " + world.name + "."
        else:
            return ""