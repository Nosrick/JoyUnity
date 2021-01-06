from JoyLib.Code.States import WorldState

class LocalArea:
    def Interact(self, instigator, listener):
        return WorldState.playerWorld.GetLocalAreaInfo(listener)