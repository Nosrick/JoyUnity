from JoyLib.Code.Entities import NeedIndex

class Drink:
    def Interact(self, entity, item):
        entity.FulfillNeed(NeedIndex.Drink, item.value)
        entity.backpack.Remove(item)