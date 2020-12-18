using UnityEditor;

namespace DevionGames.Graphs
{
    [CustomPropertyDrawer(typeof(FlowGraph),true)]
    public class FlowGraphPropertyDrawer : GraphPropertyDrawer<FlowGraphView>{}
}