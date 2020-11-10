from JoyLib.Code.Entities import StatisticIndex
from JoyLib.Code.Entities import NeedIndex
from JoyLib.Code.Conversation.Subengines import RumourType
from JoyLib.Code.Conversation.Subengines import RumourMill
from JoyLib.Code.Entities.AI import NeedAIData
from JoyLib.Code.Conversation import TopicData
from JoyLib.Code.Helpers import ActionLog
from JoyLib.Code.Helpers import LogType

class SexYes:
    def Interact(self, instigator, listener):
        instigatorSkill = (instigator.statistics[StatisticIndex.Personality] + instigator.statistics[StatisticIndex.Cognition] + instigator.statistics[StatisticIndex.Endurance]) / 3
        receiverSkill = (listener.statistics[StatisticIndex.Personality] + listener.statistics[StatisticIndex.Cognition] + listener.statistics[StatisticIndex.Endurance]) / 3
        instigator.FulfillNeed(NeedIndex.Sex, receiverSkill)
        instigator.InfluenceMe(listener.GUID, receiverSkill)
        listener.FulfillNeed(NeedIndex.Sex, instigatorSkill)
        listener.InfluenceMe(instigator.GUID, instigatorSkill)
        RumourMill.AddRumour(instigator, listener, RumourType.Scandal)
        ActionLog.AddText(instigator.name + "(" + str(instigator.GUID) + ") is making love to " + listener.name + "(" + str(listener.GUID) + ").", LogType.Debug)
        return "tThanks, love."