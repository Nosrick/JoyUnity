from JoyLib.Code.Entities.AI import RelationshipStatus
from Conversation.SexYes import SexYes
from Conversation.SexNo import SexNo

class SexDecide:
    def Interact(self, instigator, listener):
        for entity in listener.family:
            if(entity.Value == RelationshipStatus.Spouse and not entity.Key == instigator):
                return "I'm taken."

        if(listener.HasRelationship(instigator.GUID) > listener.matingThreshold and listener.IsViableMate(instigator) and instigator.IsViableMate(listener)):
            yes = SexYes()
            return yes.Interact(instigator, listener)
        else:
            no = SexNo()
            return no.Interact(instigator, listener)