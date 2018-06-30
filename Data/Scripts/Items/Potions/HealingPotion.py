from JoyLib.Code.Entities import NeedIndex

class HealingPotion:
    def Interact(self, entity, item):
        entity.HealMe(entity.hitPoints)
        entity.backpack.Remove(item)
        entity.FulfillNeed(NeedIndex.Drink, 3)