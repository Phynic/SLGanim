using UnityEngine;
class HumanPlayer : Player
{
    public override void Play(RoundManager roundManager)
    {
        roundManager.RoundState = new RoundStateWaitingForInput(roundManager);
    }
}