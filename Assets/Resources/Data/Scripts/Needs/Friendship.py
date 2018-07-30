from Needs.AbstractNeed import AbstractNeed

import System.Collections.Generic

from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType
from JoyLib.Code.Entities import NeedIndex
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Entities import StatisticIndex
from JoyLib.Code.Helpers import RNG
from JoyLib.Code.States import WorldState

class Friendship(AbstractNeed):
    def __init__(self):
        pass

    def FindFulfilmentObject(self, entity):
        targets = WorldState.playerWorld.SearchForEntities(entity, "Entities-s", Intent.Interact)
        if(targets.Count > 0):
            target = targets[RNG.Roll(0, targets.Count - 1)]
            return target
        else:
            target = NeedAIData()
            return target

    def Interact(self, need, actor, object):
        actor.FulfillNeed(NeedIndex.Friendship, object.statistics[StatisticIndex.Personality])
        actor.InfluenceMe(object.GUID, object.statistics[StatisticIndex.Personality])
        object.FulfillNeed(NeedIndex.Friendship, actor.statistics[StatisticIndex.Personality])
        object.InfluenceMe(actor.GUID, actor.statistics[StatisticIndex.Personality])
        ActionLog.AddText(actor.name + " (" + str(actor.GUID) + ") is talking with " + object.name + " (" + str(object.GUID) + ").", LogType.Debug)

    def OnTick(self, need, entity):
        friendships = 0
        for KeyValuePair in entity.relationships:
            if(KeyValuePair.Value >= 100):
                friendships += 1
        need.Fulfill(friendships)
        