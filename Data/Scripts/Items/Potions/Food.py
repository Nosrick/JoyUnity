from JoyLib.Code.Entities import NeedIndex

class Food:
    def Interact(self, entity, item):
        entity.FulfillNeed(NeedIndex.Food, item.value)
        entity.backpack.Remove(item)