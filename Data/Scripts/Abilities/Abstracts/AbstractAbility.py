from JoyLib.Code.Entities.Abilities import AbilityTrigger
from JoyLib.Code.Entities.Abilities import Ability
from JoyLib.Code.Entities.Abilities import AbilityTarget

class AbstractAbility:
    def __init__(self):
        self.abilityTrigger = AbilityTrigger.OnTakeHit
        self.target = AbilityTarget.Self
        self.stacking = False
        self.name = "DEFAULT ABILITY"
        self.internalName = "DEFAULTABILITY"
        self.description = "No description."
        self.file = __file__
        self.counter = 1
        self.magnitude = 1
        self.priority = 1
        self.manaCost = 0

    def ReturnSelf(self):
        return Ability(self.abilityTrigger, self.target, self.stacking, self.name, self.internalName, self.description, self.priority, self.counter, self.magnitude, self.manaCost, self.file, self)

    def OnAttack(self, attacker, defender, ability):
        pass

    def OnTakeHit(self, defender, attacker, damage, ability):
        pass

    def OnHeal(self, healingValue, ability):
        pass

    def OnPickup(self, entity, item, ability):
        pass

    def OnTick(self, entity, ability):
        pass

    def OnDeath(self, entity, killer, ability):
        pass

    def OnKill(self, entity, victim, ability):
        pass

    def Use(self, user, target, ability):
        pass