from JoyLib.Code.Entities.AI import RelationshipStatus
from Conversation.FamilyYes import FamilyYes
from Conversation.FamilyNo import FamilyNo

class FamilyDecide:
    def Interact(self, instigator, listener):
        if(not instigator.family.ContainsValue(RelationshipStatus.Spouse)):
            for entity in listener.family:
                if(entity.Value == RelationshipStatus.Spouse):
                    return "I'm taken."
            if(listener.HasRelationship(instigator.GUID) > 300 and listener.IsViableMate(instigator) and instigator.IsViableMate(listener)):
                yes = FamilyYes()
                return yes.Interact(instigator, listener)
            else:
                no = FamilyNo()
                return no.Interact(instigator, listener)
        else:
            return "How dare you!"