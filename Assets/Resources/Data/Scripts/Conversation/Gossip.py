from JoyLib.Code.Conversation.Subengines import RumourMill

class Gossip:
    def Interact(self, instigator, listener):
        return RumourMill.FetchRumour()