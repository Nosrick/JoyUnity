from Needs.AbstractNeed import AbstractNeed

from JoyLib.Code.States import WorldState
from JoyLib.Code.Entities import NeedIndex
from JoyLib.Code.Entities.AI import Intent
from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Entities.Items import ItemInstance
from JoyLib.Code.Helpers import RNG

class Hunger(AbstractNeed):
    def __init__(self):
        pass

    def FindFulfilmentObject(self, entity):
        for item in entity.backpack:
            if(item.baseType.Equals("Food")):
                item.Interact(entity)
                nothing = NeedAIData()
                return nothing

        targets = WorldState.playerWorld.SearchForObjects(entity, "Food", Intent.Interact)
        if(targets.Count > 0):
            target = targets[RNG.Roll(0, targets.Count - 1)]
        else:
            target = NeedAIData()

        value = entity.needs[NeedIndex.Food].value
        happinessThreshold = entity.needs[NeedIndex.Food].happinessThreshold

        if(value <= happinessThreshold and value > happinessThreshold - 10):
            return target
        elif(value <= happinessThreshold - 10 and value > happinessThreshold - 15):
            targets = WorldState.playerWorld.SearchForEntities(entity, "Entities-ns", Intent.Attack)
            if(targets.Count > 0):
                target = targets[RNG.Roll(0, targets.Count - 1)]
            else:
                target = NeedAIData()

                if(target.target == None):
                    targets = WorldState.playerWorld.SearchForEntities(entity, "Entities-s", Intent.Attack)
                    if(targets.Count > 0):
                        target = targets[RNG.Roll(0, targets.Count - 1)]
                    else:
                        target = NeedAIData()

        return target

    def Interact(self, need, actor, object):
        object.Interact(actor);
        WorldState.playerWorld.RemoveObject(object.position);
        need.ClearTarget();