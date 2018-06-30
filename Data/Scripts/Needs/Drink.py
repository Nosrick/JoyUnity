from Needs.AbstractNeed import AbstractNeed

from JoyLib.Code.States import WorldState
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Helpers import RNG
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType

class Drink(AbstractNeed):
    def __init__(self):
        pass

    def FindFulfilmentObject(self, entity):
        targets = WorldState.playerWorld.SearchForObjects(entity, "Drinks", Intent.Interact)
        if(targets.Count > 0):
            target = targets[RNG.Roll(0, targets.Count - 1)]
            return target
        else:
            target = NeedAIData()
            return target

    def Interact(self, need, actor, object):
        object.Interact(actor)
        WorldState.playerWorld.RemoveObject(object.position)
        need.ClearTarget()
        ActionLog.AddText(actor.name + " is drinking " + object.name, LogType.Debug)
