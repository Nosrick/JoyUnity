from Needs.AbstractNeed import AbstractNeed
from JoyLib.Code.States import WorldState
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Helpers import RNG
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType
from JoyLib.Code.Entities.AI import NeedAIData

class Property(AbstractNeed):
    def __init__(self):
        pass

    def FindFulfilmentObject(self, entity):
        targets = entity.myWorld.SearchForObjects(entity, "Ownable", Intent.Interact)
        if(targets.Count > 0):
            target = targets[RNG.Roll(0, targets.Count - 1)]
            return target
        else:
            target = NeedAIData()
            return target

    def Interact(self, need, actor, object):
        actor.myWorld.PickUpObject(actor)
        ActionLog.AddText(actor.name + " (" + str(actor.GUID) + ") now owns " + object.name, LogType.Debug)

    def OnTick(self, need, entity):
        totalWealth = 0
        for item in entity.backpack:
            totalWealth += item.value

        need.Fulfill(totalWealth / 10)