from Needs.AbstractNeed import AbstractNeed
from Needs.Friendship import Friendship

from JoyLib.Code.States import WorldState
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Entities.AI import RelationshipStatus
from JoyLib.Code.Entities import Entity
from JoyLib.Code.Entities import StatisticIndex
from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Helpers import RNG
from JoyLib.Code.Entities import NeedIndex
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType

class Family(AbstractNeed):
    def __init__(self):
        self.targetData = NeedAIData()

    def FindFulfilmentObject(self, entity):
        for familyMember in entity.family:
            self.targetData.target = familyMember.Key
            return self.targetData

        targets = WorldState.playerWorld.SearchForEntities(entity, "Entities-s", Intent.Interact)
        if(targets.Count > 0):
            for possibleTarget in targets:
                if(entity.HasRelationship(possibleTarget.target.GUID) > 200 and not entity.family.ContainsKey(possibleTarget.target.GUID)):
                    self.targetData = possibleTarget
                    break
            if(self.targetData.target == None):
                friendship = Friendship()
                self.targetData = friendship.FindFulfilmentObject(entity)

        return self.targetData

    def Interact(self, need, actor, object):
        if(actor.family.ContainsKey(object.GUID)):
            actor.FulfillNeed(NeedIndex.Family, object.statistics[StatisticIndex.Personality])
            actor.InfluenceMe(object.GUID, object.statistics[StatisticIndex.Personality])
            object.FulfillNeed(NeedIndex.Family, actor.statistics[StatisticIndex.Personality])
            object.InfluenceMe(actor.GUID, actor.statistics[StatisticIndex.Personality])
            ActionLog.AddText(actor.name + " (" + str(actor.GUID) + ") is spending family time with " + object.name + " (" + str(object.GUID) + ").", LogType.Debug)
        elif(actor.HasRelationship(object.GUID) > 200 and actor.IsViableMate(object) and object.IsViableMate(actor)):
            if(object.playerControlled):
                WorldState.TalkToPlayer(actor)
            else:
                actor.family.Add(object.GUID, RelationshipStatus.Spouse)
                object.family.Add(actor.GUID, RelationshipStatus.Spouse)
                ActionLog.AddText(actor.name + " (" + str(actor.GUID) + ") has become the spouse of " + object.name + " (" + str(object.GUID) + ").", LogType.Debug)
        else:
            friendship = Friendship()
            friendship.Interact(need, actor, object)
            