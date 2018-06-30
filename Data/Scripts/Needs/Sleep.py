from Needs.AbstractNeed import AbstractNeed

from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Entities import NeedIndex
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType

class Sleep(AbstractNeed):
    def __init__(self):
        pass

    def FindFulfilmentObject(self, entity):
        target = NeedAIData()
        self.Interact(entity.needs[NeedIndex.Sleep], entity, None)
        return target

    def Interact(self, need, actor, object):
        if(not need.contributingHappiness):
            actor.FulfillNeed(NeedIndex.Sleep, 16, 480)
            ActionLog.AddText(actor.name + " is sleeping.", LogType.Debug)
