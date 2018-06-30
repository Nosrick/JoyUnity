from JoyLib.Code.Entities.AI import NeedAIData

class AbstractNeed:
    def __init__(self):
        return self

    def ReturnSelf(self):
        return self

    def FindFulfilmentObject(self, entity):
        return NeedAIData()

    def Interact(self, need, actor, object):
        pass

    def OnTick(self, need, entity):
        pass