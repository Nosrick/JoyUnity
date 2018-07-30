from Needs.AbstractNeed import AbstractNeed
from Needs.Drink import Drink
from Needs.Hunger import Hunger
from Needs.Sleep import Sleep

from JoyLib.Code.Entities import NeedIndex

class Health(AbstractNeed):
    def __init__(self):
        self.needIndex = NeedIndex.Food

    def FindFulfilmentObject(self, entity):
        lowestValue = 1000
        self.needIndex = NeedIndex.Food
        if(entity.needs[NeedIndex.Drink].value < lowestValue):
            lowestValue = entity.needs[NeedIndex.Drink].value
            self.needIndex = NeedIndex.Drink

        if(entity.needs[NeedIndex.Sleep].value < lowestValue):
            lowestValue = entity.needs[NeedIndex.Sleep].value
            self.needIndex = NeedIndex.Sleep

        if(self.needIndex == NeedIndex.Drink):
            drink = Drink()
            return drink.FindFulfilmentObject(entity)
        elif(self.needIndex == NeedIndex.Food):
            hunger = Hunger()
            return hunger.FindFulfilmentObject(entity)
        elif(self.needIndex == NeedIndex.Sleep):
            sleep = Sleep()
            return sleep.FindFulfilmentObject(entity)

    def Interact(self, need, actor, object):
        if(self.needIndex == NeedIndex.Drink):
            drink = Drink()
            drink.Interact(need, actor, object)
        elif(self.needIndex == NeedIndex.Food):
            hunger = Hunger()
            hunger.Interact(need, actor, object)

    def OnTick(self, need, entity):
        healthBonus = (entity.needs[NeedIndex.Food].value + entity.needs[NeedIndex.Drink].value + entity.needs[NeedIndex.Sleep].value) / 30
        need.Fulfill(healthBonus)