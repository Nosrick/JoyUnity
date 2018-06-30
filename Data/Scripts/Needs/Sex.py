from Needs.AbstractNeed import AbstractNeed

import System.Collections.Generic

from JoyLib.Code.States import WorldState
from JoyLib.Code.Entities import NeedIndex
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Entities.AI import RelationshipStatus
from JoyLib.Code.Entities import StatisticIndex
from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Conversation.Subengines import RumourMill
from JoyLib.Code.Conversation.Subengines import RumourType
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType

class Sex(AbstractNeed):
    def __init__(self):
        pass

    def FindFulfilmentObject(self, entity):
        target = NeedAIData()
        if(entity.family.Count > 0):
            for familyMember in entity.family:
                if(familyMember.Value == RelationshipStatus.Spouse):
                    target.target = entity.myWorld.GetEntity(familyMember.Key)
                    return target

        target = entity.myWorld.SearchForMate(entity)

        return target

    def Interact(self, need, actor, object):
        instigatorSkill = (actor.statistics[StatisticIndex.Personality] + actor.statistics[StatisticIndex.Cognition] + actor.statistics[StatisticIndex.Endurance]) / 3
        receiverSkill = (object.statistics[StatisticIndex.Personality] + object.statistics[StatisticIndex.Cognition] + object.statistics[StatisticIndex.Endurance]) / 3
        actor.FulfillNeed(NeedIndex.Sex, receiverSkill)
        actor.InfluenceMe(object.GUID, receiverSkill)
        object.FulfillNeed(NeedIndex.Sex, instigatorSkill)
        object.InfluenceMe(actor.GUID, instigatorSkill)
        RumourMill.AddRumour(actor, object, RumourType.Scandal)
        ActionLog.AddText(actor.name + " is making love to " + object.name + ".", LogType.Debug)
