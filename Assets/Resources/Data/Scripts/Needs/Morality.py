from Needs.AbstractNeed import AbstractNeed

MORALITY_BONUS = 5

class Morality(AbstractNeed):
    def __init__(self):
        pass

    def OnTick(self, need, entity):
        need.Fulfill(MORALITY_BONUS)